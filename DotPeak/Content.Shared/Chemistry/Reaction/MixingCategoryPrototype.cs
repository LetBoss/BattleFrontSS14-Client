// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chemistry.Reaction.MixingCategoryPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

#nullable enable
namespace Content.Shared.Chemistry.Reaction;

[Prototype(null, 1)]
public sealed class MixingCategoryPrototype : IPrototype
{
  [DataField(null, false, 1, true, false, null)]
  public LocId VerbText;
  [DataField(null, false, 1, true, false, null)]
  public SpriteSpecifier Icon;

  [IdDataField(1, null)]
  public string ID { get; private set; }
}
