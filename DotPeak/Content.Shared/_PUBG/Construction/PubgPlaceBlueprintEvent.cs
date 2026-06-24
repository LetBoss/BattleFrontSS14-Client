// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Construction.PubgPlaceBlueprintEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Construction.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._PUBG.Construction;

[ByRefEvent]
public readonly record struct PubgPlaceBlueprintEvent(
  ProtoId<RMCConstructionPrototype> Recipe,
  EntityUid User,
  EntityCoordinates Coordinates,
  Direction Direction)
{
  [CompilerGenerated]
  public void Deconstruct(
    out ProtoId<RMCConstructionPrototype> Recipe,
    out EntityUid User,
    out EntityCoordinates Coordinates,
    out Direction Direction)
  {
    Recipe = this.Recipe;
    User = this.User;
    Coordinates = this.Coordinates;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(sbyte&) ref Direction = (sbyte) this.Direction;
  }
}
