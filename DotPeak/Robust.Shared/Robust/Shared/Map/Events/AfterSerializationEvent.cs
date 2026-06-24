// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Map.Events.AfterSerializationEvent
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.EntitySerialization;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Markdown.Mapping;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Map.Events;

public readonly record struct AfterSerializationEvent(
  HashSet<EntityUid> Entities,
  MappingDataNode Node,
  FileCategory Category)
;
