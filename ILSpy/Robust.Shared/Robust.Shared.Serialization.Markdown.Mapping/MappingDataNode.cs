using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Utility;
using YamlDotNet.Core;
using YamlDotNet.RepresentationModel;

namespace Robust.Shared.Serialization.Markdown.Mapping;

public sealed class MappingDataNode : DataNode<MappingDataNode>, IDictionary<string, DataNode>, ICollection<KeyValuePair<string, DataNode>>, IEnumerable<KeyValuePair<string, DataNode>>, IEnumerable
{
	private readonly Dictionary<string, DataNode> _children;

	private readonly List<KeyValuePair<string, DataNode>> _list;

	private IReadOnlyDictionary<string, ValueDataNode>? _keyNodes;

	public override bool IsEmpty => _children.Count == 0;

	public int Count => _children.Count;

	public bool IsReadOnly => false;

	public IReadOnlyDictionary<string, DataNode> Children => _children;

	public KeyValuePair<string, DataNode> this[int key] => _list[key];

	public DataNode this[string key]
	{
		get
		{
			return _children[key];
		}
		set
		{
			if (_children.TryAdd(key, value))
			{
				_list.Add(new KeyValuePair<string, DataNode>(key, value));
				return;
			}
			int num = IndexOf(key);
			if (num == -1)
			{
				throw new Exception("Key exists in Children, but not list?");
			}
			_list[num] = new KeyValuePair<string, DataNode>(key, value);
			_children[key] = value;
		}
	}

	public ICollection<string> Keys => _list.Select<KeyValuePair<string, DataNode>, string>((KeyValuePair<string, DataNode> x) => x.Key).ToArray();

	public ICollection<DataNode> Values => _list.Select<KeyValuePair<string, DataNode>, DataNode>((KeyValuePair<string, DataNode> x) => x.Value).ToArray();

	[Obsolete("Use string keys instead of ValueDataNode")]
	public DataNode this[ValueDataNode key]
	{
		get
		{
			return this[key.Value];
		}
		set
		{
			this[key.Value] = value;
		}
	}

	public MappingDataNode()
		: base(NodeMark.Invalid, NodeMark.Invalid)
	{
		_children = new Dictionary<string, DataNode>();
		_list = new List<KeyValuePair<string, DataNode>>();
	}

	public MappingDataNode(int size)
		: base(NodeMark.Invalid, NodeMark.Invalid)
	{
		_children = new Dictionary<string, DataNode>(size);
		_list = new List<KeyValuePair<string, DataNode>>(size);
	}

	public MappingDataNode(YamlMappingNode mapping)
		: base((NodeMark)((YamlNode)mapping).Start, (NodeMark)((YamlNode)mapping).End)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		_children = new Dictionary<string, DataNode>(((ICollection<KeyValuePair<YamlNode, YamlNode>>)mapping.Children).Count);
		_list = new List<KeyValuePair<string, DataNode>>(((ICollection<KeyValuePair<YamlNode, YamlNode>>)mapping.Children).Count);
		Dictionary<string, ValueDataNode> dictionary = new Dictionary<string, ValueDataNode>(((ICollection<KeyValuePair<YamlNode, YamlNode>>)mapping.Children).Count);
		foreach (KeyValuePair<YamlNode, YamlNode> item in (IEnumerable<KeyValuePair<YamlNode, YamlNode>>)mapping.Children)
		{
			item.Deconstruct(out var key, out var value);
			YamlNode obj = key;
			YamlNode node = value;
			ValueDataNode valueDataNode = new ValueDataNode((YamlScalarNode)(object)(((obj is YamlScalarNode) ? obj : null) ?? throw new NotSupportedException("Mapping data nodes must have a scalar keys")));
			Add(valueDataNode.Value, node.ToDataNode());
			dictionary.Add(valueDataNode.Value, valueDataNode);
		}
		_keyNodes = dictionary;
		TagName tag = ((YamlNode)mapping).Tag;
		object tag2;
		if (!((TagName)(ref tag)).IsEmpty)
		{
			tag = ((YamlNode)mapping).Tag;
			tag2 = ((TagName)(ref tag)).Value;
		}
		else
		{
			tag2 = null;
		}
		Tag = (string?)tag2;
	}

	public MappingDataNode(Dictionary<string, DataNode> nodes)
		: base(NodeMark.Invalid, NodeMark.Invalid)
	{
		_children = new Dictionary<string, DataNode>(nodes);
		_list = new List<KeyValuePair<string, DataNode>>(_children);
	}

	public MappingDataNode Add(string key, DataNode node)
	{
		_children.Add(key, node);
		_list.Add(new KeyValuePair<string, DataNode>(key, node));
		return this;
	}

	public int IndexOf(string key)
	{
		for (int i = 0; i < _list.Count; i++)
		{
			if (_list[i].Key == key)
			{
				return i;
			}
		}
		return -1;
	}

	void IDictionary<string, DataNode>.Add(string key, DataNode value)
	{
		Add(key, value);
	}

	public bool ContainsKey(string key)
	{
		return _children.ContainsKey(key);
	}

	bool IDictionary<string, DataNode>.Remove(string key)
	{
		return ((IDictionary<string, DataNode>)this).Remove(key);
	}

	public bool TryGetValue(string key, [NotNullWhen(true)] out DataNode? value)
	{
		return TryGet(key, out value);
	}

	public DataNode Get(string key)
	{
		return _children[key];
	}

	public T Get<T>(string key) where T : DataNode
	{
		return (T)Get(key);
	}

	public bool TryGet(string key, [NotNullWhen(true)] out DataNode? node)
	{
		return _children.TryGetValue(key, out node);
	}

	public bool TryGet<T>(string key, [NotNullWhen(true)] out T? node) where T : DataNode
	{
		node = null;
		if (!TryGet(key, out DataNode node2) || !(node2 is T val))
		{
			return false;
		}
		node = val;
		return true;
	}

	public bool Has(string key)
	{
		return _children.ContainsKey(key);
	}

	public bool Remove(string key)
	{
		if (!_children.Remove(key))
		{
			return false;
		}
		int num = IndexOf(key);
		if (num == -1)
		{
			throw new Exception("Key exists in Children, but not list?");
		}
		_list.RemoveAt(num);
		return true;
	}

	public T Cast<T>(string key) where T : DataNode
	{
		return (T)this[key];
	}

	public YamlMappingNode ToYaml()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Expected O, but got Unknown
		YamlMappingNode val = new YamlMappingNode();
		foreach (KeyValuePair<string, DataNode> item in _list)
		{
			item.Deconstruct(out var key, out var value);
			string text = key;
			DataNode node = value;
			ValueDataNode value2;
			YamlScalarNode val2 = (YamlScalarNode)((_keyNodes == null || !_keyNodes.TryGetValue(text, out value2)) ? ((object)new YamlScalarNode(text)
			{
				Style = (ScalarStyle)((ValueDataNode.IsNullLiteral(text) || string.IsNullOrWhiteSpace(text)) ? 3 : 0)
			}) : ((object)(YamlScalarNode)value2));
			val.Add((YamlNode)(object)val2, node.ToYamlNode());
		}
		((YamlNode)val).Tag = TagName.op_Implicit(Tag);
		return val;
	}

	public ValueDataNode GetKeyNode(string key)
	{
		return _keyNodes?.GetValueOrDefault(key) ?? new ValueDataNode(key);
	}

	public MappingDataNode Merge(MappingDataNode otherMapping)
	{
		MappingDataNode mappingDataNode = Copy();
		mappingDataNode.Insert(otherMapping);
		mappingDataNode.Tag = Tag;
		mappingDataNode.Start = Start;
		mappingDataNode.End = End;
		return mappingDataNode;
	}

	public void Insert(MappingDataNode otherMapping, bool skipDuplicates = false)
	{
		foreach (var (key, dataNode2) in otherMapping.Children)
		{
			if (!skipDuplicates || !Has(key))
			{
				Add(key, dataNode2.Copy());
			}
		}
	}

	public void InsertAt(int index, string key, DataNode value)
	{
		if (index > _list.Count || index < 0)
		{
			throw new ArgumentOutOfRangeException();
		}
		if (!_children.TryAdd(key, value))
		{
			throw new InvalidOperationException("Already contains key " + key);
		}
		_list.Insert(index, new KeyValuePair<string, DataNode>(key, value));
	}

	public override MappingDataNode Copy()
	{
		MappingDataNode mappingDataNode = new MappingDataNode(_children.Count)
		{
			Tag = Tag,
			Start = Start,
			End = End
		};
		foreach (var (key, dataNode2) in _list)
		{
			mappingDataNode.Add(key, dataNode2.Copy());
		}
		mappingDataNode._keyNodes = _keyNodes;
		return mappingDataNode;
	}

	public MappingDataNode ShallowClone()
	{
		MappingDataNode mappingDataNode = new MappingDataNode(_children.Count)
		{
			Tag = Tag,
			Start = Start,
			End = End
		};
		foreach (var (key, node) in _list)
		{
			mappingDataNode.Add(key, node);
		}
		return mappingDataNode;
	}

	public MappingDataNode? RecursiveExcept(MappingDataNode node)
	{
		MappingDataNode mappingDataNode = new MappingDataNode
		{
			Tag = Tag,
			Start = Start,
			End = End
		};
		foreach (var (key, dataNode2) in _list)
		{
			if (!node._children.TryGetValue(key, out DataNode value))
			{
				mappingDataNode.Add(key, dataNode2.Copy());
				continue;
			}
			DataNode dataNode3 = dataNode2.Except(value);
			if (dataNode3 != null)
			{
				mappingDataNode.Add(key, dataNode3);
			}
		}
		if (mappingDataNode._children.Count != 0)
		{
			return mappingDataNode;
		}
		return null;
	}

	public override MappingDataNode? Except(MappingDataNode node)
	{
		MappingDataNode mappingDataNode = new MappingDataNode
		{
			Tag = Tag,
			Start = Start,
			End = End
		};
		foreach (var (key, dataNode2) in _list)
		{
			if (!node._children.TryGetValue(key, out DataNode value) || !dataNode2.Equals(value))
			{
				mappingDataNode.Add(key, dataNode2.Copy());
			}
		}
		if (mappingDataNode._children.Count != 0)
		{
			return mappingDataNode;
		}
		return null;
	}

	public bool AnyExcept(MappingDataNode node)
	{
		foreach (KeyValuePair<string, DataNode> item in _list)
		{
			item.Deconstruct(out var key, out var value);
			string key2 = key;
			DataNode dataNode = value;
			KeyValuePair<string, DataNode>? keyValuePair = node._list.FirstOrNull<KeyValuePair<string, DataNode>>((KeyValuePair<string, DataNode> p) => p.Key.Equals(key2));
			if (!keyValuePair.HasValue)
			{
				return true;
			}
			if (!dataNode.Equals(keyValuePair.Value.Value))
			{
				return true;
			}
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (!(obj is MappingDataNode mappingDataNode))
		{
			return false;
		}
		if (_children.Count != mappingDataNode._children.Count)
		{
			return false;
		}
		if (Tag != mappingDataNode.Tag)
		{
			return false;
		}
		foreach (var (key, dataNode2) in mappingDataNode)
		{
			if (!_children.TryGetValue(key, out DataNode value) || !dataNode2.Equals(value))
			{
				return false;
			}
		}
		return true;
	}

	public List<KeyValuePair<string, DataNode>>.Enumerator GetEnumerator()
	{
		return _list.GetEnumerator();
	}

	IEnumerator<KeyValuePair<string, DataNode>> IEnumerable<KeyValuePair<string, DataNode>>.GetEnumerator()
	{
		return _list.GetEnumerator();
	}

	public override int GetHashCode()
	{
		HashCode hashCode = default(HashCode);
		foreach (var (value, value2) in _children)
		{
			hashCode.Add(value);
			hashCode.Add(value2);
		}
		return hashCode.ToHashCode();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public void Add(KeyValuePair<string, DataNode> item)
	{
		Add(item.Key, item.Value);
	}

	public void Clear()
	{
		_children.Clear();
		_list.Clear();
	}

	public bool Contains(KeyValuePair<string, DataNode> item)
	{
		return _children.ContainsKey(item.Key);
	}

	[Obsolete("Use SerializationManager.PushComposition()")]
	public override MappingDataNode PushInheritance(MappingDataNode node)
	{
		MappingDataNode mappingDataNode = Copy();
		foreach (var (key, dataNode2) in node)
		{
			if (!_children.ContainsKey(key))
			{
				mappingDataNode.Remove(key);
				mappingDataNode.Add(key, dataNode2.Copy());
			}
		}
		return mappingDataNode;
	}

	public void CopyTo(KeyValuePair<string, DataNode>[] array, int arrayIndex)
	{
		_list.CopyTo(array, arrayIndex);
	}

	public bool Remove(KeyValuePair<string, DataNode> item)
	{
		return ((IDictionary<string, DataNode>)this).Remove(item.Key);
	}

	public bool TryAdd(string key, DataNode value)
	{
		if (!_children.TryAdd(key, value))
		{
			return false;
		}
		_list.Add(new KeyValuePair<string, DataNode>(key, value));
		return true;
	}

	public bool TryAddCopy(string key, DataNode value)
	{
		bool exists;
		ref DataNode valueRefOrAddDefault = ref CollectionsMarshal.GetValueRefOrAddDefault(_children, key, out exists);
		if (exists)
		{
			return false;
		}
		valueRefOrAddDefault = value.Copy();
		_list.Add(new KeyValuePair<string, DataNode>(key, valueRefOrAddDefault));
		return true;
	}

	[Obsolete("Use string keys instead of ValueDataNode")]
	public bool TryGet(ValueDataNode key, [NotNullWhen(true)] out DataNode? value)
	{
		return TryGet(key.Value, out value);
	}

	[Obsolete("Use string keys instead of ValueDataNode")]
	public bool TryGetValue(ValueDataNode key, [NotNullWhen(true)] out DataNode? value)
	{
		return TryGet(key.Value, out value);
	}

	[Obsolete("Use string keys instead of ValueDataNode")]
	public bool TryGet<T>(ValueDataNode key, [NotNullWhen(true)] out T? node) where T : DataNode
	{
		return TryGet(key.Value, out node);
	}

	[Obsolete("Use string keys instead of ValueDataNode")]
	public bool Has(ValueDataNode key)
	{
		return Has(key.Value);
	}

	[Obsolete("Use string keys instead of ValueDataNode")]
	public T Cast<T>(ValueDataNode key) where T : DataNode
	{
		return Cast<T>(key.Value);
	}

	[Obsolete("Use string keys instead of ValueDataNode")]
	public void Add(KeyValuePair<ValueDataNode, DataNode> item)
	{
		Add(item.Key, item.Value);
	}

	[Obsolete("Use string keys instead of ValueDataNode")]
	public MappingDataNode Add(ValueDataNode key, DataNode node)
	{
		return Add(key.Value, node);
	}

	[Obsolete("Use string keys instead of ValueDataNode")]
	public void InsertAt(int index, ValueDataNode key, DataNode value)
	{
		InsertAt(index, key.Value, value);
	}

	[Obsolete("Use string keys instead of ValueDataNode")]
	public bool Contains(KeyValuePair<ValueDataNode, DataNode> item)
	{
		return _children.ContainsKey(item.Key.Value);
	}

	[Obsolete("Use string keys instead of ValueDataNode")]
	public bool Remove(ValueDataNode key)
	{
		return Remove(key.Value);
	}
}
