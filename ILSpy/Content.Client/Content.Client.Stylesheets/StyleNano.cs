using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Content.Client.ContextMenu.UI;
using Content.Client.Examine;
using Content.Client.PDA;
using Content.Client.Resources;
using Content.Client.UserInterface.Controls;
using Content.Shared.Verbs;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Maths;

namespace Content.Client.Stylesheets;

public sealed class StyleNano : StyleBase
{
	public const string StyleClassBorderedWindowPanel = "BorderedWindowPanel";

	public const string StyleClassInventorySlotBackground = "InventorySlotBackground";

	public const string StyleClassHandSlotHighlight = "HandSlotHighlight";

	public const string StyleClassChatPanel = "ChatPanel";

	public const string StyleClassChatSubPanel = "ChatSubPanel";

	public const string StyleClassTransparentBorderedWindowPanel = "TransparentBorderedWindowPanel";

	public const string StyleClassHotbarPanel = "HotbarPanel";

	public const string StyleClassTooltipPanel = "tooltipBox";

	public const string StyleClassTooltipAlertTitle = "tooltipAlertTitle";

	public const string StyleClassTooltipAlertDescription = "tooltipAlertDesc";

	public const string StyleClassTooltipAlertCooldown = "tooltipAlertCooldown";

	public const string StyleClassTooltipActionTitle = "tooltipActionTitle";

	public const string StyleClassTooltipActionDescription = "tooltipActionDesc";

	public const string StyleClassTooltipActionCooldown = "tooltipActionCooldown";

	public const string StyleClassTooltipActionDynamicMessage = "tooltipActionDynamicMessage";

	public const string StyleClassTooltipActionRequirements = "tooltipActionCooldown";

	public const string StyleClassTooltipActionCharges = "tooltipActionCharges";

	public const string StyleClassHotbarSlotNumber = "hotbarSlotNumber";

	public const string StyleClassActionSearchBox = "actionSearchBox";

	public const string StyleClassActionMenuItemRevoked = "actionMenuItemRevoked";

	public const string StyleClassChatLineEdit = "chatLineEdit";

	public const string StyleClassChatChannelSelectorButton = "chatSelectorOptionButton";

	public const string StyleClassChatFilterOptionButton = "chatFilterOptionButton";

	public const string StyleClassStorageButton = "storageButton";

	public const string StyleClassInset = "Inset";

	public const string StyleClassLobbyBackground = "LobbyBackground";

	public const string StyleClassConsoleHeading = "ConsoleHeading";

	public const string StyleClassConsoleSubHeading = "ConsoleSubHeading";

	public const string StyleClassConsoleText = "ConsoleText";

	public const string StyleClassSliderRed = "Red";

	public const string StyleClassSliderGreen = "Green";

	public const string StyleClassSliderBlue = "Blue";

	public const string StyleClassSliderWhite = "White";

	public const string StyleClassLabelHeadingBigger = "LabelHeadingBigger";

	public const string StyleClassLabelKeyText = "LabelKeyText";

	public const string StyleClassLabelSecondaryColor = "LabelSecondaryColor";

	public const string StyleClassLabelBig = "LabelBig";

	public const string StyleClassLabelSmall = "LabelSmall";

	public const string StyleClassButtonBig = "ButtonBig";

	public const string StyleClassButtonHelp = "HelpButton";

	public const string StyleClassPopupMessageSmall = "PopupMessageSmall";

	public const string StyleClassPopupMessageSmallCaution = "PopupMessageSmallCaution";

	public const string StyleClassPopupMessageMedium = "PopupMessageMedium";

	public const string StyleClassPopupMessageMediumCaution = "PopupMessageMediumCaution";

	public const string StyleClassPopupMessageLarge = "PopupMessageLarge";

	public const string StyleClassPopupMessageLargeCaution = "PopupMessageLargeCaution";

	public static readonly Color PanelDark = Color.FromHex((ReadOnlySpan<char>)"#1E1E22", (Color?)null);

	public static readonly Color NanoGold = Color.FromHex((ReadOnlySpan<char>)"#A88B5E", (Color?)null);

	public static readonly Color GoodGreenFore = Color.FromHex((ReadOnlySpan<char>)"#31843E", (Color?)null);

	public static readonly Color ConcerningOrangeFore = Color.FromHex((ReadOnlySpan<char>)"#A5762F", (Color?)null);

	public static readonly Color DangerousRedFore = Color.FromHex((ReadOnlySpan<char>)"#BB3232", (Color?)null);

	public static readonly Color DisabledFore = Color.FromHex((ReadOnlySpan<char>)"#5A5A5A", (Color?)null);

	public static readonly Color ButtonColorDefault = Color.FromHex((ReadOnlySpan<char>)"#464966", (Color?)null);

	public static readonly Color ButtonColorDefaultRed = Color.FromHex((ReadOnlySpan<char>)"#D43B3B", (Color?)null);

	public static readonly Color ButtonColorHovered = Color.FromHex((ReadOnlySpan<char>)"#575b7f", (Color?)null);

	public static readonly Color ButtonColorHoveredRed = Color.FromHex((ReadOnlySpan<char>)"#DF6B6B", (Color?)null);

	public static readonly Color ButtonColorPressed = Color.FromHex((ReadOnlySpan<char>)"#3e6c45", (Color?)null);

	public static readonly Color ButtonColorDisabled = Color.FromHex((ReadOnlySpan<char>)"#30313c", (Color?)null);

	public static readonly Color ButtonColorCautionDefault = Color.FromHex((ReadOnlySpan<char>)"#ab3232", (Color?)null);

	public static readonly Color ButtonColorCautionHovered = Color.FromHex((ReadOnlySpan<char>)"#cf2f2f", (Color?)null);

	public static readonly Color ButtonColorCautionPressed = Color.FromHex((ReadOnlySpan<char>)"#3e6c45", (Color?)null);

	public static readonly Color ButtonColorCautionDisabled = Color.FromHex((ReadOnlySpan<char>)"#602a2a", (Color?)null);

	public static readonly Color ButtonColorGoodDefault = Color.FromHex((ReadOnlySpan<char>)"#3E6C45", (Color?)null);

	public static readonly Color ButtonColorGoodHovered = Color.FromHex((ReadOnlySpan<char>)"#31843E", (Color?)null);

	public static readonly Color ButtonColorGoodDisabled = Color.FromHex((ReadOnlySpan<char>)"#164420", (Color?)null);

	public static readonly Color PointRed = Color.FromHex((ReadOnlySpan<char>)"#B02E26", (Color?)null);

	public static readonly Color PointGreen = Color.FromHex((ReadOnlySpan<char>)"#38b026", (Color?)null);

	public static readonly Color PointMagenta = Color.FromHex((ReadOnlySpan<char>)"#FF00FF", (Color?)null);

	public static readonly Color ButtonColorContext = Color.FromHex((ReadOnlySpan<char>)"#1119", (Color?)null);

	public static readonly Color ButtonColorContextHover = Color.DarkSlateGray;

	public static readonly Color ButtonColorContextPressed = Color.LightSlateGray;

	public static readonly Color ButtonColorContextDisabled = Color.Black;

	public static readonly Color ExamineButtonColorContext = Color.Transparent;

	public static readonly Color ExamineButtonColorContextHover = Color.DarkSlateGray;

	public static readonly Color ExamineButtonColorContextPressed = Color.LightSlateGray;

	public static readonly Color ExamineButtonColorContextDisabled = Color.FromHex((ReadOnlySpan<char>)"#5A5A5A", (Color?)null);

	public static readonly Color FancyTreeEvenRowColor = Color.FromHex((ReadOnlySpan<char>)"#25252A", (Color?)null);

	public static readonly Color FancyTreeOddRowColor;

	public static readonly Color FancyTreeSelectedRowColor;

	public const string StyleClassPowerStateNone = "PowerStateNone";

	public const string StyleClassPowerStateLow = "PowerStateLow";

	public const string StyleClassPowerStateGood = "PowerStateGood";

	public const string StyleClassItemStatus = "ItemStatus";

	public const string StyleClassItemStatusNotHeld = "ItemStatusNotHeld";

	public static readonly Color ItemStatusNotHeldColor;

	public const string StyleClassBackgroundBaseDark = "PanelBackgroundBaseDark";

	public const string StyleClassCrossButtonRed = "CrossButtonRed";

	public const string StyleClassButtonColorRed = "ButtonColorRed";

	public const string StyleClassButtonColorGreen = "ButtonColorGreen";

	public static readonly Color ChatBackgroundColor;

	public const string StyleClassPinButtonPinned = "pinButtonPinned";

	public const string StyleClassPinButtonUnpinned = "pinButtonUnpinned";

	public override Stylesheet Stylesheet { get; }

	public StyleNano(IResourceCache resCache)
		: base(resCache)
	{
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Expected O, but got Unknown
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Expected O, but got Unknown
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Expected O, but got Unknown
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Expected O, but got Unknown
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Expected O, but got Unknown
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Expected O, but got Unknown
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Expected O, but got Unknown
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Expected O, but got Unknown
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Expected O, but got Unknown
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Expected O, but got Unknown
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0343: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Expected O, but got Unknown
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0376: Expected O, but got Unknown
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Expected O, but got Unknown
		//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03df: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0405: Unknown result type (might be due to invalid IL or missing references)
		//IL_0412: Unknown result type (might be due to invalid IL or missing references)
		//IL_0418: Expected O, but got Unknown
		//IL_0413: Unknown result type (might be due to invalid IL or missing references)
		//IL_0418: Unknown result type (might be due to invalid IL or missing references)
		//IL_0419: Unknown result type (might be due to invalid IL or missing references)
		//IL_0424: Unknown result type (might be due to invalid IL or missing references)
		//IL_042a: Expected O, but got Unknown
		//IL_0425: Unknown result type (might be due to invalid IL or missing references)
		//IL_042a: Unknown result type (might be due to invalid IL or missing references)
		//IL_042b: Unknown result type (might be due to invalid IL or missing references)
		//IL_043b: Expected O, but got Unknown
		//IL_0436: Unknown result type (might be due to invalid IL or missing references)
		//IL_043b: Unknown result type (might be due to invalid IL or missing references)
		//IL_043c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0467: Unknown result type (might be due to invalid IL or missing references)
		//IL_046c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0474: Unknown result type (might be due to invalid IL or missing references)
		//IL_0481: Unknown result type (might be due to invalid IL or missing references)
		//IL_048e: Unknown result type (might be due to invalid IL or missing references)
		//IL_049a: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ad: Expected O, but got Unknown
		//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bc: Expected O, but got Unknown
		//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cd: Expected O, but got Unknown
		//IL_04c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0500: Unknown result type (might be due to invalid IL or missing references)
		//IL_050d: Unknown result type (might be due to invalid IL or missing references)
		//IL_051a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0520: Expected O, but got Unknown
		//IL_051b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0520: Unknown result type (might be due to invalid IL or missing references)
		//IL_0541: Unknown result type (might be due to invalid IL or missing references)
		//IL_0546: Unknown result type (might be due to invalid IL or missing references)
		//IL_0550: Expected O, but got Unknown
		//IL_0552: Expected O, but got Unknown
		//IL_055f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0565: Expected O, but got Unknown
		//IL_0560: Unknown result type (might be due to invalid IL or missing references)
		//IL_0565: Unknown result type (might be due to invalid IL or missing references)
		//IL_0586: Unknown result type (might be due to invalid IL or missing references)
		//IL_058b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0595: Expected O, but got Unknown
		//IL_0597: Expected O, but got Unknown
		//IL_05a9: Expected O, but got Unknown
		//IL_05a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d9: Expected O, but got Unknown
		//IL_05db: Expected O, but got Unknown
		//IL_05f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0605: Expected O, but got Unknown
		//IL_062e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0633: Unknown result type (might be due to invalid IL or missing references)
		//IL_063d: Expected O, but got Unknown
		//IL_0666: Unknown result type (might be due to invalid IL or missing references)
		//IL_066b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0675: Expected O, but got Unknown
		//IL_06b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c7: Expected O, but got Unknown
		//IL_06e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f0: Expected O, but got Unknown
		//IL_070c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0711: Unknown result type (might be due to invalid IL or missing references)
		//IL_0712: Unknown result type (might be due to invalid IL or missing references)
		//IL_071e: Expected O, but got Unknown
		//IL_071e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0723: Unknown result type (might be due to invalid IL or missing references)
		//IL_0724: Unknown result type (might be due to invalid IL or missing references)
		//IL_0747: Unknown result type (might be due to invalid IL or missing references)
		//IL_074c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0756: Expected O, but got Unknown
		//IL_077f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0784: Unknown result type (might be due to invalid IL or missing references)
		//IL_078e: Expected O, but got Unknown
		//IL_079c: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b9: Expected O, but got Unknown
		//IL_07c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e4: Expected O, but got Unknown
		//IL_07f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_080c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0818: Expected O, but got Unknown
		//IL_0825: Unknown result type (might be due to invalid IL or missing references)
		//IL_082a: Unknown result type (might be due to invalid IL or missing references)
		//IL_083f: Unknown result type (might be due to invalid IL or missing references)
		//IL_084b: Expected O, but got Unknown
		//IL_0858: Unknown result type (might be due to invalid IL or missing references)
		//IL_085d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0870: Expected O, but got Unknown
		//IL_08a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c1: Expected O, but got Unknown
		//IL_08c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_08db: Expected O, but got Unknown
		//IL_08dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f5: Expected O, but got Unknown
		//IL_08f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_08fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0906: Expected O, but got Unknown
		//IL_0908: Unknown result type (might be due to invalid IL or missing references)
		//IL_090d: Unknown result type (might be due to invalid IL or missing references)
		//IL_092e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0933: Unknown result type (might be due to invalid IL or missing references)
		//IL_093d: Expected O, but got Unknown
		//IL_093f: Expected O, but got Unknown
		//IL_094e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0953: Unknown result type (might be due to invalid IL or missing references)
		//IL_0974: Unknown result type (might be due to invalid IL or missing references)
		//IL_0979: Unknown result type (might be due to invalid IL or missing references)
		//IL_0983: Expected O, but got Unknown
		//IL_0985: Expected O, but got Unknown
		//IL_0994: Unknown result type (might be due to invalid IL or missing references)
		//IL_0999: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_09bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c9: Expected O, but got Unknown
		//IL_09cb: Expected O, but got Unknown
		//IL_0a1a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a1f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a29: Expected O, but got Unknown
		//IL_0a52: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a57: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a61: Expected O, but got Unknown
		//IL_0a8a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a8f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a99: Expected O, but got Unknown
		//IL_0abd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ac2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ace: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ada: Expected O, but got Unknown
		//IL_0af5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0afa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b06: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b12: Expected O, but got Unknown
		//IL_0b2d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b32: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b3e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b4a: Expected O, but got Unknown
		//IL_0b65: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b6a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b6b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b77: Expected O, but got Unknown
		//IL_0b9f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ba4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bbc: Expected O, but got Unknown
		//IL_0bc9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bd6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0be1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bf9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c07: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c14: Expected O, but got Unknown
		//IL_0c2f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c34: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c3c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c45: Expected O, but got Unknown
		//IL_0c6c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c71: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c79: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c8e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c9a: Expected O, but got Unknown
		//IL_0c9a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c9f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ca7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ca8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cb4: Expected O, but got Unknown
		//IL_0cb4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cb9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cc1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cd6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ce2: Expected O, but got Unknown
		//IL_0ce2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ce7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cf1: Expected O, but got Unknown
		//IL_0d2b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d30: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d31: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d3d: Expected O, but got Unknown
		//IL_0d3f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d44: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d45: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d51: Expected O, but got Unknown
		//IL_0d53: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d58: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d59: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d65: Expected O, but got Unknown
		//IL_0d67: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d6c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d6d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d79: Expected O, but got Unknown
		//IL_0d88: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d8d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d95: Unknown result type (might be due to invalid IL or missing references)
		//IL_0daa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0db6: Expected O, but got Unknown
		//IL_0dc4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dc9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dda: Unknown result type (might be due to invalid IL or missing references)
		//IL_0def: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dfb: Expected O, but got Unknown
		//IL_0e9a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eb6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ebb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ec9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ece: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ed8: Expected O, but got Unknown
		//IL_0ed3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ed9: Expected O, but got Unknown
		//IL_0ef5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f07: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f11: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f16: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f24: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f29: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f33: Expected O, but got Unknown
		//IL_0f2e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f34: Expected O, but got Unknown
		//IL_0f47: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f5b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f60: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f6a: Expected O, but got Unknown
		//IL_0f65: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f6b: Expected O, but got Unknown
		//IL_0f7e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f92: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f97: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fa1: Expected O, but got Unknown
		//IL_0f9c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fa2: Expected O, but got Unknown
		//IL_0fb5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fc9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fd8: Expected O, but got Unknown
		//IL_0fd3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fd9: Expected O, but got Unknown
		//IL_0fec: Unknown result type (might be due to invalid IL or missing references)
		//IL_1000: Unknown result type (might be due to invalid IL or missing references)
		//IL_1005: Unknown result type (might be due to invalid IL or missing references)
		//IL_100f: Expected O, but got Unknown
		//IL_100a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1010: Expected O, but got Unknown
		//IL_1023: Unknown result type (might be due to invalid IL or missing references)
		//IL_1037: Unknown result type (might be due to invalid IL or missing references)
		//IL_103c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1046: Expected O, but got Unknown
		//IL_1041: Unknown result type (might be due to invalid IL or missing references)
		//IL_1047: Expected O, but got Unknown
		//IL_1063: Unknown result type (might be due to invalid IL or missing references)
		//IL_1077: Unknown result type (might be due to invalid IL or missing references)
		//IL_107c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1086: Expected O, but got Unknown
		//IL_1081: Unknown result type (might be due to invalid IL or missing references)
		//IL_1087: Expected O, but got Unknown
		//IL_10a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_10b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_10bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_10c7: Expected O, but got Unknown
		//IL_10c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_10c8: Expected O, but got Unknown
		//IL_10e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_10f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_10fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_1108: Expected O, but got Unknown
		//IL_1103: Unknown result type (might be due to invalid IL or missing references)
		//IL_1109: Expected O, but got Unknown
		//IL_1216: Unknown result type (might be due to invalid IL or missing references)
		//IL_122e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1233: Unknown result type (might be due to invalid IL or missing references)
		//IL_123d: Expected O, but got Unknown
		//IL_1238: Unknown result type (might be due to invalid IL or missing references)
		//IL_123e: Expected O, but got Unknown
		//IL_125f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1295: Unknown result type (might be due to invalid IL or missing references)
		//IL_12cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_1301: Unknown result type (might be due to invalid IL or missing references)
		//IL_1341: Unknown result type (might be due to invalid IL or missing references)
		//IL_1381: Unknown result type (might be due to invalid IL or missing references)
		//IL_13c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_1401: Unknown result type (might be due to invalid IL or missing references)
		//IL_142d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1459: Unknown result type (might be due to invalid IL or missing references)
		//IL_1485: Unknown result type (might be due to invalid IL or missing references)
		//IL_14b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_14e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_14f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_14ff: Expected O, but got Unknown
		//IL_14ff: Expected O, but got Unknown
		//IL_14fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_1520: Unknown result type (might be due to invalid IL or missing references)
		//IL_152a: Unknown result type (might be due to invalid IL or missing references)
		//IL_152f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1539: Expected O, but got Unknown
		//IL_1534: Unknown result type (might be due to invalid IL or missing references)
		//IL_153a: Expected O, but got Unknown
		//IL_155c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1584: Unknown result type (might be due to invalid IL or missing references)
		//IL_15d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1656: Unknown result type (might be due to invalid IL or missing references)
		//IL_168c: Unknown result type (might be due to invalid IL or missing references)
		//IL_16c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_16f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1829: Unknown result type (might be due to invalid IL or missing references)
		//IL_185f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1895: Unknown result type (might be due to invalid IL or missing references)
		//IL_18cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_1925: Unknown result type (might be due to invalid IL or missing references)
		//IL_195b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1991: Unknown result type (might be due to invalid IL or missing references)
		//IL_19c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a8d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ac3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1af9: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b2f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b94: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bd5: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c16: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c57: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c80: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c92: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c9c: Expected O, but got Unknown
		//IL_1c9c: Expected O, but got Unknown
		//IL_1c97: Unknown result type (might be due to invalid IL or missing references)
		//IL_1cab: Unknown result type (might be due to invalid IL or missing references)
		//IL_1cb0: Unknown result type (might be due to invalid IL or missing references)
		//IL_1cba: Expected O, but got Unknown
		//IL_1cb5: Unknown result type (might be due to invalid IL or missing references)
		//IL_1cbb: Expected O, but got Unknown
		//IL_1ccf: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ce7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1cec: Unknown result type (might be due to invalid IL or missing references)
		//IL_1cf6: Expected O, but got Unknown
		//IL_1cf1: Unknown result type (might be due to invalid IL or missing references)
		//IL_1cf7: Expected O, but got Unknown
		//IL_1d07: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d1b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d20: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d2a: Expected O, but got Unknown
		//IL_1d25: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d2b: Expected O, but got Unknown
		//IL_1d48: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d6e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d78: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d7d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d87: Expected O, but got Unknown
		//IL_1d82: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d88: Expected O, but got Unknown
		//IL_1da5: Unknown result type (might be due to invalid IL or missing references)
		//IL_1db7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1dc1: Unknown result type (might be due to invalid IL or missing references)
		//IL_1dc6: Unknown result type (might be due to invalid IL or missing references)
		//IL_1dd0: Expected O, but got Unknown
		//IL_1dcb: Unknown result type (might be due to invalid IL or missing references)
		//IL_1dd1: Expected O, but got Unknown
		//IL_1de8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e1a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e2e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e33: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e3d: Expected O, but got Unknown
		//IL_1e38: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e3e: Expected O, but got Unknown
		//IL_1e5b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e6d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e77: Expected O, but got Unknown
		//IL_1e72: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e77: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e81: Expected O, but got Unknown
		//IL_1e7c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e82: Expected O, but got Unknown
		//IL_1e9f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1eb3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1eb8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ec2: Expected O, but got Unknown
		//IL_1ebd: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ec3: Expected O, but got Unknown
		//IL_1ed3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ee7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1eec: Unknown result type (might be due to invalid IL or missing references)
		//IL_1efa: Unknown result type (might be due to invalid IL or missing references)
		//IL_1eff: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f0d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f12: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f1c: Expected O, but got Unknown
		//IL_1f17: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f1d: Expected O, but got Unknown
		//IL_1f2d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f41: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f46: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f54: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f59: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f63: Expected O, but got Unknown
		//IL_1f5e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f64: Expected O, but got Unknown
		//IL_1f81: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f95: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f9a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1fa4: Expected O, but got Unknown
		//IL_1f9f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1fa5: Expected O, but got Unknown
		//IL_1fca: Unknown result type (might be due to invalid IL or missing references)
		//IL_1fde: Unknown result type (might be due to invalid IL or missing references)
		//IL_1fe3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1fed: Expected O, but got Unknown
		//IL_1fe8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1fee: Expected O, but got Unknown
		//IL_200b: Unknown result type (might be due to invalid IL or missing references)
		//IL_2024: Unknown result type (might be due to invalid IL or missing references)
		//IL_2029: Unknown result type (might be due to invalid IL or missing references)
		//IL_2033: Expected O, but got Unknown
		//IL_202e: Unknown result type (might be due to invalid IL or missing references)
		//IL_2034: Expected O, but got Unknown
		//IL_2051: Unknown result type (might be due to invalid IL or missing references)
		//IL_2065: Unknown result type (might be due to invalid IL or missing references)
		//IL_206a: Unknown result type (might be due to invalid IL or missing references)
		//IL_2074: Expected O, but got Unknown
		//IL_206f: Unknown result type (might be due to invalid IL or missing references)
		//IL_2075: Expected O, but got Unknown
		//IL_209a: Unknown result type (might be due to invalid IL or missing references)
		//IL_20ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_20b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_20bd: Expected O, but got Unknown
		//IL_20b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_20be: Expected O, but got Unknown
		//IL_20ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_20e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_20e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_20f1: Expected O, but got Unknown
		//IL_20ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_20f2: Expected O, but got Unknown
		//IL_210f: Unknown result type (might be due to invalid IL or missing references)
		//IL_2123: Unknown result type (might be due to invalid IL or missing references)
		//IL_2128: Unknown result type (might be due to invalid IL or missing references)
		//IL_2132: Expected O, but got Unknown
		//IL_212d: Unknown result type (might be due to invalid IL or missing references)
		//IL_2133: Expected O, but got Unknown
		//IL_2158: Unknown result type (might be due to invalid IL or missing references)
		//IL_216c: Unknown result type (might be due to invalid IL or missing references)
		//IL_2171: Unknown result type (might be due to invalid IL or missing references)
		//IL_217b: Expected O, but got Unknown
		//IL_2176: Unknown result type (might be due to invalid IL or missing references)
		//IL_217c: Expected O, but got Unknown
		//IL_21a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_21b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_21ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_21c4: Expected O, but got Unknown
		//IL_21bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_21c5: Expected O, but got Unknown
		//IL_21ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_2209: Unknown result type (might be due to invalid IL or missing references)
		//IL_2213: Expected O, but got Unknown
		//IL_2213: Expected O, but got Unknown
		//IL_220e: Unknown result type (might be due to invalid IL or missing references)
		//IL_2222: Unknown result type (might be due to invalid IL or missing references)
		//IL_2227: Unknown result type (might be due to invalid IL or missing references)
		//IL_2231: Expected O, but got Unknown
		//IL_222c: Unknown result type (might be due to invalid IL or missing references)
		//IL_2232: Expected O, but got Unknown
		//IL_2257: Unknown result type (might be due to invalid IL or missing references)
		//IL_2269: Unknown result type (might be due to invalid IL or missing references)
		//IL_2273: Expected O, but got Unknown
		//IL_2273: Expected O, but got Unknown
		//IL_226e: Unknown result type (might be due to invalid IL or missing references)
		//IL_2282: Unknown result type (might be due to invalid IL or missing references)
		//IL_2287: Unknown result type (might be due to invalid IL or missing references)
		//IL_2291: Expected O, but got Unknown
		//IL_228c: Unknown result type (might be due to invalid IL or missing references)
		//IL_2292: Expected O, but got Unknown
		//IL_22b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_22d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_22e0: Expected O, but got Unknown
		//IL_22e0: Expected O, but got Unknown
		//IL_22db: Unknown result type (might be due to invalid IL or missing references)
		//IL_22ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_22f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_22fe: Expected O, but got Unknown
		//IL_22f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_22ff: Expected O, but got Unknown
		//IL_2324: Unknown result type (might be due to invalid IL or missing references)
		//IL_2338: Unknown result type (might be due to invalid IL or missing references)
		//IL_233d: Unknown result type (might be due to invalid IL or missing references)
		//IL_2347: Expected O, but got Unknown
		//IL_2342: Unknown result type (might be due to invalid IL or missing references)
		//IL_2348: Expected O, but got Unknown
		//IL_236d: Unknown result type (might be due to invalid IL or missing references)
		//IL_238c: Unknown result type (might be due to invalid IL or missing references)
		//IL_2396: Expected O, but got Unknown
		//IL_2396: Expected O, but got Unknown
		//IL_2391: Unknown result type (might be due to invalid IL or missing references)
		//IL_23b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_23b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_23c0: Expected O, but got Unknown
		//IL_23bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_23c1: Expected O, but got Unknown
		//IL_23e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_23fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_23ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_2409: Expected O, but got Unknown
		//IL_2404: Unknown result type (might be due to invalid IL or missing references)
		//IL_240a: Expected O, but got Unknown
		//IL_2427: Unknown result type (might be due to invalid IL or missing references)
		//IL_243b: Unknown result type (might be due to invalid IL or missing references)
		//IL_2440: Unknown result type (might be due to invalid IL or missing references)
		//IL_244c: Unknown result type (might be due to invalid IL or missing references)
		//IL_2456: Unknown result type (might be due to invalid IL or missing references)
		//IL_245b: Unknown result type (might be due to invalid IL or missing references)
		//IL_2465: Expected O, but got Unknown
		//IL_2460: Unknown result type (might be due to invalid IL or missing references)
		//IL_2466: Expected O, but got Unknown
		//IL_2483: Unknown result type (might be due to invalid IL or missing references)
		//IL_2497: Unknown result type (might be due to invalid IL or missing references)
		//IL_249c: Unknown result type (might be due to invalid IL or missing references)
		//IL_24a6: Expected O, but got Unknown
		//IL_24a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_24a7: Expected O, but got Unknown
		//IL_24c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_24d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_24dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_24e7: Expected O, but got Unknown
		//IL_24e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_24e8: Expected O, but got Unknown
		//IL_2505: Unknown result type (might be due to invalid IL or missing references)
		//IL_2519: Unknown result type (might be due to invalid IL or missing references)
		//IL_251e: Unknown result type (might be due to invalid IL or missing references)
		//IL_2528: Expected O, but got Unknown
		//IL_2523: Unknown result type (might be due to invalid IL or missing references)
		//IL_2529: Expected O, but got Unknown
		//IL_2546: Unknown result type (might be due to invalid IL or missing references)
		//IL_255a: Unknown result type (might be due to invalid IL or missing references)
		//IL_255f: Unknown result type (might be due to invalid IL or missing references)
		//IL_2569: Expected O, but got Unknown
		//IL_2564: Unknown result type (might be due to invalid IL or missing references)
		//IL_256a: Expected O, but got Unknown
		//IL_2587: Unknown result type (might be due to invalid IL or missing references)
		//IL_259b: Unknown result type (might be due to invalid IL or missing references)
		//IL_25a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_25aa: Expected O, but got Unknown
		//IL_25a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_25ab: Expected O, but got Unknown
		//IL_25c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_25dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_25e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_25eb: Expected O, but got Unknown
		//IL_25e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_25ec: Expected O, but got Unknown
		//IL_2609: Unknown result type (might be due to invalid IL or missing references)
		//IL_261d: Unknown result type (might be due to invalid IL or missing references)
		//IL_2622: Unknown result type (might be due to invalid IL or missing references)
		//IL_262c: Expected O, but got Unknown
		//IL_2627: Unknown result type (might be due to invalid IL or missing references)
		//IL_262d: Expected O, but got Unknown
		//IL_264a: Unknown result type (might be due to invalid IL or missing references)
		//IL_265e: Unknown result type (might be due to invalid IL or missing references)
		//IL_2663: Unknown result type (might be due to invalid IL or missing references)
		//IL_266d: Expected O, but got Unknown
		//IL_2668: Unknown result type (might be due to invalid IL or missing references)
		//IL_266e: Expected O, but got Unknown
		//IL_268b: Unknown result type (might be due to invalid IL or missing references)
		//IL_269f: Unknown result type (might be due to invalid IL or missing references)
		//IL_26a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_26ae: Expected O, but got Unknown
		//IL_26a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_26af: Expected O, but got Unknown
		//IL_26cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_26df: Unknown result type (might be due to invalid IL or missing references)
		//IL_26e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_26f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_26fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_2705: Expected O, but got Unknown
		//IL_2700: Unknown result type (might be due to invalid IL or missing references)
		//IL_2706: Expected O, but got Unknown
		//IL_2723: Unknown result type (might be due to invalid IL or missing references)
		//IL_2737: Unknown result type (might be due to invalid IL or missing references)
		//IL_273c: Unknown result type (might be due to invalid IL or missing references)
		//IL_2746: Expected O, but got Unknown
		//IL_2741: Unknown result type (might be due to invalid IL or missing references)
		//IL_2747: Expected O, but got Unknown
		//IL_2764: Unknown result type (might be due to invalid IL or missing references)
		//IL_2778: Unknown result type (might be due to invalid IL or missing references)
		//IL_277d: Unknown result type (might be due to invalid IL or missing references)
		//IL_2787: Expected O, but got Unknown
		//IL_2782: Unknown result type (might be due to invalid IL or missing references)
		//IL_2788: Expected O, but got Unknown
		//IL_2798: Unknown result type (might be due to invalid IL or missing references)
		//IL_27aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_27af: Unknown result type (might be due to invalid IL or missing references)
		//IL_27bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_27ca: Expected O, but got Unknown
		//IL_27c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_27ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_27d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_27dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_27eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_27f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_27fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_2803: Unknown result type (might be due to invalid IL or missing references)
		//IL_280d: Expected O, but got Unknown
		//IL_2808: Unknown result type (might be due to invalid IL or missing references)
		//IL_280e: Expected O, but got Unknown
		//IL_282b: Unknown result type (might be due to invalid IL or missing references)
		//IL_283d: Unknown result type (might be due to invalid IL or missing references)
		//IL_2842: Unknown result type (might be due to invalid IL or missing references)
		//IL_2843: Unknown result type (might be due to invalid IL or missing references)
		//IL_2852: Expected O, but got Unknown
		//IL_284d: Unknown result type (might be due to invalid IL or missing references)
		//IL_2852: Unknown result type (might be due to invalid IL or missing references)
		//IL_2860: Unknown result type (might be due to invalid IL or missing references)
		//IL_2865: Unknown result type (might be due to invalid IL or missing references)
		//IL_2873: Unknown result type (might be due to invalid IL or missing references)
		//IL_2878: Unknown result type (might be due to invalid IL or missing references)
		//IL_2886: Unknown result type (might be due to invalid IL or missing references)
		//IL_288b: Unknown result type (might be due to invalid IL or missing references)
		//IL_2895: Expected O, but got Unknown
		//IL_2890: Unknown result type (might be due to invalid IL or missing references)
		//IL_2896: Expected O, but got Unknown
		//IL_28b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_28c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_28ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_28cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_28da: Expected O, but got Unknown
		//IL_28d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_28da: Unknown result type (might be due to invalid IL or missing references)
		//IL_28e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_28ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_28fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_2900: Unknown result type (might be due to invalid IL or missing references)
		//IL_290e: Unknown result type (might be due to invalid IL or missing references)
		//IL_2913: Unknown result type (might be due to invalid IL or missing references)
		//IL_291d: Expected O, but got Unknown
		//IL_2918: Unknown result type (might be due to invalid IL or missing references)
		//IL_291e: Expected O, but got Unknown
		//IL_292e: Unknown result type (might be due to invalid IL or missing references)
		//IL_2940: Unknown result type (might be due to invalid IL or missing references)
		//IL_2945: Unknown result type (might be due to invalid IL or missing references)
		//IL_2951: Unknown result type (might be due to invalid IL or missing references)
		//IL_2960: Expected O, but got Unknown
		//IL_295b: Unknown result type (might be due to invalid IL or missing references)
		//IL_2960: Unknown result type (might be due to invalid IL or missing references)
		//IL_296c: Unknown result type (might be due to invalid IL or missing references)
		//IL_2971: Unknown result type (might be due to invalid IL or missing references)
		//IL_297d: Unknown result type (might be due to invalid IL or missing references)
		//IL_2987: Unknown result type (might be due to invalid IL or missing references)
		//IL_2999: Expected O, but got Unknown
		//IL_2994: Unknown result type (might be due to invalid IL or missing references)
		//IL_2999: Unknown result type (might be due to invalid IL or missing references)
		//IL_29a3: Expected O, but got Unknown
		//IL_299e: Unknown result type (might be due to invalid IL or missing references)
		//IL_29a4: Expected O, but got Unknown
		//IL_29b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_29c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_29cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_29d7: Expected O, but got Unknown
		//IL_29d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_29d8: Expected O, but got Unknown
		//IL_29f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_2a09: Unknown result type (might be due to invalid IL or missing references)
		//IL_2a0e: Unknown result type (might be due to invalid IL or missing references)
		//IL_2a25: Unknown result type (might be due to invalid IL or missing references)
		//IL_2a2f: Unknown result type (might be due to invalid IL or missing references)
		//IL_2a34: Unknown result type (might be due to invalid IL or missing references)
		//IL_2a3e: Expected O, but got Unknown
		//IL_2a39: Unknown result type (might be due to invalid IL or missing references)
		//IL_2a3f: Expected O, but got Unknown
		//IL_2a5c: Unknown result type (might be due to invalid IL or missing references)
		//IL_2a70: Unknown result type (might be due to invalid IL or missing references)
		//IL_2a75: Unknown result type (might be due to invalid IL or missing references)
		//IL_2a81: Unknown result type (might be due to invalid IL or missing references)
		//IL_2a8b: Unknown result type (might be due to invalid IL or missing references)
		//IL_2a90: Unknown result type (might be due to invalid IL or missing references)
		//IL_2a9a: Expected O, but got Unknown
		//IL_2a95: Unknown result type (might be due to invalid IL or missing references)
		//IL_2a9b: Expected O, but got Unknown
		//IL_2ab8: Unknown result type (might be due to invalid IL or missing references)
		//IL_2acc: Unknown result type (might be due to invalid IL or missing references)
		//IL_2ad1: Unknown result type (might be due to invalid IL or missing references)
		//IL_2add: Unknown result type (might be due to invalid IL or missing references)
		//IL_2ae7: Unknown result type (might be due to invalid IL or missing references)
		//IL_2aec: Unknown result type (might be due to invalid IL or missing references)
		//IL_2af6: Expected O, but got Unknown
		//IL_2af1: Unknown result type (might be due to invalid IL or missing references)
		//IL_2af7: Expected O, but got Unknown
		//IL_2b14: Unknown result type (might be due to invalid IL or missing references)
		//IL_2b27: Unknown result type (might be due to invalid IL or missing references)
		//IL_2b2c: Unknown result type (might be due to invalid IL or missing references)
		//IL_2b38: Unknown result type (might be due to invalid IL or missing references)
		//IL_2b42: Unknown result type (might be due to invalid IL or missing references)
		//IL_2b47: Unknown result type (might be due to invalid IL or missing references)
		//IL_2b51: Expected O, but got Unknown
		//IL_2b4c: Unknown result type (might be due to invalid IL or missing references)
		//IL_2b52: Expected O, but got Unknown
		//IL_2b6f: Unknown result type (might be due to invalid IL or missing references)
		//IL_2b83: Unknown result type (might be due to invalid IL or missing references)
		//IL_2b88: Unknown result type (might be due to invalid IL or missing references)
		//IL_2b94: Unknown result type (might be due to invalid IL or missing references)
		//IL_2b9e: Unknown result type (might be due to invalid IL or missing references)
		//IL_2ba3: Unknown result type (might be due to invalid IL or missing references)
		//IL_2bad: Expected O, but got Unknown
		//IL_2ba8: Unknown result type (might be due to invalid IL or missing references)
		//IL_2bae: Expected O, but got Unknown
		//IL_2bcb: Unknown result type (might be due to invalid IL or missing references)
		//IL_2bde: Unknown result type (might be due to invalid IL or missing references)
		//IL_2be3: Unknown result type (might be due to invalid IL or missing references)
		//IL_2bef: Unknown result type (might be due to invalid IL or missing references)
		//IL_2bf9: Unknown result type (might be due to invalid IL or missing references)
		//IL_2bfe: Unknown result type (might be due to invalid IL or missing references)
		//IL_2c08: Expected O, but got Unknown
		//IL_2c03: Unknown result type (might be due to invalid IL or missing references)
		//IL_2c09: Expected O, but got Unknown
		//IL_2c26: Unknown result type (might be due to invalid IL or missing references)
		//IL_2c3a: Unknown result type (might be due to invalid IL or missing references)
		//IL_2c3f: Unknown result type (might be due to invalid IL or missing references)
		//IL_2c49: Expected O, but got Unknown
		//IL_2c44: Unknown result type (might be due to invalid IL or missing references)
		//IL_2c4a: Expected O, but got Unknown
		//IL_2c67: Unknown result type (might be due to invalid IL or missing references)
		//IL_2c7b: Unknown result type (might be due to invalid IL or missing references)
		//IL_2c80: Unknown result type (might be due to invalid IL or missing references)
		//IL_2c8a: Expected O, but got Unknown
		//IL_2c85: Unknown result type (might be due to invalid IL or missing references)
		//IL_2c8b: Expected O, but got Unknown
		//IL_2ca8: Unknown result type (might be due to invalid IL or missing references)
		//IL_2cbc: Unknown result type (might be due to invalid IL or missing references)
		//IL_2cc1: Unknown result type (might be due to invalid IL or missing references)
		//IL_2ccb: Expected O, but got Unknown
		//IL_2cc6: Unknown result type (might be due to invalid IL or missing references)
		//IL_2ccc: Expected O, but got Unknown
		//IL_2ce9: Unknown result type (might be due to invalid IL or missing references)
		//IL_2cfb: Unknown result type (might be due to invalid IL or missing references)
		//IL_2d05: Expected O, but got Unknown
		//IL_2d05: Expected O, but got Unknown
		//IL_2d00: Unknown result type (might be due to invalid IL or missing references)
		//IL_2d14: Unknown result type (might be due to invalid IL or missing references)
		//IL_2d19: Unknown result type (might be due to invalid IL or missing references)
		//IL_2d23: Expected O, but got Unknown
		//IL_2d1e: Unknown result type (might be due to invalid IL or missing references)
		//IL_2d24: Expected O, but got Unknown
		//IL_2d41: Unknown result type (might be due to invalid IL or missing references)
		//IL_2d67: Unknown result type (might be due to invalid IL or missing references)
		//IL_2d71: Unknown result type (might be due to invalid IL or missing references)
		//IL_2d76: Unknown result type (might be due to invalid IL or missing references)
		//IL_2d80: Expected O, but got Unknown
		//IL_2d7b: Unknown result type (might be due to invalid IL or missing references)
		//IL_2d81: Expected O, but got Unknown
		//IL_2d9e: Unknown result type (might be due to invalid IL or missing references)
		//IL_2dc4: Unknown result type (might be due to invalid IL or missing references)
		//IL_2dce: Unknown result type (might be due to invalid IL or missing references)
		//IL_2dd3: Unknown result type (might be due to invalid IL or missing references)
		//IL_2ddd: Expected O, but got Unknown
		//IL_2dd8: Unknown result type (might be due to invalid IL or missing references)
		//IL_2dde: Expected O, but got Unknown
		//IL_2dfb: Unknown result type (might be due to invalid IL or missing references)
		//IL_2e21: Unknown result type (might be due to invalid IL or missing references)
		//IL_2e2b: Unknown result type (might be due to invalid IL or missing references)
		//IL_2e30: Unknown result type (might be due to invalid IL or missing references)
		//IL_2e3a: Expected O, but got Unknown
		//IL_2e35: Unknown result type (might be due to invalid IL or missing references)
		//IL_2e3b: Expected O, but got Unknown
		//IL_2e58: Unknown result type (might be due to invalid IL or missing references)
		//IL_2e6c: Unknown result type (might be due to invalid IL or missing references)
		//IL_2e71: Unknown result type (might be due to invalid IL or missing references)
		//IL_2e7b: Expected O, but got Unknown
		//IL_2e76: Unknown result type (might be due to invalid IL or missing references)
		//IL_2e7c: Expected O, but got Unknown
		//IL_2e99: Unknown result type (might be due to invalid IL or missing references)
		//IL_2ead: Unknown result type (might be due to invalid IL or missing references)
		//IL_2eb2: Unknown result type (might be due to invalid IL or missing references)
		//IL_2ebc: Expected O, but got Unknown
		//IL_2eb7: Unknown result type (might be due to invalid IL or missing references)
		//IL_2ebd: Expected O, but got Unknown
		//IL_2eda: Unknown result type (might be due to invalid IL or missing references)
		//IL_2eee: Unknown result type (might be due to invalid IL or missing references)
		//IL_2ef3: Unknown result type (might be due to invalid IL or missing references)
		//IL_2efd: Expected O, but got Unknown
		//IL_2ef8: Unknown result type (might be due to invalid IL or missing references)
		//IL_2efe: Expected O, but got Unknown
		//IL_2f1b: Unknown result type (might be due to invalid IL or missing references)
		//IL_2f2d: Unknown result type (might be due to invalid IL or missing references)
		//IL_2f37: Unknown result type (might be due to invalid IL or missing references)
		//IL_2f3c: Unknown result type (might be due to invalid IL or missing references)
		//IL_2f46: Expected O, but got Unknown
		//IL_2f41: Unknown result type (might be due to invalid IL or missing references)
		//IL_2f47: Expected O, but got Unknown
		//IL_2f74: Unknown result type (might be due to invalid IL or missing references)
		//IL_2f86: Unknown result type (might be due to invalid IL or missing references)
		//IL_2f90: Unknown result type (might be due to invalid IL or missing references)
		//IL_2f95: Unknown result type (might be due to invalid IL or missing references)
		//IL_2f9f: Expected O, but got Unknown
		//IL_2f9a: Unknown result type (might be due to invalid IL or missing references)
		//IL_2fa0: Expected O, but got Unknown
		//IL_2fc0: Unknown result type (might be due to invalid IL or missing references)
		//IL_2fd2: Unknown result type (might be due to invalid IL or missing references)
		//IL_2fdc: Unknown result type (might be due to invalid IL or missing references)
		//IL_2fe1: Unknown result type (might be due to invalid IL or missing references)
		//IL_2feb: Expected O, but got Unknown
		//IL_2fe6: Unknown result type (might be due to invalid IL or missing references)
		//IL_2fec: Expected O, but got Unknown
		//IL_300c: Unknown result type (might be due to invalid IL or missing references)
		//IL_301e: Unknown result type (might be due to invalid IL or missing references)
		//IL_3028: Unknown result type (might be due to invalid IL or missing references)
		//IL_302d: Unknown result type (might be due to invalid IL or missing references)
		//IL_3037: Expected O, but got Unknown
		//IL_3032: Unknown result type (might be due to invalid IL or missing references)
		//IL_3038: Expected O, but got Unknown
		//IL_3058: Unknown result type (might be due to invalid IL or missing references)
		//IL_306a: Unknown result type (might be due to invalid IL or missing references)
		//IL_3074: Unknown result type (might be due to invalid IL or missing references)
		//IL_3079: Unknown result type (might be due to invalid IL or missing references)
		//IL_3083: Expected O, but got Unknown
		//IL_307e: Unknown result type (might be due to invalid IL or missing references)
		//IL_3084: Expected O, but got Unknown
		//IL_30b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_30c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_30cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_30d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_30dc: Expected O, but got Unknown
		//IL_30d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_30dd: Expected O, but got Unknown
		//IL_30fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_3111: Unknown result type (might be due to invalid IL or missing references)
		//IL_3116: Unknown result type (might be due to invalid IL or missing references)
		//IL_3120: Expected O, but got Unknown
		//IL_311b: Unknown result type (might be due to invalid IL or missing references)
		//IL_3121: Expected O, but got Unknown
		//IL_3134: Unknown result type (might be due to invalid IL or missing references)
		//IL_3148: Unknown result type (might be due to invalid IL or missing references)
		//IL_314d: Unknown result type (might be due to invalid IL or missing references)
		//IL_3157: Expected O, but got Unknown
		//IL_3152: Unknown result type (might be due to invalid IL or missing references)
		//IL_3158: Expected O, but got Unknown
		//IL_3178: Unknown result type (might be due to invalid IL or missing references)
		//IL_318c: Unknown result type (might be due to invalid IL or missing references)
		//IL_3191: Unknown result type (might be due to invalid IL or missing references)
		//IL_319b: Expected O, but got Unknown
		//IL_3196: Unknown result type (might be due to invalid IL or missing references)
		//IL_319c: Expected O, but got Unknown
		//IL_31bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_31d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_31d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_31df: Expected O, but got Unknown
		//IL_31da: Unknown result type (might be due to invalid IL or missing references)
		//IL_31e0: Expected O, but got Unknown
		//IL_3200: Unknown result type (might be due to invalid IL or missing references)
		//IL_3214: Unknown result type (might be due to invalid IL or missing references)
		//IL_3219: Unknown result type (might be due to invalid IL or missing references)
		//IL_3223: Expected O, but got Unknown
		//IL_321e: Unknown result type (might be due to invalid IL or missing references)
		//IL_3224: Expected O, but got Unknown
		//IL_3244: Unknown result type (might be due to invalid IL or missing references)
		//IL_3258: Unknown result type (might be due to invalid IL or missing references)
		//IL_325d: Unknown result type (might be due to invalid IL or missing references)
		//IL_3267: Expected O, but got Unknown
		//IL_3262: Unknown result type (might be due to invalid IL or missing references)
		//IL_3268: Expected O, but got Unknown
		//IL_3295: Unknown result type (might be due to invalid IL or missing references)
		//IL_32a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_32ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_32b8: Expected O, but got Unknown
		//IL_32b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_32b9: Expected O, but got Unknown
		//IL_32e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_32fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_32ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_3309: Expected O, but got Unknown
		//IL_3304: Unknown result type (might be due to invalid IL or missing references)
		//IL_330a: Expected O, but got Unknown
		//IL_3337: Unknown result type (might be due to invalid IL or missing references)
		//IL_334b: Unknown result type (might be due to invalid IL or missing references)
		//IL_3350: Unknown result type (might be due to invalid IL or missing references)
		//IL_335a: Expected O, but got Unknown
		//IL_3355: Unknown result type (might be due to invalid IL or missing references)
		//IL_335b: Expected O, but got Unknown
		//IL_337f: Unknown result type (might be due to invalid IL or missing references)
		//IL_3393: Unknown result type (might be due to invalid IL or missing references)
		//IL_3398: Unknown result type (might be due to invalid IL or missing references)
		//IL_33a2: Expected O, but got Unknown
		//IL_339d: Unknown result type (might be due to invalid IL or missing references)
		//IL_33a3: Expected O, but got Unknown
		//IL_33c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_33cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_33d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_33d7: Expected O, but got Unknown
		//IL_33fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_3403: Unknown result type (might be due to invalid IL or missing references)
		//IL_3408: Unknown result type (might be due to invalid IL or missing references)
		//IL_340e: Expected O, but got Unknown
		//IL_3433: Unknown result type (might be due to invalid IL or missing references)
		//IL_348a: Unknown result type (might be due to invalid IL or missing references)
		//IL_34c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_34c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_34d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_34db: Unknown result type (might be due to invalid IL or missing references)
		//IL_34e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_34ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_34fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_3501: Unknown result type (might be due to invalid IL or missing references)
		//IL_3506: Unknown result type (might be due to invalid IL or missing references)
		//IL_350c: Expected O, but got Unknown
		//IL_3530: Unknown result type (might be due to invalid IL or missing references)
		//IL_3535: Unknown result type (might be due to invalid IL or missing references)
		//IL_3543: Unknown result type (might be due to invalid IL or missing references)
		//IL_3548: Unknown result type (might be due to invalid IL or missing references)
		//IL_354d: Unknown result type (might be due to invalid IL or missing references)
		//IL_3553: Expected O, but got Unknown
		//IL_3573: Unknown result type (might be due to invalid IL or missing references)
		//IL_3587: Unknown result type (might be due to invalid IL or missing references)
		//IL_358c: Unknown result type (might be due to invalid IL or missing references)
		//IL_3596: Expected O, but got Unknown
		//IL_3591: Unknown result type (might be due to invalid IL or missing references)
		//IL_3597: Expected O, but got Unknown
		//IL_35b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_35cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_35d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_35da: Expected O, but got Unknown
		//IL_35d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_35db: Expected O, but got Unknown
		//IL_35fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_360f: Unknown result type (might be due to invalid IL or missing references)
		//IL_3614: Unknown result type (might be due to invalid IL or missing references)
		//IL_361e: Expected O, but got Unknown
		//IL_3619: Unknown result type (might be due to invalid IL or missing references)
		//IL_361f: Expected O, but got Unknown
		//IL_363f: Unknown result type (might be due to invalid IL or missing references)
		//IL_3653: Unknown result type (might be due to invalid IL or missing references)
		//IL_3658: Unknown result type (might be due to invalid IL or missing references)
		//IL_3662: Expected O, but got Unknown
		//IL_365d: Unknown result type (might be due to invalid IL or missing references)
		//IL_3663: Expected O, but got Unknown
		//IL_3683: Unknown result type (might be due to invalid IL or missing references)
		//IL_3697: Unknown result type (might be due to invalid IL or missing references)
		//IL_369c: Unknown result type (might be due to invalid IL or missing references)
		//IL_36a6: Expected O, but got Unknown
		//IL_36a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_36a7: Expected O, but got Unknown
		//IL_36c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_36db: Unknown result type (might be due to invalid IL or missing references)
		//IL_36e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_36ea: Expected O, but got Unknown
		//IL_36e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_36eb: Expected O, but got Unknown
		//IL_3718: Unknown result type (might be due to invalid IL or missing references)
		//IL_372a: Unknown result type (might be due to invalid IL or missing references)
		//IL_3734: Unknown result type (might be due to invalid IL or missing references)
		//IL_3739: Unknown result type (might be due to invalid IL or missing references)
		//IL_3743: Expected O, but got Unknown
		//IL_373e: Unknown result type (might be due to invalid IL or missing references)
		//IL_3744: Expected O, but got Unknown
		//IL_3771: Unknown result type (might be due to invalid IL or missing references)
		//IL_3783: Unknown result type (might be due to invalid IL or missing references)
		//IL_378d: Unknown result type (might be due to invalid IL or missing references)
		//IL_3792: Unknown result type (might be due to invalid IL or missing references)
		//IL_379c: Expected O, but got Unknown
		//IL_3797: Unknown result type (might be due to invalid IL or missing references)
		//IL_379d: Expected O, but got Unknown
		//IL_37ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_37dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_37e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_37eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_37f5: Expected O, but got Unknown
		//IL_37f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_37f6: Expected O, but got Unknown
		//IL_3823: Unknown result type (might be due to invalid IL or missing references)
		//IL_3835: Unknown result type (might be due to invalid IL or missing references)
		//IL_383f: Unknown result type (might be due to invalid IL or missing references)
		//IL_3844: Unknown result type (might be due to invalid IL or missing references)
		//IL_384e: Expected O, but got Unknown
		//IL_3849: Unknown result type (might be due to invalid IL or missing references)
		//IL_384f: Expected O, but got Unknown
		//IL_3889: Unknown result type (might be due to invalid IL or missing references)
		//IL_38a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_38a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_38b0: Expected O, but got Unknown
		//IL_38ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_38b1: Expected O, but got Unknown
		//IL_38d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_38e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_38ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_38f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_38fc: Expected O, but got Unknown
		//IL_38f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_38fd: Expected O, but got Unknown
		//IL_391d: Unknown result type (might be due to invalid IL or missing references)
		//IL_392f: Unknown result type (might be due to invalid IL or missing references)
		//IL_3939: Unknown result type (might be due to invalid IL or missing references)
		//IL_393e: Unknown result type (might be due to invalid IL or missing references)
		//IL_3948: Expected O, but got Unknown
		//IL_3943: Unknown result type (might be due to invalid IL or missing references)
		//IL_3949: Expected O, but got Unknown
		//IL_3969: Unknown result type (might be due to invalid IL or missing references)
		//IL_397b: Unknown result type (might be due to invalid IL or missing references)
		//IL_3985: Unknown result type (might be due to invalid IL or missing references)
		//IL_398a: Unknown result type (might be due to invalid IL or missing references)
		//IL_3994: Expected O, but got Unknown
		//IL_398f: Unknown result type (might be due to invalid IL or missing references)
		//IL_3995: Expected O, but got Unknown
		//IL_39b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_39c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_39d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_39d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_39e0: Expected O, but got Unknown
		//IL_39db: Unknown result type (might be due to invalid IL or missing references)
		//IL_39e1: Expected O, but got Unknown
		//IL_3a01: Unknown result type (might be due to invalid IL or missing references)
		//IL_3a15: Unknown result type (might be due to invalid IL or missing references)
		//IL_3a1a: Unknown result type (might be due to invalid IL or missing references)
		//IL_3a24: Expected O, but got Unknown
		//IL_3a1f: Unknown result type (might be due to invalid IL or missing references)
		//IL_3a25: Expected O, but got Unknown
		//IL_3a45: Unknown result type (might be due to invalid IL or missing references)
		//IL_3a5d: Unknown result type (might be due to invalid IL or missing references)
		//IL_3a62: Unknown result type (might be due to invalid IL or missing references)
		//IL_3a6c: Expected O, but got Unknown
		//IL_3a67: Unknown result type (might be due to invalid IL or missing references)
		//IL_3a6d: Expected O, but got Unknown
		//IL_3a9b: Unknown result type (might be due to invalid IL or missing references)
		//IL_3aa0: Unknown result type (might be due to invalid IL or missing references)
		//IL_3aaa: Expected O, but got Unknown
		//IL_3ad0: Unknown result type (might be due to invalid IL or missing references)
		//IL_3ae2: Unknown result type (might be due to invalid IL or missing references)
		//IL_3ae7: Unknown result type (might be due to invalid IL or missing references)
		//IL_3ae8: Unknown result type (might be due to invalid IL or missing references)
		//IL_3af2: Unknown result type (might be due to invalid IL or missing references)
		//IL_3aff: Unknown result type (might be due to invalid IL or missing references)
		//IL_3b11: Expected O, but got Unknown
		//IL_3b0c: Unknown result type (might be due to invalid IL or missing references)
		//IL_3b11: Unknown result type (might be due to invalid IL or missing references)
		//IL_3b1b: Expected O, but got Unknown
		//IL_3b16: Unknown result type (might be due to invalid IL or missing references)
		//IL_3b1c: Expected O, but got Unknown
		//IL_3bd7: Unknown result type (might be due to invalid IL or missing references)
		//IL_3c12: Unknown result type (might be due to invalid IL or missing references)
		//IL_3c65: Unknown result type (might be due to invalid IL or missing references)
		//IL_3cb8: Unknown result type (might be due to invalid IL or missing references)
		//IL_3ce7: Unknown result type (might be due to invalid IL or missing references)
		//IL_3cec: Unknown result type (might be due to invalid IL or missing references)
		//IL_3d01: Unknown result type (might be due to invalid IL or missing references)
		//IL_3d0b: Unknown result type (might be due to invalid IL or missing references)
		//IL_3d18: Unknown result type (might be due to invalid IL or missing references)
		//IL_3d2a: Expected O, but got Unknown
		//IL_3d56: Unknown result type (might be due to invalid IL or missing references)
		//IL_3d8b: Unknown result type (might be due to invalid IL or missing references)
		//IL_3d90: Unknown result type (might be due to invalid IL or missing references)
		//IL_3d93: Unknown result type (might be due to invalid IL or missing references)
		//IL_3d99: Unknown result type (might be due to invalid IL or missing references)
		//IL_3da5: Expected O, but got Unknown
		//IL_3dbe: Unknown result type (might be due to invalid IL or missing references)
		//IL_3df3: Unknown result type (might be due to invalid IL or missing references)
		//IL_3df8: Unknown result type (might be due to invalid IL or missing references)
		//IL_3dfb: Unknown result type (might be due to invalid IL or missing references)
		//IL_3e01: Unknown result type (might be due to invalid IL or missing references)
		//IL_3e0d: Expected O, but got Unknown
		//IL_3e56: Unknown result type (might be due to invalid IL or missing references)
		//IL_3ea3: Unknown result type (might be due to invalid IL or missing references)
		//IL_3ef0: Unknown result type (might be due to invalid IL or missing references)
		//IL_3f25: Unknown result type (might be due to invalid IL or missing references)
		//IL_3f2a: Unknown result type (might be due to invalid IL or missing references)
		//IL_3f2d: Unknown result type (might be due to invalid IL or missing references)
		//IL_3f33: Unknown result type (might be due to invalid IL or missing references)
		//IL_3f3f: Expected O, but got Unknown
		//IL_3f58: Unknown result type (might be due to invalid IL or missing references)
		//IL_3f8d: Unknown result type (might be due to invalid IL or missing references)
		//IL_3f92: Unknown result type (might be due to invalid IL or missing references)
		//IL_3f95: Unknown result type (might be due to invalid IL or missing references)
		//IL_3f9b: Unknown result type (might be due to invalid IL or missing references)
		//IL_3fa7: Expected O, but got Unknown
		//IL_3fc0: Unknown result type (might be due to invalid IL or missing references)
		//IL_4018: Unknown result type (might be due to invalid IL or missing references)
		//IL_4066: Unknown result type (might be due to invalid IL or missing references)
		//IL_40aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_40f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_4144: Unknown result type (might be due to invalid IL or missing references)
		//IL_41fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_4271: Unknown result type (might be due to invalid IL or missing references)
		//IL_42ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_42da: Unknown result type (might be due to invalid IL or missing references)
		//IL_42e4: Expected O, but got Unknown
		//IL_4304: Unknown result type (might be due to invalid IL or missing references)
		//IL_433d: Unknown result type (might be due to invalid IL or missing references)
		//IL_4376: Unknown result type (might be due to invalid IL or missing references)
		//IL_43a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_43de: Unknown result type (might be due to invalid IL or missing references)
		//IL_4417: Unknown result type (might be due to invalid IL or missing references)
		//IL_4446: Unknown result type (might be due to invalid IL or missing references)
		//IL_447f: Unknown result type (might be due to invalid IL or missing references)
		//IL_44b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_44f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_458b: Unknown result type (might be due to invalid IL or missing references)
		//IL_45ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_45e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_4618: Unknown result type (might be due to invalid IL or missing references)
		//IL_4647: Unknown result type (might be due to invalid IL or missing references)
		//IL_47d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_482b: Unknown result type (might be due to invalid IL or missing references)
		//IL_487e: Unknown result type (might be due to invalid IL or missing references)
		//IL_48ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_48f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_48fb: Expected O, but got Unknown
		//IL_492f: Unknown result type (might be due to invalid IL or missing references)
		//IL_4957: Unknown result type (might be due to invalid IL or missing references)
		//IL_499a: Unknown result type (might be due to invalid IL or missing references)
		//IL_49c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_4a05: Unknown result type (might be due to invalid IL or missing references)
		//IL_4a2d: Unknown result type (might be due to invalid IL or missing references)
		//IL_4a70: Unknown result type (might be due to invalid IL or missing references)
		//IL_4a98: Unknown result type (might be due to invalid IL or missing references)
		//IL_4adb: Unknown result type (might be due to invalid IL or missing references)
		//IL_4b1e: Unknown result type (might be due to invalid IL or missing references)
		//IL_4b61: Unknown result type (might be due to invalid IL or missing references)
		//IL_4baf: Unknown result type (might be due to invalid IL or missing references)
		//IL_4bfd: Unknown result type (might be due to invalid IL or missing references)
		//IL_4c36: Unknown result type (might be due to invalid IL or missing references)
		//IL_4c3b: Unknown result type (might be due to invalid IL or missing references)
		//IL_4c3c: Unknown result type (might be due to invalid IL or missing references)
		//IL_4c4b: Expected O, but got Unknown
		//IL_4c75: Unknown result type (might be due to invalid IL or missing references)
		//IL_4c7a: Unknown result type (might be due to invalid IL or missing references)
		//IL_4c7b: Unknown result type (might be due to invalid IL or missing references)
		//IL_4c8a: Expected O, but got Unknown
		//IL_4cb4: Unknown result type (might be due to invalid IL or missing references)
		//IL_4cb9: Unknown result type (might be due to invalid IL or missing references)
		//IL_4cba: Unknown result type (might be due to invalid IL or missing references)
		//IL_4cc9: Expected O, but got Unknown
		//IL_4cf3: Unknown result type (might be due to invalid IL or missing references)
		//IL_4cf8: Unknown result type (might be due to invalid IL or missing references)
		//IL_4cf9: Unknown result type (might be due to invalid IL or missing references)
		//IL_4d08: Expected O, but got Unknown
		//IL_4d28: Unknown result type (might be due to invalid IL or missing references)
		//IL_4d5d: Unknown result type (might be due to invalid IL or missing references)
		//IL_4d7a: Unknown result type (might be due to invalid IL or missing references)
		//IL_4d7f: Unknown result type (might be due to invalid IL or missing references)
		//IL_4d89: Expected O, but got Unknown
		//IL_4d84: Unknown result type (might be due to invalid IL or missing references)
		//IL_4d8a: Expected O, but got Unknown
		//IL_4daa: Unknown result type (might be due to invalid IL or missing references)
		//IL_4dc7: Unknown result type (might be due to invalid IL or missing references)
		//IL_4dcc: Unknown result type (might be due to invalid IL or missing references)
		//IL_4dd6: Expected O, but got Unknown
		//IL_4dd1: Unknown result type (might be due to invalid IL or missing references)
		//IL_4dd7: Expected O, but got Unknown
		//IL_4dee: Unknown result type (might be due to invalid IL or missing references)
		//IL_4e02: Unknown result type (might be due to invalid IL or missing references)
		//IL_4e07: Unknown result type (might be due to invalid IL or missing references)
		//IL_4e11: Expected O, but got Unknown
		//IL_4e0c: Unknown result type (might be due to invalid IL or missing references)
		//IL_4e12: Expected O, but got Unknown
		//IL_4e59: Unknown result type (might be due to invalid IL or missing references)
		//IL_4e71: Unknown result type (might be due to invalid IL or missing references)
		//IL_4e76: Unknown result type (might be due to invalid IL or missing references)
		//IL_4e80: Expected O, but got Unknown
		//IL_4e7b: Unknown result type (might be due to invalid IL or missing references)
		//IL_4e81: Expected O, but got Unknown
		//IL_4e8b: Unknown result type (might be due to invalid IL or missing references)
		//IL_4e95: Expected O, but got Unknown
		Font val = resCache.NotoStack("Regular", 8);
		Font val2 = resCache.NotoStack();
		Font val3 = resCache.NotoStack("Italic");
		Font val4 = resCache.NotoStack("Regular", 12);
		Font val5 = resCache.NotoStack("Italic", 12);
		Font val6 = resCache.NotoStack("Bold", 12);
		Font val7 = resCache.NotoStack("BoldItalic", 12);
		resCache.NotoStack("BoldItalic", 14);
		resCache.NotoStack("BoldItalic", 16);
		Font val8 = resCache.NotoStack("Bold", 14, display: true);
		Font val9 = resCache.NotoStack("Bold", 16, display: true);
		Font val10 = resCache.NotoStack("Regular", 15);
		Font val11 = resCache.NotoStack("Regular", 16);
		Font val12 = resCache.NotoStack("Bold", 16);
		Font val13 = resCache.NotoStack("Bold", 18);
		Font val14 = resCache.NotoStack("Bold", 20);
		Font font = resCache.GetFont("/EngineFonts/NotoSans/NotoSansMono-Regular.ttf", 12);
		Font font2 = resCache.GetFont("/Fonts/RobotoMono/RobotoMono-Bold.ttf", 11);
		Font font3 = resCache.GetFont("/Fonts/RobotoMono/RobotoMono-Bold.ttf", 12);
		Font font4 = resCache.GetFont("/Fonts/RobotoMono/RobotoMono-Bold.ttf", 14);
		Texture texture = resCache.GetTexture("/Textures/Interface/Nano/window_header.png");
		StyleBoxTexture val15 = new StyleBoxTexture
		{
			Texture = texture,
			PatchMarginBottom = 3f,
			ExpandMarginBottom = 3f,
			ContentMarginBottomOverride = 0f
		};
		Texture texture2 = resCache.GetTexture("/Textures/Interface/Nano/window_header_alert.png");
		StyleBoxTexture val16 = new StyleBoxTexture
		{
			Texture = texture2,
			PatchMarginBottom = 3f,
			ExpandMarginBottom = 3f,
			ContentMarginBottomOverride = 0f
		};
		Texture texture3 = resCache.GetTexture("/Textures/Interface/Nano/window_background.png");
		StyleBoxTexture val17 = new StyleBoxTexture
		{
			Texture = texture3
		};
		val17.SetPatchMargin((Margin)14, 2f);
		val17.SetExpandMargin((Margin)14, 2f);
		Texture texture4 = resCache.GetTexture("/Textures/Interface/Nano/window_background_bordered.png");
		StyleBoxTexture val18 = new StyleBoxTexture
		{
			Texture = texture4
		};
		val18.SetPatchMargin((Margin)15, 2f);
		StyleBoxTexture val19 = new StyleBoxTexture
		{
			Texture = texture4
		};
		val19.SetPatchMargin((Margin)15, 2f);
		Texture texture5 = resCache.GetTexture("/Textures/Interface/Inventory/inv_slot_background.png");
		StyleBoxTexture val20 = new StyleBoxTexture
		{
			Texture = texture5
		};
		val20.SetPatchMargin((Margin)15, 2f);
		((StyleBox)val20).SetContentMarginOverride((Margin)15, 0f);
		Texture texture6 = resCache.GetTexture("/Textures/Interface/Inventory/hand_slot_highlight.png");
		StyleBoxTexture val21 = new StyleBoxTexture
		{
			Texture = texture6
		};
		val21.SetPatchMargin((Margin)15, 2f);
		Texture texture7 = resCache.GetTexture("/Textures/Interface/Nano/transparent_window_background_bordered.png");
		StyleBoxTexture val22 = new StyleBoxTexture
		{
			Texture = texture7
		};
		val22.SetPatchMargin((Margin)15, 2f);
		Texture texture8 = resCache.GetTexture("/Textures/Interface/Nano/lobby_b.png");
		StyleBoxTexture val23 = new StyleBoxTexture
		{
			Texture = texture8,
			Mode = (StretchMode)1
		};
		val23.SetPatchMargin((Margin)15, 24f);
		val23.SetExpandMargin((Margin)15, -4f);
		((StyleBox)val23).SetContentMarginOverride((Margin)15, 8f);
		StyleBoxTexture val24 = new StyleBoxTexture
		{
			Texture = resCache.GetTexture("/Textures/_PUBG/Lobby/Frames/frametest.png")
		};
		val24.SetPatchMargin((Margin)15, 10f);
		StyleBoxTexture val25 = new StyleBoxTexture
		{
			Texture = texture4
		};
		val25.SetPatchMargin((Margin)15, 2f);
		val25.SetExpandMargin((Margin)15, 4f);
		StyleBoxTexture val26 = new StyleBoxTexture(base.BaseButton);
		val26.SetPatchMargin((Margin)15, 10f);
		((StyleBox)val26).SetPadding((Margin)15, 0f);
		((StyleBox)val26).SetContentMarginOverride((Margin)3, 0f);
		((StyleBox)val26).SetContentMarginOverride((Margin)12, 4f);
		StyleBoxTexture val27 = new StyleBoxTexture
		{
			Texture = Texture.White
		};
		Texture texture9 = resCache.GetTexture("/Textures/Interface/Nano/light_panel_background_bordered.png");
		StyleBoxTexture val28 = new StyleBoxTexture(base.BaseButton)
		{
			Texture = texture9
		};
		val28.SetPatchMargin((Margin)15, 2f);
		((StyleBox)val28).SetPadding((Margin)15, 2f);
		((StyleBox)val28).SetContentMarginOverride((Margin)3, 2f);
		((StyleBox)val28).SetContentMarginOverride((Margin)12, 2f);
		new StyleBoxTexture(val28).Modulate = ButtonColorHovered;
		new StyleBoxTexture(val28).Modulate = ButtonColorPressed;
		new StyleBoxTexture(val28).Modulate = ButtonColorDisabled;
		Texture texture10 = resCache.GetTexture("/Textures/Interface/Nano/black_panel_light_thin_border.png");
		Texture texture11 = resCache.GetTexture("/Textures/Interface/Nano/black_panel_red_thin_border.png");
		StyleBoxTexture val29 = new StyleBoxTexture(base.BaseButton)
		{
			Texture = texture10
		};
		val29.SetPatchMargin((Margin)15, 2f);
		((StyleBox)val29).SetPadding((Margin)15, 2f);
		((StyleBox)val29).SetContentMarginOverride((Margin)3, 2f);
		((StyleBox)val29).SetContentMarginOverride((Margin)12, 2f);
		new StyleBoxTexture(val29).Texture = texture11;
		new StyleBoxTexture(val29).Modulate = ButtonColorHovered;
		new StyleBoxTexture(val29).Modulate = ButtonColorPressed;
		Texture texture12 = resCache.GetTexture("/Textures/Interface/Nano/button.svg.96dpi.png");
		StyleBoxTexture val30 = new StyleBoxTexture
		{
			Texture = texture12
		};
		val30.SetPatchMargin((Margin)15, 10f);
		((StyleBox)val30).SetPadding((Margin)15, 0f);
		((StyleBox)val30).SetContentMarginOverride((Margin)15, 0f);
		StyleBoxTexture val31 = new StyleBoxTexture(val30)
		{
			Texture = (Texture)new AtlasTexture(texture12, UIBox2.FromDimensions(new Vector2(0f, 0f), new Vector2(14f, 24f)))
		};
		val31.SetPatchMargin((Margin)4, 0f);
		StyleBoxTexture val32 = new StyleBoxTexture(val30)
		{
			Texture = (Texture)new AtlasTexture(texture12, UIBox2.FromDimensions(new Vector2(10f, 0f), new Vector2(14f, 24f)))
		};
		val32.SetPatchMargin((Margin)8, 0f);
		StyleBoxTexture val33 = new StyleBoxTexture(val30)
		{
			Texture = (Texture)new AtlasTexture(texture12, UIBox2.FromDimensions(new Vector2(10f, 0f), new Vector2(3f, 24f)))
		};
		val33.SetPatchMargin((Margin)12, 0f);
		Texture texture13 = resCache.GetTexture("/Textures/Interface/Nano/rounded_button.svg.96dpi.png");
		StyleBoxTexture val34 = new StyleBoxTexture
		{
			Texture = texture13
		};
		val34.SetPatchMargin((Margin)15, 5f);
		((StyleBox)val34).SetPadding((Margin)15, 2f);
		Texture texture14 = resCache.GetTexture("/Textures/Interface/Nano/rounded_button_bordered.svg.96dpi.png");
		StyleBoxTexture val35 = new StyleBoxTexture
		{
			Texture = texture14
		};
		val35.SetPatchMargin((Margin)15, 5f);
		((StyleBox)val35).SetPadding((Margin)15, 2f);
		Texture texture15 = resCache.GetTexture("/Textures/Interface/Nano/rounded_button_half_bordered.svg.96dpi.png");
		StyleBoxTexture val36 = new StyleBoxTexture
		{
			Texture = texture15
		};
		val36.SetPatchMargin((Margin)15, 5f);
		((StyleBox)val36).SetPadding((Margin)15, 2f);
		((StyleBox)val36).SetPadding((Margin)1, 0f);
		((StyleBox)val36).SetPadding((Margin)2, 0f);
		Texture texture16 = resCache.GetTexture("/Textures/Interface/Nano/button_small.svg.96dpi.png");
		StyleBoxTexture val37 = new StyleBoxTexture
		{
			Texture = texture16
		};
		Texture texture17 = resCache.GetTexture("/Textures/Interface/Nano/inverted_triangle.svg.png");
		Texture texture18 = resCache.GetTexture("/Textures/Interface/Nano/lineedit.png");
		StyleBoxTexture val38 = new StyleBoxTexture
		{
			Texture = texture18
		};
		val38.SetPatchMargin((Margin)15, 3f);
		((StyleBox)val38).SetContentMarginOverride((Margin)12, 5f);
		StyleBoxFlat val39 = new StyleBoxFlat
		{
			BackgroundColor = ChatBackgroundColor
		};
		((StyleBox)new StyleBoxFlat
		{
			BackgroundColor = ChatBackgroundColor
		}).SetContentMarginOverride((Margin)15, 2f);
		Texture texture19 = resCache.GetTexture("/Textures/Interface/Nano/black_panel_dark_thin_border.png");
		StyleBoxTexture val40 = new StyleBoxTexture
		{
			Texture = texture19
		};
		val40.SetPatchMargin((Margin)15, 3f);
		((StyleBox)val40).SetContentMarginOverride((Margin)12, 5f);
		Texture texture20 = resCache.GetTexture("/Textures/Interface/Nano/tabcontainer_panel.png");
		StyleBoxTexture val41 = new StyleBoxTexture
		{
			Texture = texture20
		};
		val41.SetPatchMargin((Margin)15, 2f);
		StyleBoxFlat val42 = new StyleBoxFlat
		{
			BackgroundColor = new Color((byte)64, (byte)64, (byte)64, byte.MaxValue)
		};
		((StyleBox)val42).SetContentMarginOverride((Margin)12, 5f);
		StyleBoxFlat val43 = new StyleBoxFlat
		{
			BackgroundColor = new Color((byte)32, (byte)32, (byte)32, byte.MaxValue)
		};
		((StyleBox)val43).SetContentMarginOverride((Margin)12, 5f);
		StyleBoxFlat val44 = new StyleBoxFlat
		{
			BackgroundColor = new Color(0.25f, 0.25f, 0.25f, 1f)
		};
		((StyleBox)val44).SetContentMarginOverride((Margin)3, 14.5f);
		StyleBoxFlat val45 = new StyleBoxFlat
		{
			BackgroundColor = new Color(0.25f, 0.5f, 0.25f, 1f)
		};
		((StyleBox)val45).SetContentMarginOverride((Margin)3, 14.5f);
		StyleBoxTexture val46 = new StyleBoxTexture
		{
			Texture = resCache.GetTexture("/Textures/Interface/Nano/Monotone/monotone_button.svg.96dpi.png")
		};
		val46.SetPatchMargin((Margin)15, 11f);
		((StyleBox)val46).SetPadding((Margin)15, 1f);
		((StyleBox)val46).SetContentMarginOverride((Margin)3, 2f);
		((StyleBox)val46).SetContentMarginOverride((Margin)12, 14f);
		StyleBoxTexture val47 = new StyleBoxTexture(val46)
		{
			Texture = resCache.GetTexture("/Textures/Interface/Nano/Monotone/monotone_button_open_left.svg.96dpi.png")
		};
		StyleBoxTexture val48 = new StyleBoxTexture(val46)
		{
			Texture = resCache.GetTexture("/Textures/Interface/Nano/Monotone/monotone_button_open_right.svg.96dpi.png")
		};
		StyleBoxTexture val49 = new StyleBoxTexture(val46)
		{
			Texture = resCache.GetTexture("/Textures/Interface/Nano/Monotone/monotone_button_open_both.svg.96dpi.png")
		};
		StyleBoxTexture val50 = new StyleBoxTexture(val46)
		{
			Texture = texture12
		};
		StyleBoxTexture val51 = new StyleBoxTexture(val46)
		{
			Texture = (Texture)new AtlasTexture(texture12, UIBox2.FromDimensions(new Vector2(10f, 0f), new Vector2(14f, 24f)))
		};
		val51.SetPatchMargin((Margin)8, 0f);
		StyleBoxTexture val52 = new StyleBoxTexture(val46)
		{
			Texture = (Texture)new AtlasTexture(texture12, UIBox2.FromDimensions(new Vector2(0f, 0f), new Vector2(14f, 24f)))
		};
		val52.SetPatchMargin((Margin)4, 0f);
		StyleBoxTexture val53 = new StyleBoxTexture(val46)
		{
			Texture = (Texture)new AtlasTexture(texture12, UIBox2.FromDimensions(new Vector2(10f, 0f), new Vector2(3f, 24f)))
		};
		val53.SetPatchMargin((Margin)12, 0f);
		Texture texture21 = resCache.GetTexture("/Textures/Interface/Nano/checkbox_checked.svg.96dpi.png");
		Texture texture22 = resCache.GetTexture("/Textures/Interface/Nano/checkbox_unchecked.svg.96dpi.png");
		Texture texture23 = resCache.GetTexture("/Textures/Interface/Nano/Monotone/monotone_checkbox_checked.svg.96dpi.png");
		Texture texture24 = resCache.GetTexture("/Textures/Interface/Nano/Monotone/monotone_checkbox_unchecked.svg.96dpi.png");
		Texture texture25 = resCache.GetTexture("/Textures/Interface/Nano/tooltip.png");
		StyleBoxTexture val54 = new StyleBoxTexture
		{
			Texture = texture25
		};
		val54.SetPatchMargin((Margin)15, 2f);
		((StyleBox)val54).SetContentMarginOverride((Margin)12, 7f);
		Texture texture26 = resCache.GetTexture("/Textures/Interface/Nano/whisper.png");
		StyleBoxTexture val55 = new StyleBoxTexture
		{
			Texture = texture26
		};
		val55.SetPatchMargin((Margin)15, 2f);
		((StyleBox)val55).SetContentMarginOverride((Margin)12, 7f);
		Texture texture27 = resCache.GetTexture("/Textures/Interface/Nano/placeholder.png");
		StyleBoxTexture val56 = new StyleBoxTexture
		{
			Texture = texture27
		};
		val56.SetPatchMargin((Margin)15, 19f);
		val56.SetExpandMargin((Margin)15, -5f);
		val56.Mode = (StretchMode)1;
		StyleBoxFlat val57 = new StyleBoxFlat
		{
			BackgroundColor = new Color((byte)75, (byte)75, (byte)86, byte.MaxValue)
		};
		((StyleBox)val57).SetContentMarginOverride((Margin)3, 2f);
		((StyleBox)val57).SetContentMarginOverride((Margin)12, 4f);
		StyleBoxFlat val58 = new StyleBoxFlat
		{
			BackgroundColor = new Color((byte)10, (byte)10, (byte)12, byte.MaxValue)
		};
		((StyleBox)val58).SetContentMarginOverride((Margin)3, 2f);
		((StyleBox)val58).SetContentMarginOverride((Margin)12, 4f);
		StyleBoxFlat val59 = new StyleBoxFlat
		{
			BackgroundColor = new Color((byte)55, (byte)55, (byte)68, byte.MaxValue)
		};
		((StyleBox)val59).SetContentMarginOverride((Margin)3, 2f);
		((StyleBox)val59).SetContentMarginOverride((Margin)12, 4f);
		StyleBoxFlat val60 = new StyleBoxFlat
		{
			BackgroundColor = Color.Transparent
		};
		((StyleBox)val60).SetContentMarginOverride((Margin)3, 2f);
		((StyleBox)val60).SetContentMarginOverride((Margin)12, 4f);
		Texture texture28 = resCache.GetTexture("/Textures/Interface/Nano/square.png");
		StyleBoxTexture val61 = new StyleBoxTexture
		{
			Texture = texture28,
			ContentMarginLeftOverride = 10f
		};
		Texture texture29 = resCache.GetTexture("/Textures/Interface/Nano/nanoheading.svg.96dpi.png");
		StyleBoxTexture val62 = new StyleBoxTexture
		{
			Texture = texture29,
			PatchMarginRight = 10f,
			PatchMarginTop = 10f,
			ContentMarginTopOverride = 2f,
			ContentMarginLeftOverride = 10f,
			PaddingTop = 4f
		};
		val62.SetPatchMargin((Margin)10, 2f);
		Texture texture30 = resCache.GetTexture("/Textures/Interface/Nano/stripeback.svg.96dpi.png");
		StyleBoxTexture val63 = new StyleBoxTexture
		{
			Texture = texture30,
			Mode = (StretchMode)1
		};
		Texture texture31 = resCache.GetTexture("/Textures/Interface/Nano/slider_outline.svg.96dpi.png");
		Texture texture32 = resCache.GetTexture("/Textures/Interface/Nano/slider_fill.svg.96dpi.png");
		Texture texture33 = resCache.GetTexture("/Textures/Interface/Nano/slider_grabber.svg.96dpi.png");
		StyleBoxTexture val64 = new StyleBoxTexture
		{
			Texture = texture32,
			Modulate = Color.FromHex((ReadOnlySpan<char>)"#3E6C45", (Color?)null)
		};
		StyleBoxTexture val65 = new StyleBoxTexture
		{
			Texture = texture32,
			Modulate = PanelDark
		};
		StyleBoxTexture val66 = new StyleBoxTexture
		{
			Texture = texture31,
			Modulate = Color.FromHex((ReadOnlySpan<char>)"#494949", (Color?)null)
		};
		StyleBoxTexture val67 = new StyleBoxTexture
		{
			Texture = texture33
		};
		val64.SetPatchMargin((Margin)15, 12f);
		val65.SetPatchMargin((Margin)15, 12f);
		val66.SetPatchMargin((Margin)15, 12f);
		val67.SetPatchMargin((Margin)15, 12f);
		StyleBoxTexture val68 = new StyleBoxTexture(val64)
		{
			Modulate = Color.LimeGreen
		};
		StyleBoxTexture val69 = new StyleBoxTexture(val64)
		{
			Modulate = Color.Red
		};
		StyleBoxTexture val70 = new StyleBoxTexture(val64)
		{
			Modulate = Color.Blue
		};
		StyleBoxTexture val71 = new StyleBoxTexture(val64)
		{
			Modulate = Color.White
		};
		Font font5 = resCache.GetFont("/Fonts/Boxfont-round/Boxfont Round.ttf", 13);
		StyleBoxTexture val72 = new StyleBoxTexture
		{
			Texture = texture12,
			Modulate = Color.FromHex((ReadOnlySpan<char>)"#202023", (Color?)null)
		};
		val72.SetPatchMargin((Margin)15, 10f);
		StyleBoxTexture val73 = new StyleBoxTexture
		{
			Texture = resCache.GetTexture("/Textures/Interface/Paper/paper_background_default.svg.96dpi.png"),
			Modulate = Color.FromHex((ReadOnlySpan<char>)"#eaedde", (Color?)null)
		};
		val73.SetPatchMargin((Margin)15, 16f);
		Texture texture34 = resCache.GetTexture("/Textures/Interface/VerbIcons/group.svg.192dpi.png");
		Texture texture35 = resCache.GetTexture("/Textures/Interface/VerbIcons/group.svg.192dpi.png");
		Texture texture36 = resCache.GetTexture("/Textures/Interface/VerbIcons/drop.svg.192dpi.png");
		Texture texture37 = resCache.GetTexture("/Textures/Interface/VerbIcons/information.svg.192dpi.png");
		Texture texture38 = resCache.GetTexture("/Textures/Interface/VerbIcons/dot.svg.192dpi.png");
		Stylesheet = new Stylesheet((IReadOnlyList<StyleRule>)base.BaseRules.Concat((IEnumerable<StyleRule>)(object)new StyleRule[244]
		{
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element().Class("monospace")).Prop("font", (object)font)),
			new StyleRule((Selector)new SelectorElement(typeof(Label), (IEnumerable<string>)new string[1] { "windowTitle" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[2]
			{
				new StyleProperty("font-color", (object)NanoGold),
				new StyleProperty("font", (object)val8)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(Label), (IEnumerable<string>)new string[1] { "windowTitleAlert" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[2]
			{
				new StyleProperty("font-color", (object)Color.White),
				new StyleProperty("font", (object)val8)
			}),
			new StyleRule((Selector)new SelectorElement((Type)null, (IEnumerable<string>)new string[1] { "windowPanel" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("panel", (object)val17)
			}),
			new StyleRule((Selector)new SelectorElement((Type)null, (IEnumerable<string>)new string[1] { "BorderedWindowPanel" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("panel", (object)val18)
			}),
			new StyleRule((Selector)new SelectorElement((Type)null, (IEnumerable<string>)new string[1] { "TransparentBorderedWindowPanel" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("panel", (object)val22)
			}),
			new StyleRule((Selector)new SelectorElement((Type)null, (IEnumerable<string>)new string[1] { "InventorySlotBackground" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("panel", (object)val20)
			}),
			new StyleRule((Selector)new SelectorElement((Type)null, (IEnumerable<string>)new string[1] { "HandSlotHighlight" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("panel", (object)val21)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(PanelContainer), (IEnumerable<string>)new string[1] { "HotbarPanel" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("panel", (object)val25)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(PanelContainer), (IEnumerable<string>)new string[1] { "windowHeader" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("panel", (object)val15)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(PanelContainer), (IEnumerable<string>)new string[1] { "windowHeaderAlert" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("panel", (object)val16)
			}),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<ContainerButton>().Class("button")).Prop("stylebox", (object)base.BaseButton)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<ContainerButton>().Class("button").Class("OpenRight")).Prop("stylebox", (object)base.BaseButtonOpenRight)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<ContainerButton>().Class("button").Class("OpenLeft")).Prop("stylebox", (object)base.BaseButtonOpenLeft)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<ContainerButton>().Class("button").Class("OpenBoth")).Prop("stylebox", (object)base.BaseButtonOpenBoth)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<ContainerButton>().Class("button").Class("ButtonSquare")).Prop("stylebox", (object)base.BaseButtonSquare)),
			new StyleRule((Selector)new SelectorElement(typeof(Label), (IEnumerable<string>)new string[1] { "button" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("alignMode", (object)(AlignMode)1)
			}),
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
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<ConfirmButton>().Pseudo("confirm-normal")).Prop("modulate-self", (object)ButtonColorCautionDefault)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<ConfirmButton>().Pseudo("confirm-hover")).Prop("modulate-self", (object)ButtonColorCautionHovered)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<ConfirmButton>().Pseudo("confirm-pressed")).Prop("modulate-self", (object)ButtonColorCautionPressed)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<ConfirmButton>().Pseudo("confirm-disabled")).Prop("modulate-self", (object)ButtonColorCautionDisabled)),
			new StyleRule((Selector)new SelectorChild((Selector)new SelectorElement(typeof(Button), (IEnumerable<string>)null, (string)null, (IEnumerable<string>)new string[1] { "disabled" }), (Selector)new SelectorElement(typeof(Label), (IEnumerable<string>)null, (string)null, (IEnumerable<string>)null)), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("font-color", (object)Color.FromHex((ReadOnlySpan<char>)"#E5E5E581", (Color?)null))
			}),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element().Class("ItemStatusNotHeld")).Prop("font", (object)val3).Prop("font-color", (object)ItemStatusNotHeldColor).Prop("Margin", (object)new Thickness(4f, 0f, 0f, 2f))),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element().Class("ItemStatus")).Prop("LineHeightScale", (object)0.7f).Prop("Margin", (object)new Thickness(4f, 0f, 0f, 2f))),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<PanelContainer>().Class("contextMenuPopup")).Prop("panel", (object)val19)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<ContextMenuElement>().Class("contextMenuButton")).Prop("stylebox", (object)val27)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<ContextMenuElement>().Class("contextMenuButton").Pseudo("normal")).Prop("modulate-self", (object)ButtonColorContext)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<ContextMenuElement>().Class("contextMenuButton").Pseudo("hover")).Prop("modulate-self", (object)ButtonColorContextHover)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<ContextMenuElement>().Class("contextMenuButton").Pseudo("pressed")).Prop("modulate-self", (object)ButtonColorContextPressed)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<ContextMenuElement>().Class("contextMenuButton").Pseudo("disabled")).Prop("modulate-self", (object)ButtonColorContextDisabled)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<RichTextLabel>().Class(InteractionVerb.DefaultTextStyleClass)).Prop("font", (object)val7)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<RichTextLabel>().Class(ActivationVerb.DefaultTextStyleClass)).Prop("font", (object)val6)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<RichTextLabel>().Class(AlternativeVerb.DefaultTextStyleClass)).Prop("font", (object)val5)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<RichTextLabel>().Class(Verb.DefaultTextStyleClass)).Prop("font", (object)val4)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<TextureRect>().Class("contextMenuExpansionTexture")).Prop("texture", (object)texture34)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<TextureRect>().Class("verbMenuConfirmationTexture")).Prop("texture", (object)texture35)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<ContextMenuElement>().Class("confirmationContextMenuButton")).Prop("stylebox", (object)val27)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<ContextMenuElement>().Class("confirmationContextMenuButton").Pseudo("normal")).Prop("modulate-self", (object)ButtonColorCautionDefault)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<ContextMenuElement>().Class("confirmationContextMenuButton").Pseudo("hover")).Prop("modulate-self", (object)ButtonColorCautionHovered)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<ContextMenuElement>().Class("confirmationContextMenuButton").Pseudo("pressed")).Prop("modulate-self", (object)ButtonColorCautionPressed)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<ContextMenuElement>().Class("confirmationContextMenuButton").Pseudo("disabled")).Prop("modulate-self", (object)ButtonColorCautionDisabled)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<ExamineButton>().Class("examine-button")).Prop("stylebox", (object)val27)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<ExamineButton>().Class("examine-button").Pseudo("normal")).Prop("modulate-self", (object)ExamineButtonColorContext)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<ExamineButton>().Class("examine-button").Pseudo("hover")).Prop("modulate-self", (object)ExamineButtonColorContextHover)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<ExamineButton>().Class("examine-button").Pseudo("pressed")).Prop("modulate-self", (object)ExamineButtonColorContextPressed)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<ExamineButton>().Class("examine-button").Pseudo("disabled")).Prop("modulate-self", (object)ExamineButtonColorContextDisabled)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<DirectionIcon>().Class(DirectionIcon.StyleClassDirectionIconArrow)).Prop("texture", (object)texture36)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<DirectionIcon>().Class(DirectionIcon.StyleClassDirectionIconUnknown)).Prop("texture", (object)texture37)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<DirectionIcon>().Class(DirectionIcon.StyleClassDirectionIconHere)).Prop("texture", (object)texture38)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<ContainerButton>().Class("storageButton")).Prop("stylebox", (object)val26)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<ContainerButton>().Class("storageButton").Pseudo("normal")).Prop("modulate-self", (object)ButtonColorDefault)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<ContainerButton>().Class("storageButton").Pseudo("hover")).Prop("modulate-self", (object)ButtonColorHovered)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<ContainerButton>().Class("storageButton").Pseudo("pressed")).Prop("modulate-self", (object)ButtonColorPressed)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<ContainerButton>().Class("storageButton").Pseudo("disabled")).Prop("modulate-self", (object)ButtonColorDisabled)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<ContainerButton>().Class("list-container-button")).Prop("stylebox", (object)val61)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<ContainerButton>().Class("list-container-button").Pseudo("normal")).Prop("modulate-self", (object)new Color((byte)55, (byte)55, (byte)68, byte.MaxValue))),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<ContainerButton>().Class("list-container-button").Pseudo("hover")).Prop("modulate-self", (object)new Color((byte)75, (byte)75, (byte)86, byte.MaxValue))),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<ContainerButton>().Class("list-container-button").Pseudo("pressed")).Prop("modulate-self", (object)new Color((byte)75, (byte)75, (byte)86, byte.MaxValue))),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<ContainerButton>().Class("list-container-button").Pseudo("disabled")).Prop("modulate-self", (object)new Color((byte)10, (byte)10, (byte)12, byte.MaxValue))),
			new StyleRule((Selector)new SelectorChild((Selector)new SelectorElement(typeof(Button), (IEnumerable<string>)null, "mainMenu", (IEnumerable<string>)null), (Selector)new SelectorElement(typeof(Label), (IEnumerable<string>)null, (string)null, (IEnumerable<string>)null)), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("font", (object)val12)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(BoxContainer), (IEnumerable<string>)null, "mainMenuVBox", (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("separation", (object)2)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(LineEdit), (IEnumerable<string>)null, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("stylebox", (object)val38)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(LineEdit), (IEnumerable<string>)new string[1] { "notEditable" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("font-color", (object)new Color((byte)192, (byte)192, (byte)192, byte.MaxValue))
			}),
			new StyleRule((Selector)new SelectorElement(typeof(LineEdit), (IEnumerable<string>)null, (string)null, (IEnumerable<string>)new string[1] { "placeholder" }), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("font-color", (object)Color.Gray)
			}),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<TextEdit>().Pseudo("placeholder")).Prop("font-color", (object)Color.Gray)),
			new StyleRule((Selector)new SelectorElement(typeof(PanelContainer), (IEnumerable<string>)new string[1] { "ChatPanel" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("panel", (object)val39)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(LineEdit), (IEnumerable<string>)new string[1] { "chatLineEdit" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("stylebox", (object)new StyleBoxEmpty())
			}),
			new StyleRule((Selector)new SelectorElement(typeof(LineEdit), (IEnumerable<string>)new string[1] { "actionSearchBox" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("stylebox", (object)val40)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(TabContainer), (IEnumerable<string>)null, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[3]
			{
				new StyleProperty("panel-stylebox", (object)val41),
				new StyleProperty("tab-stylebox", (object)val42),
				new StyleProperty("tab-stylebox-inactive", (object)val43)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(ProgressBar), (IEnumerable<string>)null, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[2]
			{
				new StyleProperty("background", (object)val44),
				new StyleProperty("foreground", (object)val45)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(TextureRect), (IEnumerable<string>)new string[1] { "checkBox" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("texture", (object)texture22)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(TextureRect), (IEnumerable<string>)new string[2] { "checkBox", "checkBoxChecked" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("texture", (object)texture21)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(BoxContainer), (IEnumerable<string>)new string[1] { "checkBox" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("separation", (object)10)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(TextureRect), (IEnumerable<string>)new string[1] { "monotoneCheckBox" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("texture", (object)texture24)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(TextureRect), (IEnumerable<string>)new string[2] { "monotoneCheckBox", "checkBoxChecked" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("texture", (object)texture23)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(Tooltip), (IEnumerable<string>)null, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("panel", (object)val54)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(PanelContainer), (IEnumerable<string>)new string[1] { "tooltipBox" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("panel", (object)val54)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(PanelContainer), (IEnumerable<string>)new string[2] { "speechBox", "sayBox" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("panel", (object)val54)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(PanelContainer), (IEnumerable<string>)new string[2] { "speechBox", "whisperBox" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("panel", (object)val55)
			}),
			new StyleRule((Selector)new SelectorChild((Selector)new SelectorElement(typeof(PanelContainer), (IEnumerable<string>)new string[2] { "speechBox", "whisperBox" }, (string)null, (IEnumerable<string>)null), (Selector)new SelectorElement(typeof(RichTextLabel), (IEnumerable<string>)new string[1] { "bubbleContent" }, (string)null, (IEnumerable<string>)null)), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("font", (object)val5)
			}),
			new StyleRule((Selector)new SelectorChild((Selector)new SelectorElement(typeof(PanelContainer), (IEnumerable<string>)new string[2] { "speechBox", "emoteBox" }, (string)null, (IEnumerable<string>)null), (Selector)new SelectorElement(typeof(RichTextLabel), (IEnumerable<string>)null, (string)null, (IEnumerable<string>)null)), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("font", (object)val5)
			}),
			new StyleRule((Selector)new SelectorChild((Selector)new SelectorElement(typeof(PanelContainer), (IEnumerable<string>)new string[2] { "speechBox", "commanderSpeech" }, (string)null, (IEnumerable<string>)null), (Selector)new SelectorElement(typeof(RichTextLabel), (IEnumerable<string>)new string[1] { "bubbleContent" }, (string)null, (IEnumerable<string>)null)), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("font", (object)val12)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(PanelContainer), (IEnumerable<string>)new string[2] { "speechBox", "commanderSpeech" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("panel", (object)val54)
			}),
			new StyleRule((Selector)new SelectorChild((Selector)new SelectorElement(typeof(PanelContainer), (IEnumerable<string>)new string[2] { "speechBox", "megaphoneSpeech" }, (string)null, (IEnumerable<string>)null), (Selector)new SelectorElement(typeof(RichTextLabel), (IEnumerable<string>)new string[1] { "bubbleContent" }, (string)null, (IEnumerable<string>)null)), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("font", (object)resCache.NotoStack("Bold", 20))
			}),
			new StyleRule((Selector)new SelectorElement(typeof(PanelContainer), (IEnumerable<string>)new string[2] { "speechBox", "megaphoneSpeech" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("panel", (object)val54)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(RichTextLabel), (IEnumerable<string>)new string[1] { "LabelKeyText" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[2]
			{
				new StyleProperty("font", (object)val6),
				new StyleProperty("modulate-self", (object)NanoGold)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(RichTextLabel), (IEnumerable<string>)new string[1] { "tooltipAlertTitle" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("font", (object)val13)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(RichTextLabel), (IEnumerable<string>)new string[1] { "tooltipAlertDesc" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("font", (object)val11)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(RichTextLabel), (IEnumerable<string>)new string[1] { "tooltipAlertCooldown" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("font", (object)val11)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(RichTextLabel), (IEnumerable<string>)new string[1] { "tooltipActionTitle" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("font", (object)val12)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(RichTextLabel), (IEnumerable<string>)new string[1] { "tooltipActionDesc" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("font", (object)val10)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(RichTextLabel), (IEnumerable<string>)new string[1] { "tooltipActionCooldown" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("font", (object)val10)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(RichTextLabel), (IEnumerable<string>)new string[1] { "tooltipActionDynamicMessage" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("font", (object)val10)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(RichTextLabel), (IEnumerable<string>)new string[1] { "tooltipActionCooldown" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("font", (object)val10)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(RichTextLabel), (IEnumerable<string>)new string[1] { "tooltipActionCharges" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("font", (object)val10)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(Label), (IEnumerable<string>)new string[1] { "contextMenuIconLabel" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[2]
			{
				new StyleProperty("font", (object)val2),
				new StyleProperty("alignMode", (object)(AlignMode)2)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(RichTextLabel), (IEnumerable<string>)new string[1] { "hotbarSlotNumber" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("font", (object)val9)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(PanelContainer), (IEnumerable<string>)new string[1] { "entity-tooltip" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("panel", (object)val54)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(ItemList), (IEnumerable<string>)null, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[4]
			{
				new StyleProperty("itemlist-background", (object)new StyleBoxFlat
				{
					BackgroundColor = new Color((byte)32, (byte)32, (byte)40, byte.MaxValue)
				}),
				new StyleProperty("item-background", (object)val59),
				new StyleProperty("disabled-item-background", (object)val58),
				new StyleProperty("selected-item-background", (object)val57)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(ItemList), (IEnumerable<string>)new string[1] { "transparentItemList" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[4]
			{
				new StyleProperty("itemlist-background", (object)new StyleBoxFlat
				{
					BackgroundColor = Color.Transparent
				}),
				new StyleProperty("item-background", (object)val60),
				new StyleProperty("disabled-item-background", (object)val58),
				new StyleProperty("selected-item-background", (object)val57)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(ItemList), (IEnumerable<string>)new string[1] { "transparentBackgroundItemList" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[4]
			{
				new StyleProperty("itemlist-background", (object)new StyleBoxFlat
				{
					BackgroundColor = Color.Transparent
				}),
				new StyleProperty("item-background", (object)val59),
				new StyleProperty("disabled-item-background", (object)val58),
				new StyleProperty("selected-item-background", (object)val57)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(Tree), (IEnumerable<string>)null, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[2]
			{
				new StyleProperty("background", (object)new StyleBoxFlat
				{
					BackgroundColor = new Color((byte)32, (byte)32, (byte)40, byte.MaxValue)
				}),
				new StyleProperty("item-selected", (object)new StyleBoxFlat
				{
					BackgroundColor = new Color((byte)55, (byte)55, (byte)68, byte.MaxValue),
					ContentMarginLeftOverride = 4f
				})
			}),
			new StyleRule((Selector)new SelectorElement(typeof(Placeholder), (IEnumerable<string>)null, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("panel", (object)val56)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(Label), (IEnumerable<string>)new string[1] { "PlaceholderText" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[2]
			{
				new StyleProperty("font", (object)val11),
				new StyleProperty("font-color", (object)new Color((byte)103, (byte)103, (byte)103, (byte)128))
			}),
			new StyleRule((Selector)new SelectorElement(typeof(Label), (IEnumerable<string>)new string[1] { "LabelHeading" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[2]
			{
				new StyleProperty("font", (object)val12),
				new StyleProperty("font-color", (object)NanoGold)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(Label), (IEnumerable<string>)new string[1] { "LabelHeadingBigger" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[2]
			{
				new StyleProperty("font", (object)val14),
				new StyleProperty("font-color", (object)NanoGold)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(Label), (IEnumerable<string>)new string[1] { "LabelSubText" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[2]
			{
				new StyleProperty("font", (object)val2),
				new StyleProperty("font-color", (object)Color.DarkGray)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(Label), (IEnumerable<string>)new string[1] { "LabelKeyText" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[2]
			{
				new StyleProperty("font", (object)val6),
				new StyleProperty("font-color", (object)NanoGold)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(Label), (IEnumerable<string>)new string[1] { "LabelSecondaryColor" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[2]
			{
				new StyleProperty("font", (object)val4),
				new StyleProperty("font-color", (object)Color.DarkGray)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(Label), (IEnumerable<string>)new string[1] { "ConsoleText" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("font", (object)font2)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(Label), (IEnumerable<string>)new string[1] { "ConsoleSubHeading" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("font", (object)font3)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(Label), (IEnumerable<string>)new string[1] { "ConsoleHeading" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("font", (object)font4)
			}),
			new StyleRule((Selector)new SelectorChild((Selector)new SelectorElement(typeof(Button), (IEnumerable<string>)new string[1] { "ButtonBig" }, (string)null, (IEnumerable<string>)null), (Selector)new SelectorElement(typeof(Label), (IEnumerable<string>)null, (string)null, (IEnumerable<string>)null)), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("font", (object)val11)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(Label), (IEnumerable<string>)new string[1] { "PowerStateNone" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("font-color", (object)new Color(0.8f, 0f, 0f, 1f))
			}),
			new StyleRule((Selector)new SelectorElement(typeof(Label), (IEnumerable<string>)new string[1] { "PowerStateLow" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("font-color", (object)new Color(0.9f, 0.36f, 0f, 1f))
			}),
			new StyleRule((Selector)new SelectorElement(typeof(Label), (IEnumerable<string>)new string[1] { "PowerStateGood" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("font-color", (object)new Color(0.024f, 0.8f, 0f, 1f))
			}),
			new StyleRule((Selector)new SelectorElement(typeof(MenuButton), (IEnumerable<string>)new string[1] { "ButtonSquare" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("stylebox", (object)val33)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(MenuButton), (IEnumerable<string>)new string[1] { "OpenLeft" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("stylebox", (object)val32)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(MenuButton), (IEnumerable<string>)new string[1] { "OpenRight" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("stylebox", (object)val31)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(MenuButton), (IEnumerable<string>)null, (string)null, (IEnumerable<string>)new string[1] { "normal" }), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("modulate-self", (object)ButtonColorDefault)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(MenuButton), (IEnumerable<string>)new string[1] { "topButtonLabel" }, (string)null, (IEnumerable<string>)new string[1] { "normal" }), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("modulate-self", (object)ButtonColorDefaultRed)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(MenuButton), (IEnumerable<string>)null, (string)null, (IEnumerable<string>)new string[1] { "normal" }), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("modulate-self", (object)ButtonColorDefault)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(MenuButton), (IEnumerable<string>)null, (string)null, (IEnumerable<string>)new string[1] { "pressed" }), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("modulate-self", (object)ButtonColorPressed)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(MenuButton), (IEnumerable<string>)null, (string)null, (IEnumerable<string>)new string[1] { "hover" }), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("modulate-self", (object)ButtonColorHovered)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(MenuButton), (IEnumerable<string>)new string[1] { "topButtonLabel" }, (string)null, (IEnumerable<string>)new string[1] { "hover" }), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("modulate-self", (object)ButtonColorHoveredRed)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(Label), (IEnumerable<string>)new string[1] { "topButtonLabel" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("font", (object)val8)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(MonotoneButton), (IEnumerable<string>)null, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("stylebox", (object)val46)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(MonotoneButton), (IEnumerable<string>)new string[1] { "OpenLeft" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("stylebox", (object)val47)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(MonotoneButton), (IEnumerable<string>)new string[1] { "OpenRight" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("stylebox", (object)val48)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(MonotoneButton), (IEnumerable<string>)new string[1] { "OpenBoth" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("stylebox", (object)val49)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(MonotoneButton), (IEnumerable<string>)null, (string)null, (IEnumerable<string>)new string[1] { "pressed" }), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("stylebox", (object)val50)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(MonotoneButton), (IEnumerable<string>)new string[1] { "OpenLeft" }, (string)null, (IEnumerable<string>)new string[1] { "pressed" }), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("stylebox", (object)val51)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(MonotoneButton), (IEnumerable<string>)new string[1] { "OpenRight" }, (string)null, (IEnumerable<string>)new string[1] { "pressed" }), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("stylebox", (object)val52)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(MonotoneButton), (IEnumerable<string>)new string[1] { "OpenBoth" }, (string)null, (IEnumerable<string>)new string[1] { "pressed" }), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("stylebox", (object)val53)
			}),
			new StyleRule((Selector)new SelectorChild((Selector)(object)SelectorElement.Type(typeof(NanoHeading)), (Selector)(object)SelectorElement.Type(typeof(PanelContainer))), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("panel", (object)val62)
			}),
			new StyleRule((Selector)(object)SelectorElement.Type(typeof(StripeBack)), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("background", (object)val63)
			}),
			new StyleRule((Selector)(object)SelectorElement.Class(new string[1] { "ItemStatus" }), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("font", (object)val2)
			}),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element().Class("ItemStatusNotHeld")).Prop("font", (object)val3).Prop("font-color", (object)ItemStatusNotHeldColor)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<RichTextLabel>().Class("ItemStatus")).Prop("LineHeightScale", (object)0.7f).Prop("Margin", (object)new Thickness(0f, 0f, 0f, -6f))),
			new StyleRule((Selector)(object)SelectorElement.Type(typeof(Slider)), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[4]
			{
				new StyleProperty("background", (object)val65),
				new StyleProperty("foreground", (object)val66),
				new StyleProperty("grabber", (object)val67),
				new StyleProperty("fill", (object)val64)
			}),
			new StyleRule((Selector)(object)SelectorElement.Type(typeof(ColorableSlider)), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[2]
			{
				new StyleProperty("fillWhite", (object)val71),
				new StyleProperty("backgroundWhite", (object)val71)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(Slider), (IEnumerable<string>)new string[1] { "Red" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("fill", (object)val69)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(Slider), (IEnumerable<string>)new string[1] { "Green" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("fill", (object)val68)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(Slider), (IEnumerable<string>)new string[1] { "Blue" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("fill", (object)val70)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(Slider), (IEnumerable<string>)new string[1] { "White" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("fill", (object)val71)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(Button), (IEnumerable<string>)new string[1] { "chatSelectorOptionButton" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("stylebox", (object)val34)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(ContainerButton), (IEnumerable<string>)new string[1] { "chatFilterOptionButton" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("stylebox", (object)val35)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(ContainerButton), (IEnumerable<string>)new string[1] { "chatFilterOptionButton" }, (string)null, (IEnumerable<string>)new string[1] { "normal" }), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("modulate-self", (object)ButtonColorDefault)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(ContainerButton), (IEnumerable<string>)new string[1] { "chatFilterOptionButton" }, (string)null, (IEnumerable<string>)new string[1] { "hover" }), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("modulate-self", (object)ButtonColorHovered)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(ContainerButton), (IEnumerable<string>)new string[1] { "chatFilterOptionButton" }, (string)null, (IEnumerable<string>)new string[1] { "pressed" }), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("modulate-self", (object)ButtonColorPressed)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(ContainerButton), (IEnumerable<string>)new string[1] { "chatFilterOptionButton" }, (string)null, (IEnumerable<string>)new string[1] { "disabled" }), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("modulate-self", (object)ButtonColorDisabled)
			}),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<Button>().Class("outputPanelScrollDownButton")).Prop("stylebox", (object)val36)),
			new StyleRule((Selector)new SelectorElement(typeof(OptionButton), (IEnumerable<string>)null, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("stylebox", (object)base.BaseButton)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(OptionButton), (IEnumerable<string>)null, (string)null, (IEnumerable<string>)new string[1] { "normal" }), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("modulate-self", (object)ButtonColorDefault)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(OptionButton), (IEnumerable<string>)null, (string)null, (IEnumerable<string>)new string[1] { "hover" }), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("modulate-self", (object)ButtonColorHovered)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(OptionButton), (IEnumerable<string>)null, (string)null, (IEnumerable<string>)new string[1] { "pressed" }), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("modulate-self", (object)ButtonColorPressed)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(OptionButton), (IEnumerable<string>)null, (string)null, (IEnumerable<string>)new string[1] { "disabled" }), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("modulate-self", (object)ButtonColorDisabled)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(TextureRect), (IEnumerable<string>)new string[1] { "optionTriangle" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("texture", (object)texture17)
			}),
			new StyleRule((Selector)new SelectorElement(typeof(Label), (IEnumerable<string>)new string[1] { "optionButton" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("alignMode", (object)(AlignMode)1)
			}),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<PanelContainer>().Class("optionButtonBackground")).Prop("panel", (object)new StyleBoxFlat(Color.FromHex((ReadOnlySpan<char>)"#25252A", (Color?)null)))),
			new StyleRule((Selector)new SelectorElement(typeof(PanelContainer), (IEnumerable<string>)new string[1] { "HighDivider" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("panel", (object)new StyleBoxFlat
				{
					BackgroundColor = NanoGold,
					ContentMarginBottomOverride = 2f,
					ContentMarginLeftOverride = 2f
				})
			}),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<TextureButton>().Class("HelpButton")).Prop("texture", (object)resCache.GetTexture("/Textures/Interface/VerbIcons/information.svg.192dpi.png"))),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<Label>().Class("LabelBig")).Prop("font", (object)val11)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<Label>().Class("LabelSmall")).Prop("font", (object)val2)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<PanelContainer>().Class("AngleRect")).Prop("panel", (object)base.BaseAngleRect).Prop("modulate-self", (object)Color.FromHex((ReadOnlySpan<char>)"#25252A", (Color?)null))),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<PanelContainer>().Class("PubgLobbyFrame")).Prop("panel", (object)val24).Prop("modulate-self", (object)Color.White)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<PanelContainer>().Class("BackgroundOpenRight")).Prop("panel", (object)base.BaseButtonOpenRight).Prop("modulate-self", (object)Color.FromHex((ReadOnlySpan<char>)"#25252A", (Color?)null))),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<PanelContainer>().Class("BackgroundOpenLeft")).Prop("panel", (object)base.BaseButtonOpenLeft).Prop("modulate-self", (object)Color.FromHex((ReadOnlySpan<char>)"#25252A", (Color?)null))),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<PanelContainer>().Class("LowDivider")).Prop("panel", (object)new StyleBoxFlat
			{
				BackgroundColor = Color.FromHex((ReadOnlySpan<char>)"#444", (Color?)null),
				ContentMarginLeftOverride = 2f,
				ContentMarginBottomOverride = 2f
			})),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<Label>().Class("FancyWindowTitle")).Prop("font", (object)font5).Prop("font-color", (object)NanoGold)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<PanelContainer>().Class("WindowHeadingBackground")).Prop("panel", (object)new StyleBoxTexture(base.BaseButtonOpenLeft)
			{
				Padding = default(Thickness)
			}).Prop("modulate-self", (object)Color.FromHex((ReadOnlySpan<char>)"#1F1F23", (Color?)null))),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<PanelContainer>().Class("WindowHeadingBackgroundLight")).Prop("panel", (object)new StyleBoxTexture(base.BaseButtonOpenLeft)
			{
				Padding = default(Thickness)
			})),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<TextureButton>().Class("windowHelpButton")).Prop("texture", (object)resCache.GetTexture("/Textures/Interface/Nano/help.png")).Prop("modulate-self", (object)Color.FromHex((ReadOnlySpan<char>)"#4B596A", (Color?)null))),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<TextureButton>().Class("windowHelpButton").Pseudo("hover")).Prop("modulate-self", (object)Color.FromHex((ReadOnlySpan<char>)"#7F3636", (Color?)null))),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<TextureButton>().Class("windowHelpButton").Pseudo("pressed")).Prop("modulate-self", (object)Color.FromHex((ReadOnlySpan<char>)"#753131", (Color?)null))),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<PanelContainer>().Class("PanelBackgroundBaseDark")).Prop("panel", (object)new StyleBoxTexture(base.BaseButtonOpenBoth)
			{
				Padding = default(Thickness)
			}).Prop("modulate-self", (object)Color.FromHex((ReadOnlySpan<char>)"#1F1F23", (Color?)null))),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<PanelContainer>().Class("PanelBackgroundLight")).Prop("panel", (object)new StyleBoxTexture(base.BaseButtonOpenBoth)
			{
				Padding = default(Thickness)
			}).Prop("modulate-self", (object)Color.FromHex((ReadOnlySpan<char>)"#2F2F3B", (Color?)null))),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<TextureRect>().Class("NTLogoDark")).Prop("texture", (object)resCache.GetTexture("/Textures/Interface/Nano/ntlogo.svg.png")).Prop("modulate-self", (object)Color.FromHex((ReadOnlySpan<char>)"#757575", (Color?)null))),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<Label>().Class("WindowFooterText")).Prop("font", (object)val).Prop("font-color", (object)Color.FromHex((ReadOnlySpan<char>)"#757575", (Color?)null))),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<TextureButton>().Class("CrossButtonRed")).Prop("texture", (object)resCache.GetTexture("/Textures/Interface/Nano/cross.svg.png")).Prop("modulate-self", (object)DangerousRedFore)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<TextureButton>().Class("CrossButtonRed").Pseudo("hover")).Prop("modulate-self", (object)Color.FromHex((ReadOnlySpan<char>)"#7F3636", (Color?)null))),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<TextureButton>().Class("CrossButtonRed").Pseudo("hover")).Prop("modulate-self", (object)Color.FromHex((ReadOnlySpan<char>)"#753131", (Color?)null))),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<TextureButton>().Class("Refresh")).Prop("texture", (object)resCache.GetTexture("/Textures/Interface/Nano/circular_arrow.svg.96dpi.png"))),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<TextureButton>().Class("SpeciesInfoDefault")).Prop("texture", (object)resCache.GetTexture("/Textures/Interface/VerbIcons/information.svg.192dpi.png"))),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<TextureButton>().Class("SpeciesInfoWarning")).Prop("texture", (object)resCache.GetTexture("/Textures/Interface/info.svg.192dpi.png")).Prop("modulate-self", (object)Color.FromHex((ReadOnlySpan<char>)"#eeee11", (Color?)null))),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<PanelContainer>().Class("PaperDefaultBorder")).Prop("panel", (object)val73)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<RichTextLabel>().Class("PaperWrittenText")).Prop("font", (object)val4).Prop("modulate-self", (object)Color.FromHex((ReadOnlySpan<char>)"#111111", (Color?)null))),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<RichTextLabel>().Class("LabelSubText")).Prop("font", (object)val2).Prop("font-color", (object)Color.DarkGray)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<LineEdit>().Class("PaperLineEdit")).Prop("stylebox", (object)new StyleBoxEmpty())),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<Button>().Class("ButtonColorRed")).Prop("modulate-self", (object)ButtonColorDefaultRed)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<Button>().Class("ButtonColorRed").Pseudo("normal")).Prop("modulate-self", (object)ButtonColorDefaultRed)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<Button>().Class("ButtonColorRed").Pseudo("hover")).Prop("modulate-self", (object)ButtonColorHoveredRed)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<Button>().Class("ButtonColorGreen")).Prop("modulate-self", (object)ButtonColorGoodDefault)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<Button>().Class("ButtonColorGreen").Pseudo("normal")).Prop("modulate-self", (object)ButtonColorGoodDefault)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<Button>().Class("ButtonColorGreen").Pseudo("hover")).Prop("modulate-self", (object)ButtonColorGoodHovered)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<Button>().Class("ButtonAccept")).Prop("modulate-self", (object)ButtonColorGoodDefault)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<Button>().Class("ButtonAccept").Pseudo("normal")).Prop("modulate-self", (object)ButtonColorGoodDefault)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<Button>().Class("ButtonAccept").Pseudo("hover")).Prop("modulate-self", (object)ButtonColorGoodHovered)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<Button>().Class("ButtonAccept").Pseudo("disabled")).Prop("modulate-self", (object)ButtonColorGoodDisabled)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<Button>().Class("ButtonSmall")).Prop("stylebox", (object)val37)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Child().Parent(MutableSelector.op_Implicit((MutableSelector)(object)StylesheetHelpers.Element<Button>().Class("ButtonSmall"))).Child(MutableSelector.op_Implicit((MutableSelector)(object)StylesheetHelpers.Element<Label>()))).Prop("font", (object)val)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<Label>().Class("StatusFieldTitle")).Prop("font-color", (object)NanoGold)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<Label>().Class("Good")).Prop("font-color", (object)GoodGreenFore)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<Label>().Class("Caution")).Prop("font-color", (object)ConcerningOrangeFore)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<Label>().Class("Danger")).Prop("font-color", (object)DangerousRedFore)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<Label>().Class("Disabled")).Prop("font-color", (object)DisabledFore)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<TextureButton>().Class("RadialMenuButton")).Prop("texture", (object)resCache.GetTexture("/Textures/Interface/Radial/button_normal.png"))),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<TextureButton>().Class("RadialMenuButton").Pseudo("hover")).Prop("texture", (object)resCache.GetTexture("/Textures/Interface/Radial/button_hover.png"))),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<TextureButton>().Class("RadialMenuCloseButton")).Prop("texture", (object)resCache.GetTexture("/Textures/Interface/Radial/close_normal.png"))),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<TextureButton>().Class("RadialMenuCloseButton").Pseudo("hover")).Prop("texture", (object)resCache.GetTexture("/Textures/Interface/Radial/close_hover.png"))),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<TextureButton>().Class("RadialMenuBackButton")).Prop("texture", (object)resCache.GetTexture("/Textures/Interface/Radial/back_normal.png"))),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<TextureButton>().Class("RadialMenuBackButton").Pseudo("hover")).Prop("texture", (object)resCache.GetTexture("/Textures/Interface/Radial/back_hover.png"))),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<PanelContainer>().Class("PdaContentBackground")).Prop("panel", (object)base.BaseButtonOpenBoth).Prop("modulate-self", (object)Color.FromHex((ReadOnlySpan<char>)"#25252a", (Color?)null))),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<PanelContainer>().Class("PdaBackground")).Prop("panel", (object)base.BaseButtonOpenBoth).Prop("modulate-self", (object)Color.FromHex((ReadOnlySpan<char>)"#000000", (Color?)null))),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<PanelContainer>().Class("PdaBackgroundRect")).Prop("panel", (object)base.BaseAngleRect).Prop("modulate-self", (object)Color.FromHex((ReadOnlySpan<char>)"#717059", (Color?)null))),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<PanelContainer>().Class("PdaBorderRect")).Prop("panel", (object)base.AngleBorderRect)),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<PanelContainer>().Class("BackgroundDark")).Prop("panel", (object)new StyleBoxFlat(Color.FromHex((ReadOnlySpan<char>)"#25252A", (Color?)null)))),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<PdaSettingsButton>().Pseudo("normal")).Prop("backgroundColor", (object)Color.FromHex((ReadOnlySpan<char>)"#313138", (Color?)null)).Prop("foregroundColor", (object)Color.FromHex((ReadOnlySpan<char>)"#FFFFFF", (Color?)null))),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<PdaSettingsButton>().Pseudo("hover")).Prop("backgroundColor", (object)Color.FromHex((ReadOnlySpan<char>)"#3E6C45", (Color?)null)).Prop("foregroundColor", (object)Color.FromHex((ReadOnlySpan<char>)"#FFFFFF", (Color?)null))),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<PdaSettingsButton>().Pseudo("pressed")).Prop("backgroundColor", (object)Color.FromHex((ReadOnlySpan<char>)"#3E6C45", (Color?)null)).Prop("foregroundColor", (object)Color.FromHex((ReadOnlySpan<char>)"#FFFFFF", (Color?)null))),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<PdaSettingsButton>().Pseudo("disabled")).Prop("backgroundColor", (object)Color.FromHex((ReadOnlySpan<char>)"#313138", (Color?)null)).Prop("foregroundColor", (object)Color.FromHex((ReadOnlySpan<char>)"#5a5a5a", (Color?)null))),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<PdaProgramItem>().Pseudo("normal")).Prop("backgroundColor", (object)Color.FromHex((ReadOnlySpan<char>)"#313138", (Color?)null))),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<PdaProgramItem>().Pseudo("hover")).Prop("backgroundColor", (object)Color.FromHex((ReadOnlySpan<char>)"#3E6C45", (Color?)null))),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<PdaProgramItem>().Pseudo("pressed")).Prop("backgroundColor", (object)Color.FromHex((ReadOnlySpan<char>)"#3E6C45", (Color?)null))),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<Label>().Class("PdaContentFooterText")).Prop("font", (object)val2).Prop("font-color", (object)Color.FromHex((ReadOnlySpan<char>)"#757575", (Color?)null))),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<Label>().Class("PdaWindowFooterText")).Prop("font", (object)val2).Prop("font-color", (object)Color.FromHex((ReadOnlySpan<char>)"#333d3b", (Color?)null))),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<ContainerButton>().Identifier("tree-button").Class("even-row")).Prop("stylebox", (object)new StyleBoxFlat
			{
				BackgroundColor = FancyTreeEvenRowColor
			})),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<ContainerButton>().Identifier("tree-button").Class("odd-row")).Prop("stylebox", (object)new StyleBoxFlat
			{
				BackgroundColor = FancyTreeOddRowColor
			})),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<ContainerButton>().Identifier("tree-button").Class("selected")).Prop("stylebox", (object)new StyleBoxFlat
			{
				BackgroundColor = FancyTreeSelectedRowColor
			})),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<ContainerButton>().Identifier("tree-button").Pseudo("hover")).Prop("stylebox", (object)new StyleBoxFlat
			{
				BackgroundColor = FancyTreeSelectedRowColor
			})),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<Label>().Class("SiliconLawPositionLabel")).Prop("font-color", (object)NanoGold)),
			new StyleRule((Selector)new SelectorElement(typeof(TextureButton), (IEnumerable<string>)new string[1] { "pinButtonPinned" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("texture", (object)resCache.GetTexture("/Textures/Interface/Bwoink/pinned.png"))
			}),
			new StyleRule((Selector)new SelectorElement(typeof(TextureButton), (IEnumerable<string>)new string[1] { "pinButtonUnpinned" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("texture", (object)resCache.GetTexture("/Textures/Interface/Bwoink/un_pinned.png"))
			}),
			new StyleRule((Selector)new SelectorElement((Type)null, (IEnumerable<string>)new string[1] { "LobbyBackground" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("panel", (object)val23)
			}),
			MutableSelector.op_Implicit(((MutableSelector)StylesheetHelpers.Element<PanelContainer>().Class("Inset")).Prop("panel", (object)val72)),
			new StyleRule((Selector)new SelectorElement(typeof(Label), (IEnumerable<string>)new string[1] { "CMAlignLeft" }, (string)null, (IEnumerable<string>)null), (IReadOnlyList<StyleProperty>)(object)new StyleProperty[1]
			{
				new StyleProperty("alignMode", (object)(AlignMode)0)
			})
		}).ToList());
	}

	static StyleNano()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0373: Unknown result type (might be due to invalid IL or missing references)
		Color val = new Color(0.8f, 0.8f, 0.8f, 1f);
		FancyTreeOddRowColor = (ref FancyTreeEvenRowColor) * (ref val);
		FancyTreeSelectedRowColor = new Color((byte)55, (byte)55, (byte)68, byte.MaxValue);
		ItemStatusNotHeldColor = Color.Gray;
		ChatBackgroundColor = Color.FromHex((ReadOnlySpan<char>)"#131313", (Color?)null);
	}
}
