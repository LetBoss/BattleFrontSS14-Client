// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Controls.MonotoneCheckBox
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;

#nullable enable
namespace Content.Client.UserInterface.Controls;

public sealed class MonotoneCheckBox : CheckBox
{
  public const string StyleClassMonotoneCheckBox = "monotoneCheckBox";

  public MonotoneCheckBox() => ((Control) this.TextureRect).AddStyleClass("monotoneCheckBox");

  protected virtual void DrawModeChanged()
  {
    base.DrawModeChanged();
    ((Control) this).Modulate = ((BaseButton) this).Disabled ? Color.Gray : Color.White;
  }
}
