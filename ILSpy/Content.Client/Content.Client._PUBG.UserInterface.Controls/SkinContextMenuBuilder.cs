using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared._PUBG.Skin;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Client._PUBG.UserInterface.Controls;

public static class SkinContextMenuBuilder
{
	public static bool IsValidShopOffer(SkinShopOfferInfo offer)
	{
		if (offer.Price <= 0)
		{
			return false;
		}
		if (!(offer.Currency == "coins"))
		{
			return offer.Currency == "premium";
		}
		return true;
	}

	public static IEnumerable<SkinShopOfferInfo> SortShopOffers(IEnumerable<SkinShopOfferInfo> offers)
	{
		return from offer in offers.Where(IsValidShopOffer)
			orderby offer.Price, (!(offer.Currency == "coins")) ? 1 : 0, offer.DurationDays ?? int.MaxValue
			select offer;
	}

	public static bool HasEnoughCurrency(SkinShopOfferInfo offer, int playerCoins, int playerPremiumCoins)
	{
		if (!(offer.Currency == "coins"))
		{
			return playerPremiumCoins >= offer.Price;
		}
		return playerCoins >= offer.Price;
	}

	public static string GetOfferDurationText(SkinShopOfferInfo offer)
	{
		if (!offer.DurationDays.HasValue)
		{
			return Loc.GetString("mainmenu-shop-offer-permanent");
		}
		return Loc.GetString("mainmenu-shop-offer-days", new(string, object)[1] { ("days", offer.DurationDays.Value) });
	}

	public static string GetShopOfferText(SkinShopOfferInfo offer)
	{
		string offerDurationText = GetOfferDurationText(offer);
		if (!(offer.Currency == "coins"))
		{
			return Loc.GetString("mainmenu-shop-buy-premium", new(string, object)[2]
			{
				("price", offer.Price),
				("duration", offerDurationText)
			});
		}
		return Loc.GetString("mainmenu-shop-buy-coins", new(string, object)[2]
		{
			("price", offer.Price),
			("duration", offerDurationText)
		});
	}

	public static PanelContainer BuildItemContextMenu(IPrototypeManager prototypeManager, string itemId, bool isRecipe, int playerScrap, Action onCraft, Action onSell, Action onUse, bool canRemove, Action onRemove)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Expected O, but got Unknown
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Expected O, but got Unknown
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Expected O, but got Unknown
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Expected O, but got Unknown
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Expected O, but got Unknown
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Expected O, but got Unknown
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Expected O, but got Unknown
		PanelContainer val = new PanelContainer
		{
			MinWidth = 150f
		};
		((Control)val).StyleClasses.Add("contextMenuPopup");
		BoxContainer val2 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1
		};
		int? num = null;
		int? num2 = null;
		EntityPrototype val3 = default(EntityPrototype);
		PubgSkinItemComponent pubgSkinItemComponent = default(PubgSkinItemComponent);
		if (prototypeManager.TryIndex<EntityPrototype>(itemId, ref val3) && val3.TryGetComponent<PubgSkinItemComponent>("PubgSkinItem", ref pubgSkinItemComponent))
		{
			num = ((pubgSkinItemComponent.CraftPrice > 0) ? new int?(pubgSkinItemComponent.CraftPrice) : ((int?)null));
			num2 = ((pubgSkinItemComponent.SellPrice > 0) ? new int?(pubgSkinItemComponent.SellPrice) : ((int?)null));
		}
		if (isRecipe)
		{
			if (num.HasValue)
			{
				bool flag = playerScrap >= num.Value;
				Button val4 = new Button();
				val4.Text = Loc.GetString("mainmenu-context-craft", new(string, object)[1] { ("price", num.Value) });
				((Control)val4).MinWidth = 150f;
				((BaseButton)val4).Disabled = !flag;
				Button val5 = val4;
				((BaseButton)val5).OnPressed += delegate
				{
					onCraft();
				};
				((Control)val2).AddChild((Control)(object)val5);
				if (!flag)
				{
					Label val6 = new Label
					{
						Text = Loc.GetString("mainmenu-context-not-enough-scrap"),
						FontColorOverride = Color.Red,
						HorizontalAlignment = (HAlignment)2,
						Margin = new Thickness(5f, 2f, 5f, 2f)
					};
					((Control)val2).AddChild((Control)(object)val6);
				}
			}
			if (num2.HasValue)
			{
				Button val4 = new Button();
				val4.Text = Loc.GetString("mainmenu-context-sell", new(string, object)[1] { ("price", num2.Value) });
				((Control)val4).MinWidth = 150f;
				Button val7 = val4;
				((BaseButton)val7).OnPressed += delegate
				{
					onSell();
				};
				((Control)val2).AddChild((Control)(object)val7);
			}
		}
		else
		{
			Button val8 = new Button
			{
				Text = Loc.GetString("mainmenu-context-use"),
				MinWidth = 150f
			};
			((BaseButton)val8).OnPressed += delegate
			{
				onUse();
			};
			((Control)val2).AddChild((Control)(object)val8);
			if (canRemove)
			{
				Button val9 = new Button
				{
					Text = Loc.GetString("mainmenu-context-remove"),
					MinWidth = 150f
				};
				((BaseButton)val9).OnPressed += delegate
				{
					onRemove();
				};
				((Control)val2).AddChild((Control)(object)val9);
			}
		}
		((Control)val).AddChild((Control)(object)val2);
		return val;
	}

	public static PanelContainer BuildShopContextMenu(IPrototypeManager prototypeManager, SpriteSystem spriteSystem, int playerCoins, int playerPremiumCoins, SkinShopItemInfo shopItem, Action<string, string, int, int?> onBuy)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Expected O, but got Unknown
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Expected O, but got Unknown
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Expected O, but got Unknown
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Expected O, but got Unknown
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Expected O, but got Unknown
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Expected O, but got Unknown
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Expected O, but got Unknown
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Expected O, but got Unknown
		PanelContainer val = new PanelContainer
		{
			MinWidth = 220f
		};
		((Control)val).StyleClasses.Add("contextMenuPopup");
		BoxContainer val2 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1
		};
		if (shopItem.IsOwnedPermanent)
		{
			Label val3 = new Label
			{
				Text = Loc.GetString("mainmenu-shop-already-owned"),
				FontColorOverride = Color.LimeGreen,
				HorizontalAlignment = (HAlignment)2,
				Margin = new Thickness(0f, 2f, 0f, 0f)
			};
			((Control)val2).AddChild((Control)(object)val3);
			((Control)val).AddChild((Control)(object)val2);
			return val;
		}
		List<SkinShopOfferInfo> list = SortShopOffers(shopItem.Offers).ToList();
		if (list.Count == 0)
		{
			Label val4 = new Label
			{
				Text = Loc.GetString("mainmenu-shop-no-items"),
				FontColorOverride = Color.Gray,
				HorizontalAlignment = (HAlignment)2,
				Margin = new Thickness(0f, 2f, 0f, 0f)
			};
			((Control)val2).AddChild((Control)(object)val4);
			((Control)val).AddChild((Control)(object)val2);
			return val;
		}
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		bool flag4 = false;
		EntityPrototype val8 = default(EntityPrototype);
		foreach (SkinShopOfferInfo offer in list)
		{
			bool flag5 = HasEnoughCurrency(offer, playerCoins, playerPremiumCoins);
			if (offer.Currency == "coins")
			{
				flag = true;
				if (flag5)
				{
					flag3 = true;
				}
			}
			else
			{
				flag2 = true;
				if (flag5)
				{
					flag4 = true;
				}
			}
			string shopOfferText = GetShopOfferText(offer);
			Button val5 = new Button
			{
				MinWidth = 220f,
				Disabled = !flag5
			};
			BoxContainer val6 = new BoxContainer
			{
				Orientation = (LayoutOrientation)0,
				HorizontalAlignment = (HAlignment)2
			};
			Label val7 = new Label
			{
				Text = shopOfferText
			};
			((Control)val6).AddChild((Control)(object)val7);
			string text = ((offer.Currency == "coins") ? "SpaceCash" : "MaterialDiamond1");
			if (prototypeManager.TryIndex<EntityPrototype>(text, ref val8))
			{
				IRsiStateLike prototypeIcon = spriteSystem.GetPrototypeIcon(val8);
				TextureRect val9 = new TextureRect
				{
					Texture = ((IDirectionalTextureProvider)prototypeIcon).Default,
					SetWidth = 16f,
					SetHeight = 16f
				};
				((Control)val6).AddChild((Control)(object)val9);
			}
			((Control)val5).AddChild((Control)(object)val6);
			((BaseButton)val5).OnPressed += delegate
			{
				onBuy(offer.OfferId, offer.Currency, offer.Price, offer.DurationDays);
			};
			((Control)val2).AddChild((Control)(object)val5);
		}
		if (flag && !flag3)
		{
			Label val10 = new Label
			{
				Text = Loc.GetString("mainmenu-shop-not-enough-coins"),
				FontColorOverride = Color.Red,
				HorizontalAlignment = (HAlignment)2,
				Margin = new Thickness(5f, 3f, 5f, 0f)
			};
			((Control)val2).AddChild((Control)(object)val10);
		}
		if (flag2 && !flag4)
		{
			Label val11 = new Label
			{
				Text = Loc.GetString("mainmenu-shop-not-enough-diamonds"),
				FontColorOverride = Color.Red,
				HorizontalAlignment = (HAlignment)2,
				Margin = new Thickness(5f, 0f, 5f, 3f)
			};
			((Control)val2).AddChild((Control)(object)val11);
		}
		((Control)val).AddChild((Control)(object)val2);
		return val;
	}
}
