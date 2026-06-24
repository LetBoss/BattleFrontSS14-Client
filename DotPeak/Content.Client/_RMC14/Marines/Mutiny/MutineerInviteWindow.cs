// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Marines.Mutiny.MutineerInviteWindow
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Localization;
using System.Numerics;

#nullable enable
namespace Content.Client._RMC14.Marines.Mutiny;

public sealed class MutineerInviteWindow : DefaultWindow
{
  public Button DenyButton { get; }

  public Button AcceptButton { get; }

  public MutineerInviteWindow()
  {
    this.Title = Loc.GetString("mutineer-invite-title");
    this.AcceptButton = new Button()
    {
      Text = Loc.GetString("mutineer-invite-accept")
    };
    this.DenyButton = new Button()
    {
      Text = Loc.GetString("mutineer-invite-deny")
    };
    BoxContainer boxContainer1 = new BoxContainer()
    {
      Orientation = (BoxContainer.LayoutOrientation) 1
    };
    BoxContainer boxContainer2 = boxContainer1;
    RichTextLabel richTextLabel1 = new RichTextLabel();
    richTextLabel1.Text = "You are being asked to join a mutiny.";
    ((Control) richTextLabel1).VerticalExpand = true;
    ((Control) richTextLabel1).VerticalAlignment = (Control.VAlignment) 2;
    ((Control) boxContainer2).AddChild((Control) richTextLabel1);
    BoxContainer boxContainer3 = boxContainer1;
    RichTextLabel richTextLabel2 = new RichTextLabel();
    richTextLabel2.Text = "Read the Mutinies and Riots guidelines (Core Rules -> \"Mutinies, Riots\").";
    ((Control) richTextLabel2).VerticalExpand = true;
    ((Control) richTextLabel2).VerticalAlignment = (Control.VAlignment) 2;
    ((Control) boxContainer3).AddChild((Control) richTextLabel2);
    BoxContainer boxContainer4 = new BoxContainer()
    {
      Orientation = (BoxContainer.LayoutOrientation) 0,
      Align = (BoxContainer.AlignMode) 1
    };
    ((Control) boxContainer4).AddChild((Control) this.AcceptButton);
    ((Control) boxContainer4).AddChild(new Control()
    {
      MinSize = new Vector2(20f, 0.0f)
    });
    ((Control) boxContainer4).AddChild((Control) this.DenyButton);
    ((Control) boxContainer1).AddChild((Control) boxContainer4);
    this.Contents.AddChild((Control) boxContainer1);
  }
}
