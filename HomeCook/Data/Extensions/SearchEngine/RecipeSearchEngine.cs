using HomeCook.Data.Models;
using HomeCook.DTO.SearchEngine;
using HomeCook.Services.Interfaces;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using Directory = Lucene.Net.Store.Directory;

namespace HomeCook.Data.Extensions.SearchEngine
{
    public class RecipeSearchEngine : IRecipeSearchEngine
    {
        private static IndexWriter _indexWriter;
        const LuceneVersion version = LuceneVersion.LUCENE_48;


        public RecipeSearchEngine(IConfiguration configuration)
        {
            string workingDirectory = Environment.CurrentDirectory;
            var dictionaryPath = System.IO.Directory.GetParent(workingDirectory).Parent.FullName + "/IndexSearch";

            if (System.IO.Directory.Exists(dictionaryPath))
            {
                System.IO.Directory.Delete(dictionaryPath, true);
            }

            Directory = FSDirectory.Open(dictionaryPath);
            Analyzer = new StandardAnalyzer(version);

            var config = new IndexWriterConfig(version, Analyzer);
            //Singleton pattern for IndexWriter to ensure only one
            //IndexWriter exists per index.
            _indexWriter = _indexWriter ?? new IndexWriter(
                Directory,
                config);
        }

        public IndexWriter IndexWriter => _indexWriter;

        public Directory Directory { get; }

        public Analyzer Analyzer { get; }

        public string IdFieldKey => IndexField.Id.ToString();

        public IEnumerable<string> FieldKeys =>
            Enum.GetNames(typeof(IndexField));

        public IEnumerable<int> FieldValues =>
            Enum.GetValues(typeof(IndexField))
                .Cast<IndexField>().Cast<int>().ToList();

        

        public void AddOrUpdateRange(IEnumerable<Recipe> entities)
        {
            if (entities is null)
            {
                throw new ArgumentNullException();
            }

            var writer = IndexWriter;

            foreach (var entity in entities)
            {
                var newDocument = GetDocumentFromEntity(entity);
                if (newDocument is null)
                {
                    continue;
                }
                var indexTerm = new Term(
                    IdFieldKey,
                    GetEntityId(entity).ToString());
                
                writer.UpdateDocument(indexTerm, newDocument);
            }
            writer.Commit();
        }

        public void Remove(Recipe entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException();
            }

            var writer = IndexWriter;
            var indexTerm = new Term(
                IdFieldKey,
                GetEntityId(entity).ToString());
            writer.DeleteDocuments(indexTerm);
            writer.Commit();
        }

        public IDictionary<int, string> GetIndexedFieldNames()
        {
            return FieldValues
                .ToDictionary(
                    x => x,
                    x => GetFieldKeyByFieldId(x));
        }
        //////////////////
        ///

        public Document GetDocumentFromEntity(Recipe entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException();
            }
            if (entity.DateDeletedUtc is not null || !string.IsNullOrEmpty(entity.DeletedBy))
            {
                return null;
            }
            var recipe = entity;

            if (recipe.RecipeProducts is null ||
                recipe.RecipeCategories is null ||
                recipe.RecipeTags is null)
            {
                throw new ArgumentException(
                    "Related entity was not eagerly loaded and " +
                    "could not be loaded lazily either.");
            }

            var document = new Document();

            document.Add(new StringField(
                IndexField.Id.ToString(),
                recipe.Id.ToString(),
                Field.Store.YES));

            document.Add(new StringField(
                IndexField.PublicId.ToString(),
                recipe.PublicId,
                Field.Store.YES));

            document.Add(new TextField(
                IndexField.DateCreatedUtc.ToString(),
                recipe.DateCreatedUtc.ToString() ?? "",
                Field.Store.YES));

            document.Add(new TextField(
                IndexField.Title.ToString(),
                recipe.Title ?? "",
                Field.Store.YES));
            document.Add(new TextField(
                IndexField.Introdution.ToString(),
                recipe.Introdution ?? "",
                Field.Store.YES));
            document.Add(new SingleField(
                IndexField.Rating.ToString(),
                recipe.Rating ?? 1f,
                Field.Store.YES));
            document.Add(new TextField(
                IndexField.Portion.ToString(),
                recipe.Portion ?? "",
                Field.Store.YES));
            document.Add(new TextField(
                IndexField.PreparingTime.ToString(),
                recipe.PreparingTime ?? "",
                Field.Store.YES));
            document.Add(new SingleField(
                IndexField.Difficulty.ToString(),
                recipe.Difficulty,
                Field.Store.YES));
            document.Add(new TextField(
                IndexField.Author.ToString(),
                recipe.AuthorId ?? "",
                Field.Store.YES));

            recipe.RecipeProducts.ToList().ForEach(x =>
            {
                var field = new TextField(
                    IndexField.Product.ToString(),
                    x.Product.Name,// + " {" + x.Id + "}",
                    Field.Store.YES);

                document.Add(field);
            });

            recipe.RecipeCategories.ToList().ForEach(x =>
                document.Add(new TextField(
                    IndexField.Category.ToString(),
                    x.Category.Name, //+ " {" + x.Id + "}",
                    Field.Store.YES)));

            recipe.RecipeTags.ToList().ForEach(x =>
                document.Add(new TextField(
                    IndexField.Tag.ToString(),
                    x.Tag.Name, //+ " {" + x.Id + "}",
                    Field.Store.YES)));

            return document;
        }

        public string GetFieldKeyByFieldId(int fieldId)
        {
            if (!Enum.IsDefined(typeof(IndexField), fieldId))
            {
                throw new ArgumentException(
                    $"There's no Enum value corresponding to " +
                    $"value {fieldId}.");
            }

            return ((IndexField)fieldId).ToString();
        }

        public long GetEntityId(Recipe entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException();
            }

            return entity.Id;
        }

        public enum IndexField
        {
            Id,
            PublicId,
            DateCreatedUtc,
            Title,
            Introdution,
            Rating,
            Portion,
            Author,
            PreparingTime,
            Difficulty,
            Product,
            Category,
            Tag
        }
        ///////

        public List<LuceneRecipeSearchResultItem> Search(
            string searchString,
            RecipeFilters filters)
        {
            
            
            var queryParser = new MultiFieldQueryParser(version, FieldKeys.ToArray(), Analyzer);

            Query query;
            if (string.IsNullOrEmpty(searchString))
            {
                MatchAllDocsQuery matchAllDocs = new MatchAllDocsQuery();
                query = CreateFilteredQuery(filters, matchAllDocs);
            }
            else
            {
                query = CreateFilteredQuery(filters, queryParser.Parse(searchString));
            }
            

            if (query is null)
            {
                throw new ArgumentException(
                    $"No meaningful query could be derived from " +
                    $"the given searchString: {searchString}");
            }

            var directoryReader = DirectoryReader.Open(Directory);
            var indexSearcher = new IndexSearcher(directoryReader);


            var hits = indexSearcher.Search(query, 10).ScoreDocs;

            var recipes = new List<LuceneRecipeSearchResultItem>();
            foreach (var hit in hits)
            {
                var document = indexSearcher.Doc(hit.Doc);
                recipes.Add(new LuceneRecipeSearchResultItem()
                {
                    Id = document.Get(IndexField.PublicId.ToString()),
                    DateCreatedUtc = document.Get(IndexField.DateCreatedUtc.ToString()),
                    Title = document.Get(IndexField.Title.ToString()),
                    Difficulty = float.Parse(document.Get(IndexField.Difficulty.ToString())),
                    Rating = float.Parse(document.Get(IndexField.Rating.ToString())),
                    Author = document.Get(IndexField.Author.ToString()),
                    Products = document.GetValues(IndexField.Product.ToString()).ToList(),
                    //MainImage =_imageService.GetrecipeMainImage(long.Parse(document.Get(IndexField.PublicId.ToString())))
                });

            }

            return recipes;
        }

        private Query CreateFilteredQuery(RecipeFilters filters, Query criteria)
        {
            if (filters is null)
            {
                return criteria;
            }
            var bQuery = new BooleanQuery() { };

            foreach (var product in filters.Products)
            {
                bQuery.Add(new TermQuery(new Term(IndexField.Product.ToString(), product)), Occur.SHOULD);
            }
            foreach (var category in filters.CategoryNames)
            {
                bQuery.Add(new TermQuery(new Term(IndexField.Category.ToString(), category)), Occur.MUST);
            }
            if (filters.Rating > 1f)
            {
                //var ratingFilter = NumericRangeFilter.NewSingleRange(IndexField.Rating.ToString(), 1,filters.Rating,5,true,true);
                //bQuery.Add(new FilteredQuery(bQuery,ratingFilter),Occur.SHOULD);

                var ratingQuery = NumericRangeQuery.NewSingleRange(IndexField.Rating.ToString(), 1, filters.Rating, 5, true, true);
                bQuery.Add(ratingQuery, Occur.MUST);
            }
            if (filters.Difficulty > 1f)
            {
                //var difFilter = NumericRangeFilter.NewSingleRange(IndexField.Difficulty.ToString(), 1, filters.Difficulty, 10, true, true);
                //bQuery.Add(new FilteredQuery(bQuery, difFilter), Occur.SHOULD);
                var difQuery = NumericRangeQuery.NewSingleRange(IndexField.Difficulty.ToString(), 1, filters.Difficulty, 10, true, true);
                bQuery.Add(difQuery, Occur.MUST);
            }
            bQuery.Add(criteria, Occur.MUST);

            return bQuery;
            //var queryParser = new MultiFieldQueryParser(version, _index.FieldKeys.ToArray(), _index.Analyzer);

            //var queryresult = queryParser.Parse(searchString);
        }
    }
}
