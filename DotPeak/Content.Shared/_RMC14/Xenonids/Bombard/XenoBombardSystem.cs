// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Bombard.XenoBombardSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Projectiles;
using Content.Shared._RMC14.Xenonids.GasToggle;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared.Actions.Components;
using Content.Shared.DoAfter;
using Content.Shared.Popups;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using System;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Bombard;

public sealed class XenoBombardSystem : EntitySystem
{
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private SharedGunSystem _gun;
  [Dependency]
  private SharedXenoHiveSystem _hive;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedRMCActionsSystem _rmcActions;
  [Dependency]
  private RMCProjectileSystem _rmcProjectile;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private XenoPlasmaSystem _xenoPlasma;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<XenoBombardComponent, XenoBombardActionEvent>(new EntityEventRefHandler<XenoBombardComponent, XenoBombardActionEvent>(this.OnBombard));
    this.SubscribeLocalEvent<XenoBombardComponent, DoAfterAttemptEvent<XenoBombardDoAfterEvent>>(new EntityEventRefHandler<XenoBombardComponent, DoAfterAttemptEvent<XenoBombardDoAfterEvent>>(this.OnBombardDoAfterAttempt));
    this.SubscribeLocalEvent<XenoBombardComponent, XenoBombardDoAfterEvent>(new EntityEventRefHandler<XenoBombardComponent, XenoBombardDoAfterEvent>(this.OnBombardDoAfter));
    this.SubscribeLocalEvent<XenoBombardComponent, XenoGasToggleActionEvent>(new EntityEventRefHandler<XenoBombardComponent, XenoGasToggleActionEvent>(this.OnToggleType));
  }

  private void OnBombard(Entity<XenoBombardComponent> ent, ref XenoBombardActionEvent args)
  {
    MapCoordinates mapCoordinates1 = this._transform.GetMapCoordinates((EntityUid) ent);
    MapCoordinates mapCoordinates2 = this._transform.ToMapCoordinates(args.Target);
    if (mapCoordinates1.MapId != mapCoordinates2.MapId)
      return;
    args.Handled = true;
    if (!this._xenoPlasma.HasPlasmaPopup((Entity<XenoPlasmaComponent>) ent.Owner, ent.Comp.PlasmaCost))
      return;
    Vector2 vector2 = mapCoordinates2.Position - mapCoordinates1.Position;
    if ((double) vector2.Length() > (double) ent.Comp.Range)
      mapCoordinates2 = mapCoordinates1.Offset(Vector2Helpers.Normalized(vector2) * (float) ent.Comp.Range);
    this._audio.PlayPredicted(ent.Comp.PrepareSound, (EntityUid) ent, new EntityUid?((EntityUid) ent));
    XenoBombardDoAfterEvent @event = new XenoBombardDoAfterEvent()
    {
      Coordinates = mapCoordinates2
    };
    if (!this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, (EntityUid) ent, ent.Comp.Delay, (DoAfterEvent) @event, new EntityUid?((EntityUid) ent), new EntityUid?((EntityUid) args.Action))
    {
      BreakOnMove = true,
      RootEntity = true
    }))
      return;
    this._rmcActions.DisableSharedCooldownEvents((Entity<ActionSharedCooldownComponent>) args.Action.Owner, (EntityUid) ent);
    this._popup.PopupClient(this.Loc.GetString("rmc-glob-start-self"), (EntityUid) ent, new EntityUid?((EntityUid) ent));
    this._popup.PopupEntity(this.Loc.GetString("rmc-glob-start-others", ("user", (object) ent)), (EntityUid) ent, Filter.PvsExcept((EntityUid) ent), true, PopupType.MediumCaution);
  }

  private void OnBombardDoAfterAttempt(
    Entity<XenoBombardComponent> ent,
    ref DoAfterAttemptEvent<XenoBombardDoAfterEvent> args)
  {
    EntityUid? target = args.Event.Target;
    if (!target.HasValue)
      return;
    EntityUid valueOrDefault = target.GetValueOrDefault();
    ActionComponent comp;
    if (!this.HasComp<InstantActionComponent>(valueOrDefault) || !this.TryComp<ActionComponent>(valueOrDefault, out comp) || comp.Enabled)
      return;
    this._rmcActions.EnableSharedCooldownEvents((Entity<ActionSharedCooldownComponent>) valueOrDefault, (EntityUid) ent);
    args.Cancel();
  }

  private void OnBombardDoAfter(Entity<XenoBombardComponent> ent, ref XenoBombardDoAfterEvent args)
  {
    EntityUid? target = args.Target;
    if (!target.HasValue)
      return;
    EntityUid valueOrDefault = target.GetValueOrDefault();
    this._rmcActions.EnableSharedCooldownEvents((Entity<ActionSharedCooldownComponent>) valueOrDefault, (EntityUid) ent);
    if (args.Cancelled || args.Handled)
      return;
    args.Handled = true;
    if (!this._xenoPlasma.TryRemovePlasmaPopup((Entity<XenoPlasmaComponent>) ent.Owner, ent.Comp.PlasmaCost) || this._net.IsClient)
      return;
    MapCoordinates mapCoordinates = this._transform.GetMapCoordinates((EntityUid) ent);
    if (mapCoordinates.MapId != args.Coordinates.MapId)
      return;
    Vector2 direction = args.Coordinates.Position - mapCoordinates.Position;
    EntityUid entityUid = this.Spawn((string) ent.Comp.Projectile, mapCoordinates, rotation: new Angle());
    this._hive.SetSameHive((Entity<HiveMemberComponent>) ent.Owner, (Entity<HiveMemberComponent>) entityUid);
    ProjectileMaxRangeComponent maxRangeComponent = this.EnsureComp<ProjectileMaxRangeComponent>(entityUid);
    this._rmcProjectile.SetMaxRange((Entity<ProjectileMaxRangeComponent>) (entityUid, maxRangeComponent), direction.Length());
    this._gun.ShootProjectile(entityUid, direction, Vector2.Zero, new EntityUid?((EntityUid) ent), new EntityUid?((EntityUid) ent), 7.5f);
    this._audio.PlayEntity(ent.Comp.ShootSound, (EntityUid) ent, (EntityUid) ent);
    this._rmcActions.ActivateSharedCooldown((Entity<ActionSharedCooldownComponent>) valueOrDefault, (EntityUid) ent);
    this._popup.PopupClient(this.Loc.GetString("rmc-glob-shoot-self"), (EntityUid) ent, new EntityUid?((EntityUid) ent));
    this._popup.PopupEntity(this.Loc.GetString("rmc-glob-shoot-others", ("user", (object) ent)), (EntityUid) ent, Filter.PvsExcept((EntityUid) ent), true, PopupType.MediumCaution);
  }

  private void OnToggleType(Entity<XenoBombardComponent> ent, ref XenoGasToggleActionEvent args)
  {
    if (ent.Comp.Projectiles.Length == 0)
      return;
    int num = Array.IndexOf<EntProtoId>(ent.Comp.Projectiles, ent.Comp.Projectile);
    int index = num == -1 || num >= ent.Comp.Projectiles.Length - 1 ? 0 : num + 1;
    ent.Comp.Projectile = ent.Comp.Projectiles[index];
    this.Dirty<XenoBombardComponent>(ent);
  }
}
