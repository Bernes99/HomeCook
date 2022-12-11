using HomeCook.Data.Models;

namespace HomeCook.Data.Extensions.SearchEngine
{
    public interface ILuceneWriter
    {
        void AddOrUpdateRange(IEnumerable<Recipe> entities);
        IDictionary<int, string> GetIndexedFieldNames();
        void Remove(Recipe entity);
    }
}