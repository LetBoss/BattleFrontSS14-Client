// Decompiled with JetBrains decompiler
// Type: Content.Client.Administration.UI.CustomControls.UICommandButton
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client.Administration.UI.CustomControls;

public sealed class UICommandButton : CommandButton
{
  private DefaultWindow? _window;

  public Type? WindowType { get; set; }

  protected override void Execute(BaseButton.ButtonEventArgs obj)
  {
    if (this.WindowType == (Type) null)
      return;
    this._window = (DefaultWindow) IoCManager.Resolve<IDynamicTypeFactory>().CreateInstance(this.WindowType, false, true);
    ((BaseWindow) this._window)?.OpenCentered();
  }
}
