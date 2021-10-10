using System.Collections.Generic;
using System.Linq;

namespace Jammo.Spelling.English
{
    public class DictionaryTreeNode : DictionaryTreeElement
    {
        public readonly List<DictionaryTreeElement> Children = new();
        public readonly string Filter;

        public DictionaryTreeNode(string filter, DictionaryTreeNode parent)
        {
            Filter = filter;
            Parent = parent;
        }

        public DictionaryTreeNode GetOrCreateNode(string filter)
        {
            foreach (var child in Children.OfType<DictionaryTreeNode>())
            {
                if (child.Filter == filter)
                    return child;
            }

            return new DictionaryTreeNode(filter, this);
        }

        public bool TryAddChild(DictionaryTreeElement element)
        {
            if (Children.Contains(element))
                return false;
            
            Children.Add(element);

            return true;
        }
    }
}