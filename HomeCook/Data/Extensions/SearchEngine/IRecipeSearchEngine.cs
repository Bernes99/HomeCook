using HomeCook.Data.Models;
using HomeCook.DTO.SearchEngine;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Index;

namespace HomeCook.Data.Extensions.SearchEngine
{
    public interface IRecipeSearchEngine
    {
        Analyzer Analyzer { get; }
        Lucene.Net.Store.Directory Directory { get; }
        IEnumerable<string> FieldKeys { get; }
        IEnumerable<int> FieldValues { get; }
        string IdFieldKey { get; }
        IndexWriter IndexWriter { get; }

        void AddOrUpdateRange(IEnumerable<Recipe> entities);
        Document GetDocumentFromEntity(Recipe entity);
        long GetEntityId(Recipe entity);
        string GetFieldKeyByFieldId(int fieldId);
        IDictionary<int, string> GetIndexedFieldNames();
        void Remove(Recipe entity);
        List<LuceneRecipeSearchResultItem> Search(string searchString, RecipeFilters filters);
    }
}