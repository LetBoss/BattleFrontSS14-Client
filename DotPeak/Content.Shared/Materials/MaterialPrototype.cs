// Decompiled with JetBrains decompiler
// Type: Content.Shared.Materials.MaterialPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;
using Robust.Shared.Utility;

#nullable enable
namespace Content.Shared.Materials;

[Robust.Shared.Prototypes.Prototype(null, 1)]
public sealed class MaterialPrototype : IPrototype, IInheritingPrototype
{
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId? StackEntity;
  [DataField(null, false, 1, false, false, null)]
  public string Name = string.Empty;
  [DataField(null, false, 1, false, false, null)]
  public string Unit = "materials-unit-sheet";
  [DataField(null, false, 1, true, false, null)]
  public double Price;

  [Robust.Shared.ViewVariables.ViewVariables]
  [ParentDataField(typeof (AbstractPrototypeIdArraySerializer<MaterialPrototype>), 1)]
  public string[]? Parents { get; private set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  [AbstractDataField(1)]
  public bool Abstract { get; private set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  [IdDataField(1, null)]
  public string ID { get; private set; }

  [DataField(null, false, 1, false, false, null)]
  public Color Color { get; private set; } = Color.Gray;

  [DataField(null, false, 1, false, false, null)]
  public SpriteSpecifier Icon { get; private set; } = SpriteSpecifier.Invalid;
}
