// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.MapChangedEvent
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Map;
using System;

#nullable disable
namespace Robust.Shared.GameObjects;

[Obsolete("Use map creation or deletion events")]
public sealed class MapChangedEvent : EntityEventArgs
{
  public EntityUid Uid;

  public MapChangedEvent(EntityUid uid, MapId map, bool created)
  {
    this.Uid = uid;
    this.Map = map;
    this.Created = created;
  }

  public MapId Map { get; }

  public bool Created { get; }

  public bool Destroyed => !this.Created;
}
