// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Projectile.XenoProjectilePredictedHitEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;
using System;

#nullable disable
namespace Content.Shared._RMC14.Xenonids.Projectile;

[NetSerializable]
[Serializable]
public sealed class XenoProjectilePredictedHitEvent(
  int id,
  NetEntity target,
  GameTick lastRealTick) : EntityEventArgs
{
  public readonly int Id = id;
  public readonly NetEntity Target = target;
  public readonly GameTick LastRealTick = lastRealTick;
}
