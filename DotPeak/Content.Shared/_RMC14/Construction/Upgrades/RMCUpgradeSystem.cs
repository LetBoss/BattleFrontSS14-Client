// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Construction.Upgrades.RMCUpgradeSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Xenonids.Acid;
using Content.Shared.Damage;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Stacks;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Construction.Upgrades;

public sealed class RMCUpgradeSystem : EntitySystem
{
  [Dependency]
  private IComponentFactory _compFactory;
  [Dependency]
  private SkillsSystem _skills;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private SharedUserInterfaceSystem _ui;
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private SharedStackSystem _stack;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private IPrototypeManager _prototypes;
  [Dependency]
  private DamageableSystem _damageable;
  [Dependency]
  private EntityWhitelistSystem _whitelist;
  [Dependency]
  private SharedXenoAcidSystem _xenoAcid;
  private readonly Dictionary<EntProtoId, RMCConstructionUpgradeComponent> _upgradePrototypes = new Dictionary<EntProtoId, RMCConstructionUpgradeComponent>();
  private Robust.Shared.GameObjects.EntityQuery<RMCConstructionUpgradeItemComponent> _upgradeItemQuery;

  public override void Initialize()
  {
    this._upgradeItemQuery = this.GetEntityQuery<RMCConstructionUpgradeItemComponent>();
    this.SubscribeLocalEvent<RMCConstructionUpgradeTargetComponent, InteractUsingEvent>(new EntityEventRefHandler<RMCConstructionUpgradeTargetComponent, InteractUsingEvent>(this.OnInteractUsing));
    this.Subs.BuiEvents<RMCConstructionUpgradeTargetComponent>((object) RMCConstructionUpgradeUiKey.Key, (BoundUserInterfaceRegisterExt.BuiEventSubscriber<RMCConstructionUpgradeTargetComponent>) (subs => subs.Event<RMCConstructionUpgradeBuiMsg>(new EntityEventRefHandler<RMCConstructionUpgradeTargetComponent, RMCConstructionUpgradeBuiMsg>(this.OnUpgradeBuiMsg))));
    this.SubscribeLocalEvent<PrototypesReloadedEventArgs>(new EntityEventHandler<PrototypesReloadedEventArgs>(this.OnPrototypesReloaded));
    this.RefreshUpgradePrototypes();
  }

  private void OnInteractUsing(
    Entity<RMCConstructionUpgradeTargetComponent> ent,
    ref InteractUsingEvent args)
  {
    if (args.Handled)
      return;
    EntityUid user = args.User;
    RMCConstructionUpgradeItemComponent component;
    if (!this._upgradeItemQuery.TryComp(args.Used, out component) || !this._whitelist.IsValid(component.Whitelist, (EntityUid) ent))
      return;
    if (!this._skills.HasSkill((Entity<SkillsComponent>) user, ent.Comp.Skill, ent.Comp.SkillAmountRequired))
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-construction-failure", (nameof (ent), (object) ent)), (EntityUid) ent, new EntityUid?(user), PopupType.SmallCaution);
      args.Handled = true;
    }
    else if (this._xenoAcid.IsMelted((EntityUid) ent))
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-construction-melted"), (EntityUid) ent, new EntityUid?(user), PopupType.SmallCaution);
      args.Handled = true;
    }
    else
    {
      if (ent.Comp.Upgrades == null)
        return;
      this._ui.OpenUi((Entity<UserInterfaceComponent>) ent.Owner, (Enum) RMCConstructionUpgradeUiKey.Key, new EntityUid?(user));
      args.Handled = true;
    }
  }

  private void OnUpgradeBuiMsg(
    Entity<RMCConstructionUpgradeTargetComponent> ent,
    ref RMCConstructionUpgradeBuiMsg args)
  {
    this._ui.CloseUi((Entity<UserInterfaceComponent>) ent.Owner, (Enum) RMCConstructionUpgradeUiKey.Key);
    EntityUid actor = args.Actor;
    RMCConstructionUpgradeComponent upgradeComponent;
    if (!this._upgradePrototypes.TryGetValue(args.Upgrade, out upgradeComponent))
      return;
    EntityUid? uid1 = new EntityUid?();
    foreach (EntityUid uid2 in this._hands.EnumerateHeld((Entity<HandsComponent>) actor))
    {
      if (this._upgradeItemQuery.HasComp(uid2))
      {
        uid1 = new EntityUid?(uid2);
        break;
      }
    }
    if (!uid1.HasValue)
      return;
    if (upgradeComponent.Material.HasValue)
    {
      StackComponent comp;
      if (this.TryComp<StackComponent>(uid1, out comp))
      {
        ProtoId<StackPrototype>? stackTypeId = (ProtoId<StackPrototype>?) comp.StackTypeId;
        ProtoId<StackPrototype>? material = upgradeComponent.Material;
        if ((stackTypeId.HasValue == material.HasValue ? (stackTypeId.HasValue ? (stackTypeId.GetValueOrDefault() == material.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0 && !this._stack.Use(uid1.Value, upgradeComponent.Amount, comp))
        {
          this._popup.PopupClient(this.Loc.GetString((string) upgradeComponent.FailurePopup, (nameof (ent), (object) ent)), (EntityUid) ent, new EntityUid?(actor), PopupType.SmallCaution);
          return;
        }
      }
    }
    else
      this.QueueDel(uid1);
    if (this._net.IsClient)
      return;
    MapCoordinates mapCoordinates = this._transform.GetMapCoordinates((EntityUid) ent);
    Angle worldRotation = this._transform.GetWorldRotation((EntityUid) ent);
    DamageSpecifier damage = (DamageSpecifier) null;
    DamageableComponent comp1;
    if (this.TryComp<DamageableComponent>((EntityUid) ent, out comp1))
      damage = comp1.Damage;
    EntityUid entityUid = this.Spawn((string) upgradeComponent.UpgradedEntity, mapCoordinates, rotation: DirectionExtensions.ToAngle(((Angle) ref worldRotation).GetCardinalDir()));
    this._popup.PopupEntity(this.Loc.GetString((string) upgradeComponent.UpgradedPopup), entityUid, actor);
    DamageableComponent comp2;
    if (damage != null && this.TryComp<DamageableComponent>(entityUid, out comp2))
      this._damageable.SetDamage(entityUid, comp2, damage);
    RMCConstructionUpgradedEvent args1 = new RMCConstructionUpgradedEvent(entityUid, ent.Owner);
    this.RaiseLocalEvent<RMCConstructionUpgradedEvent>(ent.Owner, args1);
    this.RaiseLocalEvent<RMCConstructionUpgradedEvent>(entityUid, args1, true);
    this.QueueDel(new EntityUid?(ent.Owner));
  }

  private void OnPrototypesReloaded(PrototypesReloadedEventArgs ev)
  {
    if (!ev.WasModified<EntityPrototype>())
      return;
    this.RefreshUpgradePrototypes();
  }

  private void RefreshUpgradePrototypes()
  {
    this._upgradePrototypes.Clear();
    foreach (EntityPrototype enumeratePrototype in this._prototypes.EnumeratePrototypes<EntityPrototype>())
    {
      RMCConstructionUpgradeComponent component;
      if (enumeratePrototype.TryGetComponent<RMCConstructionUpgradeComponent>(out component, this._compFactory))
        this._upgradePrototypes[(EntProtoId) enumeratePrototype.ID] = component;
    }
  }
}
