// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.Markdown.YamlNodeHelpers
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Sequence;
using Robust.Shared.Serialization.Markdown.Value;
using System;
using YamlDotNet.RepresentationModel;

#nullable enable
namespace Robust.Shared.Serialization.Markdown;

public static class YamlNodeHelpers
{
  public static DataNode ToDataNode(this YamlNode node)
  {
    switch (node)
    {
      case YamlScalarNode node1:
        return (DataNode) new ValueDataNode(node1);
      case YamlMappingNode mapping:
        return (DataNode) new MappingDataNode(mapping);
      case YamlSequenceNode sequence:
        return (DataNode) new SequenceDataNode(sequence);
      default:
        throw new ArgumentOutOfRangeException(nameof (node));
    }
  }

  public static T ToDataNodeCast<T>(this YamlNode node) where T : DataNode => (T) node.ToDataNode();

  public static YamlNode ToYamlNode(this DataNode node)
  {
    switch (node)
    {
      case ValueDataNode yamlNode:
        return (YamlNode) (YamlScalarNode) yamlNode;
      case MappingDataNode mappingDataNode:
        return (YamlNode) mappingDataNode.ToYaml();
      case SequenceDataNode sequenceDataNode:
        return (YamlNode) sequenceDataNode.ToSequenceNode();
      default:
        throw new ArgumentOutOfRangeException(nameof (node));
    }
  }
}
