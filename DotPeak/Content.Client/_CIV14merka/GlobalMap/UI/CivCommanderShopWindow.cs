// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.GlobalMap.UI.CivCommanderShopWindow
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._CIV14merka.Teams;
using Content.Client.UserInterface.Controls;
using Content.Shared._CIV14merka.Commander;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Content.Client._CIV14merka.GlobalMap.UI;

public sealed class CivCommanderShopWindow : FancyWindow
{
  [Dependency]
  private IEntityManager _entityManager;
  [Dependency]
  private IPrototypeManager _prototypeManager;
  private readonly SpriteSystem _sprite;
  private readonly CivGlobalMapSystem _system;
  private readonly TextureRect _teamIcon;
  private readonly Label _titleLabel;
  private readonly Label _currencyLabel;
  private readonly Control _shopTab;
  private readonly Control _requestsTab;
  private readonly BoxContainer _placementEntries;
  private readonly BoxContainer _serviceEntries;
  private readonly Label _requestsHintLabel;
  private readonly BoxContainer _requestList;
  private readonly Label _emptyPlacementLabel;
  private readonly Label _emptyServiceLabel;
  private readonly Label _emptyRequestLabel;
  private readonly Dictionary<string, CivCommanderShopWindow.ShopEntryCard> _shopCards = new Dictionary<string, CivCommanderShopWindow.ShopEntryCard>();
  private readonly Dictionary<string, CivCommanderShopWindow.RequestCard> _requestCards = new Dictionary<string, CivCommanderShopWindow.RequestCard>();
  private CivCommanderState? _state;

  public CivCommanderShopWindow(CivGlobalMapSystem system)
  {
    IoCManager.InjectDependencies<CivCommanderShopWindow>(this);
    this._sprite = this._entityManager.System<SpriteSystem>();
    this._system = system;
    this.Title = Loc.GetString("civ-commander-shop-title");
    ((Control) this).MinSize = new Vector2(1020f, 620f);
    ((Control) this).SetSize = new Vector2(1120f, 720f);
    this.Resizable = true;
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
    boxContainer1.SeparationOverride = new int?(12);
    ((Control) boxContainer1).HorizontalExpand = true;
    ((Control) boxContainer1).VerticalExpand = true;
    ((Control) boxContainer1).Margin = new Thickness(8f);
    BoxContainer boxContainer2 = boxContainer1;
    PanelContainer panelContainer1 = CivCommanderShopWindow.MakePanel(Color.FromHex((ReadOnlySpan<char>) "#1A2129", new Color?()), Color.FromHex((ReadOnlySpan<char>) "#4A5C72", new Color?()));
    BoxContainer boxContainer3 = new BoxContainer();
    boxContainer3.Orientation = (BoxContainer.LayoutOrientation) 0;
    boxContainer3.SeparationOverride = new int?(12);
    ((Control) boxContainer3).HorizontalExpand = true;
    BoxContainer boxContainer4 = boxContainer3;
    TextureRect textureRect = new TextureRect();
    ((Control) textureRect).MinSize = new Vector2(56f, 56f);
    ((Control) textureRect).MaxSize = new Vector2(56f, 56f);
    textureRect.Stretch = (TextureRect.StretchMode) 7;
    ((Control) textureRect).Visible = false;
    this._teamIcon = textureRect;
    ((Control) boxContainer4).AddChild(CivCommanderShopWindow.MakeIconPanel((Control) this._teamIcon, Color.FromHex((ReadOnlySpan<char>) "#4D87D9", new Color?())));
    Label label = new Label();
    label.Text = Loc.GetString("civ-commander-shop-title-side");
    ((Control) label).StyleClasses.Add("FancyWindowTitle");
    ((Control) label).VerticalAlignment = (Control.VAlignment) 2;
    this._titleLabel = label;
    ((Control) boxContainer4).AddChild((Control) this._titleLabel);
    ((Control) boxContainer4).AddChild(new Control()
    {
      HorizontalExpand = true
    });
    this._currencyLabel = new Label()
    {
      Text = Loc.GetString("civ-commander-shop-currency", new (string, object)[1]
      {
        ("amount", (object) 0)
      }),
      FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#8CF09B", new Color?()))
    };
    PanelContainer panelContainer2 = new PanelContainer();
    StyleBoxFlat styleBoxFlat = new StyleBoxFlat();
    styleBoxFlat.BackgroundColor = Color.FromHex((ReadOnlySpan<char>) "#172129", new Color?());
    styleBoxFlat.BorderColor = Color.FromHex((ReadOnlySpan<char>) "#6CC07A", new Color?());
    styleBoxFlat.BorderThickness = new Thickness(1f);
    ((StyleBox) styleBoxFlat).ContentMarginLeftOverride = new float?(14f);
    ((StyleBox) styleBoxFlat).ContentMarginTopOverride = new float?(10f);
    ((StyleBox) styleBoxFlat).ContentMarginRightOverride = new float?(14f);
    ((StyleBox) styleBoxFlat).ContentMarginBottomOverride = new float?(10f);
    panelContainer2.PanelOverride = (StyleBox) styleBoxFlat;
    PanelContainer panelContainer3 = panelContainer2;
    ((Control) panelContainer3).AddChild((Control) this._currencyLabel);
    ((Control) boxContainer4).AddChild((Control) panelContainer3);
    ((Control) panelContainer1).AddChild((Control) boxContainer4);
    ((Control) boxContainer2).AddChild((Control) panelContainer1);
    BoxContainer boxContainer5 = new BoxContainer();
    boxContainer5.Orientation = (BoxContainer.LayoutOrientation) 1;
    boxContainer5.SeparationOverride = new int?(10);
    ((Control) boxContainer5).HorizontalExpand = true;
    this._placementEntries = boxContainer5;
    BoxContainer boxContainer6 = new BoxContainer();
    boxContainer6.Orientation = (BoxContainer.LayoutOrientation) 1;
    boxContainer6.SeparationOverride = new int?(10);
    ((Control) boxContainer6).HorizontalExpand = true;
    this._serviceEntries = boxContainer6;
    BoxContainer boxContainer7 = new BoxContainer();
    boxContainer7.Orientation = (BoxContainer.LayoutOrientation) 1;
    boxContainer7.SeparationOverride = new int?(10);
    ((Control) boxContainer7).HorizontalExpand = true;
    this._requestList = boxContainer7;
    this._emptyPlacementLabel = CivCommanderShopWindow.MakeEmptyLabel(Loc.GetString("civ-commander-shop-empty-placement"));
    this._emptyServiceLabel = CivCommanderShopWindow.MakeEmptyLabel(Loc.GetString("civ-commander-shop-empty-service"));
    this._emptyRequestLabel = CivCommanderShopWindow.MakeEmptyLabel(Loc.GetString("civ-commander-shop-empty-request"));
    TabContainer tabContainer1 = new TabContainer();
    ((Control) tabContainer1).HorizontalExpand = true;
    ((Control) tabContainer1).VerticalExpand = true;
    TabContainer tabContainer2 = tabContainer1;
    BoxContainer boxContainer8 = new BoxContainer();
    boxContainer8.Orientation = (BoxContainer.LayoutOrientation) 1;
    boxContainer8.SeparationOverride = new int?(12);
    ((Control) boxContainer8).HorizontalExpand = true;
    ((Control) boxContainer8).VerticalExpand = true;
    BoxContainer boxContainer9 = boxContainer8;
    this._shopTab = (Control) boxContainer9;
    TabContainer.SetTabTitle(this._shopTab, Loc.GetString("civ-commander-shop-tab-shop"));
    BoxContainer boxContainer10 = new BoxContainer();
    boxContainer10.Orientation = (BoxContainer.LayoutOrientation) 0;
    boxContainer10.SeparationOverride = new int?(12);
    ((Control) boxContainer10).HorizontalExpand = true;
    ((Control) boxContainer10).VerticalExpand = true;
    BoxContainer boxContainer11 = boxContainer10;
    PanelContainer panelContainer4 = CivCommanderShopWindow.MakeSectionPanel(Loc.GetString("civ-commander-shop-section-placement"), Color.FromHex((ReadOnlySpan<char>) "#6CC07A", new Color?()), (Control) this._placementEntries);
    ((Control) panelContainer4).SizeFlagsStretchRatio = 1f;
    ((Control) boxContainer11).AddChild((Control) panelContainer4);
    PanelContainer panelContainer5 = CivCommanderShopWindow.MakeSectionPanel(Loc.GetString("civ-commander-shop-section-service"), Color.FromHex((ReadOnlySpan<char>) "#4D87D9", new Color?()), (Control) this._serviceEntries);
    ((Control) panelContainer5).SizeFlagsStretchRatio = 1f;
    ((Control) boxContainer11).AddChild((Control) panelContainer5);
    ((Control) boxContainer9).AddChild((Control) boxContainer11);
    ((Control) tabContainer2).AddChild((Control) boxContainer9);
    BoxContainer boxContainer12 = new BoxContainer();
    boxContainer12.Orientation = (BoxContainer.LayoutOrientation) 1;
    boxContainer12.SeparationOverride = new int?(8);
    ((Control) boxContainer12).HorizontalExpand = true;
    ((Control) boxContainer12).VerticalExpand = true;
    BoxContainer boxContainer13 = boxContainer12;
    this._requestsTab = (Control) boxContainer13;
    TabContainer.SetTabTitle(this._requestsTab, Loc.GetString("civ-commander-shop-tab-requests"));
    this._requestsHintLabel = new Label()
    {
      Text = Loc.GetString("civ-commander-shop-requests-hint-none"),
      FontColorOverride = new Color?(Color.LightGray)
    };
    ((Control) boxContainer13).AddChild((Control) this._requestsHintLabel);
    ScrollContainer scrollContainer1 = new ScrollContainer();
    ((Control) scrollContainer1).HorizontalExpand = true;
    ((Control) scrollContainer1).VerticalExpand = true;
    scrollContainer1.VScrollEnabled = true;
    scrollContainer1.HScrollEnabled = false;
    ScrollContainer scrollContainer2 = scrollContainer1;
    ((Control) scrollContainer2).AddChild((Control) this._requestList);
    ((Control) boxContainer13).AddChild((Control) scrollContainer2);
    ((Control) tabContainer2).AddChild((Control) boxContainer13);
    ((Control) boxContainer2).AddChild((Control) tabContainer2);
    this.ContentsContainer.AddChild((Control) boxContainer2);
  }

  public void UpdateState(CivCommanderState? state)
  {
    this._state = state;
    this.UpdateView();
  }

  private void UpdateView()
  {
    TabContainer.SetTabTitle(this._shopTab, Loc.GetString("civ-commander-shop-tab-shop"));
    if (this._state == null)
    {
      ((Control) this._teamIcon).Visible = false;
      this._titleLabel.Text = Loc.GetString("civ-commander-shop-title-side");
      this._currencyLabel.Text = Loc.GetString("civ-commander-shop-currency", new (string, object)[1]
      {
        ("amount", (object) 0)
      });
      this._currencyLabel.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#8CF09B", new Color?()));
      this.ClearShopCards();
      this.ClearRequestCards();
      CivCommanderShopWindow.SetEmptyState((Control) this._placementEntries, (Control) this._emptyPlacementLabel, true);
      CivCommanderShopWindow.SetEmptyState((Control) this._serviceEntries, (Control) this._emptyServiceLabel, true);
      CivCommanderShopWindow.SetEmptyState((Control) this._requestList, (Control) this._emptyRequestLabel, true);
      this._requestsHintLabel.Text = Loc.GetString("civ-commander-shop-requests-hint-none");
      TabContainer.SetTabTitle(this._requestsTab, Loc.GetString("civ-commander-shop-tab-requests"));
    }
    else
    {
      Color teamAccent = CivCommanderShopWindow.GetTeamAccent(this._state.TeamId);
      string str = Loc.GetString(this._state.TeamId == 2 ? "civ-team-short-rf" : "civ-team-short-usa");
      this._teamIcon.Texture = this._sprite.Frame0((SpriteSpecifier) CivTeamIconResolver.GetTeamBadge(this._state.TeamId));
      ((Control) this._teamIcon).Visible = true;
      this._titleLabel.Text = Loc.GetString("civ-commander-shop-title-team", new (string, object)[1]
      {
        ("team", (object) str)
      });
      this._currencyLabel.Text = Loc.GetString("civ-commander-shop-currency", new (string, object)[1]
      {
        ("amount", (object) this._state.Currency)
      });
      this._currencyLabel.FontColorOverride = new Color?(teamAccent);
      this.UpdateShopEntries(this._state);
      this.UpdateRequestEntries(this._state);
    }
  }

  private void UpdateShopEntries(CivCommanderState state)
  {
    HashSet<string> stringSet = new HashSet<string>();
    int index1 = 0;
    int index2 = 0;
    foreach (CivCommanderShopEntryState entryState in (IEnumerable<CivCommanderShopEntryState>) state.ShopEntries.OrderBy<CivCommanderShopEntryState, int>((Func<CivCommanderShopEntryState, int>) (entry => this.ResolveOrder(entry.EntryId))).ThenBy<CivCommanderShopEntryState, string>((Func<CivCommanderShopEntryState, string>) (entry => entry.EntryId)))
    {
      CivCommanderShopEntryPrototype entry;
      if (this._prototypeManager.TryIndex<CivCommanderShopEntryPrototype>(entryState.EntryId, ref entry) && entry.Enabled)
      {
        stringSet.Add(entryState.EntryId);
        CivCommanderShopWindow.ShopEntryCard shopEntryCard;
        if (!this._shopCards.TryGetValue(entryState.EntryId, out shopEntryCard))
        {
          shopEntryCard = this.CreateShopEntryCard(entry);
          this._shopCards[entryState.EntryId] = shopEntryCard;
        }
        this.UpdateShopEntryCard(shopEntryCard, entry, entryState, state.Currency);
        if (entry.Kind == CivCommanderShopEntryKind.EntityPlacement)
        {
          CivCommanderShopWindow.PlaceChild((Control) this._placementEntries, (Control) shopEntryCard.Root, index1);
          ++index1;
        }
        else
        {
          CivCommanderShopWindow.PlaceChild((Control) this._serviceEntries, (Control) shopEntryCard.Root, index2);
          ++index2;
        }
      }
    }
    foreach ((string key, CivCommanderShopWindow.ShopEntryCard shopEntryCard) in this._shopCards.ToList<KeyValuePair<string, CivCommanderShopWindow.ShopEntryCard>>())
    {
      if (!stringSet.Contains(key))
      {
        ((Control) shopEntryCard.Root).Parent?.RemoveChild((Control) shopEntryCard.Root);
        this._shopCards.Remove(key);
      }
    }
    CivCommanderShopWindow.SetEmptyState((Control) this._placementEntries, (Control) this._emptyPlacementLabel, index1 == 0);
    CivCommanderShopWindow.SetEmptyState((Control) this._serviceEntries, (Control) this._emptyServiceLabel, index2 == 0);
  }

  private CivCommanderShopWindow.ShopEntryCard CreateShopEntryCard(
    CivCommanderShopEntryPrototype entry)
  {
    Color entryAccent = CivCommanderShopWindow.GetEntryAccent(entry);
    PanelContainer root = CivCommanderShopWindow.MakePanel(Color.FromHex((ReadOnlySpan<char>) "#202732", new Color?()), entryAccent);
    ((Control) root).HorizontalExpand = true;
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 0;
    boxContainer1.SeparationOverride = new int?(12);
    ((Control) boxContainer1).HorizontalExpand = true;
    BoxContainer boxContainer2 = boxContainer1;
    Texture texture;
    if (this.TryGetEntryTexture(entry, out texture))
    {
      TextureRect textureRect = new TextureRect();
      textureRect.Texture = texture;
      ((Control) textureRect).MinSize = new Vector2(52f, 52f);
      ((Control) textureRect).MaxSize = new Vector2(52f, 52f);
      textureRect.Stretch = (TextureRect.StretchMode) 7;
      TextureRect icon = textureRect;
      ((Control) boxContainer2).AddChild(CivCommanderShopWindow.MakeIconPanel((Control) icon, entryAccent));
    }
    BoxContainer boxContainer3 = new BoxContainer();
    boxContainer3.Orientation = (BoxContainer.LayoutOrientation) 1;
    boxContainer3.SeparationOverride = new int?(8);
    ((Control) boxContainer3).HorizontalExpand = true;
    BoxContainer boxContainer4 = boxContainer3;
    BoxContainer boxContainer5 = new BoxContainer();
    boxContainer5.Orientation = (BoxContainer.LayoutOrientation) 0;
    boxContainer5.SeparationOverride = new int?(8);
    ((Control) boxContainer5).HorizontalExpand = true;
    BoxContainer boxContainer6 = boxContainer5;
    Label label1 = new Label();
    ((Control) label1).StyleClasses.Add("FancyWindowTitle");
    ((Control) label1).HorizontalExpand = true;
    label1.ClipText = true;
    Label titleLabel = label1;
    ((Control) boxContainer6).AddChild((Control) titleLabel);
    ((Control) boxContainer4).AddChild((Control) boxContainer6);
    BoxContainer boxContainer7 = new BoxContainer();
    boxContainer7.Orientation = (BoxContainer.LayoutOrientation) 0;
    boxContainer7.SeparationOverride = new int?(10);
    ((Control) boxContainer7).HorizontalExpand = true;
    BoxContainer boxContainer8 = boxContainer7;
    RichTextLabel richTextLabel = new RichTextLabel();
    ((Control) richTextLabel).VerticalAlignment = (Control.VAlignment) 2;
    ((Control) richTextLabel).Visible = false;
    RichTextLabel comparisonPriceLabel = richTextLabel;
    ((Control) boxContainer8).AddChild((Control) comparisonPriceLabel);
    Label label2 = new Label();
    ((Control) label2).StyleClasses.Add("FancyWindowTitle");
    label2.FontColorOverride = new Color?(entryAccent);
    ((Control) label2).VerticalAlignment = (Control.VAlignment) 2;
    Label priceLabel = label2;
    ((Control) boxContainer8).AddChild((Control) priceLabel);
    Label label3 = new Label();
    label3.Text = Loc.GetString("civ-commander-shop-unit");
    label3.FontColorOverride = new Color?(((Color) ref entryAccent).WithAlpha(0.9f));
    ((Control) label3).VerticalAlignment = (Control.VAlignment) 2;
    Label unitLabel = label3;
    ((Control) boxContainer8).AddChild((Control) unitLabel);
    Control cooldownSpacer = new Control()
    {
      HorizontalExpand = true,
      Visible = false
    };
    ((Control) boxContainer8).AddChild(cooldownSpacer);
    Label label4 = new Label();
    ((Control) label4).VerticalAlignment = (Control.VAlignment) 2;
    ((Control) label4).Visible = false;
    Label cooldownLabel = label4;
    ((Control) boxContainer8).AddChild((Control) cooldownLabel);
    ((Control) boxContainer4).AddChild((Control) boxContainer8);
    BoxContainer boxContainer9 = new BoxContainer();
    boxContainer9.Orientation = (BoxContainer.LayoutOrientation) 0;
    boxContainer9.SeparationOverride = new int?(8);
    ((Control) boxContainer9).HorizontalExpand = true;
    BoxContainer boxContainer10 = boxContainer9;
    ((Control) boxContainer10).AddChild(new Control()
    {
      HorizontalExpand = true
    });
    Button button = new Button();
    ((Control) button).MinSize = new Vector2(156f, 38f);
    Button actionButton = button;
    ((BaseButton) actionButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
    {
      if (entry.Kind == CivCommanderShopEntryKind.EntityPlacement)
        this._system.TryStartCommanderShopPlacement(entry.ID);
      else
        this._system.RequestCommanderShopPurchase(entry.ID);
    });
    ((Control) boxContainer10).AddChild((Control) actionButton);
    ((Control) boxContainer4).AddChild((Control) boxContainer10);
    ((Control) boxContainer2).AddChild((Control) boxContainer4);
    ((Control) root).AddChild((Control) boxContainer2);
    return new CivCommanderShopWindow.ShopEntryCard(root, titleLabel, comparisonPriceLabel, priceLabel, unitLabel, cooldownSpacer, cooldownLabel, actionButton);
  }

  private void UpdateShopEntryCard(
    CivCommanderShopWindow.ShopEntryCard card,
    CivCommanderShopEntryPrototype entry,
    CivCommanderShopEntryState entryState,
    int currency)
  {
    Color entryAccent = CivCommanderShopWindow.GetEntryAccent(entry);
    card.TitleLabel.Text = entry.Name;
    card.PriceLabel.Text = $"{entryState.Price}";
    card.PriceLabel.FontColorOverride = new Color?(entryAccent);
    card.UnitLabel.FontColorOverride = new Color?(((Color) ref entryAccent).WithAlpha(0.9f));
    card.ActionButton.Text = CivCommanderShopWindow.BuildActionLabel(entry, entryState.Price);
    ((BaseButton) card.ActionButton).Disabled = currency < entryState.Price;
    if (!CivCommanderShopWindow.HasPriceCooldown(entry))
    {
      ((Control) card.ComparisonPriceLabel).Visible = false;
      card.CooldownSpacer.Visible = false;
      ((Control) card.CooldownLabel).Visible = false;
    }
    else
    {
      int num = (double) entryState.PriceCooldownRemainingSeconds > 0.0 ? entryState.BasePrice : CivCommanderShopWindow.GetFollowUpPrice(entry, entryState);
      bool flag = num > 0 && num != entryState.Price;
      ((Control) card.ComparisonPriceLabel).Visible = flag;
      if (flag)
        card.ComparisonPriceLabel.Text = $"[color=#C9D2DC][s]{num}[/s][/color]";
      card.CooldownSpacer.Visible = true;
      ((Control) card.CooldownLabel).Visible = true;
      Label cooldownLabel = card.CooldownLabel;
      string str;
      if ((double) entryState.PriceCooldownRemainingSeconds <= 0.0)
        str = Loc.GetString("civ-commander-shop-discount-active");
      else
        str = Loc.GetString("civ-commander-shop-discount-returns", new (string, object)[1]
        {
          ("time", (object) CivCommanderShopWindow.FormatCooldown(entryState.PriceCooldownRemainingSeconds))
        });
      cooldownLabel.Text = str;
      card.CooldownLabel.FontColorOverride = new Color?((double) entryState.PriceCooldownRemainingSeconds > 0.0 ? entryAccent : Color.FromHex((ReadOnlySpan<char>) "#C9D2DC", new Color?()));
    }
  }

  private void UpdateRequestEntries(CivCommanderState state)
  {
    List<PurchaseRequestEntryState> list = state.PurchaseRequests.OrderByDescending<PurchaseRequestEntryState, double>((Func<PurchaseRequestEntryState, double>) (entry => entry.RequestTime)).ToList<PurchaseRequestEntryState>();
    if (list.Count == 0)
    {
      this.ClearRequestCards();
      this._requestsHintLabel.Text = Loc.GetString("civ-commander-shop-requests-hint-empty");
      TabContainer.SetTabTitle(this._requestsTab, Loc.GetString("civ-commander-shop-tab-requests"));
      CivCommanderShopWindow.SetEmptyState((Control) this._requestList, (Control) this._emptyRequestLabel, true);
    }
    else
    {
      HashSet<string> stringSet = new HashSet<string>();
      for (int index = 0; index < list.Count; ++index)
      {
        PurchaseRequestEntryState request = list[index];
        stringSet.Add(request.RequestId);
        CivCommanderShopWindow.RequestCard requestCard;
        if (!this._requestCards.TryGetValue(request.RequestId, out requestCard))
        {
          requestCard = this.CreateRequestCard(request.RequestId);
          this._requestCards[request.RequestId] = requestCard;
        }
        this.UpdateRequestCard(requestCard, request, state.Currency);
        CivCommanderShopWindow.PlaceChild((Control) this._requestList, (Control) requestCard.Root, index);
      }
      foreach ((string key, CivCommanderShopWindow.RequestCard requestCard) in this._requestCards.ToList<KeyValuePair<string, CivCommanderShopWindow.RequestCard>>())
      {
        if (!stringSet.Contains(key))
        {
          ((Control) requestCard.Root).Parent?.RemoveChild((Control) requestCard.Root);
          this._requestCards.Remove(key);
        }
      }
      this._requestsHintLabel.Text = Loc.GetString("civ-commander-shop-requests-hint-active");
      TabContainer.SetTabTitle(this._requestsTab, Loc.GetString("civ-commander-shop-tab-requests-count", new (string, object)[1]
      {
        ("count", (object) list.Count)
      }));
      CivCommanderShopWindow.SetEmptyState((Control) this._requestList, (Control) this._emptyRequestLabel, false);
    }
  }

  private CivCommanderShopWindow.RequestCard CreateRequestCard(string requestId)
  {
    Color border = Color.FromHex((ReadOnlySpan<char>) "#C28D36", new Color?());
    PanelContainer root = CivCommanderShopWindow.MakePanel(Color.FromHex((ReadOnlySpan<char>) "#202732", new Color?()), border);
    ((Control) root).HorizontalExpand = true;
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
    boxContainer1.SeparationOverride = new int?(8);
    ((Control) boxContainer1).HorizontalExpand = true;
    ((Control) boxContainer1).VerticalExpand = false;
    BoxContainer boxContainer2 = boxContainer1;
    BoxContainer boxContainer3 = new BoxContainer();
    boxContainer3.Orientation = (BoxContainer.LayoutOrientation) 0;
    boxContainer3.SeparationOverride = new int?(8);
    ((Control) boxContainer3).HorizontalExpand = true;
    BoxContainer boxContainer4 = boxContainer3;
    Label label = new Label();
    ((Control) label).StyleClasses.Add("FancyWindowTitle");
    ((Control) label).HorizontalExpand = true;
    label.ClipText = true;
    Label requesterLabel = label;
    ((Control) boxContainer4).AddChild((Control) requesterLabel);
    Label totalLabel = new Label()
    {
      FontColorOverride = new Color?(border)
    };
    ((Control) boxContainer4).AddChild((Control) totalLabel);
    ((Control) boxContainer2).AddChild((Control) boxContainer4);
    Label metaLabel = new Label()
    {
      FontColorOverride = new Color?(Color.LightGray)
    };
    ((Control) boxContainer2).AddChild((Control) metaLabel);
    BoxContainer boxContainer5 = new BoxContainer();
    boxContainer5.Orientation = (BoxContainer.LayoutOrientation) 1;
    boxContainer5.SeparationOverride = new int?(4);
    ((Control) boxContainer5).HorizontalExpand = true;
    BoxContainer items = boxContainer5;
    ((Control) boxContainer2).AddChild((Control) items);
    BoxContainer boxContainer6 = new BoxContainer();
    boxContainer6.Orientation = (BoxContainer.LayoutOrientation) 0;
    boxContainer6.SeparationOverride = new int?(8);
    ((Control) boxContainer6).HorizontalExpand = true;
    BoxContainer boxContainer7 = boxContainer6;
    ((Control) boxContainer7).AddChild(new Control()
    {
      HorizontalExpand = true
    });
    Button button1 = new Button();
    button1.Text = Loc.GetString("civ-commander-shop-approve");
    ((Control) button1).MinSize = new Vector2(120f, 34f);
    Button approveButton = button1;
    ((BaseButton) approveButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this._system.ApprovePurchaseRequest(requestId));
    ((Control) boxContainer7).AddChild((Control) approveButton);
    Button button2 = new Button();
    button2.Text = Loc.GetString("civ-commander-shop-deny");
    ((Control) button2).MinSize = new Vector2(120f, 34f);
    Button button3 = button2;
    ((BaseButton) button3).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this._system.DenyPurchaseRequest(requestId));
    ((Control) boxContainer7).AddChild((Control) button3);
    ((Control) boxContainer2).AddChild((Control) boxContainer7);
    ((Control) root).AddChild((Control) boxContainer2);
    return new CivCommanderShopWindow.RequestCard(root, requesterLabel, totalLabel, metaLabel, items, approveButton);
  }

  private void UpdateRequestCard(
    CivCommanderShopWindow.RequestCard card,
    PurchaseRequestEntryState request,
    int currency)
  {
    card.RequesterLabel.Text = request.RequesterName;
    card.TotalLabel.Text = Loc.GetString("civ-commander-shop-request-total", new (string, object)[1]
    {
      ("price", (object) request.TotalPrice)
    });
    card.MetaLabel.Text = Loc.GetString("civ-commander-shop-request-meta", new (string, object)[2]
    {
      ("faction", (object) request.Faction),
      ("time", (object) CivCommanderShopWindow.FormatRequestTime(request.RequestTime))
    });
    ((BaseButton) card.ApproveButton).Disabled = currency < request.TotalPrice;
    while (card.ItemLabels.Count > request.Items.Count)
    {
      int index = card.ItemLabels.Count - 1;
      Label itemLabel = card.ItemLabels[index];
      ((Control) card.Items).RemoveChild((Control) itemLabel);
      card.ItemLabels.RemoveAt(index);
    }
    for (int index = 0; index < request.Items.Count; ++index)
    {
      if (index >= card.ItemLabels.Count)
      {
        Label label1 = new Label();
        label1.FontColorOverride = new Color?(Color.White);
        ((Control) label1).HorizontalExpand = true;
        label1.ClipText = true;
        Label label2 = label1;
        card.ItemLabels.Add(label2);
        ((Control) card.Items).AddChild((Control) label2);
      }
      PurchaseRequestItemState requestItemState = request.Items[index];
      card.ItemLabels[index].Text = Loc.GetString("civ-commander-shop-request-item", new (string, object)[2]
      {
        ("name", (object) requestItemState.ItemName),
        ("count", (object) requestItemState.Quantity)
      });
      ((Control) card.ItemLabels[index]).SetPositionInParent(index);
    }
  }

  private void ClearShopCards()
  {
    foreach (CivCommanderShopWindow.ShopEntryCard shopEntryCard in this._shopCards.Values)
      ((Control) shopEntryCard.Root).Parent?.RemoveChild((Control) shopEntryCard.Root);
    this._shopCards.Clear();
    CivCommanderShopWindow.SetEmptyState((Control) this._placementEntries, (Control) this._emptyPlacementLabel, false);
    CivCommanderShopWindow.SetEmptyState((Control) this._serviceEntries, (Control) this._emptyServiceLabel, false);
  }

  private void ClearRequestCards()
  {
    foreach (CivCommanderShopWindow.RequestCard requestCard in this._requestCards.Values)
      ((Control) requestCard.Root).Parent?.RemoveChild((Control) requestCard.Root);
    this._requestCards.Clear();
    CivCommanderShopWindow.SetEmptyState((Control) this._requestList, (Control) this._emptyRequestLabel, false);
  }

  private static void SetEmptyState(Control parent, Control emptyControl, bool visible)
  {
    if (visible)
    {
      if (!parent.Children.Contains(emptyControl))
        parent.AddChild(emptyControl);
      emptyControl.SetPositionInParent(0);
    }
    else
    {
      if (!parent.Children.Contains(emptyControl))
        return;
      parent.RemoveChild(emptyControl);
    }
  }

  private static void PlaceChild(Control parent, Control child, int index)
  {
    if (child.Parent != parent)
    {
      child.Parent?.RemoveChild(child);
      parent.AddChild(child);
    }
    child.SetPositionInParent(index);
  }

  private int ResolveOrder(string entryId)
  {
    CivCommanderShopEntryPrototype shopEntryPrototype;
    return !this._prototypeManager.TryIndex<CivCommanderShopEntryPrototype>(entryId, ref shopEntryPrototype) ? int.MaxValue : shopEntryPrototype.Order;
  }

  private bool TryGetEntryTexture(CivCommanderShopEntryPrototype entry, out Texture texture)
  {
    texture = this._sprite.GetFallbackTexture();
    if (entry.IconEntity.HasValue)
    {
      IPrototypeManager prototypeManager = this._prototypeManager;
      EntProtoId? iconEntity = entry.IconEntity;
      string str = iconEntity.HasValue ? EntProtoId.op_Implicit(iconEntity.GetValueOrDefault()) : (string) null;
      EntityPrototype entityPrototype;
      ref EntityPrototype local = ref entityPrototype;
      if (prototypeManager.TryIndex<EntityPrototype>(str, ref local))
      {
        IconComponent iconComponent;
        if (entityPrototype.TryGetComponent<IconComponent>(ref iconComponent, this._entityManager.ComponentFactory))
        {
          texture = this._sprite.GetIcon(iconComponent);
          return true;
        }
        SpriteComponent spriteComponent;
        if (!entityPrototype.TryGetComponent<SpriteComponent>(ref spriteComponent, this._entityManager.ComponentFactory))
          return false;
        foreach (ISpriteLayer allLayer in spriteComponent.AllLayers)
        {
          if (allLayer.Visible)
          {
            if (allLayer.Texture != null)
            {
              texture = allLayer.Texture;
              return true;
            }
            RSI rsi = allLayer.Rsi ?? allLayer.ActualRsi ?? spriteComponent.BaseRSI;
            if (rsi != null)
            {
              RSI.StateId rsiState = allLayer.RsiState;
              RSI.State state;
              if (((RSI.StateId) ref rsiState).IsValid && rsi.TryGetState(allLayer.RsiState, ref state))
              {
                texture = state.Frame0;
                return true;
              }
            }
          }
        }
        return false;
      }
    }
    return false;
  }

  private static string BuildActionLabel(CivCommanderShopEntryPrototype entry, int price)
  {
    bool flag = entry.Kind == CivCommanderShopEntryKind.EntityPlacement;
    if (price <= 0)
      return Loc.GetString(flag ? "civ-commander-shop-action-place" : "civ-commander-shop-action-buy");
    return Loc.GetString(flag ? "civ-commander-shop-action-place-price" : "civ-commander-shop-action-buy-price", new (string, object)[1]
    {
      (nameof (price), (object) price)
    });
  }

  private static bool HasPriceCooldown(CivCommanderShopEntryPrototype entry)
  {
    return entry.PriceAfterPurchaseCooldownSeconds > 0 && (double) entry.PriceAfterPurchaseMultiplier > 1.0;
  }

  private static int GetFollowUpPrice(
    CivCommanderShopEntryPrototype entry,
    CivCommanderShopEntryState entryState)
  {
    return !CivCommanderShopWindow.HasPriceCooldown(entry) || entryState.BasePrice <= 0 ? entryState.Price : Math.Max(entryState.Price, (int) MathF.Ceiling((float) entryState.BasePrice * entry.PriceAfterPurchaseMultiplier));
  }

  private static string FormatRequestTime(double timeSeconds)
  {
    TimeSpan timeSpan = TimeSpan.FromSeconds((long) Math.Max(0, (int) Math.Round(timeSeconds)));
    return timeSpan.TotalHours < 1.0 ? timeSpan.ToString("mm\\:ss") : timeSpan.ToString("hh\\:mm\\:ss");
  }

  private static string FormatCooldown(float remainingSeconds)
  {
    TimeSpan timeSpan = TimeSpan.FromSeconds((long) Math.Max(0, (int) MathF.Ceiling(remainingSeconds)));
    return timeSpan.TotalHours < 1.0 ? timeSpan.ToString("mm\\:ss") : timeSpan.ToString("hh\\:mm\\:ss");
  }

  private static PanelContainer MakeSectionPanel(string title, Color accent, Control content)
  {
    PanelContainer panelContainer = new PanelContainer();
    ((Control) panelContainer).HorizontalExpand = true;
    ((Control) panelContainer).VerticalExpand = true;
    StyleBoxFlat styleBoxFlat = new StyleBoxFlat();
    styleBoxFlat.BackgroundColor = Color.FromHex((ReadOnlySpan<char>) "#151A22", new Color?());
    styleBoxFlat.BorderColor = accent;
    styleBoxFlat.BorderThickness = new Thickness(1f);
    ((StyleBox) styleBoxFlat).ContentMarginLeftOverride = new float?(12f);
    ((StyleBox) styleBoxFlat).ContentMarginTopOverride = new float?(12f);
    ((StyleBox) styleBoxFlat).ContentMarginRightOverride = new float?(12f);
    ((StyleBox) styleBoxFlat).ContentMarginBottomOverride = new float?(12f);
    panelContainer.PanelOverride = (StyleBox) styleBoxFlat;
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
    boxContainer1.SeparationOverride = new int?(10);
    ((Control) boxContainer1).HorizontalExpand = true;
    ((Control) boxContainer1).VerticalExpand = true;
    BoxContainer boxContainer2 = boxContainer1;
    BoxContainer boxContainer3 = boxContainer2;
    Label label = new Label();
    label.Text = title;
    ((Control) label).StyleClasses.Add("FancyWindowTitle");
    label.FontColorOverride = new Color?(accent);
    ((Control) boxContainer3).AddChild((Control) label);
    ScrollContainer scrollContainer1 = new ScrollContainer();
    ((Control) scrollContainer1).HorizontalExpand = true;
    ((Control) scrollContainer1).VerticalExpand = true;
    scrollContainer1.VScrollEnabled = true;
    scrollContainer1.HScrollEnabled = false;
    ScrollContainer scrollContainer2 = scrollContainer1;
    ((Control) scrollContainer2).AddChild(content);
    ((Control) boxContainer2).AddChild((Control) scrollContainer2);
    ((Control) panelContainer).AddChild((Control) boxContainer2);
    return panelContainer;
  }

  private static Label MakeEmptyLabel(string text)
  {
    return new Label()
    {
      Text = text,
      FontColorOverride = new Color?(Color.LightGray)
    };
  }

  private static PanelContainer MakePanel(Color background, Color border)
  {
    PanelContainer panelContainer = new PanelContainer();
    StyleBoxFlat styleBoxFlat = new StyleBoxFlat();
    styleBoxFlat.BackgroundColor = background;
    styleBoxFlat.BorderColor = border;
    styleBoxFlat.BorderThickness = new Thickness(1f);
    ((StyleBox) styleBoxFlat).ContentMarginLeftOverride = new float?(12f);
    ((StyleBox) styleBoxFlat).ContentMarginTopOverride = new float?(12f);
    ((StyleBox) styleBoxFlat).ContentMarginRightOverride = new float?(12f);
    ((StyleBox) styleBoxFlat).ContentMarginBottomOverride = new float?(12f);
    panelContainer.PanelOverride = (StyleBox) styleBoxFlat;
    return panelContainer;
  }

  private static Control MakeIconPanel(Control icon, Color accent)
  {
    PanelContainer panelContainer = new PanelContainer();
    ((Control) panelContainer).MinSize = new Vector2(72f, 72f);
    ((Control) panelContainer).MaxSize = new Vector2(72f, 72f);
    StyleBoxFlat styleBoxFlat = new StyleBoxFlat();
    styleBoxFlat.BackgroundColor = ((Color) ref accent).WithAlpha(0.14f);
    styleBoxFlat.BorderColor = accent;
    styleBoxFlat.BorderThickness = new Thickness(1f);
    ((StyleBox) styleBoxFlat).ContentMarginLeftOverride = new float?(10f);
    ((StyleBox) styleBoxFlat).ContentMarginTopOverride = new float?(10f);
    ((StyleBox) styleBoxFlat).ContentMarginRightOverride = new float?(10f);
    ((StyleBox) styleBoxFlat).ContentMarginBottomOverride = new float?(10f);
    panelContainer.PanelOverride = (StyleBox) styleBoxFlat;
    ((Control) panelContainer).AddChild(icon);
    return (Control) panelContainer;
  }

  private static Color GetEntryAccent(CivCommanderShopEntryPrototype entry)
  {
    if (entry.Kind == CivCommanderShopEntryKind.EntityPlacement)
      return Color.FromHex((ReadOnlySpan<char>) "#6CC07A", new Color?());
    return entry.ServiceType != CivCommanderShopPurchaseType.RefillFactionSupply ? Color.FromHex((ReadOnlySpan<char>) "#4D87D9", new Color?()) : Color.FromHex((ReadOnlySpan<char>) "#C28D36", new Color?());
  }

  private static Color GetTeamAccent(int teamId)
  {
    return teamId != 2 ? Color.FromHex((ReadOnlySpan<char>) "#4D87D9", new Color?()) : Color.FromHex((ReadOnlySpan<char>) "#C24E4E", new Color?());
  }

  private sealed class ShopEntryCard
  {
    public PanelContainer Root { get; }

    public Label TitleLabel { get; }

    public RichTextLabel ComparisonPriceLabel { get; }

    public Label PriceLabel { get; }

    public Label UnitLabel { get; }

    public Control CooldownSpacer { get; }

    public Label CooldownLabel { get; }

    public Button ActionButton { get; }

    public ShopEntryCard(
      PanelContainer root,
      Label titleLabel,
      RichTextLabel comparisonPriceLabel,
      Label priceLabel,
      Label unitLabel,
      Control cooldownSpacer,
      Label cooldownLabel,
      Button actionButton)
    {
      this.Root = root;
      this.TitleLabel = titleLabel;
      this.ComparisonPriceLabel = comparisonPriceLabel;
      this.PriceLabel = priceLabel;
      this.UnitLabel = unitLabel;
      this.CooldownSpacer = cooldownSpacer;
      this.CooldownLabel = cooldownLabel;
      this.ActionButton = actionButton;
    }
  }

  private sealed class RequestCard
  {
    public PanelContainer Root { get; }

    public Label RequesterLabel { get; }

    public Label TotalLabel { get; }

    public Label MetaLabel { get; }

    public BoxContainer Items { get; }

    public List<Label> ItemLabels { get; } = new List<Label>();

    public Button ApproveButton { get; }

    public RequestCard(
      PanelContainer root,
      Label requesterLabel,
      Label totalLabel,
      Label metaLabel,
      BoxContainer items,
      Button approveButton)
    {
      this.Root = root;
      this.RequesterLabel = requesterLabel;
      this.TotalLabel = totalLabel;
      this.MetaLabel = metaLabel;
      this.Items = items;
      this.ApproveButton = approveButton;
    }
  }
}
