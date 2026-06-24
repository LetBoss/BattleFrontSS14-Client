// Decompiled with JetBrains decompiler
// Type: Content.Client.Stylesheets.StyleBase
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
using System.Numerics;

#nullable enable
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
    Font font1 = resCache.GetFont(new string[3]
    {
      "/Fonts/NotoSans/NotoSans-Regular.ttf",
      "/Fonts/NotoSans/NotoSansSymbols-Regular.ttf",
      "/Fonts/NotoSans/NotoSansSymbols2-Regular.ttf"
    }, 12);
    Font font2 = resCache.GetFont(new string[3]
    {
      "/Fonts/NotoSans/NotoSans-Italic.ttf",
      "/Fonts/NotoSans/NotoSansSymbols-Regular.ttf",
      "/Fonts/NotoSans/NotoSansSymbols2-Regular.ttf"
    }, 12);
    Texture texture1 = resCache.GetTexture("/Textures/Interface/Nano/cross.svg.png");
    Texture texture2 = resCache.GetTexture("/Textures/Interface/Nano/button.svg.96dpi.png");
    this.BaseButton = new StyleBoxTexture()
    {
      Texture = texture2
    };
    this.BaseButton.SetPatchMargin((StyleBox.Margin) 15, 10f);
    ((StyleBox) this.BaseButton).SetPadding((StyleBox.Margin) 15, 1f);
    ((StyleBox) this.BaseButton).SetContentMarginOverride((StyleBox.Margin) 3, 2f);
    ((StyleBox) this.BaseButton).SetContentMarginOverride((StyleBox.Margin) 12, 14f);
    this.BaseButtonOpenRight = new StyleBoxTexture(this.BaseButton)
    {
      Texture = (Texture) new AtlasTexture(texture2, UIBox2.FromDimensions(new Vector2(0.0f, 0.0f), new Vector2(14f, 24f)))
    };
    this.BaseButtonOpenRight.SetPatchMargin((StyleBox.Margin) 4, 0.0f);
    ((StyleBox) this.BaseButtonOpenRight).SetContentMarginOverride((StyleBox.Margin) 4, 8f);
    ((StyleBox) this.BaseButtonOpenRight).SetPadding((StyleBox.Margin) 4, 2f);
    this.BaseButtonOpenLeft = new StyleBoxTexture(this.BaseButton)
    {
      Texture = (Texture) new AtlasTexture(texture2, UIBox2.FromDimensions(new Vector2(10f, 0.0f), new Vector2(14f, 24f)))
    };
    this.BaseButtonOpenLeft.SetPatchMargin((StyleBox.Margin) 8, 0.0f);
    ((StyleBox) this.BaseButtonOpenLeft).SetContentMarginOverride((StyleBox.Margin) 8, 8f);
    ((StyleBox) this.BaseButtonOpenLeft).SetPadding((StyleBox.Margin) 8, 1f);
    this.BaseButtonOpenBoth = new StyleBoxTexture(this.BaseButton)
    {
      Texture = (Texture) new AtlasTexture(texture2, UIBox2.FromDimensions(new Vector2(10f, 0.0f), new Vector2(3f, 24f)))
    };
    this.BaseButtonOpenBoth.SetPatchMargin((StyleBox.Margin) 12, 0.0f);
    ((StyleBox) this.BaseButtonOpenBoth).SetContentMarginOverride((StyleBox.Margin) 12, 8f);
    ((StyleBox) this.BaseButtonOpenBoth).SetPadding((StyleBox.Margin) 4, 2f);
    ((StyleBox) this.BaseButtonOpenBoth).SetPadding((StyleBox.Margin) 8, 1f);
    this.BaseButtonSquare = new StyleBoxTexture(this.BaseButton)
    {
      Texture = (Texture) new AtlasTexture(texture2, UIBox2.FromDimensions(new Vector2(10f, 0.0f), new Vector2(3f, 24f)))
    };
    this.BaseButtonSquare.SetPatchMargin((StyleBox.Margin) 12, 0.0f);
    ((StyleBox) this.BaseButtonSquare).SetContentMarginOverride((StyleBox.Margin) 12, 8f);
    ((StyleBox) this.BaseButtonSquare).SetPadding((StyleBox.Margin) 4, 2f);
    ((StyleBox) this.BaseButtonSquare).SetPadding((StyleBox.Margin) 8, 1f);
    this.BaseAngleRect = new StyleBoxTexture()
    {
      Texture = texture2
    };
    this.BaseAngleRect.SetPatchMargin((StyleBox.Margin) 15, 10f);
    this.AngleBorderRect = new StyleBoxTexture()
    {
      Texture = resCache.GetTexture("/Textures/Interface/Nano/geometric_panel_border.svg.96dpi.png")
    };
    this.AngleBorderRect.SetPatchMargin((StyleBox.Margin) 15, 10f);
    StyleBoxFlat styleBoxFlat1 = new StyleBoxFlat();
    Color gray1 = Color.Gray;
    styleBoxFlat1.BackgroundColor = ((Color) ref gray1).WithAlpha(0.35f);
    ((StyleBox) styleBoxFlat1).ContentMarginLeftOverride = new float?(10f);
    ((StyleBox) styleBoxFlat1).ContentMarginTopOverride = new float?(10f);
    StyleBoxFlat styleBoxFlat2 = styleBoxFlat1;
    StyleBoxFlat styleBoxFlat3 = new StyleBoxFlat();
    Color color1 = new Color((byte) 140, (byte) 140, (byte) 140, byte.MaxValue);
    styleBoxFlat3.BackgroundColor = ((Color) ref color1).WithAlpha(0.35f);
    ((StyleBox) styleBoxFlat3).ContentMarginLeftOverride = new float?(10f);
    ((StyleBox) styleBoxFlat3).ContentMarginTopOverride = new float?(10f);
    StyleBoxFlat styleBoxFlat4 = styleBoxFlat3;
    StyleBoxFlat styleBoxFlat5 = new StyleBoxFlat();
    Color color2 = new Color((byte) 160 /*0xA0*/, (byte) 160 /*0xA0*/, (byte) 160 /*0xA0*/, byte.MaxValue);
    styleBoxFlat5.BackgroundColor = ((Color) ref color2).WithAlpha(0.35f);
    ((StyleBox) styleBoxFlat5).ContentMarginLeftOverride = new float?(10f);
    ((StyleBox) styleBoxFlat5).ContentMarginTopOverride = new float?(10f);
    StyleBoxFlat styleBoxFlat6 = styleBoxFlat5;
    StyleBoxFlat styleBoxFlat7 = new StyleBoxFlat();
    Color gray2 = Color.Gray;
    styleBoxFlat7.BackgroundColor = ((Color) ref gray2).WithAlpha(0.35f);
    ((StyleBox) styleBoxFlat7).ContentMarginTopOverride = new float?(10f);
    StyleBoxFlat styleBoxFlat8 = styleBoxFlat7;
    StyleBoxFlat styleBoxFlat9 = new StyleBoxFlat();
    Color color3 = new Color((byte) 140, (byte) 140, (byte) 140, byte.MaxValue);
    styleBoxFlat9.BackgroundColor = ((Color) ref color3).WithAlpha(0.35f);
    ((StyleBox) styleBoxFlat9).ContentMarginTopOverride = new float?(10f);
    StyleBoxFlat styleBoxFlat10 = styleBoxFlat9;
    StyleBoxFlat styleBoxFlat11 = new StyleBoxFlat();
    Color color4 = new Color((byte) 160 /*0xA0*/, (byte) 160 /*0xA0*/, (byte) 160 /*0xA0*/, byte.MaxValue);
    styleBoxFlat11.BackgroundColor = ((Color) ref color4).WithAlpha(0.35f);
    ((StyleBox) styleBoxFlat11).ContentMarginTopOverride = new float?(10f);
    StyleBoxFlat styleBoxFlat12 = styleBoxFlat11;
    this.BaseRules = new StyleRule[11]
    {
      new StyleRule((Selector) new SelectorElement((Type) null, (IEnumerable<string>) null, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
      {
        new StyleProperty("font", (object) font1)
      }),
      new StyleRule((Selector) new SelectorElement((Type) null, (IEnumerable<string>) new string[1]
      {
        "Italic"
      }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
      {
        new StyleProperty("font", (object) font2)
      }),
      new StyleRule((Selector) new SelectorElement(typeof (TextureButton), (IEnumerable<string>) new string[1]
      {
        "windowCloseButton"
      }, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[2]
      {
        new StyleProperty("texture", (object) texture1),
        new StyleProperty("modulate-self", (object) Color.FromHex((ReadOnlySpan<char>) "#4B596A", new Color?()))
      }),
      new StyleRule((Selector) new SelectorElement(typeof (TextureButton), (IEnumerable<string>) new string[1]
      {
        "windowCloseButton"
      }, (string) null, (IEnumerable<string>) new string[1]
      {
        "hover"
      }), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
      {
        new StyleProperty("modulate-self", (object) Color.FromHex((ReadOnlySpan<char>) "#7F3636", new Color?()))
      }),
      new StyleRule((Selector) new SelectorElement(typeof (TextureButton), (IEnumerable<string>) new string[1]
      {
        "windowCloseButton"
      }, (string) null, (IEnumerable<string>) new string[1]
      {
        "pressed"
      }), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
      {
        new StyleProperty("modulate-self", (object) Color.FromHex((ReadOnlySpan<char>) "#753131", new Color?()))
      }),
      new StyleRule((Selector) new SelectorElement(typeof (VScrollBar), (IEnumerable<string>) null, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
      {
        new StyleProperty("grabber", (object) styleBoxFlat2)
      }),
      new StyleRule((Selector) new SelectorElement(typeof (VScrollBar), (IEnumerable<string>) null, (string) null, (IEnumerable<string>) new string[1]
      {
        "hover"
      }), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
      {
        new StyleProperty("grabber", (object) styleBoxFlat4)
      }),
      new StyleRule((Selector) new SelectorElement(typeof (VScrollBar), (IEnumerable<string>) null, (string) null, (IEnumerable<string>) new string[1]
      {
        "grabbed"
      }), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
      {
        new StyleProperty("grabber", (object) styleBoxFlat6)
      }),
      new StyleRule((Selector) new SelectorElement(typeof (HScrollBar), (IEnumerable<string>) null, (string) null, (IEnumerable<string>) null), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
      {
        new StyleProperty("grabber", (object) styleBoxFlat8)
      }),
      new StyleRule((Selector) new SelectorElement(typeof (HScrollBar), (IEnumerable<string>) null, (string) null, (IEnumerable<string>) new string[1]
      {
        "hover"
      }), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
      {
        new StyleProperty("grabber", (object) styleBoxFlat10)
      }),
      new StyleRule((Selector) new SelectorElement(typeof (HScrollBar), (IEnumerable<string>) null, (string) null, (IEnumerable<string>) new string[1]
      {
        "grabbed"
      }), (IReadOnlyList<StyleProperty>) new StyleProperty[1]
      {
        new StyleProperty("grabber", (object) styleBoxFlat12)
      })
    };
  }
}
