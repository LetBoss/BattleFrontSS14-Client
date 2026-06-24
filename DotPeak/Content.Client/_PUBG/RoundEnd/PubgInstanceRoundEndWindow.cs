// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.RoundEnd.PubgInstanceRoundEndWindow
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._PUBG.RoundEnd;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Content.Client._PUBG.RoundEnd;

public sealed class PubgInstanceRoundEndWindow : DefaultWindow
{
  private readonly RichTextLabel _summaryText;
  private readonly BoxContainer _partyEntriesContainer;

  public PubgInstanceRoundEndWindow()
  {
    this.Title = Loc.GetString("pubg-roundend-title");
    ((BaseWindow) this).Resizable = false;
    ((Control) this).MinSize = new Vector2(840f, 620f);
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
    boxContainer1.SeparationOverride = new int?(8);
    ((Control) boxContainer1).Margin = new Thickness(10f);
    ((Control) boxContainer1).VerticalExpand = true;
    ((Control) boxContainer1).HorizontalExpand = true;
    BoxContainer boxContainer2 = boxContainer1;
    TabContainer tabContainer1 = new TabContainer();
    ((Control) tabContainer1).VerticalExpand = true;
    ((Control) tabContainer1).HorizontalExpand = true;
    TabContainer tabContainer2 = tabContainer1;
    BoxContainer boxContainer3 = new BoxContainer();
    boxContainer3.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer3).Name = Loc.GetString("pubg-roundend-tab-summary");
    BoxContainer boxContainer4 = boxContainer3;
    ScrollContainer scrollContainer1 = new ScrollContainer();
    ((Control) scrollContainer1).VerticalExpand = true;
    ((Control) scrollContainer1).HorizontalExpand = true;
    ScrollContainer scrollContainer2 = scrollContainer1;
    RichTextLabel richTextLabel = new RichTextLabel();
    ((Control) richTextLabel).VerticalExpand = true;
    ((Control) richTextLabel).HorizontalExpand = true;
    this._summaryText = richTextLabel;
    ((Control) scrollContainer2).AddChild((Control) this._summaryText);
    ((Control) boxContainer4).AddChild((Control) scrollContainer2);
    ((Control) tabContainer2).AddChild((Control) boxContainer4);
    BoxContainer boxContainer5 = new BoxContainer();
    boxContainer5.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer5).Name = Loc.GetString("pubg-roundend-tab-party");
    boxContainer5.SeparationOverride = new int?(6);
    BoxContainer boxContainer6 = boxContainer5;
    Label label = new Label()
    {
      Text = Loc.GetString("pubg-roundend-party-subtitle"),
      FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#B7C2D8", new Color?()))
    };
    ((Control) boxContainer6).AddChild((Control) label);
    ((Control) boxContainer6).AddChild(this.BuildPartyHeaderRow());
    ScrollContainer scrollContainer3 = new ScrollContainer();
    ((Control) scrollContainer3).VerticalExpand = true;
    ((Control) scrollContainer3).HorizontalExpand = true;
    ScrollContainer scrollContainer4 = scrollContainer3;
    BoxContainer boxContainer7 = new BoxContainer();
    boxContainer7.Orientation = (BoxContainer.LayoutOrientation) 1;
    boxContainer7.SeparationOverride = new int?(8);
    ((Control) boxContainer7).VerticalExpand = true;
    ((Control) boxContainer7).HorizontalExpand = true;
    this._partyEntriesContainer = boxContainer7;
    ((Control) scrollContainer4).AddChild((Control) this._partyEntriesContainer);
    ((Control) boxContainer6).AddChild((Control) scrollContainer4);
    ((Control) tabContainer2).AddChild((Control) boxContainer6);
    ((Control) boxContainer2).AddChild((Control) tabContainer2);
    Button button1 = new Button();
    button1.Text = Loc.GetString("pubg-roundend-close");
    ((Control) button1).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) button1).MinSize = new Vector2(160f, 34f);
    Button button2 = button1;
    ((BaseButton) button2).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => ((BaseWindow) this).Close());
    ((Control) boxContainer2).AddChild((Control) button2);
    this.Contents.AddChild((Control) boxContainer2);
  }

  public void SetTitleText(string titleKey) => this.Title = Loc.GetString(titleKey);

  public void SetRoundEndText(string text)
  {
    FormattedMessage formattedMessage;
    if (FormattedMessage.TryFromMarkup(text, ref formattedMessage))
      this._summaryText.SetMessage(formattedMessage, new Color?());
    else
      this._summaryText.SetMessage(text, new Color?());
  }

  public void SetPartyEntries(List<PubgRoundEndPartyEntry> entries)
  {
    ((Control) this._partyEntriesContainer).RemoveAllChildren();
    if (entries.Count == 0)
    {
      ((Control) this._partyEntriesContainer).AddChild((Control) new Label()
      {
        Text = Loc.GetString("pubg-roundend-party-empty"),
        FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#A3AEBD", new Color?()))
      });
    }
    else
    {
      foreach (IGrouping<int, PubgRoundEndPartyEntry> source in entries.GroupBy<PubgRoundEndPartyEntry, int>((Func<PubgRoundEndPartyEntry, int>) (entry => entry.PartyId)).OrderBy<IGrouping<int, PubgRoundEndPartyEntry>, int>((Func<IGrouping<int, PubgRoundEndPartyEntry>, int>) (group => group.Key > 0 ? group.Key : int.MaxValue)).ToList<IGrouping<int, PubgRoundEndPartyEntry>>())
      {
        PanelContainer panelContainer = new PanelContainer();
        StyleBoxFlat styleBoxFlat = new StyleBoxFlat()
        {
          BackgroundColor = Color.FromHex((ReadOnlySpan<char>) "#1B1F2CEE", new Color?()),
          BorderColor = Color.FromHex((ReadOnlySpan<char>) "#4F5F82", new Color?()),
          BorderThickness = new Thickness(1f)
        };
        ((StyleBox) styleBoxFlat).SetContentMarginOverride((StyleBox.Margin) 15, 8f);
        panelContainer.PanelOverride = (StyleBox) styleBoxFlat;
        BoxContainer boxContainer1 = new BoxContainer();
        boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
        boxContainer1.SeparationOverride = new int?(6);
        ((Control) boxContainer1).HorizontalExpand = true;
        BoxContainer boxContainer2 = boxContainer1;
        BoxContainer boxContainer3 = boxContainer2;
        Label label1 = new Label();
        Label label2 = label1;
        string str;
        if (source.Key <= 0)
          str = Loc.GetString("pubg-roundend-party-group-solo");
        else
          str = Loc.GetString("pubg-roundend-party-group", new (string, object)[1]
          {
            ("party", (object) source.Key)
          });
        label2.Text = str;
        label1.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#FFD05C", new Color?()));
        ((Control) label1).StyleClasses.Add("LabelHeading");
        Label label3 = label1;
        ((Control) boxContainer3).AddChild((Control) label3);
        foreach (PubgRoundEndPartyEntry entry in (IEnumerable<PubgRoundEndPartyEntry>) source.OrderBy<PubgRoundEndPartyEntry, int>((Func<PubgRoundEndPartyEntry, int>) (e => e.Placement)).ThenByDescending<PubgRoundEndPartyEntry, int>((Func<PubgRoundEndPartyEntry, int>) (e => e.Kills)).ThenBy<PubgRoundEndPartyEntry, string>((Func<PubgRoundEndPartyEntry, string>) (e => e.Username)))
          ((Control) boxContainer2).AddChild(this.BuildPartyEntryRow(entry));
        ((Control) panelContainer).AddChild((Control) boxContainer2);
        ((Control) this._partyEntriesContainer).AddChild((Control) panelContainer);
      }
    }
  }

  private Control BuildPartyHeaderRow()
  {
    PanelContainer panelContainer = new PanelContainer();
    panelContainer.PanelOverride = (StyleBox) new StyleBoxFlat()
    {
      BackgroundColor = Color.FromHex((ReadOnlySpan<char>) "#101521EE", new Color?()),
      BorderColor = Color.FromHex((ReadOnlySpan<char>) "#4F5F82", new Color?()),
      BorderThickness = new Thickness(1f)
    };
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 0;
    ((Control) boxContainer1).Margin = new Thickness(8f, 6f);
    boxContainer1.SeparationOverride = new int?(10);
    ((Control) boxContainer1).HorizontalExpand = true;
    BoxContainer boxContainer2 = boxContainer1;
    BoxContainer boxContainer3 = boxContainer2;
    Label label1 = new Label();
    label1.Text = Loc.GetString("pubg-roundend-party-col-placement");
    ((Control) label1).MinWidth = 68f;
    label1.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#A7B6CE", new Color?()));
    ((Control) boxContainer3).AddChild((Control) label1);
    BoxContainer boxContainer4 = boxContainer2;
    Label label2 = new Label();
    label2.Text = Loc.GetString("pubg-roundend-party-col-player");
    ((Control) label2).HorizontalExpand = true;
    label2.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#A7B6CE", new Color?()));
    ((Control) boxContainer4).AddChild((Control) label2);
    BoxContainer boxContainer5 = boxContainer2;
    Label label3 = new Label();
    label3.Text = Loc.GetString("pubg-roundend-party-col-kills");
    ((Control) label3).MinWidth = 52f;
    label3.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#A7B6CE", new Color?()));
    ((Control) boxContainer5).AddChild((Control) label3);
    BoxContainer boxContainer6 = boxContainer2;
    Label label4 = new Label();
    label4.Text = Loc.GetString("pubg-roundend-party-col-damage");
    ((Control) label4).MinWidth = 68f;
    label4.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#A7B6CE", new Color?()));
    ((Control) boxContainer6).AddChild((Control) label4);
    BoxContainer boxContainer7 = boxContainer2;
    Label label5 = new Label();
    label5.Text = Loc.GetString("pubg-roundend-party-col-taken");
    ((Control) label5).MinWidth = 75f;
    label5.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#A7B6CE", new Color?()));
    ((Control) boxContainer7).AddChild((Control) label5);
    ((Control) panelContainer).AddChild((Control) boxContainer2);
    return (Control) panelContainer;
  }

  private Control BuildPartyEntryRow(PubgRoundEndPartyEntry entry)
  {
    BoxContainer boxContainer = new BoxContainer();
    boxContainer.Orientation = (BoxContainer.LayoutOrientation) 0;
    boxContainer.SeparationOverride = new int?(10);
    ((Control) boxContainer).HorizontalExpand = true;
    Label label1 = new Label();
    label1.Text = $"#{entry.Placement}";
    ((Control) label1).MinWidth = 68f;
    label1.FontColorOverride = new Color?(this.GetPlacementColor(entry.Placement));
    ((Control) boxContainer).AddChild((Control) label1);
    Label label2 = new Label();
    label2.Text = entry.Username;
    ((Control) label2).HorizontalExpand = true;
    label2.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#E2E8F4", new Color?()));
    ((Control) boxContainer).AddChild((Control) label2);
    Label label3 = new Label();
    label3.Text = entry.Kills.ToString();
    ((Control) label3).MinWidth = 52f;
    label3.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#FF9C97", new Color?()));
    ((Control) boxContainer).AddChild((Control) label3);
    Label label4 = new Label();
    label4.Text = entry.DamageDealt.ToString();
    ((Control) label4).MinWidth = 68f;
    label4.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#8DC8FF", new Color?()));
    ((Control) boxContainer).AddChild((Control) label4);
    Label label5 = new Label();
    label5.Text = entry.DamageTaken.ToString();
    ((Control) label5).MinWidth = 75f;
    label5.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#B8C0D0", new Color?()));
    ((Control) boxContainer).AddChild((Control) label5);
    return (Control) boxContainer;
  }

  private Color GetPlacementColor(int placement)
  {
    Color placementColor;
    if (placement <= 10)
    {
      switch (placement - 1)
      {
        case 0:
          placementColor = Color.FromHex((ReadOnlySpan<char>) "#FFD700", new Color?());
          break;
        case 1:
          placementColor = Color.FromHex((ReadOnlySpan<char>) "#C0C0C0", new Color?());
          break;
        case 2:
          placementColor = Color.FromHex((ReadOnlySpan<char>) "#CD7F32", new Color?());
          break;
        default:
          placementColor = Color.FromHex((ReadOnlySpan<char>) "#73B6FF", new Color?());
          break;
      }
    }
    else
      placementColor = Color.FromHex((ReadOnlySpan<char>) "#A8B4C7", new Color?());
    return placementColor;
  }
}
