// Decompiled with JetBrains decompiler
// Type: Content.Shared.Mind.RoleTypePrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using System;

#nullable enable
namespace Content.Shared.Mind;

[Prototype(null, 1)]
public sealed class RoleTypePrototype : IPrototype
{
  public static readonly LocId FallbackName = (LocId) "role-type-crew-aligned-name";
  public const string FallbackSymbol = "";
  public static readonly Color FallbackColor = Color.FromHex((ReadOnlySpan<char>) "#eeeeee", new Color?());
  [DataField(null, false, 1, false, false, null)]
  public LocId Name = RoleTypePrototype.FallbackName;
  [DataField(null, false, 1, false, false, null)]
  public Color Color = RoleTypePrototype.FallbackColor;
  [DataField(null, false, 1, false, false, null)]
  public string Symbol = "";

  [IdDataField(1, null)]
  public string ID { get; private set; }
}
