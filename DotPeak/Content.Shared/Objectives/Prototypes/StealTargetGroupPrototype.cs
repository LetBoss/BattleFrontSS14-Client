// Decompiled with JetBrains decompiler
// Type: Content.Shared.Objectives.StealTargetGroupPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

#nullable enable
namespace Content.Shared.Objectives;

[Prototype(null, 1)]
public sealed class StealTargetGroupPrototype : IPrototype
{
  [IdDataField(1, null)]
  public string ID { get; private set; }

  [DataField(null, false, 1, false, false, null)]
  public LocId Name { get; private set; } = (LocId) string.Empty;

  [DataField(null, false, 1, false, false, null)]
  public SpriteSpecifier Sprite { get; private set; } = SpriteSpecifier.Invalid;
}
