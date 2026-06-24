using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Robust.Shared.Utility;

public static class Rope
{
	public abstract class Node
	{
		public abstract long Weight { get; }

		public abstract short Depth { get; }
	}

	[DebuggerDisplay("W: {Weight}, Text: {Text}")]
	public sealed class Leaf : Node
	{
		public static readonly Leaf Empty = new Leaf("");

		public string Text { get; }

		public override long Weight => Text.Length;

		public override short Depth => 0;

		public Leaf(string text)
		{
			Text = text;
		}
	}

	[DebuggerDisplay("W: {Weight}")]
	public sealed class Branch : Node
	{
		public Node Left { get; }

		public Node? Right { get; }

		public override long Weight { get; }

		public override short Depth { get; }

		public Branch(Node left, Node? right)
		{
			Left = left;
			Right = right;
			Weight = CalcTotalLength(left);
			Depth = checked((short)(Math.Max(left.Depth, right?.Depth ?? 0) + 1));
		}
	}

	internal static readonly int[] FibonacciSequence = new int[46]
	{
		0, 1, 1, 2, 3, 5, 8, 13, 21, 34,
		55, 89, 233, 377, 610, 987, 1597, 2584, 4181, 6765,
		10946, 17711, 28657, 46368, 75025, 121393, 196418, 317811, 514229, 832040,
		1346269, 2178309, 3524578, 5702887, 9227465, 14930352, 24157817, 39088169, 63245986, 102334155,
		165580141, 267914296, 433494437, 701408733, 1134903170, 1836311903
	};

	public static long CalcTotalLength(Node? node)
	{
		if (!(node is Branch branch))
		{
			if (!(node is Leaf { Weight: var weight }))
			{
				return 0L;
			}
			return weight;
		}
		return branch.Weight + CalcTotalLength(branch.Right);
	}

	public static IEnumerable<Leaf> CollectLeaves(Node node)
	{
		Stack<Branch> stack = new Stack<Branch>();
		yield return RunTillLeaf(stack, node);
		Branch result;
		while (stack.TryPop(out result))
		{
			if (result.Right != null)
			{
				yield return RunTillLeaf(stack, result.Right);
			}
		}
		static Leaf RunTillLeaf(Stack<Branch> stack2, Node left)
		{
			while (left is Branch branch)
			{
				stack2.Push(branch);
				left = branch.Left;
			}
			return (Leaf)left;
		}
	}

	public static IEnumerable<Leaf> CollectLeavesReverse(Node node)
	{
		Stack<Branch> stack = new Stack<Branch>();
		Leaf leaf = RunTillLeaf(stack, node);
		if (leaf != null)
		{
			yield return leaf;
		}
		Branch result;
		while (stack.TryPop(out result))
		{
			leaf = RunTillLeaf(stack, result.Left);
			if (leaf != null)
			{
				yield return leaf;
			}
		}
		static Leaf? RunTillLeaf(Stack<Branch> stack2, Node? right)
		{
			while (right is Branch branch)
			{
				stack2.Push(branch);
				right = branch.Right;
			}
			return (Leaf)right;
		}
	}

	public static IEnumerable<Rune> EnumerateRunes(Node node)
	{
		foreach (Leaf item in CollectLeaves(node))
		{
			foreach (Rune item2 in item.Text.EnumerateRunes())
			{
				yield return item2;
			}
		}
	}

	public static IEnumerable<Rune> EnumerateRunes(Node node, long startPos)
	{
		long pos = 0L;
		IEnumerator<Leaf> leaves = CollectLeaves(node).GetEnumerator();
		while (leaves.MoveNext())
		{
			Leaf current = leaves.Current;
			if (pos + current.Weight < startPos)
			{
				pos += current.Weight;
				continue;
			}
			foreach (Rune rune in leaves.Current.Text.EnumerateRunes())
			{
				if (pos >= startPos)
				{
					yield return rune;
				}
				pos += rune.Utf16SequenceLength;
			}
			while (leaves.MoveNext())
			{
				Leaf current2 = leaves.Current;
				foreach (Rune item in current2.Text.EnumerateRunes())
				{
					yield return item;
				}
			}
			break;
		}
	}

	public static IEnumerable<Rune> EnumerateRunesReverse(Node node)
	{
		foreach (Leaf item in CollectLeavesReverse(node))
		{
			StringEnumerateHelpers.SubstringReverseRuneEnumerator enumerator2 = new StringEnumerateHelpers.SubstringReverseRuneEnumerator(item.Text, item.Text.Length);
			while (enumerator2.MoveNext())
			{
				yield return enumerator2.Current;
			}
		}
	}

	public static IEnumerable<Rune> EnumerateRunesReverse(Node node, long endPos)
	{
		long pos = CalcTotalLength(node);
		foreach (Rune rune in EnumerateRunesReverse(node))
		{
			if (pos <= endPos)
			{
				yield return rune;
			}
			pos -= rune.Utf16SequenceLength;
		}
	}

	public static bool IsBalanced(Node node)
	{
		short depth = node.Depth;
		if (depth > FibonacciSequence.Length - 2)
		{
			return false;
		}
		return FibonacciSequence[depth + 2] <= node.Weight;
	}

	public static Node Rebalance(Node node)
	{
		if (IsBalanced(node))
		{
			return node;
		}
		return Merge(CollectLeaves(node).ToArray());
		static Node Merge(ReadOnlySpan<Leaf> leaves)
		{
			if (leaves.Length == 1)
			{
				return leaves[0];
			}
			if (leaves.Length == 2)
			{
				return new Branch(leaves[0], leaves[1]);
			}
			int num = leaves.Length / 2;
			Node left = Merge(leaves.Slice(0, num));
			int num2 = num;
			return new Branch(left, Merge(leaves.Slice(num2, leaves.Length - num2)));
		}
	}

	public static char Index(Node rope, long index)
	{
		if (!(rope is Branch branch))
		{
			if (rope is Leaf leaf)
			{
				return leaf.Text[(int)index];
			}
			throw new ArgumentOutOfRangeException("rope");
		}
		if (branch.Weight > index)
		{
			return Index(branch.Left, index);
		}
		if (branch.Right == null)
		{
			throw new IndexOutOfRangeException();
		}
		return Index(branch.Right, index - branch.Weight);
	}

	public static Node Insert(Node rope, long index, string value)
	{
		var (left, right) = Split(rope, index);
		return Concat(left, Concat(new Leaf(value), right));
	}

	public static Node Concat(Node left, Node right)
	{
		return new Branch(left, right);
	}

	public static Node Concat(Node left, string right)
	{
		return Concat(left, new Leaf(right));
	}

	public static Node Concat(string left, Node right)
	{
		return Concat(new Leaf(left), right);
	}

	public static (Node left, Node right) Split(Node rope, long index)
	{
		if (!(rope is Branch branch))
		{
			if (rope is Leaf leaf)
			{
				Leaf item = new Leaf(leaf.Text.Substring(0, (int)index));
				string text = leaf.Text;
				int num = (int)index;
				Leaf item2 = new Leaf(text.Substring(num, text.Length - num));
				return (left: item, right: item2);
			}
			throw new ArgumentOutOfRangeException("rope");
		}
		if (branch.Weight > index)
		{
			var (node, left) = Split(branch.Left, index);
			return (left: Rebalance(node), right: Rebalance(new Branch(left, branch.Right)));
		}
		if (branch.Weight < index)
		{
			var (right, node2) = Split(branch.Right ?? Leaf.Empty, index - branch.Weight);
			return (left: Rebalance(new Branch(branch.Left, right)), right: Rebalance(node2));
		}
		return (left: branch.Left, right: branch.Right ?? Leaf.Empty);
	}

	public static Node Delete(Node rope, long start, long length)
	{
		Node item = Split(rope, start).left;
		Node item2 = Split(rope, start + length).right;
		return Concat(item, item2);
	}

	public static Node ReplaceSubstring(Node rope, long start, long length, string text)
	{
		(Node left, Node right) tuple = Split(rope, start);
		Node item = tuple.left;
		Node item2 = Split(tuple.right, length).right;
		return Concat(item, Concat(text, item2));
	}

	public static bool TryGetRuneAt(Node rope, long index, out Rune value)
	{
		char c = Index(rope, index);
		if (!char.IsSurrogate(c))
		{
			value = new Rune(c);
			return true;
		}
		if (char.IsLowSurrogate(c))
		{
			value = default(Rune);
			return false;
		}
		char c2 = Index(rope, index + 1);
		if (!char.IsLowSurrogate(c2))
		{
			value = default(Rune);
			return false;
		}
		value = new Rune(c, c2);
		return true;
	}

	public static string Collapse(Node rope)
	{
		return string.Create(checked((int)CalcTotalLength(rope)), rope, delegate(Span<char> span, Node node)
		{
			foreach (Leaf item in CollectLeaves(node))
			{
				string text = item.Text;
				text.CopyTo(span);
				int length = text.Length;
				span = span.Slice(length, span.Length - length);
			}
		});
	}

	public static string CollapseSubstring(Node rope, Range range)
	{
		string text = Collapse(rope);
		Range range2 = range;
		return text[range2.Start..range2.End];
	}

	public static long RuneShiftLeft(long index, Node rope)
	{
		index--;
		if (char.IsLowSurrogate(Index(rope, index)))
		{
			index--;
		}
		return index;
	}

	public static long RuneShiftRight(long index, Node rope)
	{
		if (char.IsHighSurrogate(Index(rope, index)))
		{
			return index + 2;
		}
		return index + 1;
	}

	public static bool IsNullOrEmpty([NotNullWhen(false)] Node? rope)
	{
		if (rope == null)
		{
			return true;
		}
		return CalcTotalLength(rope) == 0;
	}
}
