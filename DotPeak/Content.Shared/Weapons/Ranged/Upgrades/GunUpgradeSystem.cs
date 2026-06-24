// Decompiled with JetBrains decompiler
// Type: Content.Shared.Weapons.Ranged.Upgrades.GunUpgradeSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Projectiles;
using Content.Shared.Tag;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Systems;
using Content.Shared.Weapons.Ranged.Upgrades.Components;
using Content.Shared.Whitelist;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Components;
using Robust.Shared.Prototypes;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared.Weapons.Ranged.Upgrades;

public sealed class GunUpgradeSystem : EntitySystem
{
  [Dependency]
  private ISharedAdminLogManager _adminLog;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private SharedGunSystem _gun;
  [Dependency]
  private EntityWhitelistSystem _entityWhitelist;
  [Dependency]
  private SharedPopupSystem _popup;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<UpgradeableGunComponent, ComponentInit>(new EntityEventRefHandler<UpgradeableGunComponent, ComponentInit>(this.OnInit));
    this.SubscribeLocalEvent<UpgradeableGunComponent, AfterInteractUsingEvent>(new EntityEventRefHandler<UpgradeableGunComponent, AfterInteractUsingEvent>(this.OnAfterInteractUsing));
    this.SubscribeLocalEvent<UpgradeableGunComponent, ExaminedEvent>(new EntityEventRefHandler<UpgradeableGunComponent, ExaminedEvent>(this.OnExamine));
    this.SubscribeLocalEvent<UpgradeableGunComponent, GunRefreshModifiersEvent>(new EntityEventRefHandler<UpgradeableGunComponent, GunRefreshModifiersEvent>(this.RelayEvent<GunRefreshModifiersEvent>));
    this.SubscribeLocalEvent<UpgradeableGunComponent, GunShotEvent>(new EntityEventRefHandler<UpgradeableGunComponent, GunShotEvent>(this.RelayEvent<GunShotEvent>));
    this.SubscribeLocalEvent<GunUpgradeFireRateComponent, GunRefreshModifiersEvent>(new EntityEventRefHandler<GunUpgradeFireRateComponent, GunRefreshModifiersEvent>(this.OnFireRateRefresh));
    this.SubscribeLocalEvent<GunUpgradeSpeedComponent, GunRefreshModifiersEvent>(new EntityEventRefHandler<GunUpgradeSpeedComponent, GunRefreshModifiersEvent>(this.OnSpeedRefresh));
    this.SubscribeLocalEvent<GunUpgradeDamageComponent, GunShotEvent>(new EntityEventRefHandler<GunUpgradeDamageComponent, GunShotEvent>(this.OnDamageGunShot));
  }

  private void RelayEvent<T>(Entity<UpgradeableGunComponent> ent, ref T args) where T : notnull
  {
    foreach (Entity<GunUpgradeComponent> currentUpgrade in this.GetCurrentUpgrades(ent))
      this.RaiseLocalEvent<T>((EntityUid) currentUpgrade, ref args);
  }

  private void OnExamine(Entity<UpgradeableGunComponent> ent, ref ExaminedEvent args)
  {
    using (args.PushGroup("UpgradeableGunComponent"))
    {
      foreach (Entity<GunUpgradeComponent> currentUpgrade in this.GetCurrentUpgrades(ent))
        args.PushMarkup(this.Loc.GetString((string) currentUpgrade.Comp.ExamineText));
    }
  }

  private void OnInit(Entity<UpgradeableGunComponent> ent, ref ComponentInit args)
  {
    this._container.EnsureContainer<Container>((EntityUid) ent, ent.Comp.UpgradesContainerId);
  }

  private void OnAfterInteractUsing(
    Entity<UpgradeableGunComponent> ent,
    ref AfterInteractUsingEvent args)
  {
    GunUpgradeComponent comp;
    if (args.Handled || !args.CanReach || !this.TryComp<GunUpgradeComponent>(args.Used, out comp))
      return;
    if (this.GetCurrentUpgrades(ent).Count >= ent.Comp.MaxUpgradeCount)
    {
      this._popup.PopupPredicted(this.Loc.GetString("upgradeable-gun-popup-upgrade-limit"), (EntityUid) ent, new EntityUid?(args.User));
    }
    else
    {
      if (this._entityWhitelist.IsWhitelistFail(ent.Comp.Whitelist, args.Used))
        return;
      if (this.GetCurrentUpgradeTags(ent).ToHashSet<ProtoId<TagPrototype>>().IsSupersetOf((IEnumerable<ProtoId<TagPrototype>>) comp.Tags))
      {
        this._popup.PopupPredicted(this.Loc.GetString("upgradeable-gun-popup-already-present"), (EntityUid) ent, new EntityUid?(args.User));
      }
      else
      {
        this._audio.PlayPredicted(ent.Comp.InsertSound, (EntityUid) ent, new EntityUid?(args.User));
        this._popup.PopupClient(this.Loc.GetString("gun-upgrade-popup-insert", ("upgrade", (object) args.Used), ("gun", (object) ent.Owner)), new EntityUid?(args.User));
        this._gun.RefreshModifiers((Entity<GunComponent>) ent.Owner);
        args.Handled = this._container.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) args.Used, this._container.GetContainer((EntityUid) ent, ent.Comp.UpgradesContainerId));
        ISharedAdminLogManager adminLog = this._adminLog;
        LogStringHandler logStringHandler = new LogStringHandler(29, 3);
        logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) args.User), "player", "ToPrettyString(args.User)");
        logStringHandler.AppendLiteral(" inserted gun upgrade ");
        logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) args.Used), "ToPrettyString(args.Used)");
        logStringHandler.AppendLiteral(" into ");
        logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) ent.Owner), "ToPrettyString(ent.Owner)");
        logStringHandler.AppendLiteral(".");
        ref LogStringHandler local = ref logStringHandler;
        adminLog.Add(LogType.Action, LogImpact.Low, ref local);
      }
    }
  }

  private void OnFireRateRefresh(
    Entity<GunUpgradeFireRateComponent> ent,
    ref GunRefreshModifiersEvent args)
  {
    args.FireRate *= ent.Comp.Coefficient;
  }

  private void OnSpeedRefresh(
    Entity<GunUpgradeSpeedComponent> ent,
    ref GunRefreshModifiersEvent args)
  {
    args.ProjectileSpeed *= ent.Comp.Coefficient;
  }

  private void OnDamageGunShot(Entity<GunUpgradeDamageComponent> ent, ref GunShotEvent args)
  {
    foreach ((EntityUid? Uid, IShootable Shootable) tuple in args.Ammo)
    {
      ProjectileComponent comp;
      if (this.TryComp<ProjectileComponent>(tuple.Uid, out comp))
        comp.Damage += ent.Comp.Damage;
    }
  }

  public HashSet<Entity<GunUpgradeComponent>> GetCurrentUpgrades(Entity<UpgradeableGunComponent> ent)
  {
    BaseContainer container;
    if (!this._container.TryGetContainer((EntityUid) ent, ent.Comp.UpgradesContainerId, out container))
      return new HashSet<Entity<GunUpgradeComponent>>();
    HashSet<Entity<GunUpgradeComponent>> currentUpgrades = new HashSet<Entity<GunUpgradeComponent>>();
    foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) container.ContainedEntities)
    {
      GunUpgradeComponent comp;
      if (this.TryComp<GunUpgradeComponent>(containedEntity, out comp))
        currentUpgrades.Add((Entity<GunUpgradeComponent>) (containedEntity, comp));
    }
    return currentUpgrades;
  }

  public IEnumerable<ProtoId<TagPrototype>> GetCurrentUpgradeTags(
    Entity<UpgradeableGunComponent> ent)
  {
    foreach (Entity<GunUpgradeComponent> currentUpgrade in this.GetCurrentUpgrades(ent))
    {
      List<ProtoId<TagPrototype>>.Enumerator enumerator = currentUpgrade.Comp.Tags.GetEnumerator();
      while (enumerator.MoveNext())
        yield return enumerator.Current;
      enumerator = new List<ProtoId<TagPrototype>>.Enumerator();
    }
  }
}
