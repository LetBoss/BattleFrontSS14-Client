// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.Markdown.Mapping.MappingDataNodeExtensions
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Serialization.Markdown.Sequence;
using Robust.Shared.Serialization.Markdown.Value;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Serialization.Markdown.Mapping;

public static class MappingDataNodeExtensions
{
  public static MappingDataNode Add(this MappingDataNode mapping, string key, string value)
  {
    mapping.Add(key, (DataNode) new ValueDataNode(value));
    return mapping;
  }

  public static MappingDataNode Add(
    this MappingDataNode mapping,
    string key,
    List<string> sequence)
  {
    mapping.Add(key, (DataNode) new SequenceDataNode(sequence));
    return mapping;
  }
}
