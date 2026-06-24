// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Wieldable.RMCWieldableSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Armor;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Wieldable.Components;
using Content.Shared._RMC14.Wieldable.Events;
using Content.Shared.Hands;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction.Events;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.Timing;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Wieldable;
using Content.Shared.Wieldable.Components;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Wieldable;

public sealed class RMCWieldableSystem : EntitySystem
{
  [Robust.Shared.IoC.Dependency]
  private SharedHandsSystem _hands;
  [Robust.Shared.IoC.Dependency]
  private IGameTiming _timing;
  [Robust.Shared.IoC.Dependency]
  private MovementSpeedModifierSystem _movementSpeed;
  [Robust.Shared.IoC.Dependency]
  private SharedPopupSystem _popupSystem;
  [Robust.Shared.IoC.Dependency]
  private UseDelaySystem _useDelaySystem;
  [Robust.Shared.IoC.Dependency]
  private SkillsSystem _skills;
  [Robust.Shared.IoC.Dependency]
  private SharedContainerSystem _container;
  private const string WieldUseDelayId = "RMCWieldDelay";
  private static readonly EntProtoId<SkillDefinitionComponent> WieldSkill = (EntProtoId<SkillDefinitionComponent>) "RMCSkillFirearms";

  public override void Initialize()
  {
    this.SubscribeLocalEvent<WieldableSpeedModifiersComponent, GotEquippedHandEvent>(new EntityEventRefHandler<WieldableSpeedModifiersComponent, GotEquippedHandEvent>(this.OnGotEquippedHand));
    this.SubscribeLocalEvent<WieldableSpeedModifiersComponent, GotUnequippedHandEvent>(new EntityEventRefHandler<WieldableSpeedModifiersComponent, GotUnequippedHandEvent>(this.OnGotUnequippedHand));
    this.SubscribeLocalEvent<WieldableSpeedModifiersComponent, HeldRelayedEvent<RefreshMovementSpeedModifiersEvent>>(new EntityEventRefHandler<WieldableSpeedModifiersComponent, HeldRelayedEvent<RefreshMovementSpeedModifiersEvent>>(this.OnRefreshMovementSpeedModifiers));
    this.SubscribeLocalEvent<WieldableSpeedModifiersComponent, ItemUnwieldedEvent>(new EntityEventRefHandler<WieldableSpeedModifiersComponent, ItemUnwieldedEvent>(this.OnItemUnwielded));
    this.SubscribeLocalEvent<WieldableSpeedModifiersComponent, ItemWieldedEvent>(new EntityEventRefHandler<WieldableSpeedModifiersComponent, ItemWieldedEvent>(this.OnItemWielded));
    this.SubscribeLocalEvent<WieldableSpeedModifiersComponent, MapInitEvent>(new EntityEventRefHandler<WieldableSpeedModifiersComponent, MapInitEvent>(this.OnMapInit));
    this.SubscribeLocalEvent<WieldDelayComponent, GotEquippedHandEvent>(new EntityEventRefHandler<WieldDelayComponent, GotEquippedHandEvent>(this.OnGotEquippedHand));
    this.SubscribeLocalEvent<WieldDelayComponent, MapInitEvent>(new EntityEventRefHandler<WieldDelayComponent, MapInitEvent>(this.OnMapInit));
    this.SubscribeLocalEvent<WieldDelayComponent, UseInHandEvent>(new EntityEventRefHandler<WieldDelayComponent, UseInHandEvent>(this.OnUseInHand));
    this.SubscribeLocalEvent<WieldDelayComponent, ShotAttemptedEvent>(new EntityEventRefHandler<WieldDelayComponent, ShotAttemptedEvent>(this.OnShotAttempt));
    this.SubscribeLocalEvent<WieldDelayComponent, ItemWieldedEvent>(new EntityEventRefHandler<WieldDelayComponent, ItemWieldedEvent>(this.OnItemWieldedWithDelay));
  }

  private void OnMapInit(Entity<WieldableSpeedModifiersComponent> wieldable, ref MapInitEvent args)
  {
    this.RefreshSpeedModifiers((Entity<WieldableSpeedModifiersComponent>) (wieldable.Owner, wieldable.Comp));
  }

  private void OnMapInit(Entity<WieldDelayComponent> wieldable, ref MapInitEvent args)
  {
    wieldable.Comp.ModifiedDelay = wieldable.Comp.BaseDelay;
    this.Dirty<WieldDelayComponent>(wieldable);
  }

  private void OnGotEquippedHand(
    Entity<WieldableSpeedModifiersComponent> wieldable,
    ref GotEquippedHandEvent args)
  {
    this._movementSpeed.RefreshMovementSpeedModifiers(args.User);
  }

  private void OnGotUnequippedHand(
    Entity<WieldableSpeedModifiersComponent> wieldable,
    ref GotUnequippedHandEvent args)
  {
    this._movementSpeed.RefreshMovementSpeedModifiers(args.User);
  }

  private void OnRefreshMovementSpeedModifiers(
    Entity<WieldableSpeedModifiersComponent> wieldable,
    ref HeldRelayedEvent<RefreshMovementSpeedModifiersEvent> args)
  {
    args.Args.ModifySpeed(wieldable.Comp.ModifiedWalk, wieldable.Comp.ModifiedSprint);
  }

  public void RefreshSpeedModifiers(Entity<WieldableSpeedModifiersComponent?> wieldable)
  {
    wieldable.Comp = this.EnsureComp<WieldableSpeedModifiersComponent>((EntityUid) wieldable);
    float Walk = wieldable.Comp.Base;
    float Sprint = wieldable.Comp.Base;
    TransformComponent comp1;
    RMCArmorSpeedTierUserComponent comp2;
    if (this.TryComp(wieldable.Owner, out comp1) && comp1.ParentUid.Valid && this.TryComp<RMCArmorSpeedTierUserComponent>(comp1.ParentUid, out comp2))
    {
      switch (comp2.SpeedTier)
      {
        case "light":
          Walk = wieldable.Comp.Light;
          Sprint = wieldable.Comp.Light;
          break;
        case "medium":
          Walk = wieldable.Comp.Medium;
          Sprint = wieldable.Comp.Medium;
          break;
        case "heavy":
          Walk = wieldable.Comp.Heavy;
          Sprint = wieldable.Comp.Heavy;
          break;
      }
    }
    WieldableComponent comp3;
    if (!this.TryComp<WieldableComponent>(wieldable.Owner, out comp3) || !comp3.Wielded)
    {
      Walk = 1f;
      Sprint = 1f;
    }
    GetWieldableSpeedModifiersEvent args = new GetWieldableSpeedModifiersEvent(Walk, Sprint);
    this.RaiseLocalEvent<GetWieldableSpeedModifiersEvent>((EntityUid) wieldable, ref args);
    wieldable.Comp.ModifiedWalk = (double) args.Walk > 0.0 ? args.Walk : 0.0f;
    wieldable.Comp.ModifiedSprint = (double) args.Sprint > 0.0 ? args.Sprint : 0.0f;
    this.Dirty<WieldableSpeedModifiersComponent>(wieldable);
    this.RefreshModifiersOnParent(wieldable.Owner);
  }

  private void OnItemUnwielded(
    Entity<WieldableSpeedModifiersComponent> wieldable,
    ref ItemUnwieldedEvent args)
  {
    this.RefreshSpeedModifiers((Entity<WieldableSpeedModifiersComponent>) (wieldable.Owner, wieldable.Comp));
  }

  private void OnItemWielded(
    Entity<WieldableSpeedModifiersComponent> wieldable,
    ref ItemWieldedEvent args)
  {
    this.RefreshSpeedModifiers((Entity<WieldableSpeedModifiersComponent>) (wieldable.Owner, wieldable.Comp));
  }

  private void RefreshModifiersOnParent(EntityUid wieldableUid)
  {
    TransformComponent comp;
    if (!this.TryComp(wieldableUid, out comp) || !comp.ParentUid.Valid)
      return;
    EntityUid? activeItem = this._hands.GetActiveItem((Entity<HandsComponent>) comp.ParentUid);
    if (!activeItem.HasValue || activeItem.GetValueOrDefault() != wieldableUid)
      return;
    this._movementSpeed.RefreshMovementSpeedModifiers(comp.ParentUid);
  }

  private void OnGotEquippedHand(
    Entity<WieldDelayComponent> wieldable,
    ref GotEquippedHandEvent args)
  {
    this._useDelaySystem.SetLength((Entity<UseDelayComponent>) wieldable.Owner, wieldable.Comp.ModifiedDelay, "RMCWieldDelay");
    this._useDelaySystem.TryResetDelay(wieldable.Owner, id: "RMCWieldDelay");
  }

  private void OnUseInHand(Entity<WieldDelayComponent> wieldable, ref UseInHandEvent args)
  {
    UseDelayComponent comp;
    if (!this.TryComp<UseDelayComponent>(wieldable.Owner, out comp) || !this._useDelaySystem.IsDelayed((Entity<UseDelayComponent>) (wieldable.Owner, comp), "RMCWieldDelay"))
      return;
    args.Handled = true;
    UseDelayInfo info;
    if (!this._useDelaySystem.TryGetDelayInfo((Entity<UseDelayComponent>) (wieldable.Owner, comp), out info, "RMCWieldDelay"))
      return;
    this._popupSystem.PopupClient(this.Loc.GetString("rmc-wield-use-delay", ("seconds", (object) $"{(info.EndTime - this._timing.CurTime).TotalSeconds:F1}"), (nameof (wieldable), (object) wieldable.Owner)), args.User, new EntityUid?(args.User));
  }

  public void RefreshWieldDelay(Entity<WieldDelayComponent?> wieldable)
  {
    wieldable.Comp = this.EnsureComp<WieldDelayComponent>((EntityUid) wieldable);
    GetWieldDelayEvent args = new GetWieldDelayEvent(wieldable.Comp.BaseDelay);
    this.RaiseLocalEvent<GetWieldDelayEvent>((EntityUid) wieldable, ref args);
    wieldable.Comp.ModifiedDelay = args.Delay >= TimeSpan.Zero ? args.Delay : TimeSpan.Zero;
    this.Dirty<WieldDelayComponent>(wieldable);
  }

  private void OnItemWieldedWithDelay(
    Entity<WieldDelayComponent> wieldable,
    ref ItemWieldedEvent args)
  {
    TimeSpan modifiedDelay = wieldable.Comp.ModifiedDelay;
    BaseContainer container;
    if (this._container.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) ((EntityUid) wieldable, (TransformComponent) null), out container))
      modifiedDelay -= TimeSpan.FromSeconds(0.2) * (double) this._skills.GetSkill((Entity<SkillsComponent>) container.Owner, RMCWieldableSystem.WieldSkill);
    this._useDelaySystem.SetLength((Entity<UseDelayComponent>) wieldable.Owner, modifiedDelay, "RMCWieldDelay");
    this._useDelaySystem.TryResetDelay(wieldable.Owner, id: "RMCWieldDelay");
  }

  public void OnShotAttempt(Entity<WieldDelayComponent> wieldable, ref ShotAttemptedEvent args)
  {
    UseDelayComponent comp;
    UseDelayInfo info;
    if (!wieldable.Comp.PreventFiring || !this.TryComp<UseDelayComponent>(wieldable.Owner, out comp) || !this._useDelaySystem.IsDelayed((Entity<UseDelayComponent>) (wieldable.Owner, comp), "RMCWieldDelay") || !this._useDelaySystem.TryGetDelayInfo((Entity<UseDelayComponent>) (wieldable.Owner, comp), out info, "RMCWieldDelay"))
      return;
    args.Cancel();
    DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
    interpolatedStringHandler.AppendFormatted<double>((info.EndTime - this._timing.CurTime).TotalSeconds, "F1");
    interpolatedStringHandler.ToStringAndClear();
  }
}
