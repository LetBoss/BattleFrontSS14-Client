// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Explosion.Implosion.RMCImplosionSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Stun;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;

#nullable enable
namespace Content.Shared._RMC14.Explosion.Implosion;

public sealed class RMCImplosionSystem : EntitySystem
{
  [Dependency]
  private RMCSizeStunSystem _sizeStun;
  [Dependency]
  private EntityLookupSystem _entityLookup;

  public void Implode(RMCImplosion implosion, MapCoordinates origin)
  {
    foreach (EntityUid target in this._entityLookup.GetEntitiesInRange(origin, implosion.PullRange, LookupFlags.Uncontained))
      this._sizeStun.KnockBack(target, new MapCoordinates?(origin), -implosion.PullDistance, -implosion.PullDistance, implosion.PullSpeed, implosion.IgnoreSize);
  }
}
