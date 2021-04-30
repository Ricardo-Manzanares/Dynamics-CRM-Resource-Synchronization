using CRMResourceSynchronization.Core.DiffPlex.DiffBuilder.Model;

namespace CRMResourceSynchronization.Core.DiffPlex.DiffBuilder
{
    public interface IInlineDiffBuilder
    {
        DiffPaneModel BuildDiffModel(string oldText, string newText);
        DiffPaneModel BuildDiffModel(string oldText, string newText, bool ignoreWhitespace, bool ignoreCase, IChunker chunker);
    }
}
