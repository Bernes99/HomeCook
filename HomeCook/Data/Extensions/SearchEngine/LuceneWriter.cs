using HomeCook.Data.Models;
using Lucene.Net.Index;

namespace HomeCook.Data.Extensions.SearchEngine
{
    public class LuceneWriter : ILuceneWriter
    {
        private readonly IRecipeIndex _index;
        protected DefaultDbContext Context { get; }

        public LuceneWriter(IRecipeIndex index, DefaultDbContext context)
        {
            _index = index;
            Context = context;
        }

        public void AddOrUpdateRange(IEnumerable<Recipe> entities)
        {
            if (entities is null)
            {
                throw new ArgumentNullException();
            }

            var writer = _index.IndexWriter;

            foreach (var entity in entities)
            {
                var indexTerm = new Term(
                    _index.IdFieldKey,
                    _index.GetEntityId(entity).ToString());
                var newDocument = _index.GetDocumentFromEntity(entity);
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

            var writer = _index.IndexWriter;
            var indexTerm = new Term(
                _index.IdFieldKey,
                _index.GetEntityId(entity).ToString());
            writer.DeleteDocuments(indexTerm);
            writer.Commit();
        }

        public IDictionary<int, string> GetIndexedFieldNames()
        {
            return _index.FieldValues
                .ToDictionary(
                    x => x,
                    x => _index.GetFieldKeyByFieldId(x));
        }
    }
}
