// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Input.RMCInputSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.CCVar;
using Content.Shared.Movement.Components;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Player;
using System;

#nullable enable
namespace Content.Shared._RMC14.Input;

public sealed class RMCInputSystem : EntitySystem
{
  [Dependency]
  private IConfigurationManager _config;
  [Dependency]
  private INetManager _net;
  private bool _activeInputMoverEnabled;
  private Robust.Shared.GameObjects.EntityQuery<ActorComponent> _actorQuery;

  public override void Initialize()
  {
    this._actorQuery = this.GetEntityQuery<ActorComponent>();
    this.SubscribeLocalEvent<ActiveInputMoverComponent, MapInitEvent>(new EntityEventRefHandler<ActiveInputMoverComponent, MapInitEvent>(this.OnActiveMapInit));
    this.SubscribeLocalEvent<ActiveInputMoverComponent, PlayerAttachedEvent>(new EntityEventRefHandler<ActiveInputMoverComponent, PlayerAttachedEvent>(this.OnActiveAttached));
    this.SubscribeLocalEvent<ActiveInputMoverComponent, PlayerDetachedEvent>(new EntityEventRefHandler<ActiveInputMoverComponent, PlayerDetachedEvent>(this.OnActiveDetached));
    this.Subs.CVar<bool>(this._config, RMCCVars.RMCActiveInputMoverEnabled, (Action<bool>) (v => this._activeInputMoverEnabled = v), true);
  }

  private void OnActiveMapInit(Entity<ActiveInputMoverComponent> ent, ref MapInitEvent args)
  {
    if (!this._activeInputMoverEnabled || this._net.IsClient)
      return;
    if (this._actorQuery.HasComp((EntityUid) ent))
      this.EnsureComp<InputMoverComponent>((EntityUid) ent);
    else
      this.RemCompDeferred<InputMoverComponent>((EntityUid) ent);
  }

  private void OnActiveAttached(Entity<ActiveInputMoverComponent> ent, ref PlayerAttachedEvent args)
  {
    if (!this._activeInputMoverEnabled)
      return;
    this.EnsureComp<InputMoverComponent>((EntityUid) ent);
  }

  private void OnActiveDetached(Entity<ActiveInputMoverComponent> ent, ref PlayerDetachedEvent args)
  {
    if (!this._activeInputMoverEnabled)
      return;
    this.RemCompDeferred<InputMoverComponent>((EntityUid) ent);
  }
}
