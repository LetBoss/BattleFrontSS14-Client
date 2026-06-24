using System;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Sequence;
using Robust.Shared.Serialization.Markdown.Value;
using YamlDotNet.RepresentationModel;

namespace Robust.Shared.Serialization.Markdown;

public static class YamlNodeHelpers
{
	public static DataNode ToDataNode(this YamlNode node)
	{
		YamlScalarNode val = (YamlScalarNode)(object)((node is YamlScalarNode) ? node : null);
		if (val == null)
		{
			YamlMappingNode val2 = (YamlMappingNode)(object)((node is YamlMappingNode) ? node : null);
			if (val2 == null)
			{
				YamlSequenceNode val3 = (YamlSequenceNode)(object)((node is YamlSequenceNode) ? node : null);
				if (val3 != null)
				{
					return new SequenceDataNode(val3);
				}
				throw new ArgumentOutOfRangeException("node");
			}
			return new MappingDataNode(val2);
		}
		return new ValueDataNode(val);
	}

	public static T ToDataNodeCast<T>(this YamlNode node) where T : DataNode
	{
		return (T)node.ToDataNode();
	}

	public static YamlNode ToYamlNode(this DataNode node)
	{
		if (!(node is ValueDataNode valueDataNode))
		{
			if (!(node is MappingDataNode mappingDataNode))
			{
				if (node is SequenceDataNode sequenceDataNode)
				{
					return (YamlNode)(object)sequenceDataNode.ToSequenceNode();
				}
				throw new ArgumentOutOfRangeException("node");
			}
			return (YamlNode)(object)mappingDataNode.ToYaml();
		}
		return (YamlNode)(object)(YamlScalarNode)valueDataNode;
	}
}
