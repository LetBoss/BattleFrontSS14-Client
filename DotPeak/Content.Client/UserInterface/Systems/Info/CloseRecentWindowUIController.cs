// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Systems.Info.CloseRecentWindowUIController
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Input;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Input;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.UserInterface.Systems.Info;

public sealed class CloseRecentWindowUIController : UIController
{
  [Dependency]
  private IInputManager _inputManager;
  [Dependency]
  private IUserInterfaceManager _uiManager;
  private List<BaseWindow> recentlyInteractedWindows = new List<BaseWindow>();

  public virtual void Initialize()
  {
    this._uiManager.OnKeyBindDown += new Action<Control>(this.OnKeyBindDown);
    ((Control) this._uiManager.WindowRoot).OnChildAdded += new Action<Control>(this.OnRootChildAdded);
    // ISSUE: method pointer
    this._inputManager.SetInputCommand(EngineKeyFunctions.WindowCloseRecent, InputCmdHandler.FromDelegate(new StateInputCmdDelegate((object) this, __methodptr(\u003CInitialize\u003Eb__3_0)), (StateInputCmdDelegate) null, true, true));
  }

  public void CloseMostRecentWindow()
  {
    for (int index = this.recentlyInteractedWindows.Count - 1; index >= 0; --index)
    {
      BaseWindow interactedWindow = this.recentlyInteractedWindows[index];
      this.recentlyInteractedWindows.RemoveAt(index);
      if (interactedWindow.IsOpen)
      {
        interactedWindow.Close();
        break;
      }
    }
  }

  private void OnKeyBindDown(Control control)
  {
    BaseWindow windowForControl = this.GetWindowForControl(control);
    if (windowForControl == null)
      return;
    this.SetMostRecentlyInteractedWindow(windowForControl);
  }

  public void SetMostRecentlyInteractedWindow(BaseWindow window)
  {
    for (int index = this.recentlyInteractedWindows.Count - 1; index >= 0; --index)
    {
      if (this.recentlyInteractedWindows[index] == window)
      {
        if (index == this.recentlyInteractedWindows.Count - 1)
          return;
        this.recentlyInteractedWindows.RemoveAt(index);
        break;
      }
    }
    this.recentlyInteractedWindows.Add(window);
  }

  private BaseWindow? GetWindowForControl(Control? control)
  {
    if (control == null)
      return (BaseWindow) null;
    return control is BaseWindow ? (BaseWindow) control : this.GetWindowForControl(control.Parent);
  }

  private void OnRootChildAdded(Control control)
  {
    if (!(control is BaseWindow))
      return;
    this.SetMostRecentlyInteractedWindow((BaseWindow) control);
  }

  public bool HasClosableWindow()
  {
    for (int index = this.recentlyInteractedWindows.Count - 1; index >= 0; --index)
    {
      if (this.recentlyInteractedWindows[index].IsOpen)
        return true;
    }
    return false;
  }
}
