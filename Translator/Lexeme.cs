using System;

namespace Translator
{
    public class Lexeme
    {
        public string Name;
        public string Type;
        public Lexeme(string name, string type)
        {
            Name = name;
            Type = type;
        }
    }
}
