// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.RMCAirShotSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Areas;
using Content.Shared._RMC14.CameraShake;
using Content.Shared._RMC14.Dropship.Weapon;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Weapons.Common;
using Content.Shared._RMC14.Xenonids.Devour;
using Content.Shared.CombatMode;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.IdentityManagement;
using Content.Shared.Popups;
using Content.Shared.Weapons.Ranged;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Weapons.Ranged;

public sealed class RMCAirShotSystem : EntitySystem
{
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private AreaSystem _area;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedCombatModeSystem _combat;
  [Dependency]
  private SkillsSystem _skills;
  [Dependency]
  private RMCCameraShakeSystem _cameraShake;
  [Dependency]
  private ISharedPlayerManager _player;
  [Dependency]
  private SharedDropshipWeaponSystem _dropship;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<RMCAirShotComponent, UniqueActionEvent>(new EntityEventRefHandler<RMCAirShotComponent, UniqueActionEvent>(this.OnUniqueAction), new Type[1]
    {
      typeof (CMGunSystem)
    });
    this.SubscribeLocalEvent<RMCAirShotComponent, AirShotDoAfterEvent>(new EntityEventRefHandler<RMCAirShotComponent, AirShotDoAfterEvent>(this.OnAirShotDoAfter));
    this.SubscribeLocalEvent<RMCAirShotComponent, ExaminedEvent>(new EntityEventRefHandler<RMCAirShotComponent, ExaminedEvent>(this.OnAirShotExamined));
  }

  private void OnUniqueAction(Entity<RMCAirShotComponent> ent, ref UniqueActionEvent args)
  {
    if (args.Handled || ent.Comp.RequiresCombat && !this._combat.IsInCombatMode(new EntityUid?(args.UserUid)) || ent.Comp.RequiredSkills != null && !this._skills.HasAllSkills((Entity<SkillsComponent>) args.UserUid, ent.Comp.RequiredSkills) || this.HasComp<DevouredComponent>(args.UserUid))
      return;
    this.AttemptAirShot(ent, args.UserUid);
    args.Handled = true;
  }

  private void OnAirShotDoAfter(Entity<RMCAirShotComponent> ent, ref AirShotDoAfterEvent args)
  {
    GunComponent comp1;
    if (args.DoAfter.Cancelled || !this.TryComp<GunComponent>((EntityUid) ent, out comp1))
      return;
    if (this._net.IsServer)
    {
      TakeAmmoEvent args1 = new TakeAmmoEvent(1, new List<(EntityUid?, IShootable)>(), this.GetCoordinates(args.Coordinates), new EntityUid?(args.User));
      this.RaiseLocalEvent<TakeAmmoEvent>((EntityUid) ent, args1);
      foreach ((EntityUid? nullable, IShootable _) in args1.Ammo)
      {
        RMCAirProjectileComponent comp2;
        if (this.TryComp<RMCAirProjectileComponent>(nullable, out comp2))
        {
          EntProtoId? prototype = comp2.Prototype;
          EntityUid entityUid = this.Spawn(prototype.HasValue ? (string) prototype.GetValueOrDefault() : (string) null, this.GetCoordinates(args.Coordinates));
          if (this.HasComp<FlareSignalComponent>(entityUid))
          {
            int nextId = this._dropship.ComputeNextId();
            string userAbbreviation = this._dropship.GetUserAbbreviation(args.User, nextId);
            this._dropship.MakeDropshipTarget(entityUid, userAbbreviation);
            ent.Comp.LastFlareId = userAbbreviation;
            this.Dirty<RMCAirShotComponent>(ent);
          }
        }
        this.Del(nullable);
      }
    }
    this._audio.PlayPredicted(comp1.SoundGunshotModified, (EntityUid) ent, new EntityUid?(args.User));
    if (ent.Comp.ShakeAmount > 0)
    {
      Filter filter = Filter.Pvs(args.User, 0.35f);
      List<ICommonSession> players = new List<ICommonSession>();
      this._popup.PopupClient(this.Loc.GetString("rmc-gun-shoot-air-self", ("weapon", (object) ent)), args.User, new EntityUid?(args.User), PopupType.LargeCaution);
      foreach (ICommonSession recipient in filter.Recipients)
      {
        EntityUid? attachedEntity = recipient.AttachedEntity;
        if (attachedEntity.HasValue)
        {
          EntityUid valueOrDefault = attachedEntity.GetValueOrDefault();
          ICommonSession session;
          if (!this._player.TryGetSessionByEntity(args.User, out session) || session != recipient)
          {
            this._popup.PopupEntity(this.Loc.GetString("rmc-gun-shoot-air-other", ("user", (object) Identity.Name(args.User, (IEntityManager) this.EntityManager, new EntityUid?(valueOrDefault))), ("weapon", (object) Identity.Name((EntityUid) ent, (IEntityManager) this.EntityManager, new EntityUid?(valueOrDefault)))), args.User, recipient, PopupType.LargeCaution);
            attachedEntity = recipient.AttachedEntity;
            if (attachedEntity.HasValue)
            {
              attachedEntity = recipient.AttachedEntity;
              RMCSizeComponent comp3;
              if (this.TryComp<RMCSizeComponent>(attachedEntity.Value, out comp3) && comp3.Size != RMCSizes.Humanoid)
                players.Add(recipient);
            }
          }
        }
      }
      filter.RemovePlayers((IEnumerable<ICommonSession>) players);
      if (this._net.IsServer)
        this._cameraShake.ShakeCamera(filter, ent.Comp.ShakeAmount, ent.Comp.ShakeStrength);
    }
    UpdateClientAmmoEvent args2 = new UpdateClientAmmoEvent(-1);
    this.RaiseLocalEvent<UpdateClientAmmoEvent>((EntityUid) ent, ref args2);
  }

  private void OnAirShotExamined(Entity<RMCAirShotComponent> ent, ref ExaminedEvent args)
  {
    using (args.PushGroup("RMCAirShotComponent", 5))
    {
      if (ent.Comp.RequiredSkills == null || this._skills.HasAllSkills((Entity<SkillsComponent>) args.Examiner, ent.Comp.RequiredSkills))
        args.PushMarkup(this.Loc.GetString("rmc-gun-shoot-air-examine", ("harm", (object) ent.Comp.RequiresCombat)));
      string lastFlareId = ent.Comp.LastFlareId;
      if (lastFlareId == null)
        return;
      args.PushMarkup(this.Loc.GetString("rmc-flare-gun-examine", ("id", (object) lastFlareId)));
    }
  }

  private void AttemptAirShot(Entity<RMCAirShotComponent> ent, EntityUid shooter)
  {
    GetAmmoCountEvent args = new GetAmmoCountEvent();
    this.RaiseLocalEvent<GetAmmoCountEvent>((EntityUid) ent, ref args);
    if (args.Count <= 0)
      return;
    EntityCoordinates moverCoordinates = this._transform.GetMoverCoordinates(shooter);
    if (!ent.Comp.IgnoreRoof && !this._area.CanCAS(moverCoordinates))
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-gun-shoot-air-blocked"), moverCoordinates, new EntityUid?(shooter), PopupType.SmallCaution);
    }
    else
    {
      AirShotDoAfterEvent @event = new AirShotDoAfterEvent(this.GetNetCoordinates(moverCoordinates));
      this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, shooter, ent.Comp.PreparationTime, (DoAfterEvent) @event, new EntityUid?((EntityUid) ent))
      {
        BreakOnMove = ent.Comp.DoAfterBreakOnMove,
        NeedHand = true,
        BreakOnHandChange = true,
        MovementThreshold = 0.5f
      });
    }
  }
}
