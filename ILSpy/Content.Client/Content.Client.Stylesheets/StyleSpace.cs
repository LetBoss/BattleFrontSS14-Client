using System;
using System.Collections.Generic;
using System.Linq;
using Content.Client.Resources;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;

namespace Content.Client.Stylesheets;

public sealed class StyleSpace : StyleBase
{
	public static readonly Color SpaceRed = Color.FromHex((ReadOnlySpan<char>)"#9b2236", (Color?)null);

	public static readonly Color ButtonColorDefault = Color.FromHex((ReadOnlySpan<char>)"#464966", (Color?)null);

	public static readonly Color ButtonColorHovered = Color.FromHex((ReadOnlySpan<char>)"#575b7f", (Color?)null);

	public static readonly Color ButtonColorPressed = Color.FromHex((ReadOnlySpan<char>)"#3e6c45", (Color?)null);

	public static readonly Color ButtonColorDisabled = Color.FromHex((ReadOnlySpan<char>)"#30313c", (Color?)null);

	public static readonly Color ButtonColorCautionDefault = Color.FromHex((ReadOnlySpan<char>)"#ab3232", (Color?)null);

	public static readonly Color ButtonColorCautionHovered = Color.FromHex((ReadOnlySpan<char>)"#cf2f2f", (Color?)null);

	public static readonly Color ButtonColorCautionPressed = Color.FromHex((ReadOnlySpan<char>)"#3e6c45", (Color?)null);

	public static readonly Color ButtonColorCautionDisabled = Color.FromHex((ReadOnlySpan<char>)"#602a2a", (Color?)null);

	public override Stylesheet Stylesheet { get; }

	public StyleSpace(IResourceCache resCache)
		: base(resCache)
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Expected O, but got Unknown
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Expected O, but got Unknown
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Expected O, but got Unknown
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Expected O, but got Unknown
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Expected O, but got Unknown
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Expected O, but got Unknown
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Unknown result type (might be due to invalid IL or missing references)
		//IL_0441: Unknown result type (might be due to invalid IL or missing references)
		//IL_0481: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0501: Unknown result type (might be due to invalid IL or missing references)
		//IL_0579: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0645: Unknown result type (might be due to invalid IL or missing references)
		//IL_0671: Unknown result type (might be due to invalid IL or missing references)
		//IL_069d: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_073a: Unknown result type (might be due to invalid IL or missing references)
		//IL_074e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0753: Unknown result type (might be due to invalid IL or missing references)
		//IL_0761: Unknown result type (might be due to invalid IL or missing references)
		//IL_0766: Unknown result type (might be due to invalid IL or missing references)
		//IL_0774: Unknown result type (might be due to invalid IL or missing references)
		//IL_0779: Unknown result type (might be due to invalid IL or missing references)
		//IL_0783: Expected O, but got Unknown
		//IL_077e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0784: Expected O, but got Unknown
		//IL_078e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0798: Expected O, but got Unknown
		Font font = resCache.GetFont(new string[3] { "/Fonts/NotoSans/NotoSans-Regular.ttf", "/Fonts/NotoSans/NotoSansSymbols-Regular.ttf", "/Fonts/NotoSans/NotoSansSymbols2-Regular.ttf" }, 10);
		Font font2 = resCache.GetFont(new string[3] { "/Fonts/NotoSans/NotoSans-Bold.ttf", "/Fonts/NotoSans/NotoSansSymbols-Regular.ttf", "/Fonts/NotoSans/NotoSansSymbols2-Regular.ttf" }, 16);
		StyleBoxFlat val = new StyleBoxFlat
		{
			BackgroundColor = new Color(0.25f, 0.25f, 0.25f, 1f)
		};
		((StyleBox)val).SetContentMarginOverride((Margin)3, 14.5f);
		StyleBoxFlat val2 = new StyleBoxFlat
		{
			BackgroundColor = new Color(0.25f, 0.5f, 0.25f, 1f)
		};
		((StyleBox)val2).SetContentMarginOverride((Margin)3, 14.5f);
		Texture texture = resCache.GetTexture("/Textures/Interface/Nano/inverted_triangle.svg.png");
		StyleBoxTexture val3 = new StyleBoxTexture();
		val3.SetPatchMargin((Margin)15, 2f);
		StyleBoxFlat val4 = new StyleBoxFlat
		{
			BackgroundColor = new Color((byte)64, (byte)64, (byte)64, byte.MaxValue)
		};
		((StyleBox)val4).SetContentMarginOverride((Margin)12, 5f);
		StyleBoxFlat val5 = new StyleBoxFlat
		{
			BackgroundColor = new Color((byte)32, (byte)32, (byte)32, byte.MaxValue)
		};
		((StyleBox)val5).SetContentMarginOverride((Margin)12, 5f);
		Stylesheet = new Stylesheet((IReadOnlyList<StyleRule>)base.BaseRules.Concat((IEnumerable<StyleRule>)(object)new StyleRule[29]
		{
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<Label>().Class("LabelHeading")).Prop("font", (object)font2).Prop("font-color", (object)SpaceRed)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<Label>().Class("LabelSubText")).Prop("font", (object)font).Prop("font-color", (object)Color.DarkGray)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<PanelContainer>().Class("HighDivider")).Prop("panel", (object)new StyleBoxFlat
			{
				BackgroundColor = SpaceRed,
				ContentMarginBottomOverride = 2f,
				ContentMarginLeftOverride = 2f
			})),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<PanelContainer>().Class("LowDivider")).Prop("panel", (object)new StyleBoxFlat
			{
				BackgroundColor = Color.FromHex((ReadOnlySpan<char>)"#444", (Color?)null),
				ContentMarginLeftOverride = 2f,
				ContentMarginBottomOverride = 2f
			})),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<ContainerButton>().Class("button")).Prop("stylebox", (object)base.BaseButton)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<ContainerButton>().Class("button").Class("OpenRight")).Prop("stylebox", (object)base.BaseButtonOpenRight)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<ContainerButton>().Class("button").Class("OpenLeft")).Prop("stylebox", (object)base.BaseButtonOpenLeft)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<ContainerButton>().Class("button").Class("OpenBoth")).Prop("stylebox", (object)base.BaseButtonOpenBoth)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<ContainerButton>().Class("button").Class("ButtonSquare")).Prop("stylebox", (object)base.BaseButtonSquare)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<ContainerButton>().Class("button").Pseudo("normal")).Prop("modulate-self", (object)ButtonColorDefault)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<ContainerButton>().Class("button").Pseudo("hover")).Prop("modulate-self", (object)ButtonColorHovered)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<ContainerButton>().Class("button").Pseudo("pressed")).Prop("modulate-self", (object)ButtonColorPressed)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<ContainerButton>().Class("button").Pseudo("disabled")).Prop("modulate-self", (object)ButtonColorDisabled)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<ContainerButton>().Class("button").Class("Caution")
				.Pseudo("normal")).Prop("modulate-self", (object)ButtonColorCautionDefault)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<ContainerButton>().Class("button").Class("Caution")
				.Pseudo("hover")).Prop("modulate-self", (object)ButtonColorCautionHovered)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<ContainerButton>().Class("button").Class("Caution")
				.Pseudo("pressed")).Prop("modulate-self", (object)ButtonColorCautionPressed)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<ContainerButton>().Class("button").Class("Caution")
				.Pseudo("disabled")).Prop("modulate-self", (object)ButtonColorCautionDisabled)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<Label>().Class("button")).Prop("alignMode", (object)(AlignMode)1)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<PanelContainer>().Class("AngleRect")).Prop("panel", (object)base.BaseAngleRect).Prop("modulate-self", (object)Color.FromHex((ReadOnlySpan<char>)"#202030", (Color?)null))),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Child().Parent(MutableSelector.op_Implicit((MutableSelector)(object)StylesheetHelpers.Element<Button>().Class("disabled"))).Child(MutableSelector.op_Implicit((MutableSelector)(object)StylesheetHelpers.Element<Label>()))).Prop("font-color", (object)Color.FromHex((ReadOnlySpan<char>)"#E5E5E581", (Color?)null))),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<ProgressBar>()).Prop("background", (object)val).Prop("foreground", (object)val2)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<OptionButton>()).Prop("stylebox", (object)base.BaseButton)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<OptionButton>().Pseudo("normal")).Prop("modulate-self", (object)ButtonColorDefault)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<OptionButton>().Pseudo("hover")).Prop("modulate-self", (object)ButtonColorHovered)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<OptionButton>().Pseudo("pressed")).Prop("modulate-self", (object)ButtonColorPressed)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<OptionButton>().Pseudo("disabled")).Prop("modulate-self", (object)ButtonColorDisabled)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<TextureRect>().Class("optionTriangle")).Prop("texture", (object)texture)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<Label>().Class("optionButton")).Prop("alignMode", (object)(AlignMode)1)),
			new StyleRule((Selector)new SelectorElement(typeof(TabContainer), (IEnumerable<string>)null, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[3]
			{
				new StyleProperty("panel-stylebox", (object)val3),
				new StyleProperty("tab-stylebox", (object)val4),
				new StyleProperty("tab-stylebox-inactive", (object)val5)
			})
		}).ToList());
	}
}
