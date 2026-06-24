// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Mobs.CMMobStateSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Sprite;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Player;
using System;

#nullable enable
namespace Content.Shared._RMC14.Mobs;

public sealed class CMMobStateSystem : EntitySystem
{
  [Dependency]
  private IConsoleHost _host;
  [Dependency]
  private MobStateSystem _mobState;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedRMCSpriteSystem _rmcSprite;
  [Dependency]
  private SharedUserInterfaceSystem _ui;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<MobStateActionsComponent, CMGhostActionEvent>(new EntityEventRefHandler<MobStateActionsComponent, CMGhostActionEvent>(this.OnMobStateActionsGhost));
    this.SubscribeLocalEvent<RMCMobStateDrawDepthComponent, GetDrawDepthEvent>(new EntityEventRefHandler<RMCMobStateDrawDepthComponent, GetDrawDepthEvent>(this.OnMobStateDrawDepth));
    this.SubscribeLocalEvent<RMCMobStateDrawDepthComponent, MobStateChangedEvent>(new EntityEventRefHandler<RMCMobStateDrawDepthComponent, MobStateChangedEvent>(this.OnMobStateChanged));
    this.Subs.BuiEvents<MobStateActionsComponent>((object) CMMobStateActionsUI.Key, (BoundUserInterfaceRegisterExt.BuiEventSubscriber<MobStateActionsComponent>) (subs => subs.Event<CMGhostActionBuiMsg>(new EntityEventRefHandler<MobStateActionsComponent, CMGhostActionBuiMsg>(this.OnGhostActionBuiMsg))));
  }

  private void OnMobStateActionsGhost(
    Entity<MobStateActionsComponent> ent,
    ref CMGhostActionEvent args)
  {
    if (args.Handled)
      return;
    if (this._mobState.IsDead((EntityUid) ent))
    {
      ActorComponent comp;
      if (!this._net.IsServer || !this.TryComp<ActorComponent>((EntityUid) ent, out comp))
        return;
      this._host.ExecuteCommand(comp.PlayerSession, "ghost");
    }
    else
    {
      args.Handled = true;
      this._ui.OpenUi((Entity<UserInterfaceComponent>) ent.Owner, (Enum) CMMobStateActionsUI.Key, new EntityUid?((EntityUid) ent));
    }
  }

  private void OnMobStateDrawDepth(
    Entity<RMCMobStateDrawDepthComponent> ent,
    ref GetDrawDepthEvent args)
  {
    MobStateComponent comp;
    Content.Shared.DrawDepth.DrawDepth drawDepth;
    if (!this.TryComp<MobStateComponent>((EntityUid) ent, out comp) || args.DrawDepth != ent.Comp.Default || !ent.Comp.DrawDepths.TryGetValue(comp.CurrentState, out drawDepth))
      return;
    args.DrawDepth = drawDepth;
  }

  private void OnMobStateChanged(
    Entity<RMCMobStateDrawDepthComponent> ent,
    ref MobStateChangedEvent args)
  {
    int num = (int) this._rmcSprite.UpdateDrawDepth((EntityUid) ent);
  }

  private void OnGhostActionBuiMsg(
    Entity<MobStateActionsComponent> ent,
    ref CMGhostActionBuiMsg args)
  {
    EntityUid? entity;
    if (!this._mobState.IsIncapacitated((EntityUid) ent) || !this.TryGetEntity(args.Entity, out entity))
      return;
    EntityUid? nullable = entity;
    EntityUid actor = args.Actor;
    if ((nullable.HasValue ? (nullable.GetValueOrDefault() != actor ? 1 : 0) : 1) != 0)
      return;
    this._ui.CloseUi((Entity<UserInterfaceComponent>) ent.Owner, (Enum) CMMobStateActionsUI.Key);
    ActorComponent comp;
    if (!this._net.IsServer || !this.TryComp<ActorComponent>(args.Actor, out comp))
      return;
    this._host.ExecuteCommand(comp.PlayerSession, "ghost");
  }
}
