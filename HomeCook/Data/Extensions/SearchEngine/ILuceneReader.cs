using HomeCook.DTO.SearchEngine;

namespace HomeCook.Data.Extensions.SearchEngine
{
    public interface ILuceneReader
    {
        List<LuceneRecipeSearchResultItem> Search(string searchString, RecipeFilters filters, int? fieldIdToSearch = null, IEnumerable<int> documentIdsToSearch = null);
    }
}