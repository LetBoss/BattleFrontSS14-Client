// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Replays.ReplayRecordingStopped
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Serialization.Markdown.Mapping;

#nullable enable
namespace Robust.Shared.Replays;

public sealed class ReplayRecordingStopped
{
  public required MappingDataNode Metadata { get; init; }

  public required IReplayFileWriter Writer { get; init; }

  internal ReplayRecordingStopped()
  {
  }
}
