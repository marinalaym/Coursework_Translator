using System;

namespace Translator
{
    public class AnaliseException: Exception
    {
        public AnaliseException(string message) : base(message) { }
        public AnaliseException(string expected, string current) : base($"Ожидалось: {expected}, а встретилось: {current}") { }
    }
}
