// Decompiled with JetBrains decompiler
// Type: Content.Shared.Radio.RadioChannelPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

#nullable enable
namespace Content.Shared.Radio;

[Prototype(null, 1)]
public sealed class RadioChannelPrototype : IPrototype
{
  [DataField("longRange", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public bool LongRange;
  [DataField(null, false, 1, false, false, null)]
  public bool Tower;
  [DataField(null, false, 1, false, false, null)]
  public bool Planet = true;
  [DataField(null, false, 1, false, false, null)]
  public Color? ColorblindColor;

  [DataField("name", false, 1, false, false, null)]
  public LocId Name { get; private set; } = (LocId) string.Empty;

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadOnly)]
  public string LocalizedName => Loc.GetString((string) this.Name);

  [DataField("keycode", false, 1, false, false, null)]
  public char KeyCode { get; private set; }

  [DataField("frequency", false, 1, false, false, null)]
  public int Frequency { get; private set; }

  [DataField("color", false, 1, false, false, null)]
  public Color Color { get; private set; } = Color.Lime;

  [IdDataField(1, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public string ID { get; private set; }
}
