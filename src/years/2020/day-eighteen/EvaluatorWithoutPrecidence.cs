using Superpower.Model;

namespace DayEighteen2020
{
    public class EvaluatorWithoutPrecidence : IEvaluator
    {
        private readonly IEnumerator<Token<ArithmeticExpressionToken>> _tokens;

        // Constructor - just store the tokenizer
        public EvaluatorWithoutPrecidence(TokenList<ArithmeticExpressionToken> tokens)
        {
            _tokens = tokens.GetEnumerator();
        }

        public long ParseAndEvaluate()
        {
            return ParseExpression().Eval();
        }

        // Parse an entire expression and check EOF was reached
        public Node ParseExpression()
        {
            _tokens.MoveNext();
            // For the moment, all we understand is add and subtract
            var expr = ParseAddMultiply();

            // Check everything was consumed
            if (_tokens.MoveNext() is true)
                throw new InvalidOperationException("Unexpected characters at end of expression");

            return expr;
        }

        // Parse an sequence of add/subtract operators
        Node ParseAddMultiply()
        {
            // Parse the left hand side
            var lhs = ParseLeaf();

            while (true)
            {
                var current = _tokens.Current;

                // Work out the operator
                Func<long, long, long>? op = null;
                if (current.Kind == ArithmeticExpressionToken.Plus)
                {
                    op = (a, b) => a + b;
                }
                else if (current.Kind == ArithmeticExpressionToken.Times)
                {
                    op = (a, b) => a * b;
                }

                // Binary operator found?
                if (op == null)
                    return lhs;

                // Skip the operator
                _tokens.MoveNext();

                // Parse the right hand side of the expression
                var rhs = ParseLeaf();

                // Create a binary node and use it as the left-hand side from now on
                lhs = new BinaryNode(lhs, rhs, op);
            }
        }

        // Parse a leaf node
        // (For the moment this is just a number)
        Node ParseLeaf()
        {
            // Parenthesis?
            if (_tokens.Current.Kind == ArithmeticExpressionToken.LParen)
            {
                _tokens.MoveNext();

                // Parse a top-level expression
                var node = ParseAddMultiply();

                // Check and skip ')'
                if (_tokens.Current.Kind != ArithmeticExpressionToken.RParen)
                    throw new InvalidOperationException("Missing close parenthesis");

                _tokens.MoveNext();

                // Return
                return node;
            }

            if (_tokens.Current.Kind == ArithmeticExpressionToken.Number)
            {
                var node = new NumberNode(long.Parse(_tokens.Current.Span.ToStringValue()));
                _tokens.MoveNext();
                return node;
            }

            // Don't Understand
            throw new InvalidOperationException();
        }
    }
}
