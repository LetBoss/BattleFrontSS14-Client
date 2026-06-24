using System;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;

namespace Content.Client._PUBG.UserInterface.Systems.Gulag;

public sealed class GulagAdminOfferWindow : PanelContainer
{
	public event Action<bool>? OnResponse;

	public GulagAdminOfferWindow()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Expected O, but got Unknown
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Expected O, but got Unknown
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Expected O, but got Unknown
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Expected O, but got Unknown
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Expected O, but got Unknown
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Expected O, but got Unknown
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Expected O, but got Unknown
		StyleBoxFlat val = new StyleBoxFlat
		{
			BackgroundColor = Color.FromHex((ReadOnlySpan<char>)"#1a1a1aF0", (Color?)null),
			BorderColor = Color.FromHex((ReadOnlySpan<char>)"#FFA500", (Color?)null),
			BorderThickness = new Thickness(4f)
		};
		((StyleBox)val).SetContentMarginOverride((Margin)15, 25f);
		((PanelContainer)this).PanelOverride = (StyleBox)(object)val;
		((Control)this).MinWidth = 450f;
		((Control)this).MinHeight = 220f;
		BoxContainer val2 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			SeparationOverride = 20,
			HorizontalAlignment = (HAlignment)2,
			VerticalAlignment = (VAlignment)2
		};
		Label val3 = new Label
		{
			Text = "\ud83d\udc7d ПОМЕХА В ГУЛАГ \ud83d\udc7d",
			FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#FFA500", (Color?)null),
			HorizontalAlignment = (HAlignment)2
		};
		Label val4 = new Label
		{
			Text = "Вы хотите стать помехой в текущем бою ГУЛАГ?\nВы будете переведены в случайное NPC существо на арене.",
			FontColorOverride = Color.White,
			HorizontalAlignment = (HAlignment)2
		};
		BoxContainer val5 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			SeparationOverride = 15,
			HorizontalAlignment = (HAlignment)2
		};
		Button val6 = new Button
		{
			Text = "✓ Принять",
			MinWidth = 120f
		};
		Button val7 = new Button
		{
			Text = "✗ Отказаться",
			MinWidth = 120f
		};
		((BaseButton)val6).OnPressed += delegate
		{
			this.OnResponse?.Invoke(obj: true);
		};
		((BaseButton)val7).OnPressed += delegate
		{
			this.OnResponse?.Invoke(obj: false);
		};
		((Control)val5).AddChild((Control)(object)val6);
		((Control)val5).AddChild((Control)(object)val7);
		((Control)val2).AddChild((Control)(object)val3);
		((Control)val2).AddChild((Control)(object)val4);
		((Control)val2).AddChild((Control)(object)val5);
		((Control)this).AddChild((Control)(object)val2);
	}
}
