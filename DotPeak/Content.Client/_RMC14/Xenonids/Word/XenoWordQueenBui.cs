// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Xenonids.Word.XenoWordQueenBui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Xenonids.Word;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.Utility;
using System;

#nullable enable
namespace Content.Client._RMC14.Xenonids.Word;

public sealed class XenoWordQueenBui(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  [Robust.Shared.ViewVariables.ViewVariables]
  private XenoWordQueenWindow? _window;

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<XenoWordQueenWindow>((BoundUserInterface) this);
    ((BaseButton) this._window.SendButton).OnPressed += new Action<BaseButton.ButtonEventArgs>(this.Send);
  }

  private void Send(BaseButton.ButtonEventArgs args)
  {
    if (this._window == null)
      return;
    string text = Rope.Collapse(this._window.Text.TextRope);
    if (string.IsNullOrWhiteSpace(text))
      return;
    this.SendPredictedMessage((BoundUserInterfaceMessage) new XenoWordQueenBuiMsg(text));
    ((BaseWindow) this._window).Close();
  }
}
