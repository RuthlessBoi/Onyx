using Onyx.Syntax.Nodes;
using Onyx.Syntax.Nodes.Members;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Onyx.Binding
{
    internal sealed class BoundNamespace : IEnumerable<BoundNamespace>, IAnnotatable
    {
        internal static readonly BoundNamespace Global = new BoundNamespace("Global");

        public string Name { get; }
        public ImmutableArray<TemplateDeclarationSyntax> Templates => templates.ToImmutableArray();
        public ImmutableArray<FunctionDeclarationSyntax> Functions => functions.ToImmutableArray();
        public ImmutableArray<BoundAnnotation> Annotations => annotations.ToImmutableArray();

        private readonly List<BoundNamespace> children;
        private readonly List<TemplateDeclarationSyntax> templates;
        private readonly List<FunctionDeclarationSyntax> functions;
        private readonly List<BoundAnnotation> annotations;

        public BoundNamespace(string name)
        {
            Name = name;

            children = new List<BoundNamespace>();
            templates = new List<TemplateDeclarationSyntax>();
            functions = new List<FunctionDeclarationSyntax>();
            annotations = new List<BoundAnnotation>();
        }

        public void Annotate(BoundAnnotation? annotation)
        {
            if (annotation == null)
                return;

            annotations.AddRange(annotation.Annotations);
            annotations.Add(annotation);
        }
        public void Annotate(IEnumerable<BoundAnnotation> annotations)
        {
            foreach (var annotation in annotations)
                Annotate(annotation);
        }

        public BoundNamespace QueryNamespaces(NamespaceSyntax syntax)
        {
            if (syntax.HasNext)
            {
                foreach (var child in children)
                {
                    if (child.Name == syntax.Current.Text)
                    {
                        syntax.MoveNext();
                        return child.QueryNamespaces(syntax);
                    }
                }
            }

            return this;
        }
        public BoundNamespace RebuildNamespaces(NamespaceSyntax syntax, IEnumerable<MemberSyntax> members)
        {
            if (syntax.HasNext)
            {
                foreach (var child in children)
                {
                    if (child.Name == syntax.Current.Text)
                    {
                        syntax.MoveNext();
                        child.RebuildNamespaces(syntax, members);
                    }
                }
            } 
            else
                Add(members);

            return this;
        }
        public void Add(IEnumerable<MemberSyntax> members)
        {
            foreach (var member in members)
            {
                if (member is TemplateDeclarationSyntax tds)
                    templates.Add(tds);
                else if (member is FunctionDeclarationSyntax fds)
                    functions.Add(fds);
            }
        }

        public static BoundNamespace Rebuild(NamespaceSyntax syntax, IEnumerable<MemberSyntax> members) => Global.RebuildNamespaces(syntax, members);
        public static BoundNamespace Query(NamespaceSyntax syntax) => Global.QueryNamespaces(syntax);

        public IEnumerator<BoundNamespace> GetEnumerator() => children.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
