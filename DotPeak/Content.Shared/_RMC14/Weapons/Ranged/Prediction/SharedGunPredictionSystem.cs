// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.Prediction.SharedGunPredictionSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.CCVar;
using Content.Shared.CombatMode;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Player;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Weapons.Ranged.Prediction;

public abstract class SharedGunPredictionSystem : EntitySystem
{
  [Dependency]
  private SharedCombatModeSystem _combatMode;
  [Dependency]
  private IConfigurationManager _config;
  [Dependency]
  private SharedGunSystem _gun;
  [Dependency]
  private CMGunSystem _akimbo;

  public bool GunPrediction { get; private set; }

  public override void Initialize()
  {
    this.Subs.CVar<bool>(this._config, RMCCVars.RMCGunPrediction, (Action<bool>) (v => this.GunPrediction = v), true);
  }

  public List<EntityUid>? ShootRequested(
    NetEntity netGun,
    NetCoordinates coordinates,
    NetEntity? target,
    List<int>? projectiles,
    ICommonSession session)
  {
    EntityUid? attachedEntity = session.AttachedEntity;
    EntityUid gunEntity;
    GunComponent gunComp;
    if (!attachedEntity.HasValue || !this._combatMode.IsInCombatMode(attachedEntity) || !this._gun.TryGetGun(attachedEntity.Value, out gunEntity, out gunComp))
      return (List<EntityUid>) null;
    EntityUid entity = this.GetEntity(netGun);
    EntityUid gunUid;
    GunComponent gun;
    if (gunEntity == entity)
    {
      gunUid = gunEntity;
      gun = gunComp;
    }
    else
    {
      EntityUid offHand;
      GunComponent comp;
      if (!this._akimbo.TryGetAkimboOffHand(attachedEntity.Value, (Entity<GunComponent>) (gunEntity, gunComp), out offHand) || !(offHand == entity) || !this.TryComp<GunComponent>(entity, out comp))
        return (List<EntityUid>) null;
      gunUid = entity;
      gun = comp;
    }
    EntityCoordinates coordinates1 = this.GetCoordinates(coordinates);
    if (!coordinates1.IsValid((IEntityManager) this.EntityManager))
      return (List<EntityUid>) null;
    gun.ShootCoordinates = new EntityCoordinates?(coordinates1);
    gun.Target = this.GetEntity(target);
    return this._gun.AttemptShoot(attachedEntity.Value, gunUid, gun, projectiles, session);
  }
}
