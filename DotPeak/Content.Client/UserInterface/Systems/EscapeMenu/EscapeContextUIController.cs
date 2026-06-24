// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Systems.EscapeMenu.EscapeContextUIController
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.UserInterface.Systems.Info;
using Content.Shared.Input;
using Robust.Client.Input;
using Robust.Client.UserInterface.Controllers;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Client.UserInterface.Systems.EscapeMenu;

public sealed class EscapeContextUIController : UIController
{
  [Dependency]
  private IInputManager _inputManager;
  [Dependency]
  private CloseRecentWindowUIController _closeRecentWindowUIController;
  [Dependency]
  private EscapeUIController _escapeUIController;

  public virtual void Initialize()
  {
    // ISSUE: method pointer
    this._inputManager.SetInputCommand(ContentKeyFunctions.EscapeContext, InputCmdHandler.FromDelegate(new StateInputCmdDelegate((object) this, __methodptr(\u003CInitialize\u003Eb__3_0)), (StateInputCmdDelegate) null, true, true));
  }

  private void CloseWindowOrOpenGameMenu()
  {
    if (this._closeRecentWindowUIController.HasClosableWindow())
      this._closeRecentWindowUIController.CloseMostRecentWindow();
    else
      this._escapeUIController.ToggleWindow();
  }
}
