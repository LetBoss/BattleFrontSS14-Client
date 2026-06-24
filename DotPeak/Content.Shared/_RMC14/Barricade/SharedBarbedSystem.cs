// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Barricade.SharedBarbedSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Armor;
using Content.Shared._RMC14.Barricade.Components;
using Content.Shared._RMC14.Construction.Upgrades;
using Content.Shared._RMC14.Xenonids.Acid;
using Content.Shared._RMC14.Xenonids.Leap;
using Content.Shared.Climbing.Events;
using Content.Shared.Damage;
using Content.Shared.DoAfter;
using Content.Shared.Doors;
using Content.Shared.Doors.Components;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Stacks;
using Content.Shared.Tools.Components;
using Content.Shared.Tools.Systems;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Network;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Systems;
using System;

#nullable enable
namespace Content.Shared._RMC14.Barricade;

public abstract class SharedBarbedSystem : EntitySystem
{
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private DamageableSystem _damageableSystem;
  [Dependency]
  private SharedDoAfterSystem _doAfterSystem;
  [Dependency]
  private FixtureSystem _fixture;
  [Dependency]
  private SharedPhysicsSystem _physics;
  [Dependency]
  private SharedPopupSystem _popupSystem;
  [Dependency]
  private SharedStackSystem _stacks;
  [Dependency]
  private SharedToolSystem _toolSystem;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private INetManager _netManager;
  [Dependency]
  private SharedXenoAcidSystem _xenoAcid;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<BarbedComponent, ExaminedEvent>(new EntityEventRefHandler<BarbedComponent, ExaminedEvent>(this.OnExamined));
    this.SubscribeLocalEvent<BarbedComponent, AttackedEvent>(new EntityEventRefHandler<BarbedComponent, AttackedEvent>(this.OnAttacked));
    this.SubscribeLocalEvent<BarbedComponent, InteractUsingEvent>(new EntityEventRefHandler<BarbedComponent, InteractUsingEvent>(this.OnInteractUsing));
    this.SubscribeLocalEvent<BarbedComponent, DoAfterAttemptEvent<BarbedDoAfterEvent>>(new EntityEventRefHandler<BarbedComponent, DoAfterAttemptEvent<BarbedDoAfterEvent>>(this.OnDoAfterAttempt));
    this.SubscribeLocalEvent<BarbedComponent, BarbedDoAfterEvent>(new EntityEventRefHandler<BarbedComponent, BarbedDoAfterEvent>(this.OnDoAfter));
    this.SubscribeLocalEvent<BarbedComponent, CutBarbedDoAfterEvent>(new EntityEventRefHandler<BarbedComponent, CutBarbedDoAfterEvent>(this.WireCutterOnDoAfter));
    this.SubscribeLocalEvent<BarbedComponent, DoorStateChangedEvent>(new EntityEventRefHandler<BarbedComponent, DoorStateChangedEvent>(this.OnDoorStateChanged));
    this.SubscribeLocalEvent<BarbedComponent, AttemptClimbEvent>(new EntityEventRefHandler<BarbedComponent, AttemptClimbEvent>(this.OnClimbAttempt));
    this.SubscribeLocalEvent<BarbedComponent, CMGetArmorPiercingEvent>(new EntityEventRefHandler<BarbedComponent, CMGetArmorPiercingEvent>(this.OnGetArmorPiercing));
    this.SubscribeLocalEvent<BarbedComponent, RMCConstructionUpgradedEvent>(new EntityEventRefHandler<BarbedComponent, RMCConstructionUpgradedEvent>(this.OnConstructionUpgraded));
    this.SubscribeLocalEvent<BarbedComponent, XenoLeapHitAttempt>(new EntityEventRefHandler<BarbedComponent, XenoLeapHitAttempt>(this.OnXenoLeapHitAttempt), after: new Type[1]
    {
      typeof (XenoLeapSystem)
    });
  }

  private void OnExamined(Entity<BarbedComponent> ent, ref ExaminedEvent args)
  {
    if (!ent.Comp.IsBarbed)
      return;
    using (args.PushGroup(nameof (SharedBarbedSystem)))
      args.PushMarkup(this.Loc.GetString("rmc-barricade-examine-barbed"));
  }

  private void OnAttacked(Entity<BarbedComponent> barbed, ref AttackedEvent args)
  {
    if (!barbed.Comp.IsBarbed)
      return;
    this._damageableSystem.TryChangeDamage(new EntityUid?(args.User), barbed.Comp.ThornsDamage, origin: new EntityUid?((EntityUid) barbed), tool: new EntityUid?((EntityUid) barbed));
    this._popupSystem.PopupClient(this.Loc.GetString("barbed-wire-damage"), (EntityUid) barbed, new EntityUid?(args.User), PopupType.SmallCaution);
  }

  private void OnInteractUsing(Entity<BarbedComponent> ent, ref InteractUsingEvent args)
  {
    if (this._xenoAcid.IsMelted((EntityUid) ent))
    {
      this._popupSystem.PopupClient(this.Loc.GetString("rmc-construction-melted"), (EntityUid) ent, new EntityUid?(args.User), PopupType.SmallCaution);
      args.Handled = true;
    }
    else if (!ent.Comp.IsBarbed && this.HasComp<BarbedWireComponent>(args.Used))
    {
      BarbedDoAfterEvent @event = new BarbedDoAfterEvent();
      if (!this._doAfterSystem.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, args.User, ent.Comp.WireTime, (DoAfterEvent) @event, new EntityUid?((EntityUid) ent), new EntityUid?((EntityUid) ent), new EntityUid?(args.Used))
      {
        BreakOnMove = true,
        BreakOnDamage = true,
        NeedHand = true,
        AttemptFrequency = AttemptFrequency.EveryTick,
        CancelDuplicate = false,
        DuplicateCondition = DuplicateConditions.SameTarget
      }))
        return;
      args.Handled = true;
      this._popupSystem.PopupClient(this.Loc.GetString("barbed-wire-slot-wiring"), (EntityUid) ent, new EntityUid?(args.User));
    }
    else if (ent.Comp.IsBarbed && this.HasComp<BarbedWireComponent>(args.Used))
    {
      args.Handled = true;
      this._popupSystem.PopupClient(this.Loc.GetString("barbed-wire-slot-insert-full"), (EntityUid) ent, new EntityUid?(args.User));
    }
    else
    {
      ToolComponent comp;
      if (!ent.Comp.IsBarbed || !this.TryComp<ToolComponent>(args.Used, out comp) || !this._toolSystem.HasQuality(args.Used, (string) ent.Comp.RemoveQuality, comp))
        return;
      args.Handled = true;
      this._popupSystem.PopupClient(this.Loc.GetString("barbed-wire-cutting-action-begin"), (EntityUid) ent, new EntityUid?(args.User));
      EntityManager entityManager = this.EntityManager;
      EntityUid user = args.User;
      TimeSpan cutTime = ent.Comp.CutTime;
      CutBarbedDoAfterEvent @event = new CutBarbedDoAfterEvent();
      EntityUid? eventTarget = new EntityUid?((EntityUid) ent);
      EntityUid? nullable = new EntityUid?(args.Used);
      EntityUid? target = new EntityUid?();
      EntityUid? used = nullable;
      this._doAfterSystem.TryStartDoAfter(new DoAfterArgs((IEntityManager) entityManager, user, cutTime, (DoAfterEvent) @event, eventTarget, target, used)
      {
        BreakOnMove = true,
        BreakOnDamage = true,
        NeedHand = true
      });
    }
  }

  private void OnDoAfterAttempt(
    Entity<BarbedComponent> barbed,
    ref DoAfterAttemptEvent<BarbedDoAfterEvent> args)
  {
    if (!barbed.Comp.IsBarbed)
      return;
    args.Cancel();
  }

  private void OnDoAfter(Entity<BarbedComponent> barbed, ref BarbedDoAfterEvent args)
  {
    if (!args.Used.HasValue || args.Cancelled || args.Handled || barbed.Comp.IsBarbed)
      return;
    args.Handled = true;
    StackComponent comp;
    if (this.TryComp<StackComponent>(args.Used.Value, out comp))
      this._stacks.Use(args.Used.Value, 1, comp);
    barbed.Comp.IsBarbed = true;
    this.Dirty<BarbedComponent>(barbed);
    this.UpdateBarricade(barbed, true);
    this._audio.PlayPredicted(barbed.Comp.BarbSound, barbed.Owner, new EntityUid?(args.User));
    this._popupSystem.PopupClient(this.Loc.GetString("barbed-wire-slot-insert-success"), barbed.Owner, new EntityUid?(args.User));
  }

  private void WireCutterOnDoAfter(Entity<BarbedComponent> barbed, ref CutBarbedDoAfterEvent args)
  {
    if (args.Cancelled || args.Handled)
      return;
    args.Handled = true;
    barbed.Comp.IsBarbed = false;
    this.Dirty<BarbedComponent>(barbed);
    this.UpdateBarricade(barbed, true);
    this._audio.PlayPredicted(barbed.Comp.CutSound, barbed.Owner, new EntityUid?(args.User));
    this._popupSystem.PopupClient(this.Loc.GetString("barbed-wire-cutting-action-finish"), barbed.Owner, new EntityUid?(args.User));
    if (this._netManager.IsClient)
      return;
    EntityCoordinates moverCoordinates = this._transform.GetMoverCoordinates((EntityUid) barbed);
    this.Spawn((string) barbed.Comp.Spawn, moverCoordinates);
  }

  private void OnDoorStateChanged(Entity<BarbedComponent> barbed, ref DoorStateChangedEvent args)
  {
    this.UpdateBarricade(barbed);
  }

  private void OnClimbAttempt(Entity<BarbedComponent> barbed, ref AttemptClimbEvent args)
  {
    if (!barbed.Comp.IsBarbed)
      return;
    args.Cancelled = true;
    this._popupSystem.PopupClient(this.Loc.GetString("barbed-wire-cant-climb"), barbed.Owner, new EntityUid?(args.User));
  }

  private void OnGetArmorPiercing(Entity<BarbedComponent> barbed, ref CMGetArmorPiercingEvent args)
  {
    if (!barbed.Comp.IsBarbed)
      return;
    args.Piercing = 1000;
  }

  private void OnConstructionUpgraded(
    Entity<BarbedComponent> barbed,
    ref RMCConstructionUpgradedEvent args)
  {
    BarbedComponent barbedComponent = this.EnsureComp<BarbedComponent>(args.New);
    barbedComponent.IsBarbed = barbed.Comp.IsBarbed;
    this.Dirty(args.New, (IComponent) barbedComponent);
    this.UpdateBarricade((Entity<BarbedComponent>) (args.New, barbedComponent), true);
  }

  private void OnXenoLeapHitAttempt(Entity<BarbedComponent> ent, ref XenoLeapHitAttempt args)
  {
    if (!ent.Comp.IsBarbed)
      return;
    this._damageableSystem.TryChangeDamage(new EntityUid?(args.Leaper), ent.Comp.ThornsDamage, origin: new EntityUid?((EntityUid) ent), tool: new EntityUid?((EntityUid) ent));
  }

  protected void UpdateBarricade(Entity<BarbedComponent> barbed, bool updateBarbed = false)
  {
    DoorComponent comp;
    bool flag = this.TryComp<DoorComponent>((EntityUid) barbed, out comp) && comp.State == DoorState.Open;
    BarbedWireVisuals barbedWireVisuals = !barbed.Comp.IsBarbed ? BarbedWireVisuals.UnWired : (!flag ? BarbedWireVisuals.WiredClosed : BarbedWireVisuals.WiredOpen);
    if (updateBarbed)
    {
      BarbedStateChangedEvent args = new BarbedStateChangedEvent();
      this.RaiseLocalEvent<BarbedStateChangedEvent>((EntityUid) barbed, ref args);
    }
    Fixture fixtureOrNull = this._fixture.GetFixtureOrNull((EntityUid) barbed, barbed.Comp.FixtureId);
    if (fixtureOrNull != null)
    {
      if (barbed.Comp.IsBarbed)
        this._physics.AddCollisionLayer((EntityUid) barbed, barbed.Comp.FixtureId, fixtureOrNull, 33554432 /*0x02000000*/);
      else
        this._physics.RemoveCollisionLayer((EntityUid) barbed, barbed.Comp.FixtureId, fixtureOrNull, 33554432 /*0x02000000*/);
    }
    this._appearance.SetData((EntityUid) barbed, (Enum) BarbedWireVisualLayers.Wire, (object) barbedWireVisuals);
  }
}
