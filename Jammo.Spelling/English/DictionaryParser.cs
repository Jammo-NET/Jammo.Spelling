using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Jammo.ParserTools;

namespace Jammo.Spelling.English
{
    public static class DictionaryParser
    {
        public static async IAsyncEnumerable<DictionaryTreeWord> Parse(StreamReader reader)
        {
            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                line = line.Replace('—', '-');
                // Remove EM DASH

                var lexer = new Lexer(line);
                var state = new StateMachine<ParserState>(ParserState.Word);

                var word = string.Empty;
                var definition = string.Empty;
                var type = WordType.None;

                foreach (var token in lexer)
                {
                    switch (state.Current)
                    {
                        case ParserState.Word:
                        {
                            switch (token.Id)
                            {
                                case LexerTokenId.Alphabetic:
                                case LexerTokenId.AlphaNumeric:
                                case LexerTokenId.Dash:
                                case LexerTokenId.Slash:
                                {
                                    var tokens = new List<LexerToken> { token };
                                    
                                    tokens.AddRange(lexer.TakeWhile(t =>
                                        t.Is(LexerTokenId.Alphabetic) ||
                                        t.Is(LexerTokenId.AlphaNumeric) ||
                                        t.Is(LexerTokenId.Dash) ||
                                        t.Is(LexerTokenId.Dash)));

                                    word = string.Concat(tokens.SelectMany(t => t.ToString()));
                                    state.MoveTo(ParserState.Type);

                                    break;    
                                }
                            }
                            
                            break;
                        }
                        case ParserState.Type:
                        {
                            switch (token.Id)
                            {
                                case LexerTokenId.Space:
                                {
                                    continue;
                                }
                                case LexerTokenId.Alphabetic:
                                case LexerTokenId.AlphaNumeric:
                                case LexerTokenId.Dash:
                                {
                                    var tokens = new List<LexerToken> { token };
                                    
                                    tokens.AddRange(lexer.TakeWhile(t =>
                                        !t.Is(LexerTokenId.Period)));

                                    switch (string.Concat(tokens.SelectMany(t => t.ToString())).ToLower())
                                    {
                                        case "prefix":
                                        {
                                            type |= WordType.Prefix;
                                            
                                            break;
                                        }
                                        case "suffix":
                                        {
                                            type |= WordType.Suffix;
                                            
                                            break;
                                        }
                                        case "abbr":
                                        {
                                            type |= WordType.Abbreviation;
                                            
                                            break;
                                        }
                                        case "n" or "-n":
                                        {
                                            type |= WordType.Noun;
                                            
                                            break;
                                        }
                                        case "v" or "-v":
                                        {
                                            type |= WordType.Verb;
                                            
                                            break;
                                        }
                                        case "adv" or "-adv":
                                        {
                                            type |= WordType.Adverb;
                                            
                                            break;
                                        }
                                        case "adj" or "-adj":
                                        {
                                            type |= WordType.Adjective;
                                            
                                            break;
                                        }
                                        case "attrib" or "-attrib":
                                        {
                                            type |= WordType.Attribute;
                                            
                                            break;
                                        }
                                    }
                                    
                                    break;
                                }
                            }
                            
                            break;
                        }
                        case ParserState.ExtraTyping:
                        {
                            break;
                        }
                        case ParserState.Definition:
                        {
                            break;
                        }
                        case ParserState.Origin:
                        {
                            break;
                        }
                        case ParserState.OriginDefinition:
                        {
                            break;
                        }
                    }
                }

                if (type != WordType.None) // Contains bits other than None
                    type &= ~WordType.None;

                yield return new DictionaryTreeWord(word, definition, type);
            }
        }

        private enum ParserState
        {
            Word,
            Type,
            ExtraTyping,
            Definition,
            Origin,
            OriginDefinition,
        }
    }
}