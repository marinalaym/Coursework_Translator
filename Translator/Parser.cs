using System;
using System.Collections.Generic;

namespace Translator
{
    public class Parser
    {
        private List<Token> tokens;
        private int i = 0;
        private List<string> strExpr = new List<string>();
        public List<string> stringsAnalyzedExpr = new List<string>();

        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
        }

        public void Program()
        {
            CheckBrace(tokens);
            if (tokens[i].Type != Token.TokenType.VOID) throw new AnaliseException("void", tokens[i].Value);
            i++;
            if (tokens[i].Type != Token.TokenType.MAIN) throw new AnaliseException("main", tokens[i].Value);
            i++;
            if (tokens[i].Type != Token.TokenType.LPAR) throw new AnaliseException("(", tokens[i].Value);
            i++;
            if (tokens[i].Type != Token.TokenType.RPAR) throw new AnaliseException(")", tokens[i].Value);
            i++;
            if (tokens[i].Type != Token.TokenType.LBRACE) throw new AnaliseException("{", tokens[i].Value);
            i++;
            OperatorList();
            if (tokens[i].Type != Token.TokenType.RBRACE) throw new AnaliseException("}", tokens[i].Value);
            if (i != tokens.Count - 1) throw new Exception("В конце кода лишний символ.");
        }
        private void OperatorList()
        {
            Operator();
            i++;
            AdditionalOperatorList();
        }
        private void AdditionalOperatorList()
        {
            if (tokens[i].Type == Token.TokenType.INT ||
                tokens[i].Type == Token.TokenType.FLOAT ||
                tokens[i].Type == Token.TokenType.DOUBLE ||
                tokens[i].Type == Token.TokenType.IDENTIFIER ||
                tokens[i].Type == Token.TokenType.WHILE)
                OperatorList();
            else if (tokens[i].Type == Token.TokenType.RBRACE)
                return;
            else
                throw new AnaliseException("или int, или float, или double, или идентификатор, или while, или }", tokens[i].Value);
        }
        private void Operator()
        {
            if (tokens[i].Type == Token.TokenType.INT ||
                tokens[i].Type == Token.TokenType.FLOAT ||
                tokens[i].Type == Token.TokenType.DOUBLE)
            {
                Declaration();
            }
            else if (tokens[i].Type == Token.TokenType.IDENTIFIER)
            {
                strExpr.Clear();
                Assignment();
            }
            else if (tokens[i].Type == Token.TokenType.WHILE)
            {
                Cycle();
            }
            else throw new AnaliseException("или int, или float, или double, или идентификатор, или while", tokens[i].Value);
        }
        private void Declaration()
        {
            Type();
            i++;
            VariablesList();
        }
        private void Type()
        {
            if (tokens[i].Type != Token.TokenType.INT &&
                tokens[i].Type != Token.TokenType.FLOAT &&
                tokens[i].Type != Token.TokenType.DOUBLE)
            {
                throw new AnaliseException("или int, или float, или double", tokens[i].Value);
            }
        }
        private void VariablesList()
        {
            if (tokens[i].Type != Token.TokenType.IDENTIFIER) throw new AnaliseException("идентификатор", tokens[i].Value);
            else
            {
                i++;
                if (tokens[i].Type == Token.TokenType.SEMICOLON ||
                    tokens[i].Type == Token.TokenType.COMMA)
                {
                    AdditionalVariablesList();
                }
                else if (tokens[i].Type == Token.TokenType.EQUALS)
                {
                    i--;
                    strExpr.Clear();
                    Assignment();
                    AdditionalVariablesList();
                }
                else throw new AnaliseException("или \";\", или \",\", или =", tokens[i].Value);
            }
        }
        private void AdditionalVariablesList()
        {
            if (tokens[i].Type == Token.TokenType.COMMA)
            {
                i++;
                VariablesList();
            }
        }
        private void Assignment()
        {
            if (tokens[i].Type != Token.TokenType.IDENTIFIER) throw new AnaliseException("идентификатор", tokens[i].Value);
            strExpr.Add(tokens[i].Value);
            i++;
            if (tokens[i].Type != Token.TokenType.EQUALS) throw new AnaliseException("=", tokens[i].Value); ;
            strExpr.Add(tokens[i].Value);   
            i++;
            if (tokens[i].Type == Token.TokenType.IDENTIFIER ||
                tokens[i].Type == Token.TokenType.LITERAL ||
                tokens[i].Type == Token.TokenType.LPAR)
                Expr();
            else
                throw new AnaliseException("или литерал, или идентификатор, или (", tokens[i].Value);
        }
        private void Expr()
        {
            List<Token> expr = new List<Token>();
            while (tokens[i].Type != Token.TokenType.SEMICOLON &&
                tokens[i].Type != Token.TokenType.COMMA)
            {
                strExpr.Add(tokens[i].Value);
                expr.Add(tokens[i]);
                i++;
                if (i == tokens.Count-1 &&
                    tokens[i].Type != Token.TokenType.SEMICOLON &&
                tokens[i].Type != Token.TokenType.COMMA)
                    throw new AnaliseException("Отсутствует ; или , после арифметического выражения");
            }
            Bauer_Zamelzon bauer_Zamelzon = new Bauer_Zamelzon(expr);
            bauer_Zamelzon.Start();
            ExprAndTroykas(bauer_Zamelzon.strings);
        }
        private void Cycle()
        {
            if (tokens[i].Type != Token.TokenType.WHILE) throw new AnaliseException("while", tokens[i].Value);
            i++;
            if (tokens[i].Type != Token.TokenType.LPAR) throw new AnaliseException("(", tokens[i].Value);
            i++;
            BooleanExpression();
            if (tokens[i].Type != Token.TokenType.RPAR) throw new AnaliseException(")", tokens[i].Value); ;
            i++;
            CycleBody();
        }
        private void CycleBody()
        {
            if (tokens[i].Type == Token.TokenType.INT ||
               tokens[i].Type == Token.TokenType.FLOAT ||
               tokens[i].Type == Token.TokenType.DOUBLE ||
               tokens[i].Type == Token.TokenType.IDENTIFIER ||
               tokens[i].Type == Token.TokenType.WHILE)
            {
                Operator();
            }
            else if (tokens[i].Type == Token.TokenType.LBRACE)
            {
                i++;
                OperatorList();
                if (tokens[i].Type != Token.TokenType.RBRACE) throw new AnaliseException("}", tokens[i].Value); 
            } 
            else throw new AnaliseException("или int, или float, или double, или идентификатор, или while, или {", tokens[i].Value);
        }
        private void BooleanExpression()
        {
            Operand();
            Sign();
            i++;
            Operand();
        }
        private void Operand()
        {
            if (tokens[i].Type != Token.TokenType.IDENTIFIER &&
                tokens[i].Type != Token.TokenType.LITERAL)
            {
                throw new AnaliseException("или литерал, или идентификатор", tokens[i].Value);
            }
            else i++;
        }
        private void Sign()
        {
            if (tokens[i].Type == Token.TokenType.GREATER ||
                tokens[i].Type == Token.TokenType.LESS)
            {
                i++;
                switch (tokens[i].Type)
                {
                    case Token.TokenType.IDENTIFIER:
                        i--;
                        break;
                    case Token.TokenType.LITERAL:
                        i--;
                        break;
                    case Token.TokenType.EQUALS:
                        break;
                    default:
                        throw new AnaliseException("или литерал, или идентификатор, или =", tokens[i].Value);
                }
            }
            else if (tokens[i].Type == Token.TokenType.EQUALS ||
                tokens[i].Type == Token.TokenType.EXCLAM)
            {
                i++;
                if (tokens[i].Type != Token.TokenType.EQUALS)
                    throw new AnaliseException("=", tokens[i].Value);
            }
            else
                throw new AnaliseException("или =, или !", tokens[i].Value);
        }
        private void ExprAndTroykas(List<string> troykas)
        {
            string str = String.Join("", strExpr);
            stringsAnalyzedExpr.Add(str);
            foreach (string s in troykas)
            {
                stringsAnalyzedExpr.Add(s);
            }
            stringsAnalyzedExpr.Add("");
        }

        private void CheckBrace(List<Token> tokensForCheck)
        {
            int CountLBrace = 0;
            int CountRBrace = 0;
            int j = 0;
            while (j < tokensForCheck.Count)
            {
                if (tokensForCheck[j].Type == Token.TokenType.LBRACE)
                {
                    CountLBrace++;
                }
                else if (tokensForCheck[j].Type == Token.TokenType.RBRACE)
                {
                    CountRBrace++;
                }
                j++;
            }
            if (CountRBrace < CountLBrace)
            {
                throw new AnaliseException("Не хватает } в конце кода");
            }
        }
    }
}
