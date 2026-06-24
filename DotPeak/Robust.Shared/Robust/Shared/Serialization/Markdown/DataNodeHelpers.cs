// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.Markdown.DataNodeHelpers
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Sequence;
using Robust.Shared.Serialization.Markdown.Value;
using System;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Serialization.Markdown;

public static class DataNodeHelpers
{
  public static IEnumerable<DataNode> GetAllNodes(DataNode node)
  {
    switch (node)
    {
      case MappingDataNode node1:
        return DataNodeHelpers.GetAllNodes(node1);
      case SequenceDataNode node2:
        return DataNodeHelpers.GetAllNodes(node2);
      case ValueDataNode node3:
        return DataNodeHelpers.GetAllNodes(node3);
      default:
        throw new ArgumentOutOfRangeException(nameof (node));
    }
  }

  private static IEnumerable<DataNode> GetAllNodes(MappingDataNode node)
  {
    yield return (DataNode) node;
    foreach ((string key, DataNode node1) in node)
    {
      yield return (DataNode) node.GetKeyNode(key);
      IEnumerator<DataNode> enumerator = DataNodeHelpers.GetAllNodes(node1).GetEnumerator();
      while (enumerator.MoveNext())
        yield return enumerator.Current;
      enumerator = (IEnumerator<DataNode>) null;
      node1 = (DataNode) null;
    }
  }

  private static IEnumerable<DataNode> GetAllNodes(SequenceDataNode node)
  {
    yield return (DataNode) node;
    foreach (DataNode node1 in node)
    {
      IEnumerator<DataNode> enumerator = DataNodeHelpers.GetAllNodes(node1).GetEnumerator();
      while (enumerator.MoveNext())
        yield return enumerator.Current;
      enumerator = (IEnumerator<DataNode>) null;
    }
  }

  private static IEnumerable<DataNode> GetAllNodes(ValueDataNode node)
  {
    yield return (DataNode) node;
  }
}
