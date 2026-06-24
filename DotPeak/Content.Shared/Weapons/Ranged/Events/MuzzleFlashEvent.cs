// Decompiled with JetBrains decompiler
// Type: Content.Shared.Weapons.Ranged.Events.MuzzleFlashEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using System;
using System.Numerics;

#nullable enable
namespace Content.Shared.Weapons.Ranged.Events;

[NetSerializable]
[Serializable]
public sealed class MuzzleFlashEvent : EntityEventArgs
{
  public NetEntity Uid;
  public string Prototype;
  public Vector2 Offset;
  public Vector2 OriginOffset;
  public Angle Angle;

  public MuzzleFlashEvent(
    NetEntity uid,
    string prototype,
    Angle angle,
    Vector2 offset = default (Vector2),
    Vector2 originOffset = default (Vector2))
  {
    this.Uid = uid;
    this.Prototype = prototype;
    this.Angle = angle;
    this.Offset = offset;
    this.OriginOffset = originOffset;
  }
}
