// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Controls.Placeholder
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;

#nullable enable
namespace Content.Client.UserInterface.Controls;

public sealed class Placeholder : PanelContainer
{
  public const string StyleClassPlaceholderText = "PlaceholderText";
  private readonly Label _label;

  public string? PlaceholderText
  {
    get => this._label.Text;
    set => this._label.Text = value;
  }

  public Placeholder()
  {
    Label label = new Label();
    ((Control) label).VerticalAlignment = (Control.VAlignment) 0;
    label.Align = (Label.AlignMode) 1;
    label.VAlign = (Label.VAlignMode) 1;
    this._label = label;
    ((Control) this._label).AddStyleClass(nameof (PlaceholderText));
    ((Control) this).AddChild((Control) this._label);
  }
}
