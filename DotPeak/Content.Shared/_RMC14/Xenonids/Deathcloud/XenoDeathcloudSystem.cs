// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Deathcloud.XenoDeathcloudSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared.Coordinates;
using Content.Shared.Mobs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Deathcloud;

public sealed class XenoDeathcloudSystem : EntitySystem
{
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedXenoHiveSystem _hive;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<XenoDeathcloudComponent, MobStateChangedEvent>(new EntityEventRefHandler<XenoDeathcloudComponent, MobStateChangedEvent>(this.OnStateChanged));
  }

  private void OnStateChanged(Entity<XenoDeathcloudComponent> xeno, ref MobStateChangedEvent args)
  {
    if (this._net.IsClient || args.NewMobState != MobState.Dead)
      return;
    EntityUid dest = this.SpawnAtPosition((string) xeno.Comp.Spawn, xeno.Owner.ToCoordinates());
    this._hive.SetSameHive((Entity<HiveMemberComponent>) xeno.Owner, (Entity<HiveMemberComponent>) dest);
  }
}
