// Decompiled with JetBrains decompiler
// Type: Content.Client.Shuttles.Systems.ShuttleConsoleSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Input;
using Content.Shared.Shuttles.Components;
using Content.Shared.Shuttles.Systems;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using System;

#nullable enable
namespace Content.Client.Shuttles.Systems;

public sealed class ShuttleConsoleSystem : SharedShuttleConsoleSystem
{
  [Dependency]
  private IInputManager _input;
  [Dependency]
  private IPlayerManager _playerManager;

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<PilotComponent, ComponentHandleState>(new ComponentEventRefHandler<PilotComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    IInputCmdContext iinputCmdContext = this._input.Contexts.New("shuttle", "common");
    iinputCmdContext.AddFunction(ContentKeyFunctions.ShuttleStrafeUp);
    iinputCmdContext.AddFunction(ContentKeyFunctions.ShuttleStrafeDown);
    iinputCmdContext.AddFunction(ContentKeyFunctions.ShuttleStrafeLeft);
    iinputCmdContext.AddFunction(ContentKeyFunctions.ShuttleStrafeRight);
    iinputCmdContext.AddFunction(ContentKeyFunctions.ShuttleRotateLeft);
    iinputCmdContext.AddFunction(ContentKeyFunctions.ShuttleRotateRight);
    iinputCmdContext.AddFunction(ContentKeyFunctions.ShuttleBrake);
  }

  public virtual void Shutdown()
  {
    base.Shutdown();
    this._input.Contexts.Remove("shuttle");
  }

  protected override void HandlePilotShutdown(
    EntityUid uid,
    PilotComponent component,
    ComponentShutdown args)
  {
    base.HandlePilotShutdown(uid, component, args);
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    EntityUid entityUid = uid;
    if ((localEntity.HasValue ? (EntityUid.op_Inequality(localEntity.GetValueOrDefault(), entityUid) ? 1 : 0) : 1) != 0)
      return;
    this._input.Contexts.SetActiveContext("human");
  }

  private void OnHandleState(
    EntityUid uid,
    PilotComponent component,
    ref ComponentHandleState args)
  {
    if (!(((ComponentHandleState) ref args).Current is SharedShuttleConsoleSystem.PilotComponentState current))
      return;
    EntityUid? nullable = this.EnsureEntity<PilotComponent>(current.Console, uid);
    if (!nullable.HasValue)
    {
      component.Console = new EntityUid?();
      this._input.Contexts.SetActiveContext("human");
    }
    else if (!this.HasComp<ShuttleConsoleComponent>(nullable))
    {
      this.Log.Warning($"Unable to set Helmsman console to {nullable}");
    }
    else
    {
      component.Console = nullable;
      this.ActionBlockerSystem.UpdateCanMove(uid);
      this._input.Contexts.SetActiveContext("shuttle");
    }
  }
}
