// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.Rope
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

#nullable enable
namespace Robust.Shared.Utility;

public static class Rope
{
  internal static readonly int[] FibonacciSequence = new int[46]
  {
    0,
    1,
    1,
    2,
    3,
    5,
    8,
    13,
    21,
    34,
    55,
    89,
    233,
    377,
    610,
    987,
    1597,
    2584,
    4181,
    6765,
    10946,
    17711,
    28657,
    46368,
    75025,
    121393,
    196418,
    317811,
    514229,
    832040,
    1346269,
    2178309,
    3524578,
    5702887,
    9227465,
    14930352,
    24157817,
    39088169,
    63245986,
    102334155,
    165580141,
    267914296,
    433494437,
    701408733,
    1134903170,
    1836311903
  };

  public static long CalcTotalLength(Rope.Node? node)
  {
    long num;
    switch (node)
    {
      case Rope.Branch branch:
        num = branch.Weight + Rope.CalcTotalLength(branch.Right);
        break;
      case Rope.Leaf leaf:
        num = leaf.Weight;
        break;
      default:
        num = 0L;
        break;
    }
    return num;
  }

  public static IEnumerable<Rope.Leaf> CollectLeaves(Rope.Node node)
  {
    Stack<Rope.Branch> stack = new Stack<Rope.Branch>();
    yield return RunTillLeaf(stack, node);
    Rope.Branch result;
    while (stack.TryPop(out result))
    {
      if (result.Right != null)
        yield return RunTillLeaf(stack, result.Right);
    }

    static Rope.Leaf RunTillLeaf(Stack<Rope.Branch> stack, Rope.Node node)
    {
      for (; node is Rope.Branch branch; node = branch.Left)
        stack.Push(branch);
      return (Rope.Leaf) node;
    }
  }

  public static IEnumerable<Rope.Leaf> CollectLeavesReverse(Rope.Node node)
  {
    Stack<Rope.Branch> stack = new Stack<Rope.Branch>();
    Rope.Leaf leaf1 = RunTillLeaf(stack, node);
    if (leaf1 != null)
      yield return leaf1;
    Rope.Branch result;
    while (stack.TryPop(out result))
    {
      Rope.Leaf leaf2 = RunTillLeaf(stack, result.Left);
      if (leaf2 != null)
        yield return leaf2;
    }

    static Rope.Leaf? RunTillLeaf(Stack<Rope.Branch> stack, Rope.Node? node)
    {
      for (; node is Rope.Branch branch; node = branch.Right)
        stack.Push(branch);
      return (Rope.Leaf) node;
    }
  }

  public static IEnumerable<Rune> EnumerateRunes(Rope.Node node)
  {
    foreach (Rope.Leaf collectLeaf in Rope.CollectLeaves(node))
    {
      StringRuneEnumerator stringRuneEnumerator = collectLeaf.Text.EnumerateRunes().GetEnumerator();
      while (stringRuneEnumerator.MoveNext())
        yield return stringRuneEnumerator.Current;
      stringRuneEnumerator = new StringRuneEnumerator();
    }
  }

  public static IEnumerable<Rune> EnumerateRunes(Rope.Node node, long startPos)
  {
    long pos = 0;
    IEnumerator<Rope.Leaf> leaves = Rope.CollectLeaves(node).GetEnumerator();
    while (leaves.MoveNext())
    {
      Rope.Leaf current = leaves.Current;
      if (pos + current.Weight < startPos)
      {
        pos += current.Weight;
      }
      else
      {
        StringRuneEnumerator stringRuneEnumerator = leaves.Current.Text.EnumerateRunes();
        foreach (Rune rune in stringRuneEnumerator)
        {
          if (pos >= startPos)
            yield return rune;
          pos += (long) rune.Utf16SequenceLength;
        }
        while (leaves.MoveNext())
        {
          stringRuneEnumerator = leaves.Current.Text.EnumerateRunes();
          foreach (Rune rune in stringRuneEnumerator)
            yield return rune;
        }
        break;
      }
    }
  }

  public static IEnumerable<Rune> EnumerateRunesReverse(Rope.Node node)
  {
    foreach (Rope.Leaf leaf in Rope.CollectLeavesReverse(node))
    {
      StringEnumerateHelpers.SubstringReverseRuneEnumerator enumerator = new StringEnumerateHelpers.SubstringReverseRuneEnumerator(leaf.Text, leaf.Text.Length);
      while (enumerator.MoveNext())
        yield return enumerator.Current;
      enumerator = new StringEnumerateHelpers.SubstringReverseRuneEnumerator();
    }
  }

  public static IEnumerable<Rune> EnumerateRunesReverse(Rope.Node node, long endPos)
  {
    long pos = Rope.CalcTotalLength(node);
    foreach (Rune rune in Rope.EnumerateRunesReverse(node))
    {
      if (pos <= endPos)
        yield return rune;
      pos -= (long) rune.Utf16SequenceLength;
    }
  }

  public static bool IsBalanced(Rope.Node node)
  {
    short depth = node.Depth;
    return (int) depth <= Rope.FibonacciSequence.Length - 2 && (long) Rope.FibonacciSequence[(int) depth + 2] <= node.Weight;
  }

  public static Rope.Node Rebalance(Rope.Node node)
  {
    return Rope.IsBalanced(node) ? node : Merge((ReadOnlySpan<Rope.Leaf>) Rope.CollectLeaves(node).ToArray<Rope.Leaf>());

    static Rope.Node Merge(ReadOnlySpan<Rope.Leaf> leaves)
    {
      if (leaves.Length == 1)
        return (Rope.Node) leaves[0];
      if (leaves.Length == 2)
        return (Rope.Node) new Rope.Branch((Rope.Node) leaves[0], (Rope.Node) leaves[1]);
      int length = leaves.Length / 2;
      Rope.Node left = Merge(leaves.Slice(0, length));
      ref ReadOnlySpan<Rope.Leaf> local = ref leaves;
      int start = length;
      Rope.Node right = Merge(local.Slice(start, local.Length - start));
      return (Rope.Node) new Rope.Branch(left, right);
    }
  }

  public static char Index(Rope.Node rope, long index)
  {
    switch (rope)
    {
      case Rope.Branch branch:
        if (branch.Weight > index)
          return Rope.Index(branch.Left, index);
        if (branch.Right == null)
          throw new IndexOutOfRangeException();
        return Rope.Index(branch.Right, index - branch.Weight);
      case Rope.Leaf leaf:
        return leaf.Text[(int) index];
      default:
        throw new ArgumentOutOfRangeException(nameof (rope));
    }
  }

  public static Rope.Node Insert(Rope.Node rope, long index, string value)
  {
    (Rope.Node left, Rope.Node right) = Rope.Split(rope, index);
    return Rope.Concat(left, Rope.Concat((Rope.Node) new Rope.Leaf(value), right));
  }

  public static Rope.Node Concat(Rope.Node left, Rope.Node right)
  {
    return (Rope.Node) new Rope.Branch(left, right);
  }

  public static Rope.Node Concat(Rope.Node left, string right)
  {
    return Rope.Concat(left, (Rope.Node) new Rope.Leaf(right));
  }

  public static Rope.Node Concat(string left, Rope.Node right)
  {
    return Rope.Concat((Rope.Node) new Rope.Leaf(left), right);
  }

  public static (Rope.Node left, Rope.Node right) Split(Rope.Node rope, long index)
  {
    switch (rope)
    {
      case Rope.Branch branch:
        if (branch.Weight > index)
        {
          (Rope.Node node1, Rope.Node node2) = Rope.Split(branch.Left, index);
          return (Rope.Rebalance(node1), Rope.Rebalance((Rope.Node) new Rope.Branch(node2, branch.Right)));
        }
        if (branch.Weight >= index)
          return (branch.Left, branch.Right ?? (Rope.Node) Rope.Leaf.Empty);
        (Rope.Node node3, Rope.Node node4) = Rope.Split(branch.Right ?? (Rope.Node) Rope.Leaf.Empty, index - branch.Weight);
        return (Rope.Rebalance((Rope.Node) new Rope.Branch(branch.Left, node3)), Rope.Rebalance(node4));
      case Rope.Leaf leaf3:
        Rope.Leaf leaf1 = new Rope.Leaf(leaf3.Text.Substring(0, (int) index));
        string text = leaf3.Text;
        int startIndex = (int) index;
        Rope.Leaf leaf2 = new Rope.Leaf(text.Substring(startIndex, text.Length - startIndex));
        return ((Rope.Node) leaf1, (Rope.Node) leaf2);
      default:
        throw new ArgumentOutOfRangeException(nameof (rope));
    }
  }

  public static Rope.Node Delete(Rope.Node rope, long start, long length)
  {
    return Rope.Concat(Rope.Split(rope, start).left, Rope.Split(rope, start + length).right);
  }

  public static Rope.Node ReplaceSubstring(Rope.Node rope, long start, long length, string text)
  {
    (Rope.Node left, Rope.Node right) tuple = Rope.Split(rope, start);
    Rope.Node left = tuple.left;
    Rope.Node right = Rope.Split(tuple.right, length).right;
    return Rope.Concat(left, Rope.Concat(text, right));
  }

  public static bool TryGetRuneAt(Rope.Node rope, long index, out Rune value)
  {
    char ch1 = Rope.Index(rope, index);
    if (!char.IsSurrogate(ch1))
    {
      value = new Rune(ch1);
      return true;
    }
    if (char.IsLowSurrogate(ch1))
    {
      value = new Rune();
      return false;
    }
    char ch2 = Rope.Index(rope, index + 1L);
    if (!char.IsLowSurrogate(ch2))
    {
      value = new Rune();
      return false;
    }
    value = new Rune(ch1, ch2);
    return true;
  }

  public static string Collapse(Rope.Node rope)
  {
    return string.Create<Rope.Node>(checked ((int) Rope.CalcTotalLength(rope)), rope, (SpanAction<char, Rope.Node>) ((span, node) =>
    {
      foreach (Rope.Leaf collectLeaf in Rope.CollectLeaves(node))
      {
        string text = collectLeaf.Text;
        text.CopyTo(span);
        ref Span<char> local = ref span;
        int length = text.Length;
        span = local.Slice(length, local.Length - length);
      }
    }));
  }

  public static string CollapseSubstring(Rope.Node rope, Range range)
  {
    string str = Rope.Collapse(rope);
    Range range1 = range;
    int length1 = str.Length;
    int offset = range1.Start.GetOffset(length1);
    int length2 = range1.End.GetOffset(length1) - offset;
    return str.Substring(offset, length2);
  }

  public static long RuneShiftLeft(long index, Rope.Node rope)
  {
    --index;
    if (char.IsLowSurrogate(Rope.Index(rope, index)))
      --index;
    return index;
  }

  public static long RuneShiftRight(long index, Rope.Node rope)
  {
    return char.IsHighSurrogate(Rope.Index(rope, index)) ? index + 2L : index + 1L;
  }

  public static bool IsNullOrEmpty([NotNullWhen(false)] Rope.Node? rope)
  {
    return rope == null || Rope.CalcTotalLength(rope) == 0L;
  }

  public abstract class Node
  {
    public abstract long Weight { get; }

    public abstract short Depth { get; }
  }

  [DebuggerDisplay("W: {Weight}, Text: {Text}")]
  public sealed class Leaf : Rope.Node
  {
    public static readonly Rope.Leaf Empty = new Rope.Leaf("");

    public string Text { get; }

    public Leaf(string text) => this.Text = text;

    public override long Weight => (long) this.Text.Length;

    public override short Depth => 0;
  }

  [DebuggerDisplay("W: {Weight}")]
  public sealed class Branch : Rope.Node
  {
    public Rope.Node Left { get; }

    public Rope.Node? Right { get; }

    public override long Weight { get; }

    public override short Depth { get; }

    public Branch(Rope.Node left, Rope.Node? right)
    {
      this.Left = left;
      this.Right = right;
      this.Weight = Rope.CalcTotalLength(left);
      this.Depth = checked ((short) ((int) Math.Max(left.Depth, unchecked (right != null) ? right.Depth : (short) 0) + 1));
    }
  }
}
