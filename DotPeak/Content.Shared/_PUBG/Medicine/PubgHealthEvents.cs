// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Medicine.PubgHealthUpdateEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable disable
namespace Content.Shared._PUBG.Medicine;

[NetSerializable]
[Serializable]
public sealed class PubgHealthUpdateEvent : EntityEventArgs
{
  public FixedPoint2 CurrentHp { get; }

  public FixedPoint2 MaxHp { get; }

  public PubgHealthUpdateEvent(FixedPoint2 currentHp, FixedPoint2 maxHp)
  {
    this.CurrentHp = currentHp;
    this.MaxHp = maxHp;
  }
}
