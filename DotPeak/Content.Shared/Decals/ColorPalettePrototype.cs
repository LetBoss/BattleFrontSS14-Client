// Decompiled with JetBrains decompiler
// Type: Content.Shared.Decals.ColorPalettePrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Decals;

[Prototype("palette", 1)]
public sealed class ColorPalettePrototype : IPrototype
{
  [IdDataField(1, null)]
  public string ID { get; private set; }

  [DataField("name", false, 1, false, false, null)]
  public string Name { get; private set; }

  [DataField("colors", false, 1, false, false, null)]
  public Dictionary<string, Color> Colors { get; private set; }
}
