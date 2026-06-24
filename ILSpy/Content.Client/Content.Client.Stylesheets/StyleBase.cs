using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Client.Resources;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;

namespace Content.Client.Stylesheets;

public abstract class StyleBase
{
	public const string ClassHighDivider = "HighDivider";

	public const string ClassLowDivider = "LowDivider";

	public const string StyleClassLabelHeading = "LabelHeading";

	public const string StyleClassLabelSubText = "LabelSubText";

	public const string StyleClassItalic = "Italic";

	public const string ClassAngleRect = "AngleRect";

	public const string ButtonOpenRight = "OpenRight";

	public const string ButtonOpenLeft = "OpenLeft";

	public const string ButtonOpenBoth = "OpenBoth";

	public const string ButtonSquare = "ButtonSquare";

	public const string ButtonCaution = "Caution";

	public const int DefaultGrabberSize = 10;

	public abstract Stylesheet Stylesheet { get; }

	protected StyleRule[] BaseRules { get; }

	protected StyleBoxTexture BaseButton { get; }

	protected StyleBoxTexture BaseButtonOpenRight { get; }

	protected StyleBoxTexture BaseButtonOpenLeft { get; }

	protected StyleBoxTexture BaseButtonOpenBoth { get; }

	protected StyleBoxTexture BaseButtonSquare { get; }

	protected StyleBoxTexture BaseAngleRect { get; }

	protected StyleBoxTexture AngleBorderRect { get; }

	protected StyleBase(IResourceCache resCache)
	{
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Expected O, but got Unknown
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Expected O, but got Unknown
		//IL_0105: Expected O, but got Unknown
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Expected O, but got Unknown
		//IL_0178: Expected O, but got Unknown
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Expected O, but got Unknown
		//IL_01eb: Expected O, but got Unknown
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Expected O, but got Unknown
		//IL_0271: Expected O, but got Unknown
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Expected O, but got Unknown
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Expected O, but got Unknown
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Expected O, but got Unknown
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_0385: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Expected O, but got Unknown
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e5: Expected O, but got Unknown
		//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0403: Unknown result type (might be due to invalid IL or missing references)
		//IL_0413: Expected O, but got Unknown
		//IL_0413: Unknown result type (might be due to invalid IL or missing references)
		//IL_0418: Unknown result type (might be due to invalid IL or missing references)
		//IL_042d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0432: Unknown result type (might be due to invalid IL or missing references)
		//IL_043b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0445: Unknown result type (might be due to invalid IL or missing references)
		//IL_0455: Expected O, but got Unknown
		//IL_0455: Unknown result type (might be due to invalid IL or missing references)
		//IL_045a: Unknown result type (might be due to invalid IL or missing references)
		//IL_046f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0474: Unknown result type (might be due to invalid IL or missing references)
		//IL_047d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0487: Unknown result type (might be due to invalid IL or missing references)
		//IL_0497: Expected O, but got Unknown
		//IL_04a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c7: Expected O, but got Unknown
		//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c8: Expected O, but got Unknown
		//IL_04db: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fd: Expected O, but got Unknown
		//IL_04f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fe: Expected O, but got Unknown
		//IL_051a: Unknown result type (might be due to invalid IL or missing references)
		//IL_052d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0532: Unknown result type (might be due to invalid IL or missing references)
		//IL_0552: Unknown result type (might be due to invalid IL or missing references)
		//IL_055c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0561: Unknown result type (might be due to invalid IL or missing references)
		//IL_056b: Expected O, but got Unknown
		//IL_0566: Unknown result type (might be due to invalid IL or missing references)
		//IL_056c: Expected O, but got Unknown
		//IL_0595: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d4: Expected O, but got Unknown
		//IL_05cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d5: Expected O, but got Unknown
		//IL_05fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0624: Unknown result type (might be due to invalid IL or missing references)
		//IL_062e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0633: Unknown result type (might be due to invalid IL or missing references)
		//IL_063d: Expected O, but got Unknown
		//IL_0638: Unknown result type (might be due to invalid IL or missing references)
		//IL_063e: Expected O, but got Unknown
		//IL_064d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0661: Unknown result type (might be due to invalid IL or missing references)
		//IL_0666: Unknown result type (might be due to invalid IL or missing references)
		//IL_0670: Expected O, but got Unknown
		//IL_066b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0671: Expected O, but got Unknown
		//IL_068d: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b0: Expected O, but got Unknown
		//IL_06ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b1: Expected O, but got Unknown
		//IL_06cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f0: Expected O, but got Unknown
		//IL_06eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f1: Expected O, but got Unknown
		//IL_0700: Unknown result type (might be due to invalid IL or missing references)
		//IL_0714: Unknown result type (might be due to invalid IL or missing references)
		//IL_0719: Unknown result type (might be due to invalid IL or missing references)
		//IL_0723: Expected O, but got Unknown
		//IL_071e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0724: Expected O, but got Unknown
		//IL_0741: Unknown result type (might be due to invalid IL or missing references)
		//IL_0755: Unknown result type (might be due to invalid IL or missing references)
		//IL_075a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0764: Expected O, but got Unknown
		//IL_075f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0765: Expected O, but got Unknown
		//IL_0782: Unknown result type (might be due to invalid IL or missing references)
		//IL_0796: Unknown result type (might be due to invalid IL or missing references)
		//IL_079b: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a5: Expected O, but got Unknown
		//IL_07a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a6: Expected O, but got Unknown
		Font font = resCache.GetFont(new string[3] { "/Fonts/NotoSans/NotoSans-Regular.ttf", "/Fonts/NotoSans/NotoSansSymbols-Regular.ttf", "/Fonts/NotoSans/NotoSansSymbols2-Regular.ttf" }, 12);
		Font font2 = resCache.GetFont(new string[3] { "/Fonts/NotoSans/NotoSans-Italic.ttf", "/Fonts/NotoSans/NotoSansSymbols-Regular.ttf", "/Fonts/NotoSans/NotoSansSymbols2-Regular.ttf" }, 12);
		Texture texture = resCache.GetTexture("/Textures/Interface/Nano/cross.svg.png");
		Texture texture2 = resCache.GetTexture("/Textures/Interface/Nano/button.svg.96dpi.png");
		BaseButton = new StyleBoxTexture
		{
			Texture = texture2
		};
		BaseButton.SetPatchMargin((Margin)15, 10f);
		((StyleBox)BaseButton).SetPadding((Margin)15, 1f);
		((StyleBox)BaseButton).SetContentMarginOverride((Margin)3, 2f);
		((StyleBox)BaseButton).SetContentMarginOverride((Margin)12, 14f);
		BaseButtonOpenRight = new StyleBoxTexture(BaseButton)
		{
			Texture = (Texture)new AtlasTexture(texture2, UIBox2.FromDimensions(new Vector2(0f, 0f), new Vector2(14f, 24f)))
		};
		BaseButtonOpenRight.SetPatchMargin((Margin)4, 0f);
		((StyleBox)BaseButtonOpenRight).SetContentMarginOverride((Margin)4, 8f);
		((StyleBox)BaseButtonOpenRight).SetPadding((Margin)4, 2f);
		BaseButtonOpenLeft = new StyleBoxTexture(BaseButton)
		{
			Texture = (Texture)new AtlasTexture(texture2, UIBox2.FromDimensions(new Vector2(10f, 0f), new Vector2(14f, 24f)))
		};
		BaseButtonOpenLeft.SetPatchMargin((Margin)8, 0f);
		((StyleBox)BaseButtonOpenLeft).SetContentMarginOverride((Margin)8, 8f);
		((StyleBox)BaseButtonOpenLeft).SetPadding((Margin)8, 1f);
		BaseButtonOpenBoth = new StyleBoxTexture(BaseButton)
		{
			Texture = (Texture)new AtlasTexture(texture2, UIBox2.FromDimensions(new Vector2(10f, 0f), new Vector2(3f, 24f)))
		};
		BaseButtonOpenBoth.SetPatchMargin((Margin)12, 0f);
		((StyleBox)BaseButtonOpenBoth).SetContentMarginOverride((Margin)12, 8f);
		((StyleBox)BaseButtonOpenBoth).SetPadding((Margin)4, 2f);
		((StyleBox)BaseButtonOpenBoth).SetPadding((Margin)8, 1f);
		BaseButtonSquare = new StyleBoxTexture(BaseButton)
		{
			Texture = (Texture)new AtlasTexture(texture2, UIBox2.FromDimensions(new Vector2(10f, 0f), new Vector2(3f, 24f)))
		};
		BaseButtonSquare.SetPatchMargin((Margin)12, 0f);
		((StyleBox)BaseButtonSquare).SetContentMarginOverride((Margin)12, 8f);
		((StyleBox)BaseButtonSquare).SetPadding((Margin)4, 2f);
		((StyleBox)BaseButtonSquare).SetPadding((Margin)8, 1f);
		BaseAngleRect = new StyleBoxTexture
		{
			Texture = texture2
		};
		BaseAngleRect.SetPatchMargin((Margin)15, 10f);
		AngleBorderRect = new StyleBoxTexture
		{
			Texture = resCache.GetTexture("/Textures/Interface/Nano/geometric_panel_border.svg.96dpi.png")
		};
		AngleBorderRect.SetPatchMargin((Margin)15, 10f);
		StyleBoxFlat val = new StyleBoxFlat();
		Color val2 = Color.Gray;
		val.BackgroundColor = ((Color)(ref val2)).WithAlpha(0.35f);
		((StyleBox)val).ContentMarginLeftOverride = 10f;
		((StyleBox)val).ContentMarginTopOverride = 10f;
		StyleBoxFlat val3 = val;
		StyleBoxFlat val4 = new StyleBoxFlat();
		val2 = new Color((byte)140, (byte)140, (byte)140, byte.MaxValue);
		val4.BackgroundColor = ((Color)(ref val2)).WithAlpha(0.35f);
		((StyleBox)val4).ContentMarginLeftOverride = 10f;
		((StyleBox)val4).ContentMarginTopOverride = 10f;
		StyleBoxFlat val5 = val4;
		StyleBoxFlat val6 = new StyleBoxFlat();
		val2 = new Color((byte)160, (byte)160, (byte)160, byte.MaxValue);
		val6.BackgroundColor = ((Color)(ref val2)).WithAlpha(0.35f);
		((StyleBox)val6).ContentMarginLeftOverride = 10f;
		((StyleBox)val6).ContentMarginTopOverride = 10f;
		StyleBoxFlat val7 = val6;
		StyleBoxFlat val8 = new StyleBoxFlat();
		val2 = Color.Gray;
		val8.BackgroundColor = ((Color)(ref val2)).WithAlpha(0.35f);
		((StyleBox)val8).ContentMarginTopOverride = 10f;
		StyleBoxFlat val9 = val8;
		StyleBoxFlat val10 = new StyleBoxFlat();
		val2 = new Color((byte)140, (byte)140, (byte)140, byte.MaxValue);
		val10.BackgroundColor = ((Color)(ref val2)).WithAlpha(0.35f);
		((StyleBox)val10).ContentMarginTopOverride = 10f;
		StyleBoxFlat val11 = val10;
		StyleBoxFlat val12 = new StyleBoxFlat();
		val2 = new Color((byte)160, (byte)160, (byte)160, byte.MaxValue);
		val12.BackgroundColor = ((Color)(ref val2)).WithAlpha(0.35f);
		((StyleBox)val12).ContentMarginTopOverride = 10f;
		StyleBoxFlat val13 = val12;
		BaseRules = (StyleRule[])(object)new StyleRule[11]
		{
			new StyleRule((Selector)new SelectorElement((Type)null, (IEnumerable<string>)null, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("font", (object)font)
			}),
			new StyleRule((Selector)new SelectorElement((Type)null, (IEnumerable<string>)new string[1] { "Italic" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("font", (object)font2)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(TextureButton), (IEnumerable<string>)new string[1] { "windowCloseButton" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[2]
			{
				new StyleProperty("texture", (object)texture),
				new StyleProperty("modulate-self", (object)Color.FromHex((ReadOnlySpan<char>)"#4B596A", (Color?)null))
			}),
			new StyleRule((Selector)new SelectorElement(typeof(TextureButton), (IEnumerable<string>)new string[1] { "windowCloseButton" }, (string)null, (IEnumerable<string>)new string[1] { "hover" }), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("modulate-self", (object)Color.FromHex((ReadOnlySpan<char>)"#7F3636", (Color?)null))
			}),
			new StyleRule((Selector)new SelectorElement(typeof(TextureButton), (IEnumerable<string>)new string[1] { "windowCloseButton" }, (string)null, (IEnumerable<string>)new string[1] { "pressed" }), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("modulate-self", (object)Color.FromHex((ReadOnlySpan<char>)"#753131", (Color?)null))
			}),
			new StyleRule((Selector)new SelectorElement(typeof(VScrollBar), (IEnumerable<string>)null, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("grabber", (object)val3)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(VScrollBar), (IEnumerable<string>)null, (string)null, (IEnumerable<string>)new string[1] { "hover" }), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("grabber", (object)val5)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(VScrollBar), (IEnumerable<string>)null, (string)null, (IEnumerable<string>)new string[1] { "grabbed" }), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("grabber", (object)val7)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(HScrollBar), (IEnumerable<string>)null, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("grabber", (object)val9)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(HScrollBar), (IEnumerable<string>)null, (string)null, (IEnumerable<string>)new string[1] { "hover" }), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("grabber", (object)val11)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(HScrollBar), (IEnumerable<string>)null, (string)null, (IEnumerable<string>)new string[1] { "grabbed" }), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("grabber", (object)val13)
			})
		};
	}
}
