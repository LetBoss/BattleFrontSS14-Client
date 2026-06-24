// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.RedZoneStateEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;
using System.Numerics;

#nullable disable
namespace Content.Shared._PUBG;

[NetSerializable]
[Serializable]
public sealed class RedZoneStateEvent : EntityEventArgs
{
  public bool ZoneActive;
  public Vector2 ZoneCenter;
  public float ZoneRadius;
  public bool HasActiveBomb;
  public Vector2 BombCenter;
  public float BombRadius;

  public RedZoneStateEvent(
    bool zoneActive,
    Vector2 zoneCenter,
    float zoneRadius,
    bool hasActiveBomb,
    Vector2 bombCenter,
    float bombRadius)
  {
    this.ZoneActive = zoneActive;
    this.ZoneCenter = zoneCenter;
    this.ZoneRadius = zoneRadius;
    this.HasActiveBomb = hasActiveBomb;
    this.BombCenter = bombCenter;
    this.BombRadius = bombRadius;
  }
}
