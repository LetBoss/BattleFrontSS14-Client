// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Controls.NanoHeading
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;

#nullable enable
namespace Content.Client.UserInterface.Controls;

public sealed class NanoHeading : Container
{
  private readonly Label _label;
  private readonly PanelContainer _panel;

  public NanoHeading()
  {
    PanelContainer panelContainer = new PanelContainer();
    Control.OrderedChildCollection children = ((Control) panelContainer).Children;
    Label label1 = new Label();
    ((Control) label1).StyleClasses.Add("LabelHeading");
    Label label2 = label1;
    this._label = label1;
    Label label3 = label2;
    children.Add((Control) label3);
    this._panel = panelContainer;
    ((Control) this).AddChild((Control) this._panel);
    ((Control) this).HorizontalAlignment = (Control.HAlignment) 1;
  }

  public string? Text
  {
    get => this._label.Text;
    set => this._label.Text = value;
  }
}
