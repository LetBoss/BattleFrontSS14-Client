// Decompiled with JetBrains decompiler
// Type: Content.Shared.HUD.HudThemePrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using System;

#nullable enable
namespace Content.Shared.HUD;

[Prototype(null, 1)]
public sealed class HudThemePrototype : IPrototype, IComparable<HudThemePrototype>
{
  [DataField(null, false, 1, false, false, null)]
  public int Order;

  [DataField("name", false, 1, true, false, null)]
  public string Name { get; private set; } = string.Empty;

  [IdDataField(1, null)]
  public string ID { get; private set; } = string.Empty;

  [DataField("path", false, 1, true, false, null)]
  public string Path { get; private set; } = string.Empty;

  public int CompareTo(HudThemePrototype? other) => this.Order.CompareTo((object) other?.Order);
}
