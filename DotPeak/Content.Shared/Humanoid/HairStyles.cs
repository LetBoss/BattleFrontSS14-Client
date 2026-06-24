// Decompiled with JetBrains decompiler
// Type: Content.Shared.Humanoid.HairStyles
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Humanoid.Markings;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Humanoid;

public static class HairStyles
{
  public static readonly ProtoId<MarkingPrototype> DefaultHairStyle = (ProtoId<MarkingPrototype>) "HairBald";
  public static readonly ProtoId<MarkingPrototype> DefaultFacialHairStyle = (ProtoId<MarkingPrototype>) "FacialHairShaved";
  public static readonly IReadOnlyList<Color> RealisticHairColors = (IReadOnlyList<Color>) new List<Color>()
  {
    Color.Yellow,
    Color.Black,
    Color.SandyBrown,
    Color.Brown,
    Color.Wheat,
    Color.Gray
  };
}
