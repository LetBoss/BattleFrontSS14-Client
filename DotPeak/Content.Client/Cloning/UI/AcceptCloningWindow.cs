// Decompiled with JetBrains decompiler
// Type: Content.Client.Cloning.UI.AcceptCloningWindow
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Localization;
using System.Numerics;

#nullable enable
namespace Content.Client.Cloning.UI;

public sealed class AcceptCloningWindow : DefaultWindow
{
  public readonly Button DenyButton;
  public readonly Button AcceptButton;

  public AcceptCloningWindow()
  {
    this.Title = Loc.GetString("accept-cloning-window-title");
    Control contents = this.Contents;
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
    Control.OrderedChildCollection children1 = ((Control) boxContainer1).Children;
    BoxContainer boxContainer2 = new BoxContainer();
    boxContainer2.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer2).Children.Add((Control) new Label()
    {
      Text = Loc.GetString("accept-cloning-window-prompt-text-part")
    });
    Control.OrderedChildCollection children2 = ((Control) boxContainer2).Children;
    BoxContainer boxContainer3 = new BoxContainer();
    boxContainer3.Orientation = (BoxContainer.LayoutOrientation) 0;
    boxContainer3.Align = (BoxContainer.AlignMode) 1;
    Control.OrderedChildCollection children3 = ((Control) boxContainer3).Children;
    Button button1 = new Button();
    button1.Text = Loc.GetString("accept-cloning-window-accept-button");
    Button button2 = button1;
    this.AcceptButton = button1;
    Button button3 = button2;
    children3.Add((Control) button3);
    ((Control) boxContainer3).Children.Add(new Control()
    {
      MinSize = new Vector2(20f, 0.0f)
    });
    Control.OrderedChildCollection children4 = ((Control) boxContainer3).Children;
    Button button4 = new Button();
    button4.Text = Loc.GetString("accept-cloning-window-deny-button");
    Button button5 = button4;
    this.DenyButton = button4;
    Button button6 = button5;
    children4.Add((Control) button6);
    children2.Add((Control) boxContainer3);
    children1.Add((Control) boxContainer2);
    contents.AddChild((Control) boxContainer1);
  }
}
