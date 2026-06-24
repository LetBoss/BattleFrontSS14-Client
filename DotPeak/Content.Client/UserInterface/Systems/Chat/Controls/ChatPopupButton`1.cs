// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Systems.Chat.Controls.ChatPopupButton`1
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client.UserInterface.Systems.Chat.Controls;

public abstract class ChatPopupButton<TPopup> : Button where TPopup : Robust.Client.UserInterface.Controls.Popup, new()
{
  private readonly IGameTiming _gameTiming;
  public readonly TPopup Popup;
  private uint _frameLastPopupChanged;

  protected ChatPopupButton()
  {
    this._gameTiming = IoCManager.Resolve<IGameTiming>();
    ((BaseButton) this).ToggleMode = true;
    ((BaseButton) this).OnToggled += new Action<BaseButton.ButtonToggledEventArgs>(this.OnButtonToggled);
    this.Popup = ((Control) this).UserInterfaceManager.CreatePopup<TPopup>();
    ((Control) (object) this.Popup).OnVisibilityChanged += new Action<Control>(this.OnPopupVisibilityChanged);
  }

  protected virtual void KeyBindDown(GUIBoundKeyEventArgs args)
  {
    if ((int) this._frameLastPopupChanged == (int) this._gameTiming.CurFrame)
      return;
    ((BaseButton) this).KeyBindDown(args);
  }

  protected abstract UIBox2 GetPopupPosition();

  private void OnButtonToggled(BaseButton.ButtonToggledEventArgs args)
  {
    if (args.Pressed)
      this.Popup.Open(new UIBox2?(this.GetPopupPosition()), new Vector2?(), new Vector2?());
    else
      this.Popup.Close();
  }

  private void OnPopupVisibilityChanged(Control control)
  {
    ((BaseButton) this).Pressed = control.Visible;
    this._frameLastPopupChanged = this._gameTiming.CurFrame;
  }
}
