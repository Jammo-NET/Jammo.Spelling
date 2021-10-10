using System;

namespace Jammo.Spelling.English
{
    [Flags]
    public enum WordType
    {
        None = 1<<0, // 0
        
        Prefix = 1<<1,
        Suffix = 1<<2,
        
        Plural = 1<<3,
        
        Adverb = 1<<4,
        Verb = 1<<5,
        Noun = 1<<6,
        Adjective = 1<<7,
        
        Abbreviation = 1<<8,
        Slang = 1<<9,
        Attribute = 1<<10
    }
}