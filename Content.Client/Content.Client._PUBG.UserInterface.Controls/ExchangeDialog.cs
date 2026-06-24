using System;
using System.Numerics;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Client._PUBG.UserInterface.Controls;

public static class ExchangeDialog
{
	public static void Show(IUserInterfaceManager uiManager, IEntityManager entityManager, IPrototypeManager prototypeManager, int premiumCoins, int scrap, int exchangeRate, Action<int> onConfirm)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Expected O, but got Unknown
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Expected O, but got Unknown
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Expected O, but got Unknown
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Expected O, but got Unknown
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Expected O, but got Unknown
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Expected O, but got Unknown
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Expected O, but got Unknown
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Expected O, but got Unknown
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Expected O, but got Unknown
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Expected O, but got Unknown
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Expected O, but got Unknown
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Expected O, but got Unknown
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03df: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Expected O, but got Unknown
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_040a: Expected O, but got Unknown
		//IL_0424: Unknown result type (might be due to invalid IL or missing references)
		//IL_0429: Unknown result type (might be due to invalid IL or missing references)
		//IL_0439: Unknown result type (might be due to invalid IL or missing references)
		//IL_0444: Unknown result type (might be due to invalid IL or missing references)
		//IL_0459: Unknown result type (might be due to invalid IL or missing references)
		//IL_0465: Expected O, but got Unknown
		//IL_04aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b4: Expected O, but got Unknown
		//IL_0530: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Expected O, but got Unknown
		SpriteSystem val = entityManager.System<SpriteSystem>();
		PanelContainer val2 = new PanelContainer
		{
			MinWidth = 350f,
			MinHeight = 250f
		};
		((Control)val2).StyleClasses.Add("contextMenuPopup");
		BoxContainer val3 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			Margin = new Thickness(15f)
		};
		Label val4 = new Label
		{
			Text = Loc.GetString("mainmenu-exchange-title"),
			HorizontalAlignment = (HAlignment)2,
			Margin = new Thickness(0f, 0f, 0f, 15f)
		};
		((Control)val4).SetOnlyStyleClass("LabelHeading");
		Label val5 = new Label();
		val5.Text = Loc.GetString("mainmenu-exchange-rate", new(string, object)[1] { ("rate", exchangeRate) });
		((Control)val5).HorizontalAlignment = (HAlignment)2;
		((Control)val5).Margin = new Thickness(0f, 0f, 0f, 10f);
		Label val6 = val5;
		BoxContainer val7 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			Margin = new Thickness(0f, 0f, 0f, 15f)
		};
		BoxContainer val8 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			HorizontalAlignment = (HAlignment)2,
			Margin = new Thickness(0f, 0f, 0f, 5f)
		};
		EntityPrototype val9 = default(EntityPrototype);
		if (prototypeManager.TryIndex<EntityPrototype>(new ProtoId<EntityPrototype>("MaterialDiamond1"), ref val9))
		{
			IRsiStateLike prototypeIcon = val.GetPrototypeIcon(val9);
			TextureRect val10 = new TextureRect
			{
				Texture = ((IDirectionalTextureProvider)prototypeIcon).Default,
				SetWidth = 20f,
				SetHeight = 20f,
				Margin = new Thickness(0f, 0f, 5f, 0f)
			};
			((Control)val8).AddChild((Control)(object)val10);
		}
		val5 = new Label();
		val5.Text = Loc.GetString("mainmenu-exchange-balance-diamonds", new(string, object)[1] { ("amount", premiumCoins) });
		val5.FontColorOverride = Color.Gold;
		Label val11 = val5;
		((Control)val8).AddChild((Control)(object)val11);
		((Control)val7).AddChild((Control)(object)val8);
		val5 = new Label();
		val5.Text = Loc.GetString("mainmenu-exchange-balance-scrap", new(string, object)[1] { ("amount", scrap) });
		((Control)val5).HorizontalAlignment = (HAlignment)2;
		Label val12 = val5;
		((Control)val7).AddChild((Control)(object)val12);
		BoxContainer val13 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			HorizontalAlignment = (HAlignment)2,
			Margin = new Thickness(0f, 0f, 0f, 10f)
		};
		Label val14 = new Label
		{
			Text = Loc.GetString("mainmenu-exchange-input"),
			Margin = new Thickness(0f, 0f, 5f, 0f)
		};
		((Control)val13).AddChild((Control)(object)val14);
		LineEdit lineEdit = new LineEdit
		{
			MinWidth = 100f,
			Text = "1",
			HorizontalAlignment = (HAlignment)2
		};
		((Control)val13).AddChild((Control)(object)lineEdit);
		val5 = new Label();
		val5.Text = Loc.GetString("mainmenu-exchange-result", new(string, object)[1] { ("amount", exchangeRate) });
		((Control)val5).HorizontalAlignment = (HAlignment)2;
		val5.FontColorOverride = Color.LightGreen;
		((Control)val5).Margin = new Thickness(0f, 0f, 0f, 15f);
		Label resultLabel = val5;
		lineEdit.OnTextChanged += delegate(LineEditEventArgs args)
		{
			if (int.TryParse(args.Text, out var result) && result > 0)
			{
				int num = result * exchangeRate;
				resultLabel.Text = Loc.GetString("mainmenu-exchange-result", new(string, object)[1] { ("amount", num) });
			}
			else
			{
				resultLabel.Text = Loc.GetString("mainmenu-exchange-invalid");
			}
		};
		BoxContainer val15 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			HorizontalAlignment = (HAlignment)2
		};
		Button val16 = new Button
		{
			Text = Loc.GetString("mainmenu-exchange-confirm"),
			MinWidth = 120f
		};
		((Control)val16).StyleClasses.Add("ButtonColorGreen");
		((Control)val15).AddChild((Control)(object)val16);
		Button val17 = new Button
		{
			Text = Loc.GetString("mainmenu-exchange-cancel"),
			MinWidth = 120f,
			Margin = new Thickness(10f, 0f, 0f, 0f)
		};
		((Control)val15).AddChild((Control)(object)val17);
		((Control)val3).AddChild((Control)(object)val4);
		((Control)val3).AddChild((Control)(object)val6);
		((Control)val3).AddChild((Control)(object)val7);
		((Control)val3).AddChild((Control)(object)val13);
		((Control)val3).AddChild((Control)(object)resultLabel);
		((Control)val3).AddChild((Control)(object)val15);
		((Control)val2).AddChild((Control)(object)val3);
		Popup popup = new Popup();
		((Control)popup).AddChild((Control)(object)val2);
		((BaseButton)val16).OnPressed += delegate
		{
			if (int.TryParse(lineEdit.Text, out var result) && result > 0 && result <= premiumCoins)
			{
				onConfirm(result);
				popup.Close();
			}
		};
		((BaseButton)val17).OnPressed += delegate
		{
			popup.Close();
		};
		((Control)uiManager.ModalRoot).AddChild((Control)(object)popup);
		Vector2 vector = new Vector2(350f, 280f);
		Vector2 vector2 = (((Control)uiManager.ModalRoot).Size - vector) / 2f;
		popup.Open((UIBox2?)UIBox2.FromDimensions(vector2, vector), (Vector2?)null, (Vector2?)null);
	}
}
