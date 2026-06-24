using System;
using System.Collections.Generic;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Sequence;
using Robust.Shared.Serialization.Markdown.Value;

namespace Robust.Shared.Serialization.Markdown;

public static class DataNodeHelpers
{
	public static IEnumerable<DataNode> GetAllNodes(DataNode node)
	{
		if (!(node is MappingDataNode node2))
		{
			if (!(node is SequenceDataNode node3))
			{
				if (node is ValueDataNode node4)
				{
					return GetAllNodes(node4);
				}
				throw new ArgumentOutOfRangeException("node");
			}
			return GetAllNodes(node3);
		}
		return GetAllNodes(node2);
	}

	private static IEnumerable<DataNode> GetAllNodes(MappingDataNode node)
	{
		yield return node;
		foreach (var (key, v) in node)
		{
			yield return node.GetKeyNode(key);
			foreach (DataNode allNode in GetAllNodes(v))
			{
				yield return allNode;
			}
		}
	}

	private static IEnumerable<DataNode> GetAllNodes(SequenceDataNode node)
	{
		yield return node;
		foreach (DataNode item in node)
		{
			foreach (DataNode allNode in GetAllNodes(item))
			{
				yield return allNode;
			}
		}
	}

	private static IEnumerable<DataNode> GetAllNodes(ValueDataNode node)
	{
		yield return node;
	}
}
