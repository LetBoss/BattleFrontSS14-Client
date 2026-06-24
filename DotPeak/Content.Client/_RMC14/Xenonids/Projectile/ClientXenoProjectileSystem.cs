// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Xenonids.Projectile.ClientXenoProjectileSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._RMC14.Weapons.Ranged.Prediction;
using Content.Shared._RMC14.Xenonids.Projectile;
using Robust.Client.GameObjects;
using Robust.Client.Physics;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using System;

#nullable enable
namespace Content.Client._RMC14.Xenonids.Projectile;

public sealed class ClientXenoProjectileSystem : EntitySystem
{
  [Dependency]
  private GunPredictionSystem _gunPrediction;
  [Dependency]
  private IPlayerManager _player;
  [Dependency]
  private SpriteSystem _sprite;

  public virtual void Initialize()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<XenoProjectileShotComponent, ComponentStartup>(new EntityEventRefHandler<XenoProjectileShotComponent, ComponentStartup>((object) this, __methodptr(OnShotStartup)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<XenoClientProjectileShotComponent, UpdateIsPredictedEvent>(new EntityEventRefHandler<XenoClientProjectileShotComponent, UpdateIsPredictedEvent>((object) this, __methodptr(OnUpdateIsPredicted)), (Type[]) null, (Type[]) null);
  }

  private void OnShotStartup(Entity<XenoProjectileShotComponent> ent, ref ComponentStartup args)
  {
    if (!this._gunPrediction.GunPrediction)
      return;
    EntityUid? shooterEnt = ent.Comp.ShooterEnt;
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    if ((shooterEnt.HasValue == localEntity.HasValue ? (shooterEnt.HasValue ? (EntityUid.op_Inequality(shooterEnt.GetValueOrDefault(), localEntity.GetValueOrDefault()) ? 1 : 0) : 0) : 1) != 0)
      return;
    this._sprite.SetVisible(Entity<SpriteComponent>.op_Implicit(ent.Owner), false);
  }

  private void OnUpdateIsPredicted(
    Entity<XenoClientProjectileShotComponent> ent,
    ref UpdateIsPredictedEvent args)
  {
    args.IsPredicted = true;
  }
}
