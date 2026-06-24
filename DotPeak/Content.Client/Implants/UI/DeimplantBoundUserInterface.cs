// Decompiled with JetBrains decompiler
// Type: Content.Client.Implants.UI.DeimplantBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Implants;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.Implants.UI;

public sealed class DeimplantBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  [Robust.Shared.ViewVariables.ViewVariables]
  private DeimplantChoiceWindow? _window;

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<DeimplantChoiceWindow>((BoundUserInterface) this);
    this._window.OnImplantChange += (Action<string>) (implant => this.SendMessage((BoundUserInterfaceMessage) new DeimplantChangeVerbMessage(implant)));
  }

  public void UpdateState(Dictionary<string, string> implantList, string? implant)
  {
    if (this._window == null)
      return;
    this._window.UpdateImplantList(implantList);
    this._window.UpdateState(implant);
  }
}
