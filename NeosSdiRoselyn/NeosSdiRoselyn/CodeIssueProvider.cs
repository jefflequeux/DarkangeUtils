using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using Roslyn.Compilers;
using Roslyn.Compilers.Common;
using Roslyn.Compilers.CSharp;
using Roslyn.Services;
using Roslyn.Services.Editor;

namespace NeosSdiRoselyn
{
    [ExportSyntaxNodeCodeIssueProvider("NeosSdiRoselyn", LanguageNames.CSharp)]
    class CodeIssueProvider : ICodeIssueProvider
    {
        private readonly ICodeActionEditFactory editFactory;

        [ImportingConstructor]
        public CodeIssueProvider(ICodeActionEditFactory editFactory)
        {
            this.editFactory = editFactory;
        }

        public IEnumerable<CodeIssue> GetIssues(IDocument document, CommonSyntaxNode node, CancellationToken cancellationToken)
        {
            var tokens = from nodeOrToken in node.ChildNodesAndTokens()
                         where nodeOrToken.IsToken
                         select nodeOrToken.AsToken();

            foreach (var token in tokens)
            {
                var tokenText = token.GetText();

                if (tokenText.Contains('a'))
                {
                    var issueDescription = string.Format("'{0}' contains the letter 'a'", tokenText);
                    yield return new CodeIssue(CodeIssue.Severity.Warning, token.Span, issueDescription);
                }
            }
        }

        #region Unimplemented ICodeIssueProvider members

        public IEnumerable<CodeIssue> GetIssues(IDocument document, CommonSyntaxToken token, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CodeIssue> GetIssues(IDocument document, CommonSyntaxTrivia trivia, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
