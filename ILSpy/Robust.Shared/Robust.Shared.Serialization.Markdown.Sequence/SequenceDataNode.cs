using System;
using System.Collections;
using System.Collections.Generic;
using Robust.Shared.Serialization.Markdown.Value;
using YamlDotNet.Core;
using YamlDotNet.RepresentationModel;

namespace Robust.Shared.Serialization.Markdown.Sequence;

public sealed class SequenceDataNode : DataNode<SequenceDataNode>, IList<DataNode>, ICollection<DataNode>, IEnumerable<DataNode>, IEnumerable
{
	private readonly List<DataNode> _nodes;

	public IReadOnlyList<DataNode> Sequence => _nodes;

	public DataNode this[int index]
	{
		get
		{
			return _nodes[index];
		}
		set
		{
			_nodes[index] = value;
		}
	}

	public int Count => _nodes.Count;

	public bool IsReadOnly => false;

	public override bool IsEmpty => _nodes.Count == 0;

	public SequenceDataNode()
		: base(NodeMark.Invalid, NodeMark.Invalid)
	{
		_nodes = new List<DataNode>();
	}

	public SequenceDataNode(int size)
		: base(NodeMark.Invalid, NodeMark.Invalid)
	{
		_nodes = new List<DataNode>(size);
	}

	public SequenceDataNode(List<DataNode> nodes)
		: base(NodeMark.Invalid, NodeMark.Invalid)
	{
		_nodes = nodes;
	}

	public SequenceDataNode(List<string> values)
		: base(NodeMark.Invalid, NodeMark.Invalid)
	{
		_nodes = new List<DataNode>(values.Count);
		foreach (string value in values)
		{
			_nodes.Add(new ValueDataNode(value));
		}
	}

	public SequenceDataNode(YamlSequenceNode sequence)
		: base((NodeMark)((YamlNode)sequence).Start, (NodeMark)((YamlNode)sequence).End)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		_nodes = new List<DataNode>(sequence.Children.Count);
		foreach (YamlNode child in sequence.Children)
		{
			_nodes.Add(child.ToDataNode());
		}
		TagName tag = ((YamlNode)sequence).Tag;
		object tag2;
		if (!((TagName)(ref tag)).IsEmpty)
		{
			tag = ((YamlNode)sequence).Tag;
			tag2 = ((TagName)(ref tag)).Value;
		}
		else
		{
			tag2 = null;
		}
		Tag = (string?)tag2;
	}

	public SequenceDataNode(params DataNode[] nodes)
		: base(NodeMark.Invalid, NodeMark.Invalid)
	{
		_nodes = new List<DataNode>(nodes.Length);
		foreach (DataNode item in nodes)
		{
			_nodes.Add(item);
		}
	}

	public SequenceDataNode(params string[] strings)
		: base(NodeMark.Invalid, NodeMark.Invalid)
	{
		_nodes = new List<DataNode>(strings.Length);
		foreach (string value in strings)
		{
			_nodes.Add(new ValueDataNode(value));
		}
	}

	public YamlSequenceNode ToSequenceNode()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		YamlSequenceNode val = new YamlSequenceNode();
		foreach (DataNode node in _nodes)
		{
			val.Children.Add(node.ToYamlNode());
		}
		((YamlNode)val).Tag = TagName.op_Implicit(Tag);
		return val;
	}

	public int IndexOf(DataNode item)
	{
		return _nodes.IndexOf(item);
	}

	public void Insert(int index, DataNode item)
	{
		_nodes.Insert(index, item);
	}

	public void RemoveAt(int index)
	{
		_nodes.RemoveAt(index);
	}

	public void Add(DataNode node)
	{
		_nodes.Add(node);
	}

	public void Clear()
	{
		_nodes.Clear();
	}

	public bool Contains(DataNode item)
	{
		return _nodes.Contains(item);
	}

	public void CopyTo(DataNode[] array, int arrayIndex)
	{
		_nodes.CopyTo(array, arrayIndex);
	}

	public bool Remove(DataNode node)
	{
		return _nodes.Remove(node);
	}

	public T Cast<T>(int index) where T : DataNode
	{
		return (T)this[index];
	}

	public override SequenceDataNode Copy()
	{
		SequenceDataNode sequenceDataNode = new SequenceDataNode(Sequence.Count)
		{
			Tag = Tag,
			Start = Start,
			End = End
		};
		foreach (DataNode item in Sequence)
		{
			sequenceDataNode.Add(item.Copy());
		}
		return sequenceDataNode;
	}

	public List<DataNode>.Enumerator GetEnumerator()
	{
		return _nodes.GetEnumerator();
	}

	IEnumerator<DataNode> IEnumerable<DataNode>.GetEnumerator()
	{
		return _nodes.GetEnumerator();
	}

	public override int GetHashCode()
	{
		HashCode hashCode = default(HashCode);
		foreach (DataNode node in _nodes)
		{
			hashCode.Add(node);
		}
		return hashCode.ToHashCode();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public override SequenceDataNode? Except(SequenceDataNode node)
	{
		List<DataNode> list = new List<DataNode>();
		foreach (DataNode node2 in _nodes)
		{
			if (!node._nodes.Contains(node2))
			{
				list.Add(node2);
			}
		}
		if (list.Count > 0)
		{
			return new SequenceDataNode(list)
			{
				Tag = Tag,
				Start = Start,
				End = End
			};
		}
		return null;
	}

	public override bool Equals(object? obj)
	{
		if (!(obj is SequenceDataNode sequenceDataNode))
		{
			return false;
		}
		if (_nodes.Count != sequenceDataNode._nodes.Count)
		{
			return false;
		}
		for (int i = 0; i < _nodes.Count; i++)
		{
			if (!_nodes[i].Equals(sequenceDataNode._nodes[i]))
			{
				return false;
			}
		}
		return true;
	}

	[Obsolete("Use SerializationManager.PushComposition()")]
	public override SequenceDataNode PushInheritance(SequenceDataNode node)
	{
		SequenceDataNode sequenceDataNode = Copy();
		foreach (DataNode item in node)
		{
			sequenceDataNode.Add(item.Copy());
		}
		return sequenceDataNode;
	}
}
