using HomeCook.Data.Models;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Index;

namespace HomeCook.Data.Extensions.SearchEngine
{
    public interface IRecipeIndex
    {
        IEnumerable<int> FieldValues { get; }
        IEnumerable<string> FieldKeys { get; }
        string IdFieldKey { get; }
        Analyzer Analyzer { get; }
        Lucene.Net.Store.Directory Directory { get; }
        IndexWriter IndexWriter { get; }

        Document GetDocumentFromEntity(Recipe entity);
        long GetEntityId(Recipe entity);
        string GetFieldKeyByFieldId(int fieldId);
    }
}