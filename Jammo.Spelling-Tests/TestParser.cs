using System;
using System.IO;
using System.Threading.Tasks;
using Jammo.Spelling.English;
using NUnit.Framework;

namespace Jammo.Spelling_Tests
{
    [TestFixture]
    public class TestParser
    {
        private FileInfo file = new FileInfo(Path.Join(Directory.GetCurrentDirectory(), "TestDictionary.txt"));
        
        [Test]
        public async Task TestWord()
        {
            using var stream = file.OpenText();
            var words = DictionaryParser.Parse(stream);

            await foreach (var word in words)
            {
                Assert.True(word.Word == "Aardvark");
                
                return;
            }
        }

        [Test]
        public async Task TestType()
        {
            using var stream = file.OpenText();
            var words = DictionaryParser.Parse(stream);

            await foreach (var word in words)
            {
                Assert.True(word.Type == WordType.Noun);
                
                return;
            }
        }

        [Test]
        public async Task TestStorage()
        {
            using var reader = file.OpenText();
            var tree = await DictionaryTree.Create(reader);
            
            Assert.True(tree.TryGetWord("Aardvark", out _));
        }
    }
}