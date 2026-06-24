// Decompiled with JetBrains decompiler
// Type: Content.Shared.Verbs.VerbCategory
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;
using System;

#nullable enable
namespace Content.Shared.Verbs;

[NetSerializable]
[Serializable]
public sealed class VerbCategory
{
  public readonly string Text;
  public readonly SpriteSpecifier? Icon;
  public int Columns = 1;
  public readonly bool IconsOnly;
  public static readonly VerbCategory Admin = new VerbCategory("verb-categories-admin", "/Textures/Interface/character.svg.192dpi.png");
  public static readonly VerbCategory Antag = new VerbCategory("verb-categories-antag", "/Textures/Interface/VerbIcons/antag-e_sword-temp.192dpi.png", true)
  {
    Columns = 5
  };
  public static readonly VerbCategory Examine = new VerbCategory("verb-categories-examine", "/Textures/Interface/VerbIcons/examine.svg.192dpi.png");
  public static readonly VerbCategory Debug = new VerbCategory("verb-categories-debug", "/Textures/Interface/VerbIcons/debug.svg.192dpi.png");
  public static readonly VerbCategory Eject = new VerbCategory("verb-categories-eject", "/Textures/Interface/VerbIcons/eject.svg.192dpi.png");
  public static readonly VerbCategory Insert = new VerbCategory("verb-categories-insert", "/Textures/Interface/VerbIcons/insert.svg.192dpi.png");
  public static readonly VerbCategory Buckle = new VerbCategory("verb-categories-buckle", "/Textures/Interface/VerbIcons/buckle.svg.192dpi.png");
  public static readonly VerbCategory Unbuckle = new VerbCategory("verb-categories-unbuckle", "/Textures/Interface/VerbIcons/unbuckle.svg.192dpi.png");
  public static readonly VerbCategory Rotate = new VerbCategory("verb-categories-rotate", "/Textures/Interface/VerbIcons/refresh.svg.192dpi.png", true)
  {
    Columns = 5
  };
  public static readonly VerbCategory Smite = new VerbCategory("verb-categories-smite", "/Textures/Interface/VerbIcons/smite.svg.192dpi.png", true)
  {
    Columns = 6
  };
  public static readonly VerbCategory Tricks = new VerbCategory("verb-categories-tricks", "/Textures/Interface/AdminActions/tricks.png", true)
  {
    Columns = 5
  };
  public static readonly VerbCategory SetTransferAmount = new VerbCategory("verb-categories-transfer", "/Textures/Interface/VerbIcons/spill.svg.192dpi.png");
  public static readonly VerbCategory Split = new VerbCategory("verb-categories-split", (string) null);
  public static readonly VerbCategory InstrumentStyle = new VerbCategory("verb-categories-instrument-style", (string) null);
  public static readonly VerbCategory ChannelSelect = new VerbCategory("verb-categories-channel-select", (string) null);
  public static readonly VerbCategory SetSensor = new VerbCategory("verb-categories-set-sensor", (string) null);
  public static readonly VerbCategory Lever = new VerbCategory("verb-categories-lever", (string) null);
  public static readonly VerbCategory SelectType = new VerbCategory("verb-categories-select-type", (string) null);
  public static readonly VerbCategory PowerLevel = new VerbCategory("verb-categories-power-level", (string) null);
  public static readonly VerbCategory Adjust = new VerbCategory("verb-categories-adjust", "/Textures/Interface/VerbIcons/screwdriver.png");

  public VerbCategory(string text, string? icon, bool iconsOnly = false)
  {
    this.Text = Loc.GetString(text);
    this.Icon = icon == null ? (SpriteSpecifier) null : (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath(icon));
    this.IconsOnly = iconsOnly;
  }
}
