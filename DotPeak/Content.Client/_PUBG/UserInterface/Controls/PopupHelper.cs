// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.UserInterface.Controls.PopupHelper
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;
using System.Numerics;

#nullable enable
namespace Content.Client._PUBG.UserInterface.Controls;

public static class PopupHelper
{
  public static Popup OpenContextPopup(
    IUserInterfaceManager uiManager,
    Control parent,
    Control content,
    Vector2 size)
  {
    Popup popup = new Popup();
    ((Control) popup).AddChild(content);
    ((Control) uiManager.ModalRoot).AddChild((Control) popup);
    Vector2 vector2 = Vector2i.op_Implicit(Vector2i.op_Subtraction(parent.GlobalPixelPosition, ((Control) uiManager.ModalRoot).GlobalPixelPosition)) + new Vector2(0.0f, (float) parent.PixelHeight);
    popup.Open(new UIBox2?(UIBox2.FromDimensions(vector2, size)), new Vector2?(), new Vector2?());
    return popup;
  }
}
