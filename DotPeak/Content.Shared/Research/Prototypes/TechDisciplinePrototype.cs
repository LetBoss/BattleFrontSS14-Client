// Decompiled with JetBrains decompiler
// Type: Content.Shared.Research.Prototypes.TechDisciplinePrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Research.Prototypes;

[Prototype(null, 1)]
public sealed class TechDisciplinePrototype : IPrototype
{
  [DataField("name", false, 1, true, false, null)]
  public string Name = string.Empty;
  [DataField("color", false, 1, true, false, null)]
  public Color Color;
  [DataField("icon", false, 1, false, false, null)]
  public SpriteSpecifier Icon;
  [DataField("tierPrerequisites", false, 1, true, false, null)]
  public Dictionary<int, float> TierPrerequisites = new Dictionary<int, float>();
  [DataField("lockoutTier", false, 1, false, false, null)]
  public int LockoutTier = 3;

  [IdDataField(1, null)]
  public string ID { get; private set; }
}
