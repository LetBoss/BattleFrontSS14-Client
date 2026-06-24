// Decompiled with JetBrains decompiler
// Type: Content.Shared.Throwing.BeforeThrowEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using System.Numerics;

#nullable disable
namespace Content.Shared.Throwing;

[ByRefEvent]
public struct BeforeThrowEvent(
  EntityUid itemUid,
  Vector2 direction,
  float throwSpeed,
  EntityUid playerUid)
{
  public bool Cancelled = false;

  public EntityUid ItemUid { get; set; } = itemUid;

  public Vector2 Direction { get; } = direction;

  public float ThrowSpeed { get; set; } = throwSpeed;

  public EntityUid PlayerUid { get; } = playerUid;
}
