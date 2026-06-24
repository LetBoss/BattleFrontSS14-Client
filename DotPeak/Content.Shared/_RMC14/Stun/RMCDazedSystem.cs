// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Stun.RMCDazedSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Actions;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Charges.Components;
using Content.Shared.Charges.Systems;
using Content.Shared.Speech.EntitySystems;
using Content.Shared.StatusEffect;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Player;
using System;

#nullable enable
namespace Content.Shared._RMC14.Stun;

public sealed class RMCDazedSystem : EntitySystem
{
  [Dependency]
  private SharedChargesSystem _charges;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private StatusEffectsSystem _statusEffect;
  [Dependency]
  private SharedActionsSystem _actions;
  [Dependency]
  private ISharedPlayerManager _playerManager;
  [Dependency]
  private SharedStutteringSystem _stutter;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<RMCDazedComponent, DazedEvent>(new EntityEventRefHandler<RMCDazedComponent, DazedEvent>(this.OnDazed));
    this.SubscribeLocalEvent<RMCDazedComponent, ComponentShutdown>(new EntityEventRefHandler<RMCDazedComponent, ComponentShutdown>(this.OnDazedEnd));
  }

  private void OnDazed(Entity<RMCDazedComponent> ent, ref DazedEvent args)
  {
    foreach ((EntityUid entityUid, ActionComponent _) in this._actions.GetActions((EntityUid) ent))
    {
      if (this.TryComp<RMCDazeableActionComponent>(entityUid, out RMCDazeableActionComponent _))
      {
        this._actions.SetEnabled(new Entity<ActionComponent>?((Entity<ActionComponent>) entityUid), false);
        if (this.HasComp<LimitedChargesComponent>(entityUid))
          this._charges.SetCharges((Entity<LimitedChargesComponent>) entityUid, 0);
      }
    }
  }

  private void OnDazedEnd(Entity<RMCDazedComponent> ent, ref ComponentShutdown args)
  {
    foreach ((EntityUid entityUid, ActionComponent _) in this._actions.GetActions((EntityUid) ent))
    {
      if (this.TryComp<RMCDazeableActionComponent>(entityUid, out RMCDazeableActionComponent _))
      {
        this._actions.SetEnabled(new Entity<ActionComponent>?((Entity<ActionComponent>) entityUid), true);
        this._charges.ResetCharges((Entity<LimitedChargesComponent>) entityUid);
      }
    }
    ICommonSession session;
    if (!this._net.IsServer || !this._playerManager.TryGetSessionByEntity(ent.Owner, out session))
      return;
    this.RaiseNetworkEvent((EntityEventArgs) new DazedComponentShutdownEvent(), session.Channel);
  }

  public bool TryDaze(
    EntityUid uid,
    TimeSpan time,
    bool refresh = false,
    StatusEffectsComponent? status = null,
    bool stutter = false)
  {
    if (!this.Resolve<StatusEffectsComponent>(uid, ref status, false) || time <= TimeSpan.Zero || !this._statusEffect.TryAddStatusEffect<RMCDazedComponent>(uid, "Dazed", time, refresh, status))
      return false;
    if (stutter)
      this._stutter.DoStutter(uid, time, true);
    DazedEvent args = new DazedEvent(time);
    this.RaiseLocalEvent<DazedEvent>(uid, ref args);
    return true;
  }
}
