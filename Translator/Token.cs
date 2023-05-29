using System.Collections.Generic;

namespace Translator
{
    public class Token
    {
        public enum TokenType
        {
            VOID, MAIN, WHILE, INT, FLOAT, DOUBLE, LITERAL, NUMBER, IDENTIFIER,
            LPAR, RPAR, LBRACE, RBRACE, PLUS, MINUS, EQUALS, LESS, GREATER,
            DIVISION, MULTIPLICATION, COMMA, SEMICOLON, EXCLAM, NETERM, EXPR
        }

        public static Dictionary<char, TokenType> SpecialSymbols = new Dictionary<char, TokenType>() {
            { '(', TokenType.LPAR },
            { ')', TokenType.RPAR },
            { '{', TokenType.LBRACE },
            { '}', TokenType.RBRACE },
            { '+', TokenType.PLUS },
            { '-', TokenType.MINUS },
            { '=', TokenType.EQUALS },
            { '<', TokenType.LESS },
            { '>', TokenType.GREATER },
            { '/', TokenType.DIVISION },
            { '*', TokenType.MULTIPLICATION },
            { ',', TokenType.COMMA },
            { ';', TokenType.SEMICOLON },
            { '!', TokenType.EXCLAM }
        };

        public static Dictionary<string, TokenType> SpecialWords = new Dictionary<string, TokenType>() {
            { "void", TokenType.VOID },
            { "main", TokenType.MAIN },
            { "while", TokenType.WHILE },
            { "int", TokenType.INT },
            { "float", TokenType.FLOAT },
            { "double", TokenType.DOUBLE }
        };

        public static bool IsSpecialWord(string word)
        {
            if (string.IsNullOrEmpty(word))
            {
                return false;
            }
            return SpecialWords.ContainsKey(word);
        }

        public TokenType Type;
        public string Value;

        public Token(TokenType type)
        {
            Type = type;
        }
        public override string ToString()
        {
            if (Type == TokenType.IDENTIFIER || Type == TokenType.LITERAL)
                return $"{Type}, {Value}";
            else return $"{Type}";
        }
    }
}
