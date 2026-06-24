// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.UserInterface.Systems.Party.PubgPartyInviteWindow
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client._PUBG.UserInterface.Systems.Party;

public sealed class PubgPartyInviteWindow : DefaultWindow
{
  private readonly RichTextLabel _body;
  private readonly Label _timer;

  public Button AcceptButton { get; }

  public Button DeclineButton { get; }

  public PubgPartyInviteWindow()
  {
    this.Title = Loc.GetString("pubg-party-invite-title");
    RichTextLabel richTextLabel = new RichTextLabel();
    ((Control) richTextLabel).HorizontalExpand = true;
    this._body = richTextLabel;
    this._timer = new Label()
    {
      FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#BBBBBB", new Color?()))
    };
    this.AcceptButton = new Button()
    {
      Text = Loc.GetString("pubg-party-invite-accept")
    };
    this.DeclineButton = new Button()
    {
      Text = Loc.GetString("pubg-party-invite-decline")
    };
    BoxContainer boxContainer1 = new BoxContainer()
    {
      Orientation = (BoxContainer.LayoutOrientation) 0,
      SeparationOverride = new int?(8)
    };
    ((Control) boxContainer1).AddChild((Control) this.AcceptButton);
    ((Control) boxContainer1).AddChild(new Control()
    {
      MinSize = new Vector2(16f, 0.0f)
    });
    ((Control) boxContainer1).AddChild((Control) this.DeclineButton);
    BoxContainer boxContainer2 = new BoxContainer();
    boxContainer2.Orientation = (BoxContainer.LayoutOrientation) 1;
    boxContainer2.SeparationOverride = new int?(12);
    ((Control) boxContainer2).Margin = new Thickness(12f);
    BoxContainer boxContainer3 = boxContainer2;
    ((Control) boxContainer3).AddChild((Control) this._body);
    ((Control) boxContainer3).AddChild((Control) this._timer);
    ((Control) boxContainer3).AddChild((Control) boxContainer1);
    this.Contents.AddChild((Control) boxContainer3);
  }

  public void SetInviter(string inviterName)
  {
    this._body.Text = Loc.GetString("pubg-party-invite-message", new (string, object)[1]
    {
      ("name", (object) inviterName)
    });
  }

  public void SetTimerSeconds(int seconds)
  {
    this._timer.Text = Loc.GetString("pubg-party-invite-timer", new (string, object)[1]
    {
      (nameof (seconds), (object) seconds)
    });
  }

  public void SetExpired() => this._timer.Text = Loc.GetString("pubg-party-invite-expired");
}
