using HomeCook.Data.Models;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Lucene.Net.Util;
using Microsoft.IdentityModel.Protocols;
using Directory = Lucene.Net.Store.Directory;

namespace HomeCook.Data.Extensions.SearchEngine
{
    public class RecipeIndex : IRecipeIndex
    {
        private static IndexWriter _indexWriter;
        const LuceneVersion version = LuceneVersion.LUCENE_48;
        public RecipeIndex(
            IConfiguration configuration)
        {
            string workingDirectory = Environment.CurrentDirectory;
            var dictionaryPath = System.IO.Directory.GetParent(workingDirectory).Parent.Parent.FullName + "/IndexSearch";

            if (System.IO.Directory.Exists(dictionaryPath))
            {
                System.IO.Directory.Delete(dictionaryPath, true);
            }

            Directory = FSDirectory.Open(dictionaryPath);
            Analyzer = new StandardAnalyzer(version);

            var config = new IndexWriterConfig(version,Analyzer);
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

        public Document GetDocumentFromEntity(Recipe entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException();
            }

            var recipe = entity;

            if (recipe.RecipeProducts is null ||
                recipe.RecipesCategories is null ||
                recipe.RecipesTags is null)
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
                recipe.Introdution?? "",
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
                recipe.PreparingTime?? "",
                Field.Store.YES));
            document.Add(new SingleField(
                IndexField.Difficulty.ToString(),
                recipe.Difficulty,
                Field.Store.YES));

            recipe.RecipeProducts.ToList().ForEach(x =>
            {
                var field = new TextField(
                    IndexField.Product.ToString(),
                    x.Product.Name + " {" + x.Id + "}",
                    Field.Store.YES);

                document.Add(field);
            });

            recipe.RecipesCategories.ToList().ForEach(x =>
                document.Add(new TextField(
                    IndexField.Category.ToString(),
                    x.Category.Name + " {" + x.Id + "}",
                    Field.Store.YES)));

            recipe.RecipesTags.ToList().ForEach(x =>
                document.Add(new TextField(
                    IndexField.Tag.ToString(),
                    x.Tag.Name + " {" + x.Id + "}",
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

    }
}
