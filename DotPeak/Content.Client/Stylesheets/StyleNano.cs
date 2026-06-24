// Decompiled with JetBrains decompiler
// Type: Content.Client.Stylesheets.StyleNano
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
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
  public static readonly Color PanelDark = Color.FromHex((ReadOnlySpan<char>) "#1E1E22", new Color?());
  public static readonly Color NanoGold = Color.FromHex((ReadOnlySpan<char>) "#A88B5E", new Color?());
  public static readonly Color GoodGreenFore = Color.FromHex((ReadOnlySpan<char>) "#31843E", new Color?());
  public static readonly Color ConcerningOrangeFore = Color.FromHex((ReadOnlySpan<char>) "#A5762F", new Color?());
  public static readonly Color DangerousRedFore = Color.FromHex((ReadOnlySpan<char>) "#BB3232", new Color?());
  public static readonly Color DisabledFore = Color.FromHex((ReadOnlySpan<char>) "#5A5A5A", new Color?());
  public static readonly Color ButtonColorDefault = Color.FromHex((ReadOnlySpan<char>) "#464966", new Color?());
  public static readonly Color ButtonColorDefaultRed = Color.FromHex((ReadOnlySpan<char>) "#D43B3B", new Color?());
  public static readonly Color ButtonColorHovered = Color.FromHex((ReadOnlySpan<char>) "#575b7f", new Color?());
  public static readonly Color ButtonColorHoveredRed = Color.FromHex((ReadOnlySpan<char>) "#DF6B6B", new Color?());
  public static readonly Color ButtonColorPressed = Color.FromHex((ReadOnlySpan<char>) "#3e6c45", new Color?());
  public static readonly Color ButtonColorDisabled = Color.FromHex((ReadOnlySpan<char>) "#30313c", new Color?());
  public static readonly Color ButtonColorCautionDefault = Color.FromHex((ReadOnlySpan<char>) "#ab3232", new Color?());
  public static readonly Color ButtonColorCautionHovered = Color.FromHex((ReadOnlySpan<char>) "#cf2f2f", new Color?());
  public static readonly Color ButtonColorCautionPressed = Color.FromHex((ReadOnlySpan<char>) "#3e6c45", new Color?());
  public static readonly Color ButtonColorCautionDisabled = Color.FromHex((ReadOnlySpan<char>) "#602a2a", new Color?());
  public static readonly Color ButtonColorGoodDefault = Color.FromHex((ReadOnlySpan<char>) "#3E6C45", new Color?());
  public static readonly Color ButtonColorGoodHovered = Color.FromHex((ReadOnlySpan<char>) "#31843E", new Color?());
  public static readonly Color ButtonColorGoodDisabled = Color.FromHex((ReadOnlySpan<char>) "#164420", new Color?());
  public static readonly Color PointRed = Color.FromHex((ReadOnlySpan<char>) "#B02E26", new Color?());
  public static readonly Color PointGreen = Color.FromHex((ReadOnlySpan<char>) "#38b026", new Color?());
  public static readonly Color PointMagenta = Color.FromHex((ReadOnlySpan<char>) "#FF00FF", new Color?());
  public static readonly Color ButtonColorContext = Color.FromHex((ReadOnlySpan<char>) "#1119", new Color?());
  public static readonly Color ButtonColorContextHover = Color.DarkSlateGray;
  public static readonly Color ButtonColorContextPressed = Color.LightSlateGray;
  public static readonly Color ButtonColorContextDisabled = Color.Black;
  public static readonly Color ExamineButtonColorContext = Color.Transparent;
  public static readonly Color ExamineButtonColorContextHover = Color.DarkSlateGray;
  public static readonly Color ExamineButtonColorContextPressed = Color.LightSlateGray;
  public static readonly Color ExamineButtonColorContextDisabled = Color.FromHex((ReadOnlySpan<char>) "#5A5A5A", new Color?());
  public static readonly Color FancyTreeEvenRowColor = Color.FromHex((ReadOnlySpan<char>) "#25252A", new Color?());
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
    Font font1 = resCache.NotoStack(size: 8);
    Font font2 = resCache.NotoStack();
    Font font3 = resCache.NotoStack("Italic");
    Font font4 = resCache.NotoStack(size: 12);
    Font font5 = resCache.NotoStack("Italic", 12);
    Font font6 = resCache.NotoStack("Bold", 12);
    Font font7 = resCache.NotoStack("BoldItalic", 12);
    resCache.NotoStack("BoldItalic", 14);
    resCache.NotoStack("BoldItalic", 16 /*0x10*/);
    Font font8 = resCache.NotoStack("Bold", 14, true);
    Font font9 = resCache.NotoStack("Bold", 16 /*0x10*/, true);
    Font font10 = resCache.NotoStack(size: 15);
    Font font11 = resCache.NotoStack(size: 16 /*0x10*/);
    Font font12 = resCache.NotoStack("Bold", 16 /*0x10*/);
    Font font13 = resCache.NotoStack("Bold", 18);
    Font font14 = resCache.NotoStack("Bold", 20);
    Font font15 = resCache.GetFont("/EngineFonts/NotoSans/NotoSansMono-Regular.ttf", 12);
    Font font16 = resCache.GetFont("/Fonts/RobotoMono/RobotoMono-Bold.ttf", 11);
    Font font17 = resCache.GetFont("/Fonts/RobotoMono/RobotoMono-Bold.ttf", 12);
    Font font18 = resCache.GetFont("/Fonts/RobotoMono/RobotoMono-Bold.ttf", 14);
    Texture texture1 = resCache.GetTexture("/Textures/Interface/Nano/window_header.png");
    StyleBoxTexture styleBoxTexture1 = new StyleBoxTexture();
    styleBoxTexture1.Texture = texture1;
    styleBoxTexture1.PatchMarginBottom = 3f;
    styleBoxTexture1.ExpandMarginBottom = 3f;
    ((StyleBox) styleBoxTexture1).ContentMarginBottomOverride = new float?(0.0f);
    StyleBoxTexture styleBoxTexture2 = styleBoxTexture1;
    Texture texture2 = resCache.GetTexture("/Textures/Interface/Nano/window_header_alert.png");
    StyleBoxTexture styleBoxTexture3 = new StyleBoxTexture();
    styleBoxTexture3.Texture = texture2;
    styleBoxTexture3.PatchMarginBottom = 3f;
    styleBoxTexture3.ExpandMarginBottom = 3f;
    ((StyleBox) styleBoxTexture3).ContentMarginBottomOverride = new float?(0.0f);
    StyleBoxTexture styleBoxTexture4 = styleBoxTexture3;
    Texture texture3 = resCache.GetTexture("/Textures/Interface/Nano/window_background.png");
    StyleBoxTexture styleBoxTexture5 = new StyleBoxTexture()
    {
      Texture = texture3
    };
    styleBoxTexture5.SetPatchMargin((StyleBox.Margin) 14, 2f);
    styleBoxTexture5.SetExpandMargin((StyleBox.Margin) 14, 2f);
    Texture texture4 = resCache.GetTexture("/Textures/Interface/Nano/window_background_bordered.png");
    StyleBoxTexture styleBoxTexture6 = new StyleBoxTexture()
    {
      Texture = texture4
    };
    styleBoxTexture6.SetPatchMargin((StyleBox.Margin) 15, 2f);
    StyleBoxTexture styleBoxTexture7 = new StyleBoxTexture()
    {
      Texture = texture4
    };
    styleBoxTexture7.SetPatchMargin((StyleBox.Margin) 15, 2f);
    Texture texture5 = resCache.GetTexture("/Textures/Interface/Inventory/inv_slot_background.png");
    StyleBoxTexture styleBoxTexture8 = new StyleBoxTexture()
    {
      Texture = texture5
    };
    styleBoxTexture8.SetPatchMargin((StyleBox.Margin) 15, 2f);
    ((StyleBox) styleBoxTexture8).SetContentMarginOverride((StyleBox.Margin) 15, 0.0f);
    Texture texture6 = resCache.GetTexture("/Textures/Interface/Inventory/hand_slot_highlight.png");
    StyleBoxTexture styleBoxTexture9 = new StyleBoxTexture()
    {
      Texture = texture6
    };
    styleBoxTexture9.SetPatchMargin((StyleBox.Margin) 15, 2f);
    Texture texture7 = resCache.GetTexture("/Textures/Interface/Nano/transparent_window_background_bordered.png");
    StyleBoxTexture styleBoxTexture10 = new StyleBoxTexture()
    {
      Texture = texture7
    };
    styleBoxTexture10.SetPatchMargin((StyleBox.Margin) 15, 2f);
    Texture texture8 = resCache.GetTexture("/Textures/Interface/Nano/lobby_b.png");
    StyleBoxTexture styleBoxTexture11 = new StyleBoxTexture()
    {
      Texture = texture8,
      Mode = (StyleBoxTexture.StretchMode) 1
    };
    styleBoxTexture11.SetPatchMargin((StyleBox.Margin) 15, 24f);
    styleBoxTexture11.SetExpandMargin((StyleBox.Margin) 15, -4f);
    ((StyleBox) styleBoxTexture11).SetContentMarginOverride((StyleBox.Margin) 15, 8f);
    StyleBoxTexture styleBoxTexture12 = new StyleBoxTexture()
    {
      Texture = resCache.GetTexture("/Textures/_PUBG/Lobby/Frames/frametest.png")
    };
    styleBoxTexture12.SetPatchMargin((StyleBox.Margin) 15, 10f);
    StyleBoxTexture styleBoxTexture13 = new StyleBoxTexture()
    {
      Texture = texture4
    };
    styleBoxTexture13.SetPatchMargin((StyleBox.Margin) 15, 2f);
    styleBoxTexture13.SetExpandMargin((StyleBox.Margin) 15, 4f);
    StyleBoxTexture styleBoxTexture14 = new StyleBoxTexture(this.BaseButton);
    styleBoxTexture14.SetPatchMargin((StyleBox.Margin) 15, 10f);
    ((StyleBox) styleBoxTexture14).SetPadding((StyleBox.Margin) 15, 0.0f);
    ((StyleBox) styleBoxTexture14).SetContentMarginOverride((StyleBox.Margin) 3, 0.0f);
    ((StyleBox) styleBoxTexture14).SetContentMarginOverride((StyleBox.Margin) 12, 4f);
    StyleBoxTexture styleBoxTexture15 = new StyleBoxTexture()
    {
      Texture = Texture.White
    };
    Texture texture9 = resCache.GetTexture("/Textures/Interface/Nano/light_panel_background_bordered.png");
    StyleBoxTexture styleBoxTexture16 = new StyleBoxTexture(this.BaseButton);
    styleBoxTexture16.Texture = texture9;
    styleBoxTexture16.SetPatchMargin((StyleBox.Margin) 15, 2f);
    ((StyleBox) styleBoxTexture16).SetPadding((StyleBox.Margin) 15, 2f);
    ((StyleBox) styleBoxTexture16).SetContentMarginOverride((StyleBox.Margin) 3, 2f);
    ((StyleBox) styleBoxTexture16).SetContentMarginOverride((StyleBox.Margin) 12, 2f);
    new StyleBoxTexture(styleBoxTexture16).Modulate = StyleNano.ButtonColorHovered;
    new StyleBoxTexture(styleBoxTexture16).Modulate = StyleNano.ButtonColorPressed;
    new StyleBoxTexture(styleBoxTexture16).Modulate = StyleNano.ButtonColorDisabled;
    Texture texture10 = resCache.GetTexture("/Textures/Interface/Nano/black_panel_light_thin_border.png");
    Texture texture11 = resCache.GetTexture("/Textures/Interface/Nano/black_panel_red_thin_border.png");
    StyleBoxTexture styleBoxTexture17 = new StyleBoxTexture(this.BaseButton);
    styleBoxTexture17.Texture = texture10;
    styleBoxTexture17.SetPatchMargin((StyleBox.Margin) 15, 2f);
    ((StyleBox) styleBoxTexture17).SetPadding((StyleBox.Margin) 15, 2f);
    ((StyleBox) styleBoxTexture17).SetContentMarginOverride((StyleBox.Margin) 3, 2f);
    ((StyleBox) styleBoxTexture17).SetContentMarginOverride((StyleBox.Margin) 12, 2f);
    new StyleBoxTexture(styleBoxTexture17).Texture = texture11;
    new StyleBoxTexture(styleBoxTexture17).Modulate = StyleNano.ButtonColorHovered;
    new StyleBoxTexture(styleBoxTexture17).Modulate = StyleNano.ButtonColorPressed;
    Texture texture12 = resCache.GetTexture("/Textures/Interface/Nano/button.svg.96dpi.png");
    StyleBoxTexture styleBoxTexture18 = new StyleBoxTexture();
    styleBoxTexture18.Texture = texture12;
    styleBoxTexture18.SetPatchMargin((StyleBox.Margin) 15, 10f);
    ((StyleBox) styleBoxTexture18).SetPadding((StyleBox.Margin) 15, 0.0f);
    ((StyleBox) styleBoxTexture18).SetContentMarginOverride((StyleBox.Margin) 15, 0.0f);
    StyleBoxTexture styleBoxTexture19 = new StyleBoxTexture(styleBoxTexture18)
    {
      Texture = (Texture) new AtlasTexture(texture12, UIBox2.FromDimensions(new Vector2(0.0f, 0.0f), new Vector2(14f, 24f)))
    };
    styleBoxTexture19.SetPatchMargin((StyleBox.Margin) 4, 0.0f);
    StyleBoxTexture styleBoxTexture20 = new StyleBoxTexture(styleBoxTexture18)
    {
      Texture = (Texture) new AtlasTexture(texture12, UIBox2.FromDimensions(new Vector2(10f, 0.0f), new Vector2(14f, 24f)))
    };
    styleBoxTexture20.SetPatchMargin((StyleBox.Margin) 8, 0.0f);
    StyleBoxTexture styleBoxTexture21 = new StyleBoxTexture(styleBoxTexture18)
    {
      Texture = (Texture) new AtlasTexture(texture12, UIBox2.FromDimensions(new Vector2(10f, 0.0f), new Vector2(3f, 24f)))
    };
    styleBoxTexture21.SetPatchMargin((StyleBox.Margin) 12, 0.0f);
    Texture texture13 = resCache.GetTexture("/Textures/Interface/Nano/rounded_button.svg.96dpi.png");
    StyleBoxTexture styleBoxTexture22 = new StyleBoxTexture()
    {
      Texture = texture13
    };
    styleBoxTexture22.SetPatchMargin((StyleBox.Margin) 15, 5f);
    ((StyleBox) styleBoxTexture22).SetPadding((StyleBox.Margin) 15, 2f);
    Texture texture14 = resCache.GetTexture("/Textures/Interface/Nano/rounded_button_bordered.svg.96dpi.png");
    StyleBoxTexture styleBoxTexture23 = new StyleBoxTexture()
    {
      Texture = texture14
    };
    styleBoxTexture23.SetPatchMargin((StyleBox.Margin) 15, 5f);
    ((StyleBox) styleBoxTexture23).SetPadding((StyleBox.Margin) 15, 2f);
    Texture texture15 = resCache.GetTexture("/Textures/Interface/Nano/rounded_button_half_bordered.svg.96dpi.png");
    StyleBoxTexture styleBoxTexture24 = new StyleBoxTexture()
    {
      Texture = texture15
    };
    styleBoxTexture24.SetPatchMargin((StyleBox.Margin) 15, 5f);
    ((StyleBox) styleBoxTexture24).SetPadding((StyleBox.Margin) 15, 2f);
    ((StyleBox) styleBoxTexture24).SetPadding((StyleBox.Margin) 1, 0.0f);
    ((StyleBox) styleBoxTexture24).SetPadding((StyleBox.Margin) 2, 0.0f);
    Texture texture16 = resCache.GetTexture("/Textures/Interface/Nano/button_small.svg.96dpi.png");
    StyleBoxTexture styleBoxTexture25 = new StyleBoxTexture()
    {
      Texture = texture16
    };
    Texture texture17 = resCache.GetTexture("/Textures/Interface/Nano/inverted_triangle.svg.png");
    Texture texture18 = resCache.GetTexture("/Textures/Interface/Nano/lineedit.png");
    StyleBoxTexture styleBoxTexture26 = new StyleBoxTexture()
    {
      Texture = texture18
    };
    styleBoxTexture26.SetPatchMargin((StyleBox.Margin) 15, 3f);
    ((StyleBox) styleBoxTexture26).SetContentMarginOverride((StyleBox.Margin) 12, 5f);
    StyleBoxFlat styleBoxFlat1 = new StyleBoxFlat()
    {
      BackgroundColor = StyleNano.ChatBackgroundColor
    };
    ((StyleBox) new StyleBoxFlat()
    {
      BackgroundColor = StyleNano.ChatBackgroundColor
    }).SetContentMarginOverride((StyleBox.Margin) 15, 2f);
    Texture texture19 = resCache.GetTexture("/Textures/Interface/Nano/black_panel_dark_thin_border.png");
    StyleBoxTexture styleBoxTexture27 = new StyleBoxTexture()
    {
      Texture = texture19
    };
    styleBoxTexture27.SetPatchMargin((StyleBox.Margin) 15, 3f);
    ((StyleBox) styleBoxTexture27).SetContentMarginOverride((StyleBox.Margin) 12, 5f);
    Texture texture20 = resCache.GetTexture("/Textures/Interface/Nano/tabcontainer_panel.png");
    StyleBoxTexture styleBoxTexture28 = new StyleBoxTexture()
    {
      Texture = texture20
    };
    styleBoxTexture28.SetPatchMargin((StyleBox.Margin) 15, 2f);
    StyleBoxFlat styleBoxFlat2 = new StyleBoxFlat()
    {
      BackgroundColor = new Color((byte) 64 /*0x40*/, (byte) 64 /*0x40*/, (byte) 64 /*0x40*/, byte.MaxValue)
    };
    ((StyleBox) styleBoxFlat2).SetContentMarginOverride((StyleBox.Margin) 12, 5f);
    StyleBoxFlat styleBoxFlat3 = new StyleBoxFlat()
    {
      BackgroundColor = new Color((byte) 32 /*0x20*/, (byte) 32 /*0x20*/, (byte) 32 /*0x20*/, byte.MaxValue)
    };
    ((StyleBox) styleBoxFlat3).SetContentMarginOverride((StyleBox.Margin) 12, 5f);
    StyleBoxFlat styleBoxFlat4 = new StyleBoxFlat()
    {
      BackgroundColor = new Color(0.25f, 0.25f, 0.25f, 1f)
    };
    ((StyleBox) styleBoxFlat4).SetContentMarginOverride((StyleBox.Margin) 3, 14.5f);
    StyleBoxFlat styleBoxFlat5 = new StyleBoxFlat()
    {
      BackgroundColor = new Color(0.25f, 0.5f, 0.25f, 1f)
    };
    ((StyleBox) styleBoxFlat5).SetContentMarginOverride((StyleBox.Margin) 3, 14.5f);
    StyleBoxTexture styleBoxTexture29 = new StyleBoxTexture()
    {
      Texture = resCache.GetTexture("/Textures/Interface/Nano/Monotone/monotone_button.svg.96dpi.png")
    };
    styleBoxTexture29.SetPatchMargin((StyleBox.Margin) 15, 11f);
    ((StyleBox) styleBoxTexture29).SetPadding((StyleBox.Margin) 15, 1f);
    ((StyleBox) styleBoxTexture29).SetContentMarginOverride((StyleBox.Margin) 3, 2f);
    ((StyleBox) styleBoxTexture29).SetContentMarginOverride((StyleBox.Margin) 12, 14f);
    StyleBoxTexture styleBoxTexture30 = new StyleBoxTexture(styleBoxTexture29)
    {
      Texture = resCache.GetTexture("/Textures/Interface/Nano/Monotone/monotone_button_open_left.svg.96dpi.png")
    };
    StyleBoxTexture styleBoxTexture31 = new StyleBoxTexture(styleBoxTexture29)
    {
      Texture = resCache.GetTexture("/Textures/Interface/Nano/Monotone/monotone_button_open_right.svg.96dpi.png")
    };
    StyleBoxTexture styleBoxTexture32 = new StyleBoxTexture(styleBoxTexture29)
    {
      Texture = resCache.GetTexture("/Textures/Interface/Nano/Monotone/monotone_button_open_both.svg.96dpi.png")
    };
    StyleBoxTexture styleBoxTexture33 = new StyleBoxTexture(styleBoxTexture29)
    {
      Texture = texture12
    };
    StyleBoxTexture styleBoxTexture34 = new StyleBoxTexture(styleBoxTexture29)
    {
      Texture = (Texture) new AtlasTexture(texture12, UIBox2.FromDimensions(new Vector2(10f, 0.0f), new Vector2(14f, 24f)))
    };
    styleBoxTexture34.SetPatchMargin((StyleBox.Margin) 8, 0.0f);
    StyleBoxTexture styleBoxTexture35 = new StyleBoxTexture(styleBoxTexture29)
    {
      Texture = (Texture) new AtlasTexture(texture12, UIBox2.FromDimensions(new Vector2(0.0f, 0.0f), new Vector2(14f, 24f)))
    };
    styleBoxTexture35.SetPatchMargin((StyleBox.Margin) 4, 0.0f);
    StyleBoxTexture styleBoxTexture36 = new StyleBoxTexture(styleBoxTexture29)
    {
      Texture = (Texture) new AtlasTexture(texture12, UIBox2.FromDimensions(new Vector2(10f, 0.0f), new Vector2(3f, 24f)))
    };
    styleBoxTexture36.SetPatchMargin((StyleBox.Margin) 12, 0.0f);
    Texture texture21 = resCache.GetTexture("/Textures/Interface/Nano/checkbox_checked.svg.96dpi.png");
    Texture texture22 = resCache.GetTexture("/Textures/Interface/Nano/checkbox_unchecked.svg.96dpi.png");
    Texture texture23 = resCache.GetTexture("/Textures/Interface/Nano/Monotone/monotone_checkbox_checked.svg.96dpi.png");
    Texture texture24 = resCache.GetTexture("/Textures/Interface/Nano/Monotone/monotone_checkbox_unchecked.svg.96dpi.png");
    Texture texture25 = resCache.GetTexture("/Textures/Interface/Nano/tooltip.png");
    StyleBoxTexture styleBoxTexture37 = new StyleBoxTexture()
    {
      Texture = texture25
    };
    styleBoxTexture37.SetPatchMargin((StyleBox.Margin) 15, 2f);
    ((StyleBox) styleBoxTexture37).SetContentMarginOverride((StyleBox.Margin) 12, 7f);
    Texture texture26 = resCache.GetTexture("/Textures/Interface/Nano/whisper.png");
    StyleBoxTexture styleBoxTexture38 = new StyleBoxTexture()
    {
      Texture = texture26
    };
    styleBoxTexture38.SetPatchMargin((StyleBox.Margin) 15, 2f);
    ((StyleBox) styleBoxTexture38).SetContentMarginOverride((StyleBox.Margin) 12, 7f);
    Texture texture27 = resCache.GetTexture("/Textures/Interface/Nano/placeholder.png");
    StyleBoxTexture styleBoxTexture39 = new StyleBoxTexture()
    {
      Texture = texture27
    };
    styleBoxTexture39.SetPatchMargin((StyleBox.Margin) 15, 19f);
    styleBoxTexture39.SetExpandMargin((StyleBox.Margin) 15, -5f);
    styleBoxTexture39.Mode = (StyleBoxTexture.StretchMode) 1;
    StyleBoxFlat styleBoxFlat6 = new StyleBoxFlat()
    {
      BackgroundColor = new Color((byte) 75, (byte) 75, (byte) 86, byte.MaxValue)
    };
    ((StyleBox) styleBoxFlat6).SetContentMarginOverride((StyleBox.Margin) 3, 2f);
    ((StyleBox) styleBoxFlat6).SetContentMarginOverride((StyleBox.Margin) 12, 4f);
    StyleBoxFlat styleBoxFlat7 = new StyleBoxFlat()
    {
      BackgroundColor = new Color((byte) 10, (byte) 10, (byte) 12, byte.MaxValue)
    };
    ((StyleBox) styleBoxFlat7).SetContentMarginOverride((StyleBox.Margin) 3, 2f);
    ((StyleBox) styleBoxFlat7).SetContentMarginOverride((StyleBox.Margin) 12, 4f);
    StyleBoxFlat styleBoxFlat8 = new StyleBoxFlat()
    {
      BackgroundColor = new Color((byte) 55, (byte) 55, (byte) 68, byte.MaxValue)
    };
    ((StyleBox) styleBoxFlat8).SetContentMarginOverride((StyleBox.Margin) 3, 2f);
    ((StyleBox) styleBoxFlat8).SetContentMarginOverride((StyleBox.Margin) 12, 4f);
    StyleBoxFlat styleBoxFlat9 = new StyleBoxFlat()
    {
      BackgroundColor = Color.Transparent
    };
    ((StyleBox) styleBoxFlat9).SetContentMarginOverride((StyleBox.Margin) 3, 2f);
    ((StyleBox) styleBoxFlat9).SetContentMarginOverride((StyleBox.Margin) 12, 4f);
    Texture texture28 = resCache.GetTexture("/Textures/Interface/Nano/square.png");
    StyleBoxTexture styleBoxTexture40 = new StyleBoxTexture();
    styleBoxTexture40.Texture = texture28;
    ((StyleBox) styleBoxTexture40).ContentMarginLeftOverride = new float?(10f);
    StyleBoxTexture styleBoxTexture41 = styleBoxTexture40;
    Texture texture29 = resCache.GetTexture("/Textures/Interface/Nano/nanoheading.svg.96dpi.png");
    StyleBoxTexture styleBoxTexture42 = new StyleBoxTexture();
    styleBoxTexture42.Texture = texture29;
    styleBoxTexture42.PatchMarginRight = 10f;
    styleBoxTexture42.PatchMarginTop = 10f;
    ((StyleBox) styleBoxTexture42).ContentMarginTopOverride = new float?(2f);
    ((StyleBox) styleBoxTexture42).ContentMarginLeftOverride = new float?(10f);
    ((StyleBox) styleBoxTexture42).PaddingTop = 4f;
    StyleBoxTexture styleBoxTexture43 = styleBoxTexture42;
    styleBoxTexture43.SetPatchMargin((StyleBox.Margin) 10, 2f);
    Texture texture30 = resCache.GetTexture("/Textures/Interface/Nano/stripeback.svg.96dpi.png");
    StyleBoxTexture styleBoxTexture44 = new StyleBoxTexture()
    {
      Texture = texture30,
      Mode = (StyleBoxTexture.StretchMode) 1
    };
    Texture texture31 = resCache.GetTexture("/Textures/Interface/Nano/slider_outline.svg.96dpi.png");
    Texture texture32 = resCache.GetTexture("/Textures/Interface/Nano/slider_fill.svg.96dpi.png");
    Texture texture33 = resCache.GetTexture("/Textures/Interface/Nano/slider_grabber.svg.96dpi.png");
    StyleBoxTexture styleBoxTexture45 = new StyleBoxTexture()
    {
      Texture = texture32,
      Modulate = Color.FromHex((ReadOnlySpan<char>) "#3E6C45", new Color?())
    };
    StyleBoxTexture styleBoxTexture46 = new StyleBoxTexture()
    {
      Texture = texture32,
      Modulate = StyleNano.PanelDark
    };
    StyleBoxTexture styleBoxTexture47 = new StyleBoxTexture()
    {
      Texture = texture31,
      Modulate = Color.FromHex((ReadOnlySpan<char>) "#494949", new Color?())
    };
    StyleBoxTexture styleBoxTexture48 = new StyleBoxTexture()
    {
      Texture = texture33
    };
    styleBoxTexture45.SetPatchMargin((StyleBox.Margin) 15, 12f);
    styleBoxTexture46.SetPatchMargin((StyleBox.Margin) 15, 12f);
    styleBoxTexture47.SetPatchMargin((StyleBox.Margin) 15, 12f);
    styleBoxTexture48.SetPatchMargin((StyleBox.Margin) 15, 12f);
    StyleBoxTexture styleBoxTexture49 = new StyleBoxTexture(styleBoxTexture45)
    {
      Modulate = Color.LimeGreen
    };
    StyleBoxTexture styleBoxTexture50 = new StyleBoxTexture(styleBoxTexture45)
    {
      Modulate = Color.Red
    };
    StyleBoxTexture styleBoxTexture51 = new StyleBoxTexture(styleBoxTexture45)
    {
      Modulate = Color.Blue
    };
    StyleBoxTexture styleBoxTexture52 = new StyleBoxTexture(styleBoxTexture45)
    {
      Modulate = Color.White
    };
    Font font19 = resCache.GetFont("/Fonts/Boxfont-round/Boxfont Round.ttf", 13);
    StyleBoxTexture styleBoxTexture53 = new StyleBoxTexture()
    {
      Texture = texture12,
      Modulate = Color.FromHex((ReadOnlySpan<char>) "#202023", new Color?())
    };
    styleBoxTexture53.SetPatchMargin((StyleBox.Margin) 15, 10f);
    StyleBoxTexture styleBoxTexture54 = new StyleBoxTexture()
    {
      Texture = resCache.GetTexture("/Textures/Interface/Paper/paper_background_default.svg.96dpi.png"),
      Modulate = Color.FromHex((ReadOnlySpan<char>) "#eaedde", new Color?())
    };
    styleBoxTexture54.SetPatchMargin((StyleBox.Margin) 15, 16f);
    Texture texture34 = resCache.GetTexture("/Textures/Interface/VerbIcons/group.svg.192dpi.png");
    Texture texture35 = resCache.GetTexture("/Textures/Interface/VerbIcons/group.svg.192dpi.png");
    Texture texture36 = resCache.GetTexture("/Textures/Interface/VerbIcons/drop.svg.192dpi.png");
    Texture texture37 = resCache.GetTexture("/Textures/Interface/VerbIcons/information.svg.192dpi.png");
    Texture texture38 = resCache.GetTexture("/Textures/Interface/VerbIcons/dot.svg.192dpi.png");
    StyleRule[] baseRules = this.BaseRules;
    StyleRule[] second = new StyleRule[244];
    second[0] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element().Class("monospace")).Prop("font", (object) font15));
    second[1] = new StyleRule((Selector) new SelectorElement(typeof (Label), (IEnumerable<string>) new string[1]
    {
      "windowTitle"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[2]
    {
      new StyleProperty("font-color", (object) StyleNano.NanoGold),
      new StyleProperty("font", (object) font8)
    });
    second[2] = new StyleRule((Selector) new SelectorElement(typeof (Label), (IEnumerable<string>) new string[1]
    {
      "windowTitleAlert"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[2]
    {
      new StyleProperty("font-color", (object) Color.White),
      new StyleProperty("font", (object) font8)
    });
    second[3] = new StyleRule((Selector) new SelectorElement((Type) null, (IEnumerable<string>) new string[1]
    {
      "windowPanel"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("panel", (object) styleBoxTexture5)
    });
    second[4] = new StyleRule((Selector) new SelectorElement((Type) null, (IEnumerable<string>) new string[1]
    {
      "BorderedWindowPanel"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("panel", (object) styleBoxTexture6)
    });
    second[5] = new StyleRule((Selector) new SelectorElement((Type) null, (IEnumerable<string>) new string[1]
    {
      "TransparentBorderedWindowPanel"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("panel", (object) styleBoxTexture10)
    });
    second[6] = new StyleRule((Selector) new SelectorElement((Type) null, (IEnumerable<string>) new string[1]
    {
      "InventorySlotBackground"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("panel", (object) styleBoxTexture8)
    });
    second[7] = new StyleRule((Selector) new SelectorElement((Type) null, (IEnumerable<string>) new string[1]
    {
      "HandSlotHighlight"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("panel", (object) styleBoxTexture9)
    });
    second[8] = new StyleRule((Selector) new SelectorElement(typeof (PanelContainer), (IEnumerable<string>) new string[1]
    {
      "HotbarPanel"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("panel", (object) styleBoxTexture13)
    });
    second[9] = new StyleRule((Selector) new SelectorElement(typeof (PanelContainer), (IEnumerable<string>) new string[1]
    {
      "windowHeader"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("panel", (object) styleBoxTexture2)
    });
    second[10] = new StyleRule((Selector) new SelectorElement(typeof (PanelContainer), (IEnumerable<string>) new string[1]
    {
      "windowHeaderAlert"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("panel", (object) styleBoxTexture4)
    });
    second[11] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<ContainerButton>().Class("button")).Prop("stylebox", (object) this.BaseButton));
    second[12] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<ContainerButton>().Class("button").Class("OpenRight")).Prop("stylebox", (object) this.BaseButtonOpenRight));
    second[13] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<ContainerButton>().Class("button").Class("OpenLeft")).Prop("stylebox", (object) this.BaseButtonOpenLeft));
    second[14] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<ContainerButton>().Class("button").Class("OpenBoth")).Prop("stylebox", (object) this.BaseButtonOpenBoth));
    second[15] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<ContainerButton>().Class("button").Class("ButtonSquare")).Prop("stylebox", (object) this.BaseButtonSquare));
    second[16 /*0x10*/] = new StyleRule((Selector) new SelectorElement(typeof (Label), (IEnumerable<string>) new string[1]
    {
      "button"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("alignMode", (object) (Label.AlignMode) 1)
    });
    second[17] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<ContainerButton>().Class("button").Pseudo("normal")).Prop("modulate-self", (object) StyleNano.ButtonColorDefault));
    second[18] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<ContainerButton>().Class("button").Pseudo("hover")).Prop("modulate-self", (object) StyleNano.ButtonColorHovered));
    second[19] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<ContainerButton>().Class("button").Pseudo("pressed")).Prop("modulate-self", (object) StyleNano.ButtonColorPressed));
    second[20] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<ContainerButton>().Class("button").Pseudo("disabled")).Prop("modulate-self", (object) StyleNano.ButtonColorDisabled));
    second[21] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<ContainerButton>().Class("button").Class("Caution").Pseudo("normal")).Prop("modulate-self", (object) StyleNano.ButtonColorCautionDefault));
    second[22] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<ContainerButton>().Class("button").Class("Caution").Pseudo("hover")).Prop("modulate-self", (object) StyleNano.ButtonColorCautionHovered));
    second[23] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<ContainerButton>().Class("button").Class("Caution").Pseudo("pressed")).Prop("modulate-self", (object) StyleNano.ButtonColorCautionPressed));
    second[24] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<ContainerButton>().Class("button").Class("Caution").Pseudo("disabled")).Prop("modulate-self", (object) StyleNano.ButtonColorCautionDisabled));
    second[25] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<ConfirmButton>().Pseudo("confirm-normal")).Prop("modulate-self", (object) StyleNano.ButtonColorCautionDefault));
    second[26] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<ConfirmButton>().Pseudo("confirm-hover")).Prop("modulate-self", (object) StyleNano.ButtonColorCautionHovered));
    second[27] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<ConfirmButton>().Pseudo("confirm-pressed")).Prop("modulate-self", (object) StyleNano.ButtonColorCautionPressed));
    second[28] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<ConfirmButton>().Pseudo("confirm-disabled")).Prop("modulate-self", (object) StyleNano.ButtonColorCautionDisabled));
    second[29] = new StyleRule((Selector) new SelectorChild((Selector) new SelectorElement(typeof (Button), (IEnumerable<string>) null, (string) null, (IEnumerable<string>) new string[1]
    {
      "disabled"
    }), (Selector) new SelectorElement(typeof (Label), (IEnumerable<string>) null, (string) null, (IEnumerable<string>) null)), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("font-color", (object) Color.FromHex((ReadOnlySpan<char>) "#E5E5E581", new Color?()))
    });
    second[30] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element().Class("ItemStatusNotHeld")).Prop("font", (object) font3).Prop("font-color", (object) StyleNano.ItemStatusNotHeldColor).Prop("Margin", (object) new Thickness(4f, 0.0f, 0.0f, 2f)));
    second[31 /*0x1F*/] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element().Class("ItemStatus")).Prop("LineHeightScale", (object) 0.7f).Prop("Margin", (object) new Thickness(4f, 0.0f, 0.0f, 2f)));
    second[32 /*0x20*/] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<PanelContainer>().Class("contextMenuPopup")).Prop("panel", (object) styleBoxTexture7));
    second[33] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<ContextMenuElement>().Class("contextMenuButton")).Prop("stylebox", (object) styleBoxTexture15));
    second[34] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<ContextMenuElement>().Class("contextMenuButton").Pseudo("normal")).Prop("modulate-self", (object) StyleNano.ButtonColorContext));
    second[35] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<ContextMenuElement>().Class("contextMenuButton").Pseudo("hover")).Prop("modulate-self", (object) StyleNano.ButtonColorContextHover));
    second[36] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<ContextMenuElement>().Class("contextMenuButton").Pseudo("pressed")).Prop("modulate-self", (object) StyleNano.ButtonColorContextPressed));
    second[37] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<ContextMenuElement>().Class("contextMenuButton").Pseudo("disabled")).Prop("modulate-self", (object) StyleNano.ButtonColorContextDisabled));
    second[38] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<RichTextLabel>().Class(InteractionVerb.DefaultTextStyleClass)).Prop("font", (object) font7));
    second[39] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<RichTextLabel>().Class(ActivationVerb.DefaultTextStyleClass)).Prop("font", (object) font6));
    second[40] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<RichTextLabel>().Class(AlternativeVerb.DefaultTextStyleClass)).Prop("font", (object) font5));
    second[41] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<RichTextLabel>().Class(Verb.DefaultTextStyleClass)).Prop("font", (object) font4));
    second[42] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<TextureRect>().Class("contextMenuExpansionTexture")).Prop("texture", (object) texture34));
    second[43] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<TextureRect>().Class("verbMenuConfirmationTexture")).Prop("texture", (object) texture35));
    second[44] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<ContextMenuElement>().Class("confirmationContextMenuButton")).Prop("stylebox", (object) styleBoxTexture15));
    second[45] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<ContextMenuElement>().Class("confirmationContextMenuButton").Pseudo("normal")).Prop("modulate-self", (object) StyleNano.ButtonColorCautionDefault));
    second[46] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<ContextMenuElement>().Class("confirmationContextMenuButton").Pseudo("hover")).Prop("modulate-self", (object) StyleNano.ButtonColorCautionHovered));
    second[47] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<ContextMenuElement>().Class("confirmationContextMenuButton").Pseudo("pressed")).Prop("modulate-self", (object) StyleNano.ButtonColorCautionPressed));
    second[48 /*0x30*/] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<ContextMenuElement>().Class("confirmationContextMenuButton").Pseudo("disabled")).Prop("modulate-self", (object) StyleNano.ButtonColorCautionDisabled));
    second[49] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<ExamineButton>().Class("examine-button")).Prop("stylebox", (object) styleBoxTexture15));
    second[50] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<ExamineButton>().Class("examine-button").Pseudo("normal")).Prop("modulate-self", (object) StyleNano.ExamineButtonColorContext));
    second[51] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<ExamineButton>().Class("examine-button").Pseudo("hover")).Prop("modulate-self", (object) StyleNano.ExamineButtonColorContextHover));
    second[52] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<ExamineButton>().Class("examine-button").Pseudo("pressed")).Prop("modulate-self", (object) StyleNano.ExamineButtonColorContextPressed));
    second[53] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<ExamineButton>().Class("examine-button").Pseudo("disabled")).Prop("modulate-self", (object) StyleNano.ExamineButtonColorContextDisabled));
    second[54] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<DirectionIcon>().Class(DirectionIcon.StyleClassDirectionIconArrow)).Prop("texture", (object) texture36));
    second[55] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<DirectionIcon>().Class(DirectionIcon.StyleClassDirectionIconUnknown)).Prop("texture", (object) texture37));
    second[56] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<DirectionIcon>().Class(DirectionIcon.StyleClassDirectionIconHere)).Prop("texture", (object) texture38));
    second[57] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<ContainerButton>().Class("storageButton")).Prop("stylebox", (object) styleBoxTexture14));
    second[58] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<ContainerButton>().Class("storageButton").Pseudo("normal")).Prop("modulate-self", (object) StyleNano.ButtonColorDefault));
    second[59] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<ContainerButton>().Class("storageButton").Pseudo("hover")).Prop("modulate-self", (object) StyleNano.ButtonColorHovered));
    second[60] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<ContainerButton>().Class("storageButton").Pseudo("pressed")).Prop("modulate-self", (object) StyleNano.ButtonColorPressed));
    second[61] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<ContainerButton>().Class("storageButton").Pseudo("disabled")).Prop("modulate-self", (object) StyleNano.ButtonColorDisabled));
    second[62] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<ContainerButton>().Class("list-container-button")).Prop("stylebox", (object) styleBoxTexture41));
    second[63 /*0x3F*/] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<ContainerButton>().Class("list-container-button").Pseudo("normal")).Prop("modulate-self", (object) new Color((byte) 55, (byte) 55, (byte) 68, byte.MaxValue)));
    second[64 /*0x40*/] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<ContainerButton>().Class("list-container-button").Pseudo("hover")).Prop("modulate-self", (object) new Color((byte) 75, (byte) 75, (byte) 86, byte.MaxValue)));
    second[65] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<ContainerButton>().Class("list-container-button").Pseudo("pressed")).Prop("modulate-self", (object) new Color((byte) 75, (byte) 75, (byte) 86, byte.MaxValue)));
    second[66] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<ContainerButton>().Class("list-container-button").Pseudo("disabled")).Prop("modulate-self", (object) new Color((byte) 10, (byte) 10, (byte) 12, byte.MaxValue)));
    second[67] = new StyleRule((Selector) new SelectorChild((Selector) new SelectorElement(typeof (Button), (IEnumerable<string>) null, "mainMenu", (IEnumerable<string>) null), (Selector) new SelectorElement(typeof (Label), (IEnumerable<string>) null, (string) null, (IEnumerable<string>) null)), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("font", (object) font12)
    });
    second[68] = new StyleRule((Selector) new SelectorElement(typeof (BoxContainer), (IEnumerable<string>) null, "mainMenuVBox", (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("separation", (object) 2)
    });
    second[69] = new StyleRule((Selector) new SelectorElement(typeof (LineEdit), (IEnumerable<string>) null, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("stylebox", (object) styleBoxTexture26)
    });
    second[70] = new StyleRule((Selector) new SelectorElement(typeof (LineEdit), (IEnumerable<string>) new string[1]
    {
      "notEditable"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("font-color", (object) new Color((byte) 192 /*0xC0*/, (byte) 192 /*0xC0*/, (byte) 192 /*0xC0*/, byte.MaxValue))
    });
    second[71] = new StyleRule((Selector) new SelectorElement(typeof (LineEdit), (IEnumerable<string>) null, (string) null, (IEnumerable<string>) new string[1]
    {
      "placeholder"
    }), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("font-color", (object) Color.Gray)
    });
    second[72] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<TextEdit>().Pseudo("placeholder")).Prop("font-color", (object) Color.Gray));
    second[73] = new StyleRule((Selector) new SelectorElement(typeof (PanelContainer), (IEnumerable<string>) new string[1]
    {
      "ChatPanel"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("panel", (object) styleBoxFlat1)
    });
    second[74] = new StyleRule((Selector) new SelectorElement(typeof (LineEdit), (IEnumerable<string>) new string[1]
    {
      "chatLineEdit"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("stylebox", (object) new StyleBoxEmpty())
    });
    second[75] = new StyleRule((Selector) new SelectorElement(typeof (LineEdit), (IEnumerable<string>) new string[1]
    {
      "actionSearchBox"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("stylebox", (object) styleBoxTexture27)
    });
    second[76] = new StyleRule((Selector) new SelectorElement(typeof (TabContainer), (IEnumerable<string>) null, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[3]
    {
      new StyleProperty("panel-stylebox", (object) styleBoxTexture28),
      new StyleProperty("tab-stylebox", (object) styleBoxFlat2),
      new StyleProperty("tab-stylebox-inactive", (object) styleBoxFlat3)
    });
    second[77] = new StyleRule((Selector) new SelectorElement(typeof (ProgressBar), (IEnumerable<string>) null, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[2]
    {
      new StyleProperty("background", (object) styleBoxFlat4),
      new StyleProperty("foreground", (object) styleBoxFlat5)
    });
    second[78] = new StyleRule((Selector) new SelectorElement(typeof (TextureRect), (IEnumerable<string>) new string[1]
    {
      "checkBox"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("texture", (object) texture22)
    });
    second[79] = new StyleRule((Selector) new SelectorElement(typeof (TextureRect), (IEnumerable<string>) new string[2]
    {
      "checkBox",
      "checkBoxChecked"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("texture", (object) texture21)
    });
    second[80 /*0x50*/] = new StyleRule((Selector) new SelectorElement(typeof (BoxContainer), (IEnumerable<string>) new string[1]
    {
      "checkBox"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("separation", (object) 10)
    });
    second[81] = new StyleRule((Selector) new SelectorElement(typeof (TextureRect), (IEnumerable<string>) new string[1]
    {
      "monotoneCheckBox"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("texture", (object) texture24)
    });
    second[82] = new StyleRule((Selector) new SelectorElement(typeof (TextureRect), (IEnumerable<string>) new string[2]
    {
      "monotoneCheckBox",
      "checkBoxChecked"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("texture", (object) texture23)
    });
    second[83] = new StyleRule((Selector) new SelectorElement(typeof (Tooltip), (IEnumerable<string>) null, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("panel", (object) styleBoxTexture37)
    });
    second[84] = new StyleRule((Selector) new SelectorElement(typeof (PanelContainer), (IEnumerable<string>) new string[1]
    {
      "tooltipBox"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("panel", (object) styleBoxTexture37)
    });
    second[85] = new StyleRule((Selector) new SelectorElement(typeof (PanelContainer), (IEnumerable<string>) new string[2]
    {
      "speechBox",
      "sayBox"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("panel", (object) styleBoxTexture37)
    });
    second[86] = new StyleRule((Selector) new SelectorElement(typeof (PanelContainer), (IEnumerable<string>) new string[2]
    {
      "speechBox",
      "whisperBox"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("panel", (object) styleBoxTexture38)
    });
    second[87] = new StyleRule((Selector) new SelectorChild((Selector) new SelectorElement(typeof (PanelContainer), (IEnumerable<string>) new string[2]
    {
      "speechBox",
      "whisperBox"
    }, (string) null, (IEnumerable<string>) null), (Selector) new SelectorElement(typeof (RichTextLabel), (IEnumerable<string>) new string[1]
    {
      "bubbleContent"
    }, (string) null, (IEnumerable<string>) null)), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("font", (object) font5)
    });
    second[88] = new StyleRule((Selector) new SelectorChild((Selector) new SelectorElement(typeof (PanelContainer), (IEnumerable<string>) new string[2]
    {
      "speechBox",
      "emoteBox"
    }, (string) null, (IEnumerable<string>) null), (Selector) new SelectorElement(typeof (RichTextLabel), (IEnumerable<string>) null, (string) null, (IEnumerable<string>) null)), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("font", (object) font5)
    });
    second[89] = new StyleRule((Selector) new SelectorChild((Selector) new SelectorElement(typeof (PanelContainer), (IEnumerable<string>) new string[2]
    {
      "speechBox",
      "commanderSpeech"
    }, (string) null, (IEnumerable<string>) null), (Selector) new SelectorElement(typeof (RichTextLabel), (IEnumerable<string>) new string[1]
    {
      "bubbleContent"
    }, (string) null, (IEnumerable<string>) null)), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("font", (object) font12)
    });
    second[90] = new StyleRule((Selector) new SelectorElement(typeof (PanelContainer), (IEnumerable<string>) new string[2]
    {
      "speechBox",
      "commanderSpeech"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("panel", (object) styleBoxTexture37)
    });
    second[91] = new StyleRule((Selector) new SelectorChild((Selector) new SelectorElement(typeof (PanelContainer), (IEnumerable<string>) new string[2]
    {
      "speechBox",
      "megaphoneSpeech"
    }, (string) null, (IEnumerable<string>) null), (Selector) new SelectorElement(typeof (RichTextLabel), (IEnumerable<string>) new string[1]
    {
      "bubbleContent"
    }, (string) null, (IEnumerable<string>) null)), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("font", (object) resCache.NotoStack("Bold", 20))
    });
    second[92] = new StyleRule((Selector) new SelectorElement(typeof (PanelContainer), (IEnumerable<string>) new string[2]
    {
      "speechBox",
      "megaphoneSpeech"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("panel", (object) styleBoxTexture37)
    });
    second[93] = new StyleRule((Selector) new SelectorElement(typeof (RichTextLabel), (IEnumerable<string>) new string[1]
    {
      "LabelKeyText"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[2]
    {
      new StyleProperty("font", (object) font6),
      new StyleProperty("modulate-self", (object) StyleNano.NanoGold)
    });
    second[94] = new StyleRule((Selector) new SelectorElement(typeof (RichTextLabel), (IEnumerable<string>) new string[1]
    {
      "tooltipAlertTitle"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("font", (object) font13)
    });
    second[95] = new StyleRule((Selector) new SelectorElement(typeof (RichTextLabel), (IEnumerable<string>) new string[1]
    {
      "tooltipAlertDesc"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("font", (object) font11)
    });
    second[96 /*0x60*/] = new StyleRule((Selector) new SelectorElement(typeof (RichTextLabel), (IEnumerable<string>) new string[1]
    {
      "tooltipAlertCooldown"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("font", (object) font11)
    });
    second[97] = new StyleRule((Selector) new SelectorElement(typeof (RichTextLabel), (IEnumerable<string>) new string[1]
    {
      "tooltipActionTitle"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("font", (object) font12)
    });
    second[98] = new StyleRule((Selector) new SelectorElement(typeof (RichTextLabel), (IEnumerable<string>) new string[1]
    {
      "tooltipActionDesc"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("font", (object) font10)
    });
    second[99] = new StyleRule((Selector) new SelectorElement(typeof (RichTextLabel), (IEnumerable<string>) new string[1]
    {
      "tooltipActionCooldown"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("font", (object) font10)
    });
    second[100] = new StyleRule((Selector) new SelectorElement(typeof (RichTextLabel), (IEnumerable<string>) new string[1]
    {
      "tooltipActionDynamicMessage"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("font", (object) font10)
    });
    second[101] = new StyleRule((Selector) new SelectorElement(typeof (RichTextLabel), (IEnumerable<string>) new string[1]
    {
      "tooltipActionCooldown"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("font", (object) font10)
    });
    second[102] = new StyleRule((Selector) new SelectorElement(typeof (RichTextLabel), (IEnumerable<string>) new string[1]
    {
      "tooltipActionCharges"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("font", (object) font10)
    });
    second[103] = new StyleRule((Selector) new SelectorElement(typeof (Label), (IEnumerable<string>) new string[1]
    {
      "contextMenuIconLabel"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[2]
    {
      new StyleProperty("font", (object) font2),
      new StyleProperty("alignMode", (object) (Label.AlignMode) 2)
    });
    second[104] = new StyleRule((Selector) new SelectorElement(typeof (RichTextLabel), (IEnumerable<string>) new string[1]
    {
      "hotbarSlotNumber"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("font", (object) font9)
    });
    second[105] = new StyleRule((Selector) new SelectorElement(typeof (PanelContainer), (IEnumerable<string>) new string[1]
    {
      "entity-tooltip"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("panel", (object) styleBoxTexture37)
    });
    second[106] = new StyleRule((Selector) new SelectorElement(typeof (ItemList), (IEnumerable<string>) null, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[4]
    {
      new StyleProperty("itemlist-background", (object) new StyleBoxFlat()
      {
        BackgroundColor = new Color((byte) 32 /*0x20*/, (byte) 32 /*0x20*/, (byte) 40, byte.MaxValue)
      }),
      new StyleProperty("item-background", (object) styleBoxFlat8),
      new StyleProperty("disabled-item-background", (object) styleBoxFlat7),
      new StyleProperty("selected-item-background", (object) styleBoxFlat6)
    });
    second[107] = new StyleRule((Selector) new SelectorElement(typeof (ItemList), (IEnumerable<string>) new string[1]
    {
      "transparentItemList"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[4]
    {
      new StyleProperty("itemlist-background", (object) new StyleBoxFlat()
      {
        BackgroundColor = Color.Transparent
      }),
      new StyleProperty("item-background", (object) styleBoxFlat9),
      new StyleProperty("disabled-item-background", (object) styleBoxFlat7),
      new StyleProperty("selected-item-background", (object) styleBoxFlat6)
    });
    second[108] = new StyleRule((Selector) new SelectorElement(typeof (ItemList), (IEnumerable<string>) new string[1]
    {
      "transparentBackgroundItemList"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[4]
    {
      new StyleProperty("itemlist-background", (object) new StyleBoxFlat()
      {
        BackgroundColor = Color.Transparent
      }),
      new StyleProperty("item-background", (object) styleBoxFlat8),
      new StyleProperty("disabled-item-background", (object) styleBoxFlat7),
      new StyleProperty("selected-item-background", (object) styleBoxFlat6)
    });
    SelectorElement selectorElement1 = new SelectorElement(typeof (Tree), (IEnumerable<string>) null, (string) null, (IEnumerable<string>) null);
    StyleProperty[] stylePropertyArray1 = new StyleProperty[2]
    {
      new StyleProperty("background", (object) new StyleBoxFlat()
      {
        BackgroundColor = new Color((byte) 32 /*0x20*/, (byte) 32 /*0x20*/, (byte) 40, byte.MaxValue)
      }),
      null
    };
    StyleBoxFlat styleBoxFlat10 = new StyleBoxFlat();
    styleBoxFlat10.BackgroundColor = new Color((byte) 55, (byte) 55, (byte) 68, byte.MaxValue);
    ((StyleBox) styleBoxFlat10).ContentMarginLeftOverride = new float?(4f);
    stylePropertyArray1[1] = new StyleProperty("item-selected", (object) styleBoxFlat10);
    second[109] = new StyleRule((Selector) selectorElement1, (IReadOnlyList<StyleProperty>) stylePropertyArray1);
    second[110] = new StyleRule((Selector) new SelectorElement(typeof (Placeholder), (IEnumerable<string>) null, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("panel", (object) styleBoxTexture39)
    });
    second[111] = new StyleRule((Selector) new SelectorElement(typeof (Label), (IEnumerable<string>) new string[1]
    {
      "PlaceholderText"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[2]
    {
      new StyleProperty("font", (object) font11),
      new StyleProperty("font-color", (object) new Color((byte) 103, (byte) 103, (byte) 103, (byte) 128 /*0x80*/))
    });
    second[112 /*0x70*/] = new StyleRule((Selector) new SelectorElement(typeof (Label), (IEnumerable<string>) new string[1]
    {
      "LabelHeading"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[2]
    {
      new StyleProperty("font", (object) font12),
      new StyleProperty("font-color", (object) StyleNano.NanoGold)
    });
    second[113] = new StyleRule((Selector) new SelectorElement(typeof (Label), (IEnumerable<string>) new string[1]
    {
      "LabelHeadingBigger"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[2]
    {
      new StyleProperty("font", (object) font14),
      new StyleProperty("font-color", (object) StyleNano.NanoGold)
    });
    second[114] = new StyleRule((Selector) new SelectorElement(typeof (Label), (IEnumerable<string>) new string[1]
    {
      "LabelSubText"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[2]
    {
      new StyleProperty("font", (object) font2),
      new StyleProperty("font-color", (object) Color.DarkGray)
    });
    second[115] = new StyleRule((Selector) new SelectorElement(typeof (Label), (IEnumerable<string>) new string[1]
    {
      "LabelKeyText"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[2]
    {
      new StyleProperty("font", (object) font6),
      new StyleProperty("font-color", (object) StyleNano.NanoGold)
    });
    second[116] = new StyleRule((Selector) new SelectorElement(typeof (Label), (IEnumerable<string>) new string[1]
    {
      "LabelSecondaryColor"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[2]
    {
      new StyleProperty("font", (object) font4),
      new StyleProperty("font-color", (object) Color.DarkGray)
    });
    second[117] = new StyleRule((Selector) new SelectorElement(typeof (Label), (IEnumerable<string>) new string[1]
    {
      "ConsoleText"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("font", (object) font16)
    });
    second[118] = new StyleRule((Selector) new SelectorElement(typeof (Label), (IEnumerable<string>) new string[1]
    {
      "ConsoleSubHeading"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("font", (object) font17)
    });
    second[119] = new StyleRule((Selector) new SelectorElement(typeof (Label), (IEnumerable<string>) new string[1]
    {
      "ConsoleHeading"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("font", (object) font18)
    });
    second[120] = new StyleRule((Selector) new SelectorChild((Selector) new SelectorElement(typeof (Button), (IEnumerable<string>) new string[1]
    {
      "ButtonBig"
    }, (string) null, (IEnumerable<string>) null), (Selector) new SelectorElement(typeof (Label), (IEnumerable<string>) null, (string) null, (IEnumerable<string>) null)), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("font", (object) font11)
    });
    second[121] = new StyleRule((Selector) new SelectorElement(typeof (Label), (IEnumerable<string>) new string[1]
    {
      "PowerStateNone"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("font-color", (object) new Color(0.8f, 0.0f, 0.0f, 1f))
    });
    second[122] = new StyleRule((Selector) new SelectorElement(typeof (Label), (IEnumerable<string>) new string[1]
    {
      "PowerStateLow"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("font-color", (object) new Color(0.9f, 0.36f, 0.0f, 1f))
    });
    second[123] = new StyleRule((Selector) new SelectorElement(typeof (Label), (IEnumerable<string>) new string[1]
    {
      "PowerStateGood"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("font-color", (object) new Color(0.024f, 0.8f, 0.0f, 1f))
    });
    second[124] = new StyleRule((Selector) new SelectorElement(typeof (MenuButton), (IEnumerable<string>) new string[1]
    {
      "ButtonSquare"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("stylebox", (object) styleBoxTexture21)
    });
    second[125] = new StyleRule((Selector) new SelectorElement(typeof (MenuButton), (IEnumerable<string>) new string[1]
    {
      "OpenLeft"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("stylebox", (object) styleBoxTexture20)
    });
    second[126] = new StyleRule((Selector) new SelectorElement(typeof (MenuButton), (IEnumerable<string>) new string[1]
    {
      "OpenRight"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("stylebox", (object) styleBoxTexture19)
    });
    second[(int) sbyte.MaxValue] = new StyleRule((Selector) new SelectorElement(typeof (MenuButton), (IEnumerable<string>) null, (string) null, (IEnumerable<string>) new string[1]
    {
      "normal"
    }), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("modulate-self", (object) StyleNano.ButtonColorDefault)
    });
    second[128 /*0x80*/] = new StyleRule((Selector) new SelectorElement(typeof (MenuButton), (IEnumerable<string>) new string[1]
    {
      "topButtonLabel"
    }, (string) null, (IEnumerable<string>) new string[1]
    {
      "normal"
    }), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("modulate-self", (object) StyleNano.ButtonColorDefaultRed)
    });
    second[129] = new StyleRule((Selector) new SelectorElement(typeof (MenuButton), (IEnumerable<string>) null, (string) null, (IEnumerable<string>) new string[1]
    {
      "normal"
    }), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("modulate-self", (object) StyleNano.ButtonColorDefault)
    });
    second[130] = new StyleRule((Selector) new SelectorElement(typeof (MenuButton), (IEnumerable<string>) null, (string) null, (IEnumerable<string>) new string[1]
    {
      "pressed"
    }), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("modulate-self", (object) StyleNano.ButtonColorPressed)
    });
    second[131] = new StyleRule((Selector) new SelectorElement(typeof (MenuButton), (IEnumerable<string>) null, (string) null, (IEnumerable<string>) new string[1]
    {
      "hover"
    }), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("modulate-self", (object) StyleNano.ButtonColorHovered)
    });
    second[132] = new StyleRule((Selector) new SelectorElement(typeof (MenuButton), (IEnumerable<string>) new string[1]
    {
      "topButtonLabel"
    }, (string) null, (IEnumerable<string>) new string[1]
    {
      "hover"
    }), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("modulate-self", (object) StyleNano.ButtonColorHoveredRed)
    });
    second[133] = new StyleRule((Selector) new SelectorElement(typeof (Label), (IEnumerable<string>) new string[1]
    {
      "topButtonLabel"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("font", (object) font8)
    });
    second[134] = new StyleRule((Selector) new SelectorElement(typeof (MonotoneButton), (IEnumerable<string>) null, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("stylebox", (object) styleBoxTexture29)
    });
    second[135] = new StyleRule((Selector) new SelectorElement(typeof (MonotoneButton), (IEnumerable<string>) new string[1]
    {
      "OpenLeft"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("stylebox", (object) styleBoxTexture30)
    });
    second[136] = new StyleRule((Selector) new SelectorElement(typeof (MonotoneButton), (IEnumerable<string>) new string[1]
    {
      "OpenRight"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("stylebox", (object) styleBoxTexture31)
    });
    second[137] = new StyleRule((Selector) new SelectorElement(typeof (MonotoneButton), (IEnumerable<string>) new string[1]
    {
      "OpenBoth"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("stylebox", (object) styleBoxTexture32)
    });
    second[138] = new StyleRule((Selector) new SelectorElement(typeof (MonotoneButton), (IEnumerable<string>) null, (string) null, (IEnumerable<string>) new string[1]
    {
      "pressed"
    }), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("stylebox", (object) styleBoxTexture33)
    });
    second[139] = new StyleRule((Selector) new SelectorElement(typeof (MonotoneButton), (IEnumerable<string>) new string[1]
    {
      "OpenLeft"
    }, (string) null, (IEnumerable<string>) new string[1]
    {
      "pressed"
    }), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("stylebox", (object) styleBoxTexture34)
    });
    second[140] = new StyleRule((Selector) new SelectorElement(typeof (MonotoneButton), (IEnumerable<string>) new string[1]
    {
      "OpenRight"
    }, (string) null, (IEnumerable<string>) new string[1]
    {
      "pressed"
    }), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("stylebox", (object) styleBoxTexture35)
    });
    second[141] = new StyleRule((Selector) new SelectorElement(typeof (MonotoneButton), (IEnumerable<string>) new string[1]
    {
      "OpenBoth"
    }, (string) null, (IEnumerable<string>) new string[1]
    {
      "pressed"
    }), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("stylebox", (object) styleBoxTexture36)
    });
    second[142] = new StyleRule((Selector) new SelectorChild((Selector) SelectorElement.Type(typeof (NanoHeading)), (Selector) SelectorElement.Type(typeof (PanelContainer))), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("panel", (object) styleBoxTexture43)
    });
    second[143] = new StyleRule((Selector) SelectorElement.Type(typeof (StripeBack)), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("background", (object) styleBoxTexture44)
    });
    second[144 /*0x90*/] = new StyleRule((Selector) SelectorElement.Class(new string[1]
    {
      "ItemStatus"
    }), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("font", (object) font2)
    });
    second[145] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element().Class("ItemStatusNotHeld")).Prop("font", (object) font3).Prop("font-color", (object) StyleNano.ItemStatusNotHeldColor));
    second[146] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<RichTextLabel>().Class("ItemStatus")).Prop("LineHeightScale", (object) 0.7f).Prop("Margin", (object) new Thickness(0.0f, 0.0f, 0.0f, -6f)));
    second[147] = new StyleRule((Selector) SelectorElement.Type(typeof (Slider)), (IReadOnlyList<StyleProperty>) new StyleProperty[4]
    {
      new StyleProperty("background", (object) styleBoxTexture46),
      new StyleProperty("foreground", (object) styleBoxTexture47),
      new StyleProperty("grabber", (object) styleBoxTexture48),
      new StyleProperty("fill", (object) styleBoxTexture45)
    });
    second[148] = new StyleRule((Selector) SelectorElement.Type(typeof (ColorableSlider)), (IReadOnlyList<StyleProperty>) new StyleProperty[2]
    {
      new StyleProperty("fillWhite", (object) styleBoxTexture52),
      new StyleProperty("backgroundWhite", (object) styleBoxTexture52)
    });
    second[149] = new StyleRule((Selector) new SelectorElement(typeof (Slider), (IEnumerable<string>) new string[1]
    {
      "Red"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("fill", (object) styleBoxTexture50)
    });
    second[150] = new StyleRule((Selector) new SelectorElement(typeof (Slider), (IEnumerable<string>) new string[1]
    {
      "Green"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("fill", (object) styleBoxTexture49)
    });
    second[151] = new StyleRule((Selector) new SelectorElement(typeof (Slider), (IEnumerable<string>) new string[1]
    {
      "Blue"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("fill", (object) styleBoxTexture51)
    });
    second[152] = new StyleRule((Selector) new SelectorElement(typeof (Slider), (IEnumerable<string>) new string[1]
    {
      "White"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("fill", (object) styleBoxTexture52)
    });
    second[153] = new StyleRule((Selector) new SelectorElement(typeof (Button), (IEnumerable<string>) new string[1]
    {
      "chatSelectorOptionButton"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("stylebox", (object) styleBoxTexture22)
    });
    second[154] = new StyleRule((Selector) new SelectorElement(typeof (ContainerButton), (IEnumerable<string>) new string[1]
    {
      "chatFilterOptionButton"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("stylebox", (object) styleBoxTexture23)
    });
    second[155] = new StyleRule((Selector) new SelectorElement(typeof (ContainerButton), (IEnumerable<string>) new string[1]
    {
      "chatFilterOptionButton"
    }, (string) null, (IEnumerable<string>) new string[1]
    {
      "normal"
    }), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("modulate-self", (object) StyleNano.ButtonColorDefault)
    });
    second[156] = new StyleRule((Selector) new SelectorElement(typeof (ContainerButton), (IEnumerable<string>) new string[1]
    {
      "chatFilterOptionButton"
    }, (string) null, (IEnumerable<string>) new string[1]
    {
      "hover"
    }), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("modulate-self", (object) StyleNano.ButtonColorHovered)
    });
    second[157] = new StyleRule((Selector) new SelectorElement(typeof (ContainerButton), (IEnumerable<string>) new string[1]
    {
      "chatFilterOptionButton"
    }, (string) null, (IEnumerable<string>) new string[1]
    {
      "pressed"
    }), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("modulate-self", (object) StyleNano.ButtonColorPressed)
    });
    second[158] = new StyleRule((Selector) new SelectorElement(typeof (ContainerButton), (IEnumerable<string>) new string[1]
    {
      "chatFilterOptionButton"
    }, (string) null, (IEnumerable<string>) new string[1]
    {
      "disabled"
    }), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("modulate-self", (object) StyleNano.ButtonColorDisabled)
    });
    second[159] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<Button>().Class("outputPanelScrollDownButton")).Prop("stylebox", (object) styleBoxTexture24));
    second[160 /*0xA0*/] = new StyleRule((Selector) new SelectorElement(typeof (OptionButton), (IEnumerable<string>) null, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("stylebox", (object) this.BaseButton)
    });
    second[161] = new StyleRule((Selector) new SelectorElement(typeof (OptionButton), (IEnumerable<string>) null, (string) null, (IEnumerable<string>) new string[1]
    {
      "normal"
    }), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("modulate-self", (object) StyleNano.ButtonColorDefault)
    });
    second[162] = new StyleRule((Selector) new SelectorElement(typeof (OptionButton), (IEnumerable<string>) null, (string) null, (IEnumerable<string>) new string[1]
    {
      "hover"
    }), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("modulate-self", (object) StyleNano.ButtonColorHovered)
    });
    second[163] = new StyleRule((Selector) new SelectorElement(typeof (OptionButton), (IEnumerable<string>) null, (string) null, (IEnumerable<string>) new string[1]
    {
      "pressed"
    }), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("modulate-self", (object) StyleNano.ButtonColorPressed)
    });
    second[164] = new StyleRule((Selector) new SelectorElement(typeof (OptionButton), (IEnumerable<string>) null, (string) null, (IEnumerable<string>) new string[1]
    {
      "disabled"
    }), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("modulate-self", (object) StyleNano.ButtonColorDisabled)
    });
    second[165] = new StyleRule((Selector) new SelectorElement(typeof (TextureRect), (IEnumerable<string>) new string[1]
    {
      "optionTriangle"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("texture", (object) texture17)
    });
    second[166] = new StyleRule((Selector) new SelectorElement(typeof (Label), (IEnumerable<string>) new string[1]
    {
      "optionButton"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("alignMode", (object) (Label.AlignMode) 1)
    });
    second[167] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<PanelContainer>().Class("optionButtonBackground")).Prop("panel", (object) new StyleBoxFlat(Color.FromHex((ReadOnlySpan<char>) "#25252A", new Color?()))));
    SelectorElement selectorElement2 = new SelectorElement(typeof (PanelContainer), (IEnumerable<string>) new string[1]
    {
      "HighDivider"
    }, (string) null, (IEnumerable<string>) null);
    StyleProperty[] stylePropertyArray2 = new StyleProperty[1];
    StyleBoxFlat styleBoxFlat11 = new StyleBoxFlat();
    styleBoxFlat11.BackgroundColor = StyleNano.NanoGold;
    ((StyleBox) styleBoxFlat11).ContentMarginBottomOverride = new float?(2f);
    ((StyleBox) styleBoxFlat11).ContentMarginLeftOverride = new float?(2f);
    stylePropertyArray2[0] = new StyleProperty("panel", (object) styleBoxFlat11);
    second[168] = new StyleRule((Selector) selectorElement2, (IReadOnlyList<StyleProperty>) stylePropertyArray2);
    second[169] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<TextureButton>().Class("HelpButton")).Prop("texture", (object) resCache.GetTexture("/Textures/Interface/VerbIcons/information.svg.192dpi.png")));
    second[170] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<Label>().Class("LabelBig")).Prop("font", (object) font11));
    second[171] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<Label>().Class("LabelSmall")).Prop("font", (object) font2));
    second[172] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<PanelContainer>().Class("AngleRect")).Prop("panel", (object) this.BaseAngleRect).Prop("modulate-self", (object) Color.FromHex((ReadOnlySpan<char>) "#25252A", new Color?())));
    second[173] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<PanelContainer>().Class("PubgLobbyFrame")).Prop("panel", (object) styleBoxTexture12).Prop("modulate-self", (object) Color.White));
    second[174] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<PanelContainer>().Class("BackgroundOpenRight")).Prop("panel", (object) this.BaseButtonOpenRight).Prop("modulate-self", (object) Color.FromHex((ReadOnlySpan<char>) "#25252A", new Color?())));
    second[175] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<PanelContainer>().Class("BackgroundOpenLeft")).Prop("panel", (object) this.BaseButtonOpenLeft).Prop("modulate-self", (object) Color.FromHex((ReadOnlySpan<char>) "#25252A", new Color?())));
    MutableSelectorElement mutableSelectorElement1 = StylesheetHelpers.Element<PanelContainer>().Class("LowDivider");
    StyleBoxFlat styleBoxFlat12 = new StyleBoxFlat();
    styleBoxFlat12.BackgroundColor = Color.FromHex((ReadOnlySpan<char>) "#444", new Color?());
    ((StyleBox) styleBoxFlat12).ContentMarginLeftOverride = new float?(2f);
    ((StyleBox) styleBoxFlat12).ContentMarginBottomOverride = new float?(2f);
    second[176 /*0xB0*/] = MutableSelector.op_Implicit(((MutableSelector) mutableSelectorElement1).Prop("panel", (object) styleBoxFlat12));
    second[177] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<Label>().Class("FancyWindowTitle")).Prop("font", (object) font19).Prop("font-color", (object) StyleNano.NanoGold));
    MutableSelectorElement mutableSelectorElement2 = StylesheetHelpers.Element<PanelContainer>().Class("WindowHeadingBackground");
    StyleBoxTexture styleBoxTexture55 = new StyleBoxTexture(this.BaseButtonOpenLeft);
    ((StyleBox) styleBoxTexture55).Padding = new Thickness();
    second[178] = MutableSelector.op_Implicit(((MutableSelector) mutableSelectorElement2).Prop("panel", (object) styleBoxTexture55).Prop("modulate-self", (object) Color.FromHex((ReadOnlySpan<char>) "#1F1F23", new Color?())));
    MutableSelectorElement mutableSelectorElement3 = StylesheetHelpers.Element<PanelContainer>().Class("WindowHeadingBackgroundLight");
    StyleBoxTexture styleBoxTexture56 = new StyleBoxTexture(this.BaseButtonOpenLeft);
    ((StyleBox) styleBoxTexture56).Padding = new Thickness();
    second[179] = MutableSelector.op_Implicit(((MutableSelector) mutableSelectorElement3).Prop("panel", (object) styleBoxTexture56));
    second[180] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<TextureButton>().Class("windowHelpButton")).Prop("texture", (object) resCache.GetTexture("/Textures/Interface/Nano/help.png")).Prop("modulate-self", (object) Color.FromHex((ReadOnlySpan<char>) "#4B596A", new Color?())));
    second[181] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<TextureButton>().Class("windowHelpButton").Pseudo("hover")).Prop("modulate-self", (object) Color.FromHex((ReadOnlySpan<char>) "#7F3636", new Color?())));
    second[182] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<TextureButton>().Class("windowHelpButton").Pseudo("pressed")).Prop("modulate-self", (object) Color.FromHex((ReadOnlySpan<char>) "#753131", new Color?())));
    MutableSelectorElement mutableSelectorElement4 = StylesheetHelpers.Element<PanelContainer>().Class("PanelBackgroundBaseDark");
    StyleBoxTexture styleBoxTexture57 = new StyleBoxTexture(this.BaseButtonOpenBoth);
    ((StyleBox) styleBoxTexture57).Padding = new Thickness();
    second[183] = MutableSelector.op_Implicit(((MutableSelector) mutableSelectorElement4).Prop("panel", (object) styleBoxTexture57).Prop("modulate-self", (object) Color.FromHex((ReadOnlySpan<char>) "#1F1F23", new Color?())));
    MutableSelectorElement mutableSelectorElement5 = StylesheetHelpers.Element<PanelContainer>().Class("PanelBackgroundLight");
    StyleBoxTexture styleBoxTexture58 = new StyleBoxTexture(this.BaseButtonOpenBoth);
    ((StyleBox) styleBoxTexture58).Padding = new Thickness();
    second[184] = MutableSelector.op_Implicit(((MutableSelector) mutableSelectorElement5).Prop("panel", (object) styleBoxTexture58).Prop("modulate-self", (object) Color.FromHex((ReadOnlySpan<char>) "#2F2F3B", new Color?())));
    second[185] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<TextureRect>().Class("NTLogoDark")).Prop("texture", (object) resCache.GetTexture("/Textures/Interface/Nano/ntlogo.svg.png")).Prop("modulate-self", (object) Color.FromHex((ReadOnlySpan<char>) "#757575", new Color?())));
    second[186] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<Label>().Class("WindowFooterText")).Prop("font", (object) font1).Prop("font-color", (object) Color.FromHex((ReadOnlySpan<char>) "#757575", new Color?())));
    second[187] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<TextureButton>().Class("CrossButtonRed")).Prop("texture", (object) resCache.GetTexture("/Textures/Interface/Nano/cross.svg.png")).Prop("modulate-self", (object) StyleNano.DangerousRedFore));
    second[188] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<TextureButton>().Class("CrossButtonRed").Pseudo("hover")).Prop("modulate-self", (object) Color.FromHex((ReadOnlySpan<char>) "#7F3636", new Color?())));
    second[189] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<TextureButton>().Class("CrossButtonRed").Pseudo("hover")).Prop("modulate-self", (object) Color.FromHex((ReadOnlySpan<char>) "#753131", new Color?())));
    second[190] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<TextureButton>().Class("Refresh")).Prop("texture", (object) resCache.GetTexture("/Textures/Interface/Nano/circular_arrow.svg.96dpi.png")));
    second[191] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<TextureButton>().Class("SpeciesInfoDefault")).Prop("texture", (object) resCache.GetTexture("/Textures/Interface/VerbIcons/information.svg.192dpi.png")));
    second[192 /*0xC0*/] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<TextureButton>().Class("SpeciesInfoWarning")).Prop("texture", (object) resCache.GetTexture("/Textures/Interface/info.svg.192dpi.png")).Prop("modulate-self", (object) Color.FromHex((ReadOnlySpan<char>) "#eeee11", new Color?())));
    second[193] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<PanelContainer>().Class("PaperDefaultBorder")).Prop("panel", (object) styleBoxTexture54));
    second[194] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<RichTextLabel>().Class("PaperWrittenText")).Prop("font", (object) font4).Prop("modulate-self", (object) Color.FromHex((ReadOnlySpan<char>) "#111111", new Color?())));
    second[195] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<RichTextLabel>().Class("LabelSubText")).Prop("font", (object) font2).Prop("font-color", (object) Color.DarkGray));
    second[196] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<LineEdit>().Class("PaperLineEdit")).Prop("stylebox", (object) new StyleBoxEmpty()));
    second[197] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<Button>().Class("ButtonColorRed")).Prop("modulate-self", (object) StyleNano.ButtonColorDefaultRed));
    second[198] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<Button>().Class("ButtonColorRed").Pseudo("normal")).Prop("modulate-self", (object) StyleNano.ButtonColorDefaultRed));
    second[199] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<Button>().Class("ButtonColorRed").Pseudo("hover")).Prop("modulate-self", (object) StyleNano.ButtonColorHoveredRed));
    second[200] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<Button>().Class("ButtonColorGreen")).Prop("modulate-self", (object) StyleNano.ButtonColorGoodDefault));
    second[201] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<Button>().Class("ButtonColorGreen").Pseudo("normal")).Prop("modulate-self", (object) StyleNano.ButtonColorGoodDefault));
    second[202] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<Button>().Class("ButtonColorGreen").Pseudo("hover")).Prop("modulate-self", (object) StyleNano.ButtonColorGoodHovered));
    second[203] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<Button>().Class("ButtonAccept")).Prop("modulate-self", (object) StyleNano.ButtonColorGoodDefault));
    second[204] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<Button>().Class("ButtonAccept").Pseudo("normal")).Prop("modulate-self", (object) StyleNano.ButtonColorGoodDefault));
    second[205] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<Button>().Class("ButtonAccept").Pseudo("hover")).Prop("modulate-self", (object) StyleNano.ButtonColorGoodHovered));
    second[206] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<Button>().Class("ButtonAccept").Pseudo("disabled")).Prop("modulate-self", (object) StyleNano.ButtonColorGoodDisabled));
    second[207] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<Button>().Class("ButtonSmall")).Prop("stylebox", (object) styleBoxTexture25));
    second[208 /*0xD0*/] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Child().Parent(MutableSelector.op_Implicit((MutableSelector) StylesheetHelpers.Element<Button>().Class("ButtonSmall"))).Child(MutableSelector.op_Implicit((MutableSelector) StylesheetHelpers.Element<Label>()))).Prop("font", (object) font1));
    second[209] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<Label>().Class("StatusFieldTitle")).Prop("font-color", (object) StyleNano.NanoGold));
    second[210] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<Label>().Class("Good")).Prop("font-color", (object) StyleNano.GoodGreenFore));
    second[211] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<Label>().Class("Caution")).Prop("font-color", (object) StyleNano.ConcerningOrangeFore));
    second[212] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<Label>().Class("Danger")).Prop("font-color", (object) StyleNano.DangerousRedFore));
    second[213] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<Label>().Class("Disabled")).Prop("font-color", (object) StyleNano.DisabledFore));
    second[214] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<TextureButton>().Class("RadialMenuButton")).Prop("texture", (object) resCache.GetTexture("/Textures/Interface/Radial/button_normal.png")));
    second[215] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<TextureButton>().Class("RadialMenuButton").Pseudo("hover")).Prop("texture", (object) resCache.GetTexture("/Textures/Interface/Radial/button_hover.png")));
    second[216] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<TextureButton>().Class("RadialMenuCloseButton")).Prop("texture", (object) resCache.GetTexture("/Textures/Interface/Radial/close_normal.png")));
    second[217] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<TextureButton>().Class("RadialMenuCloseButton").Pseudo("hover")).Prop("texture", (object) resCache.GetTexture("/Textures/Interface/Radial/close_hover.png")));
    second[218] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<TextureButton>().Class("RadialMenuBackButton")).Prop("texture", (object) resCache.GetTexture("/Textures/Interface/Radial/back_normal.png")));
    second[219] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<TextureButton>().Class("RadialMenuBackButton").Pseudo("hover")).Prop("texture", (object) resCache.GetTexture("/Textures/Interface/Radial/back_hover.png")));
    second[220] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<PanelContainer>().Class("PdaContentBackground")).Prop("panel", (object) this.BaseButtonOpenBoth).Prop("modulate-self", (object) Color.FromHex((ReadOnlySpan<char>) "#25252a", new Color?())));
    second[221] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<PanelContainer>().Class("PdaBackground")).Prop("panel", (object) this.BaseButtonOpenBoth).Prop("modulate-self", (object) Color.FromHex((ReadOnlySpan<char>) "#000000", new Color?())));
    second[222] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<PanelContainer>().Class("PdaBackgroundRect")).Prop("panel", (object) this.BaseAngleRect).Prop("modulate-self", (object) Color.FromHex((ReadOnlySpan<char>) "#717059", new Color?())));
    second[223] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<PanelContainer>().Class("PdaBorderRect")).Prop("panel", (object) this.AngleBorderRect));
    second[224 /*0xE0*/] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<PanelContainer>().Class("BackgroundDark")).Prop("panel", (object) new StyleBoxFlat(Color.FromHex((ReadOnlySpan<char>) "#25252A", new Color?()))));
    second[225] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<PdaSettingsButton>().Pseudo("normal")).Prop("backgroundColor", (object) Color.FromHex((ReadOnlySpan<char>) "#313138", new Color?())).Prop("foregroundColor", (object) Color.FromHex((ReadOnlySpan<char>) "#FFFFFF", new Color?())));
    second[226] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<PdaSettingsButton>().Pseudo("hover")).Prop("backgroundColor", (object) Color.FromHex((ReadOnlySpan<char>) "#3E6C45", new Color?())).Prop("foregroundColor", (object) Color.FromHex((ReadOnlySpan<char>) "#FFFFFF", new Color?())));
    second[227] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<PdaSettingsButton>().Pseudo("pressed")).Prop("backgroundColor", (object) Color.FromHex((ReadOnlySpan<char>) "#3E6C45", new Color?())).Prop("foregroundColor", (object) Color.FromHex((ReadOnlySpan<char>) "#FFFFFF", new Color?())));
    second[228] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<PdaSettingsButton>().Pseudo("disabled")).Prop("backgroundColor", (object) Color.FromHex((ReadOnlySpan<char>) "#313138", new Color?())).Prop("foregroundColor", (object) Color.FromHex((ReadOnlySpan<char>) "#5a5a5a", new Color?())));
    second[229] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<PdaProgramItem>().Pseudo("normal")).Prop("backgroundColor", (object) Color.FromHex((ReadOnlySpan<char>) "#313138", new Color?())));
    second[230] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<PdaProgramItem>().Pseudo("hover")).Prop("backgroundColor", (object) Color.FromHex((ReadOnlySpan<char>) "#3E6C45", new Color?())));
    second[231] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<PdaProgramItem>().Pseudo("pressed")).Prop("backgroundColor", (object) Color.FromHex((ReadOnlySpan<char>) "#3E6C45", new Color?())));
    second[232] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<Label>().Class("PdaContentFooterText")).Prop("font", (object) font2).Prop("font-color", (object) Color.FromHex((ReadOnlySpan<char>) "#757575", new Color?())));
    second[233] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<Label>().Class("PdaWindowFooterText")).Prop("font", (object) font2).Prop("font-color", (object) Color.FromHex((ReadOnlySpan<char>) "#333d3b", new Color?())));
    second[234] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<ContainerButton>().Identifier("tree-button").Class("even-row")).Prop("stylebox", (object) new StyleBoxFlat()
    {
      BackgroundColor = StyleNano.FancyTreeEvenRowColor
    }));
    second[235] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<ContainerButton>().Identifier("tree-button").Class("odd-row")).Prop("stylebox", (object) new StyleBoxFlat()
    {
      BackgroundColor = StyleNano.FancyTreeOddRowColor
    }));
    second[236] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<ContainerButton>().Identifier("tree-button").Class("selected")).Prop("stylebox", (object) new StyleBoxFlat()
    {
      BackgroundColor = StyleNano.FancyTreeSelectedRowColor
    }));
    second[237] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<ContainerButton>().Identifier("tree-button").Pseudo("hover")).Prop("stylebox", (object) new StyleBoxFlat()
    {
      BackgroundColor = StyleNano.FancyTreeSelectedRowColor
    }));
    second[238] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<Label>().Class("SiliconLawPositionLabel")).Prop("font-color", (object) StyleNano.NanoGold));
    second[239] = new StyleRule((Selector) new SelectorElement(typeof (TextureButton), (IEnumerable<string>) new string[1]
    {
      "pinButtonPinned"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("texture", (object) resCache.GetTexture("/Textures/Interface/Bwoink/pinned.png"))
    });
    second[240 /*0xF0*/] = new StyleRule((Selector) new SelectorElement(typeof (TextureButton), (IEnumerable<string>) new string[1]
    {
      "pinButtonUnpinned"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("texture", (object) resCache.GetTexture("/Textures/Interface/Bwoink/un_pinned.png"))
    });
    second[241] = new StyleRule((Selector) new SelectorElement((Type) null, (IEnumerable<string>) new string[1]
    {
      "LobbyBackground"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("panel", (object) styleBoxTexture11)
    });
    second[242] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<PanelContainer>().Class("Inset")).Prop("panel", (object) styleBoxTexture53));
    second[243] = new StyleRule((Selector) new SelectorElement(typeof (Label), (IEnumerable<string>) new string[1]
    {
      "CMAlignLeft"
    }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
    {
      new StyleProperty("alignMode", (object) (Label.AlignMode) 0)
    });
    this.Stylesheet = new Stylesheet((IReadOnlyList<StyleRule>) ((IEnumerable<StyleRule>) baseRules).Concat<StyleRule>((IEnumerable<StyleRule>) second).ToList<StyleRule>());
  }

  static StyleNano()
  {
    ref readonly Color local1 = ref StyleNano.FancyTreeEvenRowColor;
    Color color = new Color(0.8f, 0.8f, 0.8f, 1f);
    ref Color local2 = ref color;
    StyleNano.FancyTreeOddRowColor = Color.op_Multiply(ref local1, ref local2);
    StyleNano.FancyTreeSelectedRowColor = new Color((byte) 55, (byte) 55, (byte) 68, byte.MaxValue);
    StyleNano.ItemStatusNotHeldColor = Color.Gray;
    StyleNano.ChatBackgroundColor = Color.FromHex((ReadOnlySpan<char>) "#131313", new Color?());
  }
}
