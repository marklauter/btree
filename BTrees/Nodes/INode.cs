﻿using BTrees.Types;

namespace BTrees.Nodes
{
    internal interface INode<TKey, TValue>
        : IComparable<INode<TKey, TValue>>
        where TKey : ISizeable, IComparable<TKey>
        where TValue : ISizeable, IComparable<TValue>
    {
        int Length { get; }
        bool IsOverflow { get; }
        bool IsUnderflow { get; }
        int Size { get; }
        int MaxSize { get; }
        int HalfSize { get; }

        INode<TKey, TValue>? RightSibling { get; }

        bool HasRightSibling { get; }

        int Count();

        bool ContainsKey(TKey key);

        INode<TKey, TValue> Fork();

        void Merge(INode<TKey, TValue> node);

        /// <summary>
        /// Splits the internal page and returns the right half as a new Node.
        /// </summary>
        /// <returns>Returns new right node with sibling pointers set and the new pivot key is TKeys[0]. </returns>
        INode<TKey, TValue> Split();

        void Remove(TKey key);

        void Remove(TKey key, TValue value);

        void Insert(TKey key, TValue value);

        IEnumerable<TValue> Read(TKey key);

        IEnumerable<(TKey Key, TValue Value)> Read(TKey leftBound, TKey rightBound);
    }
}
