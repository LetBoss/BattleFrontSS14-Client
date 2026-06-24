// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Weapons.Ranged.Sniper.RMCSpottingSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Gameplay;
using Content.Shared._RMC14.Rangefinder.Spotting;
using Content.Shared._RMC14.Weapons.Common;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Client.State;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Player;
using System;

#nullable enable
namespace Content.Client._RMC14.Weapons.Ranged.Sniper;

public sealed class RMCSpottingSystem : SharedRMCSpottingSystem
{
  [Dependency]
  private IEyeManager _eyeManager;
  [Dependency]
  private IInputManager _inputManager;
  [Dependency]
  private IPlayerManager _player;
  [Dependency]
  private IStateManager _state;

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<SpottingComponent, UniqueActionEvent>(new EntityEventRefHandler<SpottingComponent, UniqueActionEvent>((object) this, __methodptr(OnUniqueAction)), (Type[]) null, (Type[]) null);
    this.SubscribeLocalEvent<RequestSpotEvent>(new EntitySessionEventHandler<RequestSpotEvent>(this.OnSpotRequest), (Type[]) null, (Type[]) null);
  }

  private void OnUniqueAction(Entity<SpottingComponent> ent, ref UniqueActionEvent args)
  {
    if (!this.Timing.IsFirstTimePredicted || args.Handled || !((ISharedPlayerManager) this._player).LocalEntity.HasValue || !ent.Comp.Activated)
      return;
    MapCoordinates map = this._eyeManager.PixelToMap(this._inputManager.MouseScreenPosition);
    NetEntity? nullable = new NetEntity?();
    if (this._state.CurrentState is GameplayStateBase currentState)
      nullable = this.GetNetEntity(currentState.GetClickedEntity(map), (MetaDataComponent) null);
    if (((ISharedPlayerManager) this._player).LocalSession == null || !nullable.HasValue)
      return;
    this.RaisePredictiveEvent<RequestSpotEvent>(new RequestSpotEvent()
    {
      Target = nullable.Value,
      User = this.GetNetEntity(args.UserUid, (MetaDataComponent) null),
      SpottingTool = this.GetNetEntity(ent.Owner, (MetaDataComponent) null)
    });
    args.Handled = true;
  }

  private void OnSpotRequest(RequestSpotEvent ev, EntitySessionEventArgs args)
  {
    this.SpotRequested(ev.SpottingTool, ev.User, ev.Target);
  }
}
