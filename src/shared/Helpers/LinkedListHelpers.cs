using System;
using System.Collections.Generic;

namespace Shared
{
    public static class LinkedListHelpers
    {
        public static LinkedListNode<TNode> NextOrFirst<TNode>(this LinkedListNode<TNode> node)
        {
            if (node.Next is not null)
            {
                return node.Next;
            }

            if (node.List is null || node.List.First is null)
            {
                throw new InvalidOperationException("Must be linked to a list");
            }

            return node.List.First;
        }

        public static LinkedListNode<TNode> PreviousOrLast<TNode>(this LinkedListNode<TNode> node)
        {
            if (node.Previous is not null)
            {
                return node.Previous;
            }

            if (node.List is null || node.List.Last is null)
            {
                throw new InvalidOperationException("Must be linked to a list");
            }

            return node.List.Last;
        }
    }
}
