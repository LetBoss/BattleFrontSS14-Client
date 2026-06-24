// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.Markdown.NodeMark
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using YamlDotNet.Core;

#nullable enable
namespace Robust.Shared.Serialization.Markdown;

public readonly struct NodeMark(int line, int column) : IEquatable<NodeMark>, IComparable<NodeMark>
{
  public static NodeMark Invalid => new NodeMark(-1, -1);

  public NodeMark(Mark mark)
    : this((int) ((Mark) ref mark).Line, (int) ((Mark) ref mark).Column)
  {
  }

  public int Line { get; init; } = line;

  public int Column { get; init; } = column;

  public override string ToString() => $"Line: {this.Line}, Col: {this.Column}";

  public bool Equals(NodeMark other) => this.Line == other.Line && this.Column == other.Column;

  public override bool Equals(object? obj) => obj is NodeMark other && this.Equals(other);

  public override int GetHashCode() => HashCode.Combine<int, int>(this.Line, this.Column);

  public int CompareTo(NodeMark other)
  {
    int num = this.Line.CompareTo(other.Line);
    return num != 0 ? num : this.Column.CompareTo(other.Column);
  }

  public static implicit operator NodeMark(Mark mark) => new NodeMark(mark);

  public static bool operator ==(NodeMark left, NodeMark right) => left.Equals(right);

  public static bool operator !=(NodeMark left, NodeMark right) => !left.Equals(right);

  public static bool operator <(NodeMark? left, NodeMark? right)
  {
    return left.HasValue && right.HasValue && left.Value.CompareTo(right.Value) < 0;
  }

  public static bool operator >(NodeMark? left, NodeMark? right)
  {
    return left.HasValue && right.HasValue && left.Value.CompareTo(right.Value) > 0;
  }
}
