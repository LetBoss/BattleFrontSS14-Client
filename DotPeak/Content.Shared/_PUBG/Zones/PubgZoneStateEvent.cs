// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.PubgZoneStateEvent
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
public sealed class PubgZoneStateEvent : EntityEventArgs
{
  public Vector2 CurrentCenter;
  public float CurrentRadius;
  public Vector2 NextCenter;
  public float NextRadius;
  public int CurrentPhase;
  public ZoneState State;
  public float TimeRemaining;
  public bool Active;
  public bool Visible;
  public NetEntity ZoneMapEntity;

  public PubgZoneStateEvent(
    Vector2 currentCenter,
    float currentRadius,
    Vector2 nextCenter,
    float nextRadius,
    int currentPhase,
    ZoneState state,
    float timeRemaining,
    bool active,
    bool visible,
    NetEntity zoneMapEntity)
  {
    this.CurrentCenter = currentCenter;
    this.CurrentRadius = currentRadius;
    this.NextCenter = nextCenter;
    this.NextRadius = nextRadius;
    this.CurrentPhase = currentPhase;
    this.State = state;
    this.TimeRemaining = timeRemaining;
    this.Active = active;
    this.Visible = visible;
    this.ZoneMapEntity = zoneMapEntity;
  }
}
