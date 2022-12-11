using HomeCook.Data.Models;
using HomeCook.DTO.SearchEngine;
using HomeCook.Services.Interfaces;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Util;
using static HomeCook.Data.Extensions.SearchEngine.RecipeIndex;

namespace HomeCook.Data.Extensions.SearchEngine
{
    public class LuceneReader : ILuceneReader
    {
        private readonly IRecipeIndex _index;
        private readonly IConfiguration _configuration;
        const LuceneVersion version = LuceneVersion.LUCENE_48;
        private readonly IImageService _imageService;

        public LuceneReader(
            IRecipeIndex index,
            IConfiguration configuration,
            IImageService imageService)
        {
            _index = index;
            _configuration = configuration;
            _imageService = imageService;
        }

        public List<LuceneRecipeSearchResultItem> Search(
            string searchString,
            RecipeFilters filters,
            int? fieldIdToSearch = null,
            IEnumerable<int> documentIdsToSearch = null)
        {
            var searchResultItems =
                new List<LuceneRecipeSearchResultItem>();
            var queryParser = new MultiFieldQueryParser(version, _index.FieldKeys.ToArray(), _index.Analyzer);

            var query = CreateFilteredQuery(filters, queryParser.Parse(searchString));

            if (query is null)
            {
                throw new ArgumentException(
                    $"No meaningful query could be derived from " +
                    $"the given searchString: {searchString}");
            }

            var directoryReader = DirectoryReader.Open(_index.Directory);
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
                    MainImage = _imageService.GetrecipeMainImage(long.Parse(document.Get(IndexField.PublicId.ToString())))
                });

            }

            return recipes;
        }

        private Query CreateFilteredQuery(RecipeFilters filters, Query criteria)
        {
            var bQuery = new BooleanQuery();

            foreach (var product in filters.Products)
            {
                bQuery.Add(new TermQuery(new Term(IndexField.Product.ToString(), product)), Occur.MUST);
            }
            foreach (var category in filters.CategoryNames)
            {
                bQuery.Add(new TermQuery(new Term(IndexField.Category.ToString(), category)), Occur.MUST);
            }
            bQuery.Add(criteria, Occur.MUST);

            return bQuery;
            //var queryParser = new MultiFieldQueryParser(version, _index.FieldKeys.ToArray(), _index.Analyzer);

            //var queryresult = queryParser.Parse(searchString);
        }

    }
}
