// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.Markdown.Mapping.MappingDataNode
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using YamlDotNet.Core;
using YamlDotNet.RepresentationModel;

#nullable enable
namespace Robust.Shared.Serialization.Markdown.Mapping;

public sealed class MappingDataNode : 
  DataNode<MappingDataNode>,
  IDictionary<string, DataNode>,
  ICollection<KeyValuePair<string, DataNode>>,
  IEnumerable<KeyValuePair<string, DataNode>>,
  IEnumerable
{
  private readonly Dictionary<string, DataNode> _children;
  private readonly List<KeyValuePair<string, DataNode>> _list;
  private IReadOnlyDictionary<string, ValueDataNode>? _keyNodes;

  public override bool IsEmpty => this._children.Count == 0;

  public int Count => this._children.Count;

  public bool IsReadOnly => false;

  public IReadOnlyDictionary<string, DataNode> Children
  {
    get => (IReadOnlyDictionary<string, DataNode>) this._children;
  }

  public MappingDataNode()
    : base(NodeMark.Invalid, NodeMark.Invalid)
  {
    this._children = new Dictionary<string, DataNode>();
    this._list = new List<KeyValuePair<string, DataNode>>();
  }

  public MappingDataNode(int size)
    : base(NodeMark.Invalid, NodeMark.Invalid)
  {
    this._children = new Dictionary<string, DataNode>(size);
    this._list = new List<KeyValuePair<string, DataNode>>(size);
  }

  public MappingDataNode(YamlMappingNode mapping)
    : base((NodeMark) ((YamlNode) mapping).Start, (NodeMark) ((YamlNode) mapping).End)
  {
    this._children = new Dictionary<string, DataNode>(((ICollection<KeyValuePair<YamlNode, YamlNode>>) mapping.Children).Count);
    this._list = new List<KeyValuePair<string, DataNode>>(((ICollection<KeyValuePair<YamlNode, YamlNode>>) mapping.Children).Count);
    Dictionary<string, ValueDataNode> dictionary = new Dictionary<string, ValueDataNode>(((ICollection<KeyValuePair<YamlNode, YamlNode>>) mapping.Children).Count);
    foreach ((YamlNode key, YamlNode node1) in (IEnumerable<KeyValuePair<YamlNode, YamlNode>>) mapping.Children)
    {
      ValueDataNode valueDataNode = key is YamlScalarNode node2 ? new ValueDataNode(node2) : throw new NotSupportedException("Mapping data nodes must have a scalar keys");
      this.Add(valueDataNode.Value, node1.ToDataNode());
      dictionary.Add(valueDataNode.Value, valueDataNode);
    }
    this._keyNodes = (IReadOnlyDictionary<string, ValueDataNode>) dictionary;
    TagName tag = ((YamlNode) mapping).Tag;
    string str;
    if (!((TagName) ref tag).IsEmpty)
    {
      tag = ((YamlNode) mapping).Tag;
      str = ((TagName) ref tag).Value;
    }
    else
      str = (string) null;
    this.Tag = str;
  }

  public MappingDataNode(Dictionary<string, DataNode> nodes)
    : base(NodeMark.Invalid, NodeMark.Invalid)
  {
    this._children = new Dictionary<string, DataNode>((IDictionary<string, DataNode>) nodes);
    this._list = new List<KeyValuePair<string, DataNode>>((IEnumerable<KeyValuePair<string, DataNode>>) this._children);
  }

  public KeyValuePair<string, DataNode> this[int key] => this._list[key];

  public MappingDataNode Add(string key, DataNode node)
  {
    this._children.Add(key, node);
    this._list.Add(new KeyValuePair<string, DataNode>(key, node));
    return this;
  }

  public DataNode this[string key]
  {
    get => this._children[key];
    set
    {
      if (this._children.TryAdd(key, value))
      {
        this._list.Add(new KeyValuePair<string, DataNode>(key, value));
      }
      else
      {
        int index = this.IndexOf(key);
        if (index == -1)
          throw new Exception("Key exists in Children, but not list?");
        this._list[index] = new KeyValuePair<string, DataNode>(key, value);
        this._children[key] = value;
      }
    }
  }

  public int IndexOf(string key)
  {
    for (int index = 0; index < this._list.Count; ++index)
    {
      if (this._list[index].Key == key)
        return index;
    }
    return -1;
  }

  void IDictionary<string, DataNode>.Add(string key, DataNode value) => this.Add(key, value);

  public bool ContainsKey(string key) => this._children.ContainsKey(key);

  bool IDictionary<string, DataNode>.Remove(string key)
  {
    return ((IDictionary<string, DataNode>) this).Remove(key);
  }

  public bool TryGetValue(string key, [NotNullWhen(true)] out DataNode? value)
  {
    return this.TryGet(key, out value);
  }

  public ICollection<string> Keys
  {
    get
    {
      return (ICollection<string>) this._list.Select<KeyValuePair<string, DataNode>, string>((Func<KeyValuePair<string, DataNode>, string>) (x => x.Key)).ToArray<string>();
    }
  }

  public ICollection<DataNode> Values
  {
    get
    {
      return (ICollection<DataNode>) this._list.Select<KeyValuePair<string, DataNode>, DataNode>((Func<KeyValuePair<string, DataNode>, DataNode>) (x => x.Value)).ToArray<DataNode>();
    }
  }

  public DataNode Get(string key) => this._children[key];

  public T Get<T>(string key) where T : DataNode => (T) this.Get(key);

  public bool TryGet(string key, [NotNullWhen(true)] out DataNode? node)
  {
    return this._children.TryGetValue(key, out node);
  }

  public bool TryGet<T>(string key, [NotNullWhen(true)] out T? node) where T : DataNode
  {
    node = default (T);
    DataNode node1;
    if (!this.TryGet(key, out node1) || !(node1 is T obj))
      return false;
    node = obj;
    return true;
  }

  public bool Has(string key) => this._children.ContainsKey(key);

  public bool Remove(string key)
  {
    if (!this._children.Remove(key))
      return false;
    int index = this.IndexOf(key);
    if (index == -1)
      throw new Exception("Key exists in Children, but not list?");
    this._list.RemoveAt(index);
    return true;
  }

  public T Cast<T>(string key) where T : DataNode => (T) this[key];

  public YamlMappingNode ToYaml()
  {
    YamlMappingNode yaml = new YamlMappingNode();
    foreach ((string key, DataNode node) in this._list)
    {
      ValueDataNode valueDataNode;
      YamlScalarNode yamlScalarNode;
      if (this._keyNodes != null && this._keyNodes.TryGetValue(key, out valueDataNode))
        yamlScalarNode = (YamlScalarNode) valueDataNode;
      else
        yamlScalarNode = new YamlScalarNode(key)
        {
          Style = ValueDataNode.IsNullLiteral(key) || string.IsNullOrWhiteSpace(key) ? (ScalarStyle) 3 : (ScalarStyle) 0
        };
      yaml.Add((YamlNode) yamlScalarNode, node.ToYamlNode());
    }
    ((YamlNode) yaml).Tag = TagName.op_Implicit(this.Tag);
    return yaml;
  }

  public ValueDataNode GetKeyNode(string key)
  {
    IReadOnlyDictionary<string, ValueDataNode> keyNodes = this._keyNodes;
    return (keyNodes != null ? keyNodes.GetValueOrDefault<string, ValueDataNode>(key) : (ValueDataNode) null) ?? new ValueDataNode(key);
  }

  public MappingDataNode Merge(MappingDataNode otherMapping)
  {
    MappingDataNode mappingDataNode = this.Copy();
    mappingDataNode.Insert(otherMapping);
    mappingDataNode.Tag = this.Tag;
    mappingDataNode.Start = this.Start;
    mappingDataNode.End = this.End;
    return mappingDataNode;
  }

  public void Insert(MappingDataNode otherMapping, bool skipDuplicates = false)
  {
    foreach ((string key, DataNode dataNode) in (IEnumerable<KeyValuePair<string, DataNode>>) otherMapping.Children)
    {
      if (!skipDuplicates || !this.Has(key))
        this.Add(key, dataNode.Copy());
    }
  }

  public void InsertAt(int index, string key, DataNode value)
  {
    if (index > this._list.Count || index < 0)
      throw new ArgumentOutOfRangeException();
    if (!this._children.TryAdd(key, value))
      throw new InvalidOperationException("Already contains key " + key);
    this._list.Insert(index, new KeyValuePair<string, DataNode>(key, value));
  }

  public override MappingDataNode Copy()
  {
    MappingDataNode mappingDataNode1 = new MappingDataNode(this._children.Count);
    mappingDataNode1.Tag = this.Tag;
    mappingDataNode1.Start = this.Start;
    mappingDataNode1.End = this.End;
    MappingDataNode mappingDataNode2 = mappingDataNode1;
    foreach ((string key, DataNode dataNode) in this._list)
      mappingDataNode2.Add(key, dataNode.Copy());
    mappingDataNode2._keyNodes = this._keyNodes;
    return mappingDataNode2;
  }

  public MappingDataNode ShallowClone()
  {
    MappingDataNode mappingDataNode1 = new MappingDataNode(this._children.Count);
    mappingDataNode1.Tag = this.Tag;
    mappingDataNode1.Start = this.Start;
    mappingDataNode1.End = this.End;
    MappingDataNode mappingDataNode2 = mappingDataNode1;
    foreach ((string key, DataNode node) in this._list)
      mappingDataNode2.Add(key, node);
    return mappingDataNode2;
  }

  public MappingDataNode? RecursiveExcept(MappingDataNode node)
  {
    MappingDataNode mappingDataNode1 = new MappingDataNode();
    mappingDataNode1.Tag = this.Tag;
    mappingDataNode1.Start = this.Start;
    mappingDataNode1.End = this.End;
    MappingDataNode mappingDataNode2 = mappingDataNode1;
    foreach ((string key, DataNode dataNode) in this._list)
    {
      DataNode node1;
      if (!node._children.TryGetValue(key, out node1))
      {
        mappingDataNode2.Add(key, dataNode.Copy());
      }
      else
      {
        DataNode node2 = dataNode.Except(node1);
        if (node2 != null)
          mappingDataNode2.Add(key, node2);
      }
    }
    return mappingDataNode2._children.Count != 0 ? mappingDataNode2 : (MappingDataNode) null;
  }

  public override MappingDataNode? Except(MappingDataNode node)
  {
    MappingDataNode mappingDataNode1 = new MappingDataNode();
    mappingDataNode1.Tag = this.Tag;
    mappingDataNode1.Start = this.Start;
    mappingDataNode1.End = this.End;
    MappingDataNode mappingDataNode2 = mappingDataNode1;
    foreach ((string key, DataNode dataNode1) in this._list)
    {
      DataNode dataNode2;
      if (!node._children.TryGetValue(key, out dataNode2) || !dataNode1.Equals((object) dataNode2))
        mappingDataNode2.Add(key, dataNode1.Copy());
    }
    return mappingDataNode2._children.Count != 0 ? mappingDataNode2 : (MappingDataNode) null;
  }

  public bool AnyExcept(MappingDataNode node)
  {
    foreach (KeyValuePair<string, DataNode> keyValuePair1 in this._list)
    {
      KeyValuePair<string, DataNode> keyValuePair2 = keyValuePair1;
      (string key, DataNode dataNode1) = keyValuePair2;
      KeyValuePair<string, DataNode>? nullable = node._list.FirstOrNull<KeyValuePair<string, DataNode>>((Func<KeyValuePair<string, DataNode>, bool>) (p => p.Key.Equals(key)));
      if (!nullable.HasValue)
        return true;
      DataNode dataNode2 = dataNode1;
      keyValuePair2 = nullable.Value;
      DataNode dataNode3 = keyValuePair2.Value;
      if (!dataNode2.Equals((object) dataNode3))
        return true;
    }
    return false;
  }

  public override bool Equals(object? obj)
  {
    if (!(obj is MappingDataNode mappingDataNode) || this._children.Count != mappingDataNode._children.Count || this.Tag != mappingDataNode.Tag)
      return false;
    foreach ((string key, DataNode dataNode1) in mappingDataNode)
    {
      DataNode dataNode2;
      if (!this._children.TryGetValue(key, out dataNode2) || !dataNode1.Equals((object) dataNode2))
        return false;
    }
    return true;
  }

  public List<KeyValuePair<string, DataNode>>.Enumerator GetEnumerator()
  {
    return this._list.GetEnumerator();
  }

  IEnumerator<KeyValuePair<string, DataNode>> IEnumerable<KeyValuePair<string, DataNode>>.GetEnumerator()
  {
    return (IEnumerator<KeyValuePair<string, DataNode>>) this._list.GetEnumerator();
  }

  public override int GetHashCode()
  {
    HashCode hashCode = new HashCode();
    foreach ((string key, DataNode dataNode) in this._children)
    {
      hashCode.Add<string>(key);
      hashCode.Add<DataNode>(dataNode);
    }
    return hashCode.ToHashCode();
  }

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

  public void Add(KeyValuePair<string, DataNode> item) => this.Add(item.Key, item.Value);

  public void Clear()
  {
    this._children.Clear();
    this._list.Clear();
  }

  public bool Contains(KeyValuePair<string, DataNode> item) => this._children.ContainsKey(item.Key);

  [Obsolete("Use SerializationManager.PushComposition()")]
  public override MappingDataNode PushInheritance(MappingDataNode node)
  {
    MappingDataNode mappingDataNode = this.Copy();
    foreach ((string key, DataNode dataNode) in node)
    {
      if (!this._children.ContainsKey(key))
      {
        mappingDataNode.Remove(key);
        mappingDataNode.Add(key, dataNode.Copy());
      }
    }
    return mappingDataNode;
  }

  public void CopyTo(KeyValuePair<string, DataNode>[] array, int arrayIndex)
  {
    this._list.CopyTo(array, arrayIndex);
  }

  public bool Remove(KeyValuePair<string, DataNode> item)
  {
    return ((IDictionary<string, DataNode>) this).Remove(item.Key);
  }

  public bool TryAdd(string key, DataNode value)
  {
    if (!this._children.TryAdd(key, value))
      return false;
    this._list.Add(new KeyValuePair<string, DataNode>(key, value));
    return true;
  }

  public bool TryAddCopy(string key, DataNode value)
  {
    bool exists;
    ref DataNode local = ref CollectionsMarshal.GetValueRefOrAddDefault<string, DataNode>(this._children, key, out exists);
    if (exists)
      return false;
    local = value.Copy();
    this._list.Add(new KeyValuePair<string, DataNode>(key, local));
    return true;
  }

  [Obsolete("Use string keys instead of ValueDataNode")]
  public bool TryGet(ValueDataNode key, [NotNullWhen(true)] out DataNode? value)
  {
    return this.TryGet(key.Value, out value);
  }

  [Obsolete("Use string keys instead of ValueDataNode")]
  public DataNode this[ValueDataNode key]
  {
    get => this[key.Value];
    set => this[key.Value] = value;
  }

  [Obsolete("Use string keys instead of ValueDataNode")]
  public bool TryGetValue(ValueDataNode key, [NotNullWhen(true)] out DataNode? value)
  {
    return this.TryGet(key.Value, out value);
  }

  [Obsolete("Use string keys instead of ValueDataNode")]
  public bool TryGet<T>(ValueDataNode key, [NotNullWhen(true)] out T? node) where T : DataNode
  {
    return this.TryGet<T>(key.Value, out node);
  }

  [Obsolete("Use string keys instead of ValueDataNode")]
  public bool Has(ValueDataNode key) => this.Has(key.Value);

  [Obsolete("Use string keys instead of ValueDataNode")]
  public T Cast<T>(ValueDataNode key) where T : DataNode => this.Cast<T>(key.Value);

  [Obsolete("Use string keys instead of ValueDataNode")]
  public void Add(KeyValuePair<ValueDataNode, DataNode> item) => this.Add(item.Key, item.Value);

  [Obsolete("Use string keys instead of ValueDataNode")]
  public MappingDataNode Add(ValueDataNode key, DataNode node) => this.Add(key.Value, node);

  [Obsolete("Use string keys instead of ValueDataNode")]
  public void InsertAt(int index, ValueDataNode key, DataNode value)
  {
    this.InsertAt(index, key.Value, value);
  }

  [Obsolete("Use string keys instead of ValueDataNode")]
  public bool Contains(KeyValuePair<ValueDataNode, DataNode> item)
  {
    return this._children.ContainsKey(item.Key.Value);
  }

  [Obsolete("Use string keys instead of ValueDataNode")]
  public bool Remove(ValueDataNode key) => this.Remove(key.Value);
}
