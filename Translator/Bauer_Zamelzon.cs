using System;
using System.Collections.Generic;

namespace Translator
{
    public class Bauer_Zamelzon
    {
        private List<Troyka> listTroyka = new List<Troyka>();
        private List<Token> tokens = new List<Token>();
        private Stack<Token> E = new Stack<Token>();
        private Stack<Token> T = new Stack<Token>();
        private int lex = 0;
        private int index = 1;
        public List<string> strings = new List<string>();
        private void ListTroykaToStrings(List<Troyka> listTroyka)
        {
            int ind = 1;
            foreach(Troyka t in listTroyka)
            {
                strings.Add($"M{ind}: {t.operat.Value}, {t.operand1.Value}, {t.operand2.Value}");
                ind++;
            }
        }
        private Token GetLexem(int lex)
        {
            return tokens[lex];
        }
        public Bauer_Zamelzon(List<Token> expr)
        {
            tokens = expr;
        }
        private void K_id()
        {
            E.Push(GetLexem(lex));
            lex++;
        }
        private void K_op()
        {
            if (E.Count < 2)
                throw new Exception("Невозможно выполнить арифметическое выражение: число операндов не удовлетворяет условию");
            Troyka k = new Troyka(T.Pop(), E.Pop(), E.Pop());
            listTroyka.Add(k);
            Token token = new Token(Token.TokenType.IDENTIFIER);
            token.Value = $"M{index}";
            E.Push(token);
            index++;
        }
        public void Start()
        {
            CheckContains(tokens);
            CheckSyntax();
            Method();
            ListTroykaToStrings(listTroyka);
        }
        private void Method()
        {
            if (lex == tokens.Count)
            {
                if(T.Count == 0)
                {
                    return;
                }
                else
                {
                    EndList();
                }
            }
            else
            {
                switch (GetLexem(lex).Type)
                {
                    case Token.TokenType.IDENTIFIER:
                        K_id();
                        break;
                    case Token.TokenType.LITERAL:
                        K_id();
                        break;
                    case Token.TokenType.LPAR:
                        Lpar();
                        break;
                    case Token.TokenType.PLUS:
                        PlusOrMinus();
                        break;
                    case Token.TokenType.MINUS:
                        PlusOrMinus();
                        break;
                    case Token.TokenType.MULTIPLICATION:
                        MultiplicationOrDivision();
                        break;
                    case Token.TokenType.DIVISION:
                        MultiplicationOrDivision();
                        break;
                    case Token.TokenType.RPAR:
                        Rpar();
                        break;
                }
            }
            Method();
        }
        private void Rpar()
        {
            if (T.Count == 0)
                D5("лишняя \")\"");
            else
            {
                switch (T.Peek().Type)
                {
                    case Token.TokenType.LPAR:
                        D3();
                        break;
                    case Token.TokenType.PLUS:
                        D4();
                        break;
                    case Token.TokenType.MINUS:
                        D4();
                        break;
                    case Token.TokenType.MULTIPLICATION:
                        D4();
                        break;
                    case Token.TokenType.DIVISION:
                        D4();
                        break;
                }
            }
        }
        private void MultiplicationOrDivision()
        {
            if (T.Count == 0)
                D1();
            else
            {
                switch (T.Peek().Type)
                {
                    case Token.TokenType.LPAR:
                        D1();
                        break;
                    case Token.TokenType.PLUS:
                        D1();
                        break;
                    case Token.TokenType.MINUS:
                        D1();
                        break;
                    case Token.TokenType.MULTIPLICATION:
                        D2();
                        break;
                    case Token.TokenType.DIVISION:
                        D2();
                        break;
                }
            }
        }
        private void PlusOrMinus()
        {
            if (T.Count == 0)
                D1();
            else
            {
                switch (T.Peek().Type)
                {
                    case Token.TokenType.LPAR:
                        D1();
                        break;
                    case Token.TokenType.PLUS:
                        D2();
                        break;
                    case Token.TokenType.MINUS:
                        D2();
                        break;
                    case Token.TokenType.MULTIPLICATION:
                        D4();
                        break;
                    case Token.TokenType.DIVISION:
                        D4();
                        break;
                }
            }
        }
        private void Lpar()
        {
            if (T.Count == 0)
                D1();
            else
            {
                switch (T.Peek().Type)
                {
                    case Token.TokenType.LPAR:
                        D1();
                        break;
                    case Token.TokenType.PLUS:
                        D1();
                        break;
                    case Token.TokenType.MINUS:
                        D1();
                        break;
                    case Token.TokenType.MULTIPLICATION:
                        D1();
                        break;
                    case Token.TokenType.DIVISION:
                        D1();
                        break;
                }
            }
        }
        private void EndList()
        {
            if (T.Count == 0)
            {
                return;
            }
            else
            {
                switch (T.Peek().Type)
                {
                    case Token.TokenType.LPAR:
                        D5("лишняя \"(\"");
                        break;
                    case Token.TokenType.PLUS:
                        D4();
                        break;
                    case Token.TokenType.MINUS:
                        D4();
                        break;
                    case Token.TokenType.MULTIPLICATION:
                        D4();
                        break;
                    case Token.TokenType.DIVISION:
                        D4();
                        break;
                }
            }
        }
        private void D1()
        {
            T.Push(GetLexem(lex));
            lex++;
        }
        private void D2()
        {
            K_op();
            T.Push(GetLexem(lex));
            lex++;
        }
        private void D3()
        {
            T.Pop();
            lex++;
        }
        private void D4()
        {
            K_op();
        }
        private void D5(string error)
        {
            throw new AnaliseException($"Ошибка в арифметическом выражении: {error}");
        }
        private void CheckSyntax()
        {
            int current = 0;
            int next = 1;
            
            while(current < tokens.Count - 1)
            {
                if (tokens[current].Type == Token.TokenType.IDENTIFIER ||
                    tokens[current].Type == Token.TokenType.LITERAL)
                {
                    if (tokens[next].Type == Token.TokenType.LPAR)
                        throw new AnaliseException($"Ошибка в арифметическом выражении. Ожидалось: или +, или -, или *, или /, а встретилось: {tokens[next].Value}");
                    else {current++; next++; }
                }
                else if (tokens[current].Type == Token.TokenType.RPAR)
                {
                    if (tokens[next].Type == Token.TokenType.IDENTIFIER ||
                       tokens[next].Type == Token.TokenType.LITERAL ||
                       tokens[next].Type == Token.TokenType.LPAR)
                        throw new AnaliseException($"Ошибка в арифметическом выражении. Ожидалось: или +, или -, или *, или /, а встретилось: {tokens[next].Value}");
                    else { current++; next++; }
                }
                else { current++; next++; }
            }
        }
        private void CheckContains(List<Token> tokens)
        {
            foreach (Token token in tokens)
            {
                if (token.Type != Token.TokenType.LPAR &&
                    token.Type != Token.TokenType.RPAR &&
                    token.Type != Token.TokenType.MINUS &&
                    token.Type != Token.TokenType.PLUS &&
                    token.Type != Token.TokenType.DIVISION &&
                    token.Type != Token.TokenType.MULTIPLICATION &&
                    token.Type != Token.TokenType.IDENTIFIER &&
                    token.Type != Token.TokenType.LITERAL)
                    throw new AnaliseException($"Недопустимый символ в арифмeтическом выражении: {token.Value}");
            }
        }
    }
}

