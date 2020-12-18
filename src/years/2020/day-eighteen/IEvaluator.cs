using System;

namespace DayEighteen2020
{
    public interface IEvaluator
    {
        long ParseAndEvaluate();
    }

    public enum ArithmeticExpressionToken
    {
        None,
        Number,
        Plus,
        Times,
        LParen,
        RParen,
    }

    public abstract record Node { public abstract long Eval(); }

    public record NumberNode(long Number) : Node { public override long Eval() => Number; };

    public record BinaryNode(Node Lhs, Node Rhs, Func<long, long, long> Op) : Node
    {
        public override long Eval()
        {
            var lhs = Lhs.Eval();
            var rhs = Rhs.Eval();

            return Op(lhs, rhs);
        }
    }
}
