// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Systems.Info.CloseAllWindowsUIController
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
using System.Collections.Generic;

#nullable enable
namespace Content.Client.UserInterface.Systems.Info;

public sealed class CloseAllWindowsUIController : UIController
{
  [Dependency]
  private IInputManager _inputManager;
  [Dependency]
  private IUserInterfaceManager _uiManager;

  public virtual void Initialize()
  {
    // ISSUE: method pointer
    this._inputManager.SetInputCommand(EngineKeyFunctions.WindowCloseAll, InputCmdHandler.FromDelegate(new StateInputCmdDelegate((object) this, __methodptr(\u003CInitialize\u003Eb__2_0)), (StateInputCmdDelegate) null, true, true));
  }

  private void CloseAllWindows()
  {
    foreach (Control control in new List<Control>((IEnumerable<Control>) ((Control) this._uiManager.WindowRoot).Children))
    {
      if (control is BaseWindow)
        ((BaseWindow) control).Close();
    }
  }
}
