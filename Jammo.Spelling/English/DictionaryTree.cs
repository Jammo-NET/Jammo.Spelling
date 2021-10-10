using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Jammo.Spelling.English
{
    public class DictionaryTree
    {
        public readonly DictionaryTreeNode Root = new("", null);

        public static async Task<DictionaryTree> Create(StreamReader reader)
        {
            var tree = new DictionaryTree();
            
            await foreach (var word in DictionaryParser.Parse(reader))
            {
                var currentNode = tree.Root;

                for (var wordIndex = 1; wordIndex <= word.Word.Length; wordIndex++)
                {
                    var newNode = currentNode.GetOrCreateNode(string.Concat(word.Word.Take(wordIndex)));

                    currentNode.TryAddChild(newNode);
                    currentNode = newNode;
                }

                currentNode.Children.Add(word);
            }

            return tree;
        }

        public bool TryGetWord(string word, out IEnumerable<DictionaryTreeWord> result)
        {
            var words = new List<DictionaryTreeWord>();
            var currentNode = Root;
            var wordIndex = 1;

            result = words;

            if (string.IsNullOrWhiteSpace(word))
                return false;
            
            while (currentNode != null)
            {
                if (wordIndex > word.Length)
                    return words.Count > 0;
                
                foreach (var nodeWord in currentNode.Children.OfType<DictionaryTreeWord>())
                {
                    if (nodeWord.Word == word)
                        words.Add(nodeWord);
                }

                var foundNode = false;

                foreach (var node in currentNode.Children.OfType<DictionaryTreeNode>())
                {
                    if (node.Filter == word)
                    {
                        words.Add(node.Children
                            .OfType<DictionaryTreeWord>()
                            .FirstOrDefault(c => c.Word == word));
                    }
                    else if (node.Filter == string.Concat(word.Take(wordIndex)))
                    {
                        foundNode = true;
                        wordIndex++;
                        currentNode = node;
                        
                        break;
                    }
                }

                if (!foundNode)
                    return words.Count > 0;
            }
            
            return true;
        }

        public IEnumerable<DictionaryTreeWord> GetCompletions(string word)
        {
            yield break;
        }

        public IEnumerable<DictionaryTreeWord> FindAnagrams(string word)
        {
            yield break;
        }
    }

    public abstract class DictionaryTreeElement
    {
        public DictionaryTreeNode Parent;
    }
}
