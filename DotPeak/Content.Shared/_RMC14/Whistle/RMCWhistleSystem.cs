// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Whistle.RMCWhistleSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Sound;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Interaction.Events;
using Content.Shared.Inventory;
using Content.Shared.Timing;
using Content.Shared.Whistle;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared._RMC14.Whistle;

public sealed class RMCWhistleSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedActionsSystem _actions;
  [Dependency]
  private UseDelaySystem _useDelay;
  [Dependency]
  private WhistleSystem _whistle;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<RMCWhistleComponent, UseInHandEvent>(new EntityEventRefHandler<RMCWhistleComponent, UseInHandEvent>(this.OnUseInHand));
    this.SubscribeLocalEvent<RMCWhistleComponent, GetItemActionsEvent>(new EntityEventRefHandler<RMCWhistleComponent, GetItemActionsEvent>(this.OnGetItemActions));
    this.SubscribeLocalEvent<RMCWhistleComponent, SoundActionEvent>(new EntityEventRefHandler<RMCWhistleComponent, SoundActionEvent>(this.OnWhistleAction));
  }

  private void OnGetItemActions(Entity<RMCWhistleComponent> ent, ref GetItemActionsEvent args)
  {
    if (args.SlotFlags.GetValueOrDefault() == SlotFlags.POCKET)
      return;
    args.AddAction(ref ent.Comp.Action, (string) ent.Comp.ActionId);
  }

  public void OnWhistleAction(Entity<RMCWhistleComponent> ent, ref SoundActionEvent args)
  {
    if (!this._timing.IsFirstTimePredicted || args.Handled)
      return;
    this.TryWhistle(ent, args.Performer);
    args.Handled = true;
  }

  public void OnUseInHand(Entity<RMCWhistleComponent> ent, ref UseInHandEvent args)
  {
    this.TryWhistle(ent, args.User);
    args.Handled = true;
  }

  public void TryWhistle(Entity<RMCWhistleComponent> ent, EntityUid user)
  {
    this._whistle.TryMakeLoudWhistle((EntityUid) ent, user);
    UseDelayComponent comp;
    if (!this.TryComp<UseDelayComponent>((EntityUid) ent, out comp))
      return;
    SharedActionsSystem actions = this._actions;
    EntityUid? action1 = ent.Comp.Action;
    Entity<ActionComponent>? action2 = action1.HasValue ? new Entity<ActionComponent>?((Entity<ActionComponent>) action1.GetValueOrDefault()) : new Entity<ActionComponent>?();
    TimeSpan delay = comp.Delay;
    actions.SetCooldown(action2, delay);
    this._useDelay.SetLength((Entity<UseDelayComponent>) ent.Owner, comp.Delay);
    this._useDelay.TryResetDelay((Entity<UseDelayComponent>) (ent.Owner, comp));
  }
}
