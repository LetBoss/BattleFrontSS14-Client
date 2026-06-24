// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Map.Events.BeforeSerializationEvent
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.EntitySerialization;
using Robust.Shared.GameObjects;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Map.Events;

public readonly record struct BeforeSerializationEvent(
  HashSet<EntityUid> Entities,
  HashSet<MapId> MapIds,
  FileCategory Category = FileCategory.Unknown)
;
