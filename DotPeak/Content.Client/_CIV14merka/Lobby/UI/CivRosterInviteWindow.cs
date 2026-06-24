// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Lobby.UI.CivRosterInviteWindow
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._CIV14merka;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client._CIV14merka.Lobby.UI;

public sealed class CivRosterInviteWindow : DefaultWindow
{
  private readonly Label _label;

  public event Action? AcceptPressed;

  public event Action? DeclinePressed;

  public CivRosterInviteWindow()
  {
    this.Title = Loc.GetString("civ-lobby-invite-title");
    ((Control) this).MinSize = new Vector2(420f, 180f);
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
    boxContainer1.SeparationOverride = new int?(8);
    ((Control) boxContainer1).Margin = new Thickness(8f);
    BoxContainer boxContainer2 = boxContainer1;
    Label label = new Label();
    ((Control) label).HorizontalExpand = true;
    this._label = label;
    BoxContainer boxContainer3 = new BoxContainer()
    {
      Orientation = (BoxContainer.LayoutOrientation) 0,
      SeparationOverride = new int?(8)
    };
    Button button1 = new Button();
    button1.Text = Loc.GetString("civ-lobby-invite-accept");
    ((Control) button1).HorizontalExpand = true;
    Button button2 = button1;
    ((BaseButton) button2).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
    {
      Action acceptPressed = this.AcceptPressed;
      if (acceptPressed == null)
        return;
      acceptPressed();
    });
    Button button3 = new Button();
    button3.Text = Loc.GetString("civ-lobby-invite-decline");
    ((Control) button3).HorizontalExpand = true;
    Button button4 = button3;
    ((BaseButton) button4).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
    {
      Action declinePressed = this.DeclinePressed;
      if (declinePressed == null)
        return;
      declinePressed();
    });
    ((Control) boxContainer3).AddChild((Control) button2);
    ((Control) boxContainer3).AddChild((Control) button4);
    ((Control) boxContainer2).AddChild((Control) this._label);
    ((Control) boxContainer2).AddChild((Control) boxContainer3);
    this.Contents.AddChild((Control) boxContainer2);
  }

  public void UpdateInvite(CivRosterInvitePromptEvent msg)
  {
    this._label.Text = Loc.GetString("civ-lobby-invite-message", new (string, object)[3]
    {
      ("inviter", (object) msg.InviterName),
      ("team", (object) msg.TeamName),
      ("squad", (object) msg.SquadId)
    });
  }
}
