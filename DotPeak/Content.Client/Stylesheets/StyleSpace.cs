// Decompiled with JetBrains decompiler
// Type: Content.Client.Stylesheets.StyleSpace
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Resources;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Client.Stylesheets;

public sealed class StyleSpace : StyleBase
{
  public static readonly Color SpaceRed = Color.FromHex((ReadOnlySpan<char>) "#9b2236", new Color?());
  public static readonly Color ButtonColorDefault = Color.FromHex((ReadOnlySpan<char>) "#464966", new Color?());
  public static readonly Color ButtonColorHovered = Color.FromHex((ReadOnlySpan<char>) "#575b7f", new Color?());
  public static readonly Color ButtonColorPressed = Color.FromHex((ReadOnlySpan<char>) "#3e6c45", new Color?());
  public static readonly Color ButtonColorDisabled = Color.FromHex((ReadOnlySpan<char>) "#30313c", new Color?());
  public static readonly Color ButtonColorCautionDefault = Color.FromHex((ReadOnlySpan<char>) "#ab3232", new Color?());
  public static readonly Color ButtonColorCautionHovered = Color.FromHex((ReadOnlySpan<char>) "#cf2f2f", new Color?());
  public static readonly Color ButtonColorCautionPressed = Color.FromHex((ReadOnlySpan<char>) "#3e6c45", new Color?());
  public static readonly Color ButtonColorCautionDisabled = Color.FromHex((ReadOnlySpan<char>) "#602a2a", new Color?());

  public override Stylesheet Stylesheet { get; }

  public StyleSpace(IResourceCache resCache)
    : base(resCache)
  {
    Font font1 = resCache.GetFont(new string[3]
    {
      "/Fonts/NotoSans/NotoSans-Regular.ttf",
      "/Fonts/NotoSans/NotoSansSymbols-Regular.ttf",
      "/Fonts/NotoSans/NotoSansSymbols2-Regular.ttf"
    }, 10);
    Font font2 = resCache.GetFont(new string[3]
    {
      "/Fonts/NotoSans/NotoSans-Bold.ttf",
      "/Fonts/NotoSans/NotoSansSymbols-Regular.ttf",
      "/Fonts/NotoSans/NotoSansSymbols2-Regular.ttf"
    }, 16 /*0x10*/);
    StyleBoxFlat styleBoxFlat1 = new StyleBoxFlat()
    {
      BackgroundColor = new Color(0.25f, 0.25f, 0.25f, 1f)
    };
    ((StyleBox) styleBoxFlat1).SetContentMarginOverride((StyleBox.Margin) 3, 14.5f);
    StyleBoxFlat styleBoxFlat2 = new StyleBoxFlat()
    {
      BackgroundColor = new Color(0.25f, 0.5f, 0.25f, 1f)
    };
    ((StyleBox) styleBoxFlat2).SetContentMarginOverride((StyleBox.Margin) 3, 14.5f);
    Texture texture = resCache.GetTexture("/Textures/Interface/Nano/inverted_triangle.svg.png");
    StyleBoxTexture styleBoxTexture = new StyleBoxTexture();
    styleBoxTexture.SetPatchMargin((StyleBox.Margin) 15, 2f);
    StyleBoxFlat styleBoxFlat3 = new StyleBoxFlat()
    {
      BackgroundColor = new Color((byte) 64 /*0x40*/, (byte) 64 /*0x40*/, (byte) 64 /*0x40*/, byte.MaxValue)
    };
    ((StyleBox) styleBoxFlat3).SetContentMarginOverride((StyleBox.Margin) 12, 5f);
    StyleBoxFlat styleBoxFlat4 = new StyleBoxFlat()
    {
      BackgroundColor = new Color((byte) 32 /*0x20*/, (byte) 32 /*0x20*/, (byte) 32 /*0x20*/, byte.MaxValue)
    };
    ((StyleBox) styleBoxFlat4).SetContentMarginOverride((StyleBox.Margin) 12, 5f);
    StyleRule[] baseRules = this.BaseRules;
    StyleRule[] second = new StyleRule[29];
    second[0] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<Label>().Class("LabelHeading")).Prop("font", (object) font2).Prop("font-color", (object) StyleSpace.SpaceRed));
    second[1] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<Label>().Class("LabelSubText")).Prop("font", (object) font1).Prop("font-color", (object) Color.DarkGray));
    MutableSelectorElement mutableSelectorElement1 = StylesheetHelpers.Element<PanelContainer>().Class("HighDivider");
    StyleBoxFlat styleBoxFlat5 = new StyleBoxFlat();
    styleBoxFlat5.BackgroundColor = StyleSpace.SpaceRed;
    ((StyleBox) styleBoxFlat5).ContentMarginBottomOverride = new float?(2f);
    ((StyleBox) styleBoxFlat5).ContentMarginLeftOverride = new float?(2f);
    second[2] = MutableSelector.op_Implicit(((MutableSelector) mutableSelectorElement1).Prop("panel", (object) styleBoxFlat5));
    MutableSelectorElement mutableSelectorElement2 = StylesheetHelpers.Element<PanelContainer>().Class("LowDivider");
    StyleBoxFlat styleBoxFlat6 = new StyleBoxFlat();
    styleBoxFlat6.BackgroundColor = Color.FromHex((ReadOnlySpan<char>) "#444", new Color?());
    ((StyleBox) styleBoxFlat6).ContentMarginLeftOverride = new float?(2f);
    ((StyleBox) styleBoxFlat6).ContentMarginBottomOverride = new float?(2f);
    second[3] = MutableSelector.op_Implicit(((MutableSelector) mutableSelectorElement2).Prop("panel", (object) styleBoxFlat6));
    second[4] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<ContainerButton>().Class("button")).Prop("stylebox", (object) this.BaseButton));
    second[5] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<ContainerButton>().Class("button").Class("OpenRight")).Prop("stylebox", (object) this.BaseButtonOpenRight));
    second[6] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<ContainerButton>().Class("button").Class("OpenLeft")).Prop("stylebox", (object) this.BaseButtonOpenLeft));
    second[7] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<ContainerButton>().Class("button").Class("OpenBoth")).Prop("stylebox", (object) this.BaseButtonOpenBoth));
    second[8] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<ContainerButton>().Class("button").Class("ButtonSquare")).Prop("stylebox", (object) this.BaseButtonSquare));
    second[9] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<ContainerButton>().Class("button").Pseudo("normal")).Prop("modulate-self", (object) StyleSpace.ButtonColorDefault));
    second[10] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<ContainerButton>().Class("button").Pseudo("hover")).Prop("modulate-self", (object) StyleSpace.ButtonColorHovered));
    second[11] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<ContainerButton>().Class("button").Pseudo("pressed")).Prop("modulate-self", (object) StyleSpace.ButtonColorPressed));
    second[12] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<ContainerButton>().Class("button").Pseudo("disabled")).Prop("modulate-self", (object) StyleSpace.ButtonColorDisabled));
    second[13] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<ContainerButton>().Class("button").Class("Caution").Pseudo("normal")).Prop("modulate-self", (object) StyleSpace.ButtonColorCautionDefault));
    second[14] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<ContainerButton>().Class("button").Class("Caution").Pseudo("hover")).Prop("modulate-self", (object) StyleSpace.ButtonColorCautionHovered));
    second[15] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<ContainerButton>().Class("button").Class("Caution").Pseudo("pressed")).Prop("modulate-self", (object) StyleSpace.ButtonColorCautionPressed));
    second[16 /*0x10*/] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<ContainerButton>().Class("button").Class("Caution").Pseudo("disabled")).Prop("modulate-self", (object) StyleSpace.ButtonColorCautionDisabled));
    second[17] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<Label>().Class("button")).Prop("alignMode", (object) (Label.AlignMode) 1));
    second[18] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<PanelContainer>().Class("AngleRect")).Prop("panel", (object) this.BaseAngleRect).Prop("modulate-self", (object) Color.FromHex((ReadOnlySpan<char>) "#202030", new Color?())));
    second[19] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Child().Parent(MutableSelector.op_Implicit((MutableSelector) StylesheetHelpers.Element<Button>().Class("disabled"))).Child(MutableSelector.op_Implicit((MutableSelector) StylesheetHelpers.Element<Label>()))).Prop("font-color", (object) Color.FromHex((ReadOnlySpan<char>) "#E5E5E581", new Color?())));
    second[20] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<ProgressBar>()).Prop("background", (object) styleBoxFlat1).Prop("foreground", (object) styleBoxFlat2));
    second[21] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<OptionButton>()).Prop("stylebox", (object) this.BaseButton));
    second[22] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<OptionButton>().Pseudo("normal")).Prop("modulate-self", (object) StyleSpace.ButtonColorDefault));
    second[23] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<OptionButton>().Pseudo("hover")).Prop("modulate-self", (object) StyleSpace.ButtonColorHovered));
    second[24] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<OptionButton>().Pseudo("pressed")).Prop("modulate-self", (object) StyleSpace.ButtonColorPressed));
    second[25] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<OptionButton>().Pseudo("disabled")).Prop("modulate-self", (object) StyleSpace.ButtonColorDisabled));
    second[26] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<TextureRect>().Class("optionTriangle")).Prop("texture", (object) texture));
    second[27] = MutableSelector.op_Implicit(((MutableSelector) StylesheetHelpers.Element<Label>().Class("optionButton")).Prop("alignMode", (object) (Label.AlignMode) 1));
    second[28] = new StyleRule((Selector) new SelectorElement(typeof (TabContainer), (IEnumerable<string>) null, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[3]
    {
      new StyleProperty("panel-stylebox", (object) styleBoxTexture),
      new StyleProperty("tab-stylebox", (object) styleBoxFlat3),
      new StyleProperty("tab-stylebox-inactive", (object) styleBoxFlat4)
    });
    this.Stylesheet = new Stylesheet((IReadOnlyList<StyleRule>) ((IEnumerable<StyleRule>) baseRules).Concat<StyleRule>((IEnumerable<StyleRule>) second).ToList<StyleRule>());
  }
}
