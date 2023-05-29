using System;
using System.Collections.Generic;
using System.Linq;

namespace Translator
{
    public class LexicalAnalysis
    {
        public List<Lexeme> lexemes = new List<Lexeme>();
        public List<Token> tokens = new List<Token>();

        public List<char> buffer = new List<char>();
        public void AddToList(string typeLexeme)
        {
            string nameLexeme = BufferToString(buffer);
            lexemes.Add(new Lexeme(nameLexeme, typeLexeme));
            buffer.Clear();

            Token token;
            switch (typeLexeme)
            {
                case "Идентификатор":
                    if (Token.IsSpecialWord(nameLexeme))
                    {
                        token = new Token(Token.SpecialWords[nameLexeme]);
                        token.Value = nameLexeme;
                        tokens.Add(token);
                    }
                    else
                    {
                        token = new Token(Token.TokenType.IDENTIFIER);
                        token.Value = nameLexeme;
                        tokens.Add(token);
                    }
                    break;
                case "Литерал":
                    token = new Token(Token.TokenType.LITERAL);
                    token.Value = nameLexeme;
                    tokens.Add(token);
                    break;
                case "Разделитель":
                    token = new Token(Token.SpecialSymbols[Convert.ToChar(nameLexeme)]);
                    token.Value = nameLexeme;
                    tokens.Add(token);
                    break;
            }
        }

        public char[] separator = new char[] {';', ',', '>', '<', '=', '+', '-', '*', '/', '{', '}', '(', ')', '!'};
        public bool IsSeparator(char ch)
        {
            return separator.Contains(ch);
        }
        public void AnalyzeCode(string[] codeText)
        {
            foreach (string str in codeText)
            {
                for (int i = 0; i < str.Length; i++)
                {
                    if (str[i] == '/' && str[i + 1] == '/')
                    {
                        break;
                    }
                    else if (char.IsWhiteSpace(str[i]))
                    {
                        continue;
                    }
                    else if (char.IsDigit(str[i]))
                    {
                        while (char.IsDigit(str[i]))
                        {
                            buffer.Add(str[i]);
                            if (i + 1 < str.Length && char.IsDigit(str[i + 1]))
                            {
                                i++;
                            }
                            else
                            {
                                AddToList("Литерал");
                                break;
                            }
                        }
                    }
                    else if (char.IsLetter(str[i]))
                    {
                        while (char.IsLetterOrDigit(str[i]))
                        {
                            if ((str[i] >= 'a' && str[i] <= 'z') || (str[i] >= 'A' && str[i] <= 'Z') || char.IsDigit(str[i]))
                            {
                                buffer.Add(str[i]);
                                if (i + 1 < str.Length && char.IsLetterOrDigit(str[i + 1]))
                                {
                                    i++;
                                }
                                else
                                {
                                    if (buffer.Count > 8)
                                    {
                                        throw new AnaliseException($"Cлишком большой идентификатор: {BufferToString(buffer)}");
                                    }
                                    else
                                    {
                                        AddToList("Идентификатор");
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                throw new AnaliseException("Допустимо использовать только латинский алфавит");
                            }
                        }
                    }
                    else if (IsSeparator(str[i]))
                    {
                        buffer.Add(str[i]);
                        AddToList("Разделитель");
                    }

                    else
                    {
                        throw new AnaliseException($"Недопустимый символ: {str[i]}");
                    }
                }
            }
        }
        public string BufferToString(List<char> buffer)
        {
            string nameLexeme = "";
            foreach (char i in buffer)
            {
                nameLexeme += i;
            }
            return nameLexeme;
        }
    }
}