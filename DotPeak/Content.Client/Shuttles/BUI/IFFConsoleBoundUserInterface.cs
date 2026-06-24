// Decompiled with JetBrains decompiler
// Type: Content.Client.Shuttles.BUI.IFFConsoleBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Shuttles.UI;
using Content.Shared.Shuttles.BUIStates;
using Content.Shared.Shuttles.Events;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Shuttles.BUI;

public sealed class IFFConsoleBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  [Robust.Shared.ViewVariables.ViewVariables]
  private IFFConsoleWindow? _window;

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindowCenteredLeft<IFFConsoleWindow>((BoundUserInterface) this);
    this._window.ShowIFF += new Action<bool>(this.SendIFFMessage);
    this._window.ShowVessel += new Action<bool>(this.SendVesselMessage);
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    if (!(state is IFFConsoleBoundUserInterfaceState state1))
      return;
    this._window?.UpdateState(state1);
  }

  private void SendIFFMessage(bool obj)
  {
    this.SendMessage((BoundUserInterfaceMessage) new IFFShowIFFMessage()
    {
      Show = obj
    });
  }

  private void SendVesselMessage(bool obj)
  {
    this.SendMessage((BoundUserInterfaceMessage) new IFFShowVesselMessage()
    {
      Show = obj
    });
  }

  protected virtual void Dispose(bool disposing)
  {
    base.Dispose(disposing);
    if (!disposing)
      return;
    this._window?.Close();
    this._window = (IFFConsoleWindow) null;
  }
}
