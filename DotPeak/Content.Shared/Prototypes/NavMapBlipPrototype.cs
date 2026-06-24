// Decompiled with JetBrains decompiler
// Type: Content.Shared.Prototypes.NavMapBlipPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

#nullable enable
namespace Content.Shared.Prototypes;

[Prototype(null, 1)]
public sealed class NavMapBlipPrototype : IPrototype
{
  [DataField(null, false, 1, false, false, null)]
  public bool Selectable;
  [DataField(null, false, 1, false, false, null)]
  public bool Blinks;

  [Robust.Shared.ViewVariables.ViewVariables]
  [IdDataField(1, null)]
  public string ID { get; private set; }

  [DataField(null, false, 1, false, false, null)]
  public Color Color { get; private set; } = Color.LightGray;

  [DataField(null, false, 1, false, false, null)]
  public ResPath[]? TexturePaths { get; private set; }

  [DataField(null, false, 1, false, false, null)]
  public float Scale { get; private set; } = 1f;

  [DataField(null, false, 1, false, false, null)]
  public NavMapBlipPlacement Placement { get; private set; }
}
