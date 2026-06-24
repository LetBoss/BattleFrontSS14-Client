// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.Sponsor.UI.PubgSponsorDisplayWindow
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._PUBG.Skin;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client._PUBG.Sponsor.UI;

public sealed class PubgSponsorDisplayWindow : DefaultWindow
{
  private SponsorTierInfo? _displayTier;
  private List<SponsorActiveTierInfo> _activeTiers = new List<SponsorActiveTierInfo>();
  private SponsorDisplayMode _displayMode;
  private string? _preferredTierKey;
  private bool _hasData;
  private bool _isUpdating;
  private readonly Label _currentLabel;
  private readonly Label _updatingLabel;
  private readonly Button _autoButton;
  private readonly Button _hiddenButton;
  private readonly BoxContainer _manualContainer;

  public event Action<SponsorDisplayMode, string?>? SelectionRequested;

  public PubgSponsorDisplayWindow()
  {
    this.Title = Loc.GetString("mainmenu-sponsor-display-title");
    ((Control) this).MinSize = new Vector2(560f, 520f);
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer1).HorizontalExpand = true;
    ((Control) boxContainer1).VerticalExpand = true;
    ((Control) boxContainer1).Margin = new Thickness(12f);
    boxContainer1.SeparationOverride = new int?(8);
    BoxContainer boxContainer2 = boxContainer1;
    BoxContainer boxContainer3 = boxContainer2;
    Label label1 = new Label();
    label1.Text = Loc.GetString("mainmenu-sponsor-display-subtitle");
    ((Control) label1).HorizontalAlignment = (Control.HAlignment) 2;
    label1.FontColorOverride = new Color?(Color.Gray);
    ((Control) boxContainer3).AddChild((Control) label1);
    Label label2 = new Label();
    label2.Text = string.Empty;
    ((Control) label2).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) label2).Margin = new Thickness(0.0f, 2f, 0.0f, 0.0f);
    this._currentLabel = label2;
    ((Control) boxContainer2).AddChild((Control) this._currentLabel);
    Label label3 = new Label();
    label3.Text = Loc.GetString("mainmenu-sponsor-display-updating");
    ((Control) label3).HorizontalAlignment = (Control.HAlignment) 2;
    label3.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#88AAFF", new Color?()));
    ((Control) label3).Visible = false;
    this._updatingLabel = label3;
    ((Control) boxContainer2).AddChild((Control) this._updatingLabel);
    BoxContainer boxContainer4 = new BoxContainer();
    boxContainer4.Orientation = (BoxContainer.LayoutOrientation) 0;
    ((Control) boxContainer4).HorizontalAlignment = (Control.HAlignment) 2;
    boxContainer4.SeparationOverride = new int?(8);
    ((Control) boxContainer4).Margin = new Thickness(0.0f, 2f, 0.0f, 4f);
    BoxContainer boxContainer5 = boxContainer4;
    this._autoButton = new Button();
    ((BaseButton) this._autoButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
    {
      Action<SponsorDisplayMode, string> selectionRequested = this.SelectionRequested;
      if (selectionRequested == null)
        return;
      selectionRequested(SponsorDisplayMode.Auto, (string) null);
    });
    ((Control) boxContainer5).AddChild((Control) this._autoButton);
    this._hiddenButton = new Button();
    ((BaseButton) this._hiddenButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
    {
      Action<SponsorDisplayMode, string> selectionRequested = this.SelectionRequested;
      if (selectionRequested == null)
        return;
      selectionRequested(SponsorDisplayMode.Hidden, (string) null);
    });
    ((Control) boxContainer5).AddChild((Control) this._hiddenButton);
    ((Control) boxContainer2).AddChild((Control) boxContainer5);
    BoxContainer boxContainer6 = boxContainer2;
    Label label4 = new Label();
    label4.Text = Loc.GetString("mainmenu-sponsor-display-list-title");
    ((Control) label4).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) label4).Margin = new Thickness(0.0f, 4f, 0.0f, 0.0f);
    ((Control) boxContainer6).AddChild((Control) label4);
    ScrollContainer scrollContainer1 = new ScrollContainer();
    ((Control) scrollContainer1).HorizontalExpand = true;
    ((Control) scrollContainer1).VerticalExpand = true;
    scrollContainer1.HScrollEnabled = false;
    scrollContainer1.VScrollEnabled = true;
    ScrollContainer scrollContainer2 = scrollContainer1;
    BoxContainer boxContainer7 = new BoxContainer();
    boxContainer7.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer7).HorizontalExpand = true;
    boxContainer7.SeparationOverride = new int?(4);
    this._manualContainer = boxContainer7;
    ((Control) scrollContainer2).AddChild((Control) this._manualContainer);
    ((Control) boxContainer2).AddChild((Control) scrollContainer2);
    this.Contents.AddChild((Control) boxContainer2);
  }

  public void UpdateState(
    SponsorTierInfo? displayTier,
    List<SponsorActiveTierInfo> activeTiers,
    SponsorDisplayMode displayMode,
    string? preferredTierKey,
    bool hasData,
    bool isUpdating)
  {
    this._displayTier = displayTier;
    this._activeTiers = activeTiers;
    this._displayMode = displayMode;
    this._preferredTierKey = preferredTierKey;
    this._hasData = hasData;
    this._isUpdating = isUpdating;
    this.RefreshUi();
  }

  private void RefreshUi()
  {
    if (!this._hasData)
    {
      this._currentLabel.Text = Loc.GetString("mainmenu-sponsor-display-loading");
      ((Control) this._updatingLabel).Visible = false;
      this._autoButton.Text = Loc.GetString("mainmenu-sponsor-display-mode-auto");
      this._hiddenButton.Text = Loc.GetString("mainmenu-sponsor-display-mode-hidden");
      ((BaseButton) this._autoButton).Disabled = true;
      ((BaseButton) this._hiddenButton).Disabled = true;
      ((Control) this._manualContainer).RemoveAllChildren();
      BoxContainer manualContainer = this._manualContainer;
      Label label = new Label();
      label.Text = Loc.GetString("mainmenu-sponsor-display-loading");
      ((Control) label).HorizontalAlignment = (Control.HAlignment) 2;
      label.FontColorOverride = new Color?(Color.Gray);
      ((Control) manualContainer).AddChild((Control) label);
    }
    else
    {
      string tierName = Loc.GetString("mainmenu-sponsor-display-current-none");
      if (this._displayMode == SponsorDisplayMode.Hidden)
        tierName = Loc.GetString("mainmenu-sponsor-display-current-hidden");
      else if (this._displayTier != null)
        tierName = this._displayTier.TierName;
      this._currentLabel.Text = Loc.GetString("mainmenu-sponsor-display-current", new (string, object)[1]
      {
        ("value", (object) tierName)
      });
      ((Control) this._updatingLabel).Visible = this._isUpdating;
      string str1 = Loc.GetString("mainmenu-selected-prefix");
      bool flag1 = this._displayMode == SponsorDisplayMode.Auto;
      bool flag2 = this._displayMode == SponsorDisplayMode.Hidden;
      string str2 = Loc.GetString("mainmenu-sponsor-display-mode-auto");
      this._autoButton.Text = flag1 ? str1 + str2 : str2;
      ((BaseButton) this._autoButton).Disabled = this._isUpdating | flag1;
      string str3 = Loc.GetString("mainmenu-sponsor-display-mode-hidden");
      this._hiddenButton.Text = flag2 ? str1 + str3 : str3;
      ((BaseButton) this._hiddenButton).Disabled = this._isUpdating | flag2;
      ((Control) this._manualContainer).RemoveAllChildren();
      if (this._activeTiers.Count == 0)
      {
        BoxContainer manualContainer = this._manualContainer;
        Label label = new Label();
        label.Text = Loc.GetString("mainmenu-sponsor-display-no-tiers");
        ((Control) label).HorizontalAlignment = (Control.HAlignment) 2;
        label.FontColorOverride = new Color?(Color.Gray);
        ((Control) manualContainer).AddChild((Control) label);
      }
      else
      {
        foreach (SponsorActiveTierInfo activeTier in this._activeTiers)
        {
          bool flag3 = this._displayMode == SponsorDisplayMode.Manual && string.Equals(this._preferredTierKey, activeTier.Key, StringComparison.OrdinalIgnoreCase);
          string str4 = Loc.GetString("mainmenu-sponsor-display-mode-manual", new (string, object)[1]
          {
            ("tier", (object) activeTier.Name)
          });
          Button button1 = new Button();
          button1.Text = flag3 ? str1 + str4 : str4;
          ((BaseButton) button1).Disabled = this._isUpdating | flag3;
          ((Control) button1).HorizontalExpand = true;
          Button button2 = button1;
          string tierKey = activeTier.Key;
          ((BaseButton) button2).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
          {
            Action<SponsorDisplayMode, string> selectionRequested = this.SelectionRequested;
            if (selectionRequested == null)
              return;
            selectionRequested(SponsorDisplayMode.Manual, tierKey);
          });
          ((Control) this._manualContainer).AddChild((Control) button2);
        }
      }
    }
  }
}
