// Decompiled with JetBrains decompiler
// Type: Content.Client.Shuttles.BUI.ShuttleConsoleBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Shuttles.UI;
using Content.Shared.Shuttles.BUIStates;
using Content.Shared.Shuttles.Events;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using System;

#nullable enable
namespace Content.Client.Shuttles.BUI;

public sealed class ShuttleConsoleBoundUserInterface(EntityUid owner, Enum uiKey) : 
  BoundUserInterface(owner, uiKey)
{
  [Robust.Shared.ViewVariables.ViewVariables]
  private ShuttleConsoleWindow? _window;

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<ShuttleConsoleWindow>((BoundUserInterface) this);
    this._window.RequestFTL += new Action<MapCoordinates, Angle>(this.OnFTLRequest);
    this._window.RequestBeaconFTL += new Action<NetEntity, Angle>(this.OnFTLBeaconRequest);
    this._window.DockRequest += new Action<NetEntity, NetEntity>(this.OnDockRequest);
    this._window.UndockRequest += new Action<NetEntity>(this.OnUndockRequest);
  }

  private void OnUndockRequest(NetEntity entity)
  {
    this.SendMessage((BoundUserInterfaceMessage) new UndockRequestMessage()
    {
      DockEntity = entity
    });
  }

  private void OnDockRequest(NetEntity entity, NetEntity target)
  {
    this.SendMessage((BoundUserInterfaceMessage) new DockRequestMessage()
    {
      DockEntity = entity,
      TargetDockEntity = target
    });
  }

  private void OnFTLBeaconRequest(NetEntity ent, Angle angle)
  {
    this.SendMessage((BoundUserInterfaceMessage) new ShuttleConsoleFTLBeaconMessage()
    {
      Beacon = ent,
      Angle = angle
    });
  }

  private void OnFTLRequest(MapCoordinates obj, Angle angle)
  {
    this.SendMessage((BoundUserInterfaceMessage) new ShuttleConsoleFTLPositionMessage()
    {
      Coordinates = obj,
      Angle = angle
    });
  }

  protected virtual void Dispose(bool disposing)
  {
    base.Dispose(disposing);
    if (!disposing)
      return;
    ((Control) this._window)?.Orphan();
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    if (!(state is ShuttleBoundUserInterfaceState cState))
      return;
    this._window?.UpdateState(this.Owner, cState);
  }
}
