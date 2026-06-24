// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.Markdown.Sequence.SequenceDataNode
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Serialization.Markdown.Value;
using System;
using System.Collections;
using System.Collections.Generic;
using YamlDotNet.Core;
using YamlDotNet.RepresentationModel;

#nullable enable
namespace Robust.Shared.Serialization.Markdown.Sequence;

public sealed class SequenceDataNode : 
  DataNode<SequenceDataNode>,
  IList<DataNode>,
  ICollection<DataNode>,
  IEnumerable<DataNode>,
  IEnumerable
{
  private readonly List<DataNode> _nodes;

  public SequenceDataNode()
    : base(NodeMark.Invalid, NodeMark.Invalid)
  {
    this._nodes = new List<DataNode>();
  }

  public SequenceDataNode(int size)
    : base(NodeMark.Invalid, NodeMark.Invalid)
  {
    this._nodes = new List<DataNode>(size);
  }

  public SequenceDataNode(List<DataNode> nodes)
    : base(NodeMark.Invalid, NodeMark.Invalid)
  {
    this._nodes = nodes;
  }

  public SequenceDataNode(List<string> values)
    : base(NodeMark.Invalid, NodeMark.Invalid)
  {
    this._nodes = new List<DataNode>(values.Count);
    foreach (string str in values)
      this._nodes.Add((DataNode) new ValueDataNode(str));
  }

  public SequenceDataNode(YamlSequenceNode sequence)
    : base((NodeMark) ((YamlNode) sequence).Start, (NodeMark) ((YamlNode) sequence).End)
  {
    this._nodes = new List<DataNode>(sequence.Children.Count);
    foreach (YamlNode child in (IEnumerable<YamlNode>) sequence.Children)
      this._nodes.Add(child.ToDataNode());
    TagName tag = ((YamlNode) sequence).Tag;
    string str;
    if (!((TagName) ref tag).IsEmpty)
    {
      tag = ((YamlNode) sequence).Tag;
      str = ((TagName) ref tag).Value;
    }
    else
      str = (string) null;
    this.Tag = str;
  }

  public SequenceDataNode(params DataNode[] nodes)
    : base(NodeMark.Invalid, NodeMark.Invalid)
  {
    this._nodes = new List<DataNode>(nodes.Length);
    foreach (DataNode node in nodes)
      this._nodes.Add(node);
  }

  public SequenceDataNode(params string[] strings)
    : base(NodeMark.Invalid, NodeMark.Invalid)
  {
    this._nodes = new List<DataNode>(strings.Length);
    foreach (string str in strings)
      this._nodes.Add((DataNode) new ValueDataNode(str));
  }

  public YamlSequenceNode ToSequenceNode()
  {
    YamlSequenceNode sequenceNode = new YamlSequenceNode();
    foreach (DataNode node in this._nodes)
      sequenceNode.Children.Add(node.ToYamlNode());
    ((YamlNode) sequenceNode).Tag = TagName.op_Implicit(this.Tag);
    return sequenceNode;
  }

  public IReadOnlyList<DataNode> Sequence => (IReadOnlyList<DataNode>) this._nodes;

  public int IndexOf(DataNode item) => this._nodes.IndexOf(item);

  public void Insert(int index, DataNode item) => this._nodes.Insert(index, item);

  public void RemoveAt(int index) => this._nodes.RemoveAt(index);

  public DataNode this[int index]
  {
    get => this._nodes[index];
    set => this._nodes[index] = value;
  }

  public void Add(DataNode node) => this._nodes.Add(node);

  public void Clear() => this._nodes.Clear();

  public bool Contains(DataNode item) => this._nodes.Contains(item);

  public void CopyTo(DataNode[] array, int arrayIndex) => this._nodes.CopyTo(array, arrayIndex);

  public bool Remove(DataNode node) => this._nodes.Remove(node);

  public int Count => this._nodes.Count;

  public bool IsReadOnly => false;

  public T Cast<T>(int index) where T : DataNode => (T) this[index];

  public override bool IsEmpty => this._nodes.Count == 0;

  public override SequenceDataNode Copy()
  {
    SequenceDataNode sequenceDataNode1 = new SequenceDataNode(this.Sequence.Count);
    sequenceDataNode1.Tag = this.Tag;
    sequenceDataNode1.Start = this.Start;
    sequenceDataNode1.End = this.End;
    SequenceDataNode sequenceDataNode2 = sequenceDataNode1;
    foreach (DataNode dataNode in (IEnumerable<DataNode>) this.Sequence)
      sequenceDataNode2.Add(dataNode.Copy());
    return sequenceDataNode2;
  }

  public List<DataNode>.Enumerator GetEnumerator() => this._nodes.GetEnumerator();

  IEnumerator<DataNode> IEnumerable<DataNode>.GetEnumerator()
  {
    return (IEnumerator<DataNode>) this._nodes.GetEnumerator();
  }

  public override int GetHashCode()
  {
    HashCode hashCode = new HashCode();
    foreach (DataNode node in this._nodes)
      hashCode.Add<DataNode>(node);
    return hashCode.ToHashCode();
  }

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

  public override SequenceDataNode? Except(SequenceDataNode node)
  {
    List<DataNode> nodes = new List<DataNode>();
    foreach (DataNode node1 in this._nodes)
    {
      if (!node._nodes.Contains(node1))
        nodes.Add(node1);
    }
    if (nodes.Count <= 0)
      return (SequenceDataNode) null;
    SequenceDataNode sequenceDataNode = new SequenceDataNode(nodes);
    sequenceDataNode.Tag = this.Tag;
    sequenceDataNode.Start = this.Start;
    sequenceDataNode.End = this.End;
    return sequenceDataNode;
  }

  public override bool Equals(object? obj)
  {
    if (!(obj is SequenceDataNode sequenceDataNode) || this._nodes.Count != sequenceDataNode._nodes.Count)
      return false;
    for (int index = 0; index < this._nodes.Count; ++index)
    {
      if (!this._nodes[index].Equals((object) sequenceDataNode._nodes[index]))
        return false;
    }
    return true;
  }

  [Obsolete("Use SerializationManager.PushComposition()")]
  public override SequenceDataNode PushInheritance(SequenceDataNode node)
  {
    SequenceDataNode sequenceDataNode = this.Copy();
    foreach (DataNode dataNode in node)
      sequenceDataNode.Add(dataNode.Copy());
    return sequenceDataNode;
  }
}
