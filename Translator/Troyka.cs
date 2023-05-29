using System;

namespace Translator
{
    public class Troyka
    {
        public Token operand1;
        public Token operand2;
        public Token operat;
        public Troyka(Token operat, Token opd2, Token opd1)
        {
            this.operat = operat;
            operand2 = opd2;
            operand1 = opd1;
        }
    }
}
