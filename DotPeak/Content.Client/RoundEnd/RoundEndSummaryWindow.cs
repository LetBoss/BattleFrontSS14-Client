// Decompiled with JetBrains decompiler
// Type: Content.Client.RoundEnd.RoundEndSummaryWindow
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Message;
using Content.Shared.GameTicking;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Content.Client.RoundEnd;

public sealed class RoundEndSummaryWindow : DefaultWindow
{
  private readonly IEntityManager _entityManager;
  public int RoundId;

  public RoundEndSummaryWindow(
    string gm,
    string roundEnd,
    TimeSpan roundTimeSpan,
    int roundId,
    RoundEndMessageEvent.RoundEndPlayerInfo[] info,
    IEntityManager entityManager)
  {
    this._entityManager = entityManager;
    ((Control) this).MinSize = new Vector2(520f, 580f);
    this.Title = Loc.GetString("round-end-summary-window-title");
    this.RoundId = roundId;
    TabContainer tabContainer = new TabContainer();
    ((Control) tabContainer).AddChild((Control) this.MakeRoundEndSummaryTab(gm, roundEnd, roundTimeSpan, roundId));
    ((Control) tabContainer).AddChild((Control) this.MakePlayerManifestTab(info));
    this.Contents.AddChild((Control) tabContainer);
    ((BaseWindow) this).OpenCenteredRight();
    ((BaseWindow) this).MoveToFront();
  }

  private BoxContainer MakeRoundEndSummaryTab(
    string gamemode,
    string roundEnd,
    TimeSpan roundDuration,
    int roundId)
  {
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer1).Name = Loc.GetString("round-end-summary-window-round-end-summary-tab-title");
    BoxContainer boxContainer2 = boxContainer1;
    ScrollContainer scrollContainer1 = new ScrollContainer();
    ((Control) scrollContainer1).VerticalExpand = true;
    ((Control) scrollContainer1).Margin = new Thickness(10f);
    scrollContainer1.HScrollEnabled = false;
    ScrollContainer scrollContainer2 = scrollContainer1;
    BoxContainer boxContainer3 = new BoxContainer()
    {
      Orientation = (BoxContainer.LayoutOrientation) 1
    };
    RichTextLabel richTextLabel = new RichTextLabel();
    FormattedMessage formattedMessage = new FormattedMessage();
    formattedMessage.AddMarkupOrThrow(Loc.GetString("round-end-summary-window-round-id-label", new (string, object)[1]
    {
      (nameof (roundId), (object) roundId)
    }));
    formattedMessage.AddText(" ");
    formattedMessage.AddMarkupOrThrow(Loc.GetString("round-end-summary-window-gamemode-name-label", new (string, object)[1]
    {
      (nameof (gamemode), (object) gamemode)
    }));
    richTextLabel.SetMessage(formattedMessage, new Color?());
    ((Control) boxContainer3).AddChild((Control) richTextLabel);
    RichTextLabel label1 = new RichTextLabel();
    label1.SetMarkup(Loc.GetString("round-end-summary-window-duration-label", new (string, object)[3]
    {
      ("hours", (object) roundDuration.Hours),
      ("minutes", (object) roundDuration.Minutes),
      ("seconds", (object) roundDuration.Seconds)
    }));
    ((Control) boxContainer3).AddChild((Control) label1);
    if (!string.IsNullOrEmpty(roundEnd))
    {
      RichTextLabel label2 = new RichTextLabel();
      label2.SetMarkup(roundEnd);
      ((Control) boxContainer3).AddChild((Control) label2);
    }
    ((Control) scrollContainer2).AddChild((Control) boxContainer3);
    ((Control) boxContainer2).AddChild((Control) scrollContainer2);
    return boxContainer2;
  }

  private BoxContainer MakePlayerManifestTab(
    RoundEndMessageEvent.RoundEndPlayerInfo[] playersInfo)
  {
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer1).Name = Loc.GetString("round-end-summary-window-player-manifest-tab-title");
    BoxContainer boxContainer2 = boxContainer1;
    ScrollContainer scrollContainer1 = new ScrollContainer();
    ((Control) scrollContainer1).VerticalExpand = true;
    ((Control) scrollContainer1).Margin = new Thickness(10f);
    ScrollContainer scrollContainer2 = scrollContainer1;
    BoxContainer boxContainer3 = new BoxContainer()
    {
      Orientation = (BoxContainer.LayoutOrientation) 1
    };
    foreach (RoundEndMessageEvent.RoundEndPlayerInfo roundEndPlayerInfo in (IEnumerable<RoundEndMessageEvent.RoundEndPlayerInfo>) ((IEnumerable<RoundEndMessageEvent.RoundEndPlayerInfo>) playersInfo).OrderBy<RoundEndMessageEvent.RoundEndPlayerInfo, bool>((Func<RoundEndMessageEvent.RoundEndPlayerInfo, bool>) (p => p.Observer)).ThenBy<RoundEndMessageEvent.RoundEndPlayerInfo, bool>((Func<RoundEndMessageEvent.RoundEndPlayerInfo, bool>) (p => !p.Antag)))
    {
      BoxContainer boxContainer4 = new BoxContainer()
      {
        Orientation = (BoxContainer.LayoutOrientation) 0
      };
      RichTextLabel richTextLabel = new RichTextLabel();
      ((Control) richTextLabel).VerticalAlignment = (Control.VAlignment) 2;
      ((Control) richTextLabel).VerticalExpand = true;
      RichTextLabel label = richTextLabel;
      if (roundEndPlayerInfo.PlayerNetEntity.HasValue)
      {
        BoxContainer boxContainer5 = boxContainer4;
        SpriteView spriteView = new SpriteView(roundEndPlayerInfo.PlayerNetEntity.Value, this._entityManager);
        spriteView.OverrideDirection = new Direction?((Direction) 0);
        ((Control) spriteView).VerticalAlignment = (Control.VAlignment) 2;
        ((Control) spriteView).SetSize = new Vector2(32f, 32f);
        ((Control) spriteView).VerticalExpand = true;
        ((Control) boxContainer5).AddChild((Control) spriteView);
      }
      if (roundEndPlayerInfo.PlayerICName != null)
      {
        if (roundEndPlayerInfo.Observer)
        {
          label.SetMarkup(Loc.GetString("round-end-summary-window-player-info-if-observer-text", new (string, object)[2]
          {
            ("playerOOCName", (object) roundEndPlayerInfo.PlayerOOCName),
            ("playerICName", (object) roundEndPlayerInfo.PlayerICName)
          }));
        }
        else
        {
          string str = roundEndPlayerInfo.Antag ? "red" : "white";
          label.SetMarkup(Loc.GetString("round-end-summary-window-player-info-if-not-observer-text", new (string, object)[4]
          {
            ("playerOOCName", (object) roundEndPlayerInfo.PlayerOOCName),
            ("icNameColor", (object) str),
            ("playerICName", (object) roundEndPlayerInfo.PlayerICName),
            ("playerRole", (object) Loc.GetString(roundEndPlayerInfo.Role))
          }));
        }
      }
      ((Control) boxContainer4).AddChild((Control) label);
      ((Control) boxContainer3).AddChild((Control) boxContainer4);
    }
    ((Control) scrollContainer2).AddChild((Control) boxContainer3);
    ((Control) boxContainer2).AddChild((Control) scrollContainer2);
    return boxContainer2;
  }
}
