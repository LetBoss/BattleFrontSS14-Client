// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.UserInterface.Controls.SkinContextMenuBuilder
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._PUBG.Skin;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Client._PUBG.UserInterface.Controls;

public static class SkinContextMenuBuilder
{
  public static bool IsValidShopOffer(SkinShopOfferInfo offer)
  {
    if (offer.Price <= 0)
      return false;
    return offer.Currency == "coins" || offer.Currency == "premium";
  }

  public static IEnumerable<SkinShopOfferInfo> SortShopOffers(IEnumerable<SkinShopOfferInfo> offers)
  {
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    return (IEnumerable<SkinShopOfferInfo>) offers.Where<SkinShopOfferInfo>(SkinContextMenuBuilder.\u003C\u003EO.\u003C0\u003E__IsValidShopOffer ?? (SkinContextMenuBuilder.\u003C\u003EO.\u003C0\u003E__IsValidShopOffer = new Func<SkinShopOfferInfo, bool>(SkinContextMenuBuilder.IsValidShopOffer))).OrderBy<SkinShopOfferInfo, int>((Func<SkinShopOfferInfo, int>) (offer => offer.Price)).ThenBy<SkinShopOfferInfo, int>((Func<SkinShopOfferInfo, int>) (offer => !(offer.Currency == "coins") ? 1 : 0)).ThenBy<SkinShopOfferInfo, int>((Func<SkinShopOfferInfo, int>) (offer => offer.DurationDays ?? int.MaxValue));
  }

  public static bool HasEnoughCurrency(
    SkinShopOfferInfo offer,
    int playerCoins,
    int playerPremiumCoins)
  {
    return !(offer.Currency == "coins") ? playerPremiumCoins >= offer.Price : playerCoins >= offer.Price;
  }

  public static string GetOfferDurationText(SkinShopOfferInfo offer)
  {
    if (!offer.DurationDays.HasValue)
      return Loc.GetString("mainmenu-shop-offer-permanent");
    return Loc.GetString("mainmenu-shop-offer-days", new (string, object)[1]
    {
      ("days", (object) offer.DurationDays.Value)
    });
  }

  public static string GetShopOfferText(SkinShopOfferInfo offer)
  {
    string offerDurationText = SkinContextMenuBuilder.GetOfferDurationText(offer);
    return !(offer.Currency == "coins") ? Loc.GetString("mainmenu-shop-buy-premium", new (string, object)[2]
    {
      ("price", (object) offer.Price),
      ("duration", (object) offerDurationText)
    }) : Loc.GetString("mainmenu-shop-buy-coins", new (string, object)[2]
    {
      ("price", (object) offer.Price),
      ("duration", (object) offerDurationText)
    });
  }

  public static PanelContainer BuildItemContextMenu(
    IPrototypeManager prototypeManager,
    string itemId,
    bool isRecipe,
    int playerScrap,
    Action onCraft,
    Action onSell,
    Action onUse,
    bool canRemove,
    Action onRemove)
  {
    PanelContainer panelContainer1 = new PanelContainer();
    ((Control) panelContainer1).MinWidth = 150f;
    PanelContainer panelContainer2 = panelContainer1;
    ((Control) panelContainer2).StyleClasses.Add("contextMenuPopup");
    BoxContainer boxContainer = new BoxContainer()
    {
      Orientation = (BoxContainer.LayoutOrientation) 1
    };
    int? nullable1 = new int?();
    int? nullable2 = new int?();
    EntityPrototype entityPrototype;
    PubgSkinItemComponent skinItemComponent;
    if (prototypeManager.TryIndex<EntityPrototype>(itemId, ref entityPrototype) && entityPrototype.TryGetComponent<PubgSkinItemComponent>("PubgSkinItem", ref skinItemComponent))
    {
      nullable1 = skinItemComponent.CraftPrice > 0 ? new int?(skinItemComponent.CraftPrice) : new int?();
      nullable2 = skinItemComponent.SellPrice > 0 ? new int?(skinItemComponent.SellPrice) : new int?();
    }
    if (isRecipe)
    {
      if (nullable1.HasValue)
      {
        bool flag = playerScrap >= nullable1.Value;
        Button button1 = new Button();
        button1.Text = Loc.GetString("mainmenu-context-craft", new (string, object)[1]
        {
          ("price", (object) nullable1.Value)
        });
        ((Control) button1).MinWidth = 150f;
        ((BaseButton) button1).Disabled = !flag;
        Button button2 = button1;
        ((BaseButton) button2).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => onCraft());
        ((Control) boxContainer).AddChild((Control) button2);
        if (!flag)
        {
          Label label1 = new Label();
          label1.Text = Loc.GetString("mainmenu-context-not-enough-scrap");
          label1.FontColorOverride = new Color?(Color.Red);
          ((Control) label1).HorizontalAlignment = (Control.HAlignment) 2;
          ((Control) label1).Margin = new Thickness(5f, 2f, 5f, 2f);
          Label label2 = label1;
          ((Control) boxContainer).AddChild((Control) label2);
        }
      }
      if (nullable2.HasValue)
      {
        Button button3 = new Button();
        button3.Text = Loc.GetString("mainmenu-context-sell", new (string, object)[1]
        {
          ("price", (object) nullable2.Value)
        });
        ((Control) button3).MinWidth = 150f;
        Button button4 = button3;
        ((BaseButton) button4).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => onSell());
        ((Control) boxContainer).AddChild((Control) button4);
      }
    }
    else
    {
      Button button5 = new Button();
      button5.Text = Loc.GetString("mainmenu-context-use");
      ((Control) button5).MinWidth = 150f;
      Button button6 = button5;
      ((BaseButton) button6).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => onUse());
      ((Control) boxContainer).AddChild((Control) button6);
      if (canRemove)
      {
        Button button7 = new Button();
        button7.Text = Loc.GetString("mainmenu-context-remove");
        ((Control) button7).MinWidth = 150f;
        Button button8 = button7;
        ((BaseButton) button8).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => onRemove());
        ((Control) boxContainer).AddChild((Control) button8);
      }
    }
    ((Control) panelContainer2).AddChild((Control) boxContainer);
    return panelContainer2;
  }

  public static PanelContainer BuildShopContextMenu(
    IPrototypeManager prototypeManager,
    SpriteSystem spriteSystem,
    int playerCoins,
    int playerPremiumCoins,
    SkinShopItemInfo shopItem,
    Action<string, string, int, int?> onBuy)
  {
    PanelContainer panelContainer1 = new PanelContainer();
    ((Control) panelContainer1).MinWidth = 220f;
    PanelContainer panelContainer2 = panelContainer1;
    ((Control) panelContainer2).StyleClasses.Add("contextMenuPopup");
    BoxContainer boxContainer1 = new BoxContainer()
    {
      Orientation = (BoxContainer.LayoutOrientation) 1
    };
    if (shopItem.IsOwnedPermanent)
    {
      Label label1 = new Label();
      label1.Text = Loc.GetString("mainmenu-shop-already-owned");
      label1.FontColorOverride = new Color?(Color.LimeGreen);
      ((Control) label1).HorizontalAlignment = (Control.HAlignment) 2;
      ((Control) label1).Margin = new Thickness(0.0f, 2f, 0.0f, 0.0f);
      Label label2 = label1;
      ((Control) boxContainer1).AddChild((Control) label2);
      ((Control) panelContainer2).AddChild((Control) boxContainer1);
      return panelContainer2;
    }
    List<SkinShopOfferInfo> list = SkinContextMenuBuilder.SortShopOffers((IEnumerable<SkinShopOfferInfo>) shopItem.Offers).ToList<SkinShopOfferInfo>();
    if (list.Count == 0)
    {
      Label label3 = new Label();
      label3.Text = Loc.GetString("mainmenu-shop-no-items");
      label3.FontColorOverride = new Color?(Color.Gray);
      ((Control) label3).HorizontalAlignment = (Control.HAlignment) 2;
      ((Control) label3).Margin = new Thickness(0.0f, 2f, 0.0f, 0.0f);
      Label label4 = label3;
      ((Control) boxContainer1).AddChild((Control) label4);
      ((Control) panelContainer2).AddChild((Control) boxContainer1);
      return panelContainer2;
    }
    bool flag1 = false;
    bool flag2 = false;
    bool flag3 = false;
    bool flag4 = false;
    foreach (SkinShopOfferInfo skinShopOfferInfo in list)
    {
      SkinShopOfferInfo offer = skinShopOfferInfo;
      bool flag5 = SkinContextMenuBuilder.HasEnoughCurrency(offer, playerCoins, playerPremiumCoins);
      if (offer.Currency == "coins")
      {
        flag1 = true;
        if (flag5)
          flag3 = true;
      }
      else
      {
        flag2 = true;
        if (flag5)
          flag4 = true;
      }
      string shopOfferText = SkinContextMenuBuilder.GetShopOfferText(offer);
      Button button1 = new Button();
      ((Control) button1).MinWidth = 220f;
      ((BaseButton) button1).Disabled = !flag5;
      Button button2 = button1;
      BoxContainer boxContainer2 = new BoxContainer();
      boxContainer2.Orientation = (BoxContainer.LayoutOrientation) 0;
      ((Control) boxContainer2).HorizontalAlignment = (Control.HAlignment) 2;
      BoxContainer boxContainer3 = boxContainer2;
      Label label = new Label() { Text = shopOfferText };
      ((Control) boxContainer3).AddChild((Control) label);
      string str = offer.Currency == "coins" ? "SpaceCash" : "MaterialDiamond1";
      EntityPrototype entityPrototype;
      if (prototypeManager.TryIndex<EntityPrototype>(str, ref entityPrototype))
      {
        IRsiStateLike prototypeIcon = spriteSystem.GetPrototypeIcon(entityPrototype);
        TextureRect textureRect1 = new TextureRect();
        textureRect1.Texture = ((IDirectionalTextureProvider) prototypeIcon).Default;
        ((Control) textureRect1).SetWidth = 16f;
        ((Control) textureRect1).SetHeight = 16f;
        TextureRect textureRect2 = textureRect1;
        ((Control) boxContainer3).AddChild((Control) textureRect2);
      }
      ((Control) button2).AddChild((Control) boxContainer3);
      ((BaseButton) button2).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => onBuy(offer.OfferId, offer.Currency, offer.Price, offer.DurationDays));
      ((Control) boxContainer1).AddChild((Control) button2);
    }
    if (flag1 && !flag3)
    {
      Label label5 = new Label();
      label5.Text = Loc.GetString("mainmenu-shop-not-enough-coins");
      label5.FontColorOverride = new Color?(Color.Red);
      ((Control) label5).HorizontalAlignment = (Control.HAlignment) 2;
      ((Control) label5).Margin = new Thickness(5f, 3f, 5f, 0.0f);
      Label label6 = label5;
      ((Control) boxContainer1).AddChild((Control) label6);
    }
    if (flag2 && !flag4)
    {
      Label label7 = new Label();
      label7.Text = Loc.GetString("mainmenu-shop-not-enough-diamonds");
      label7.FontColorOverride = new Color?(Color.Red);
      ((Control) label7).HorizontalAlignment = (Control.HAlignment) 2;
      ((Control) label7).Margin = new Thickness(5f, 0.0f, 5f, 3f);
      Label label8 = label7;
      ((Control) boxContainer1).AddChild((Control) label8);
    }
    ((Control) panelContainer2).AddChild((Control) boxContainer1);
    return panelContainer2;
  }
}
