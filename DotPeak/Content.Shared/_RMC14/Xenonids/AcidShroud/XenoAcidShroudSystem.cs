// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.AcidShroud.XenoAcidShroudSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Xenonids.GasToggle;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared.Actions.Components;
using Content.Shared.Coordinates;
using Content.Shared.DoAfter;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.AcidShroud;

public sealed class XenoAcidShroudSystem : EntitySystem
{
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private SharedRMCActionsSystem _rmcActions;
  [Dependency]
  private SharedXenoHiveSystem _hive;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<XenoAcidShroudComponent, XenoAcidShroudActionEvent>(new EntityEventRefHandler<XenoAcidShroudComponent, XenoAcidShroudActionEvent>(this.OnAcidShroudAction));
    this.SubscribeLocalEvent<XenoAcidShroudComponent, DoAfterAttemptEvent<XenoAcidShroudDoAfterEvent>>(new EntityEventRefHandler<XenoAcidShroudComponent, DoAfterAttemptEvent<XenoAcidShroudDoAfterEvent>>(this.OnAcidShroudDoAfterAttempt));
    this.SubscribeLocalEvent<XenoAcidShroudComponent, XenoAcidShroudDoAfterEvent>(new EntityEventRefHandler<XenoAcidShroudComponent, XenoAcidShroudDoAfterEvent>(this.OnAcidShroudDoAfter));
    this.SubscribeLocalEvent<XenoAcidShroudComponent, XenoGasToggleActionEvent>(new EntityEventRefHandler<XenoAcidShroudComponent, XenoGasToggleActionEvent>(this.OnToggleType));
  }

  private void OnAcidShroudAction(
    Entity<XenoAcidShroudComponent> ent,
    ref XenoAcidShroudActionEvent args)
  {
    args.Handled = true;
    XenoAcidShroudDoAfterEvent @event = new XenoAcidShroudDoAfterEvent();
    this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, (EntityUid) ent, ent.Comp.DoAfter, (DoAfterEvent) @event, new EntityUid?((EntityUid) ent), new EntityUid?((EntityUid) args.Action))
    {
      BreakOnMove = true
    });
    this._rmcActions.DisableSharedCooldownEvents((Entity<ActionSharedCooldownComponent>) args.Action.Owner, (EntityUid) ent);
  }

  private void OnAcidShroudDoAfterAttempt(
    Entity<XenoAcidShroudComponent> ent,
    ref DoAfterAttemptEvent<XenoAcidShroudDoAfterEvent> args)
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

  private void OnAcidShroudDoAfter(
    Entity<XenoAcidShroudComponent> ent,
    ref XenoAcidShroudDoAfterEvent args)
  {
    EntityUid? target = args.Target;
    if (!target.HasValue)
      return;
    EntityUid valueOrDefault = target.GetValueOrDefault();
    this._rmcActions.EnableSharedCooldownEvents((Entity<ActionSharedCooldownComponent>) valueOrDefault, (EntityUid) ent);
    if (args.Cancelled || args.Handled)
      return;
    args.Handled = true;
    EntityUid dest = this.SpawnAtPosition((string) ent.Comp.Spawn, ent.Owner.ToCoordinates());
    this._hive.SetSameHive((Entity<HiveMemberComponent>) ent.Owner, (Entity<HiveMemberComponent>) dest);
    this._rmcActions.ActivateSharedCooldown((Entity<ActionSharedCooldownComponent>) valueOrDefault, (EntityUid) ent);
  }

  private void OnToggleType(Entity<XenoAcidShroudComponent> ent, ref XenoGasToggleActionEvent args)
  {
    if (ent.Comp.Gases.Length == 0)
      return;
    int num = Array.IndexOf<EntProtoId>(ent.Comp.Gases, ent.Comp.Spawn);
    int index = num == -1 || num >= ent.Comp.Gases.Length - 1 ? 0 : num + 1;
    ent.Comp.Spawn = ent.Comp.Gases[index];
    this.Dirty<XenoAcidShroudComponent>(ent);
  }
}
