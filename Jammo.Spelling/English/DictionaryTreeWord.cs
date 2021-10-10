namespace Jammo.Spelling.English
{
    public class DictionaryTreeWord : DictionaryTreeElement
    {
        public readonly string Word;
        public readonly string Definition;
        public readonly WordType Type;

        public DictionaryTreeWord(string word, string definition, WordType type)
        {
            Word = word;
            Definition = definition;
            Type = type;
        }
    }
}