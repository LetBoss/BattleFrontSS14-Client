// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chemistry.Reaction.ITileReaction
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Chemistry.Reaction;

public interface ITileReaction
{
  FixedPoint2 TileReact(
    TileRef tile,
    ReagentPrototype reagent,
    FixedPoint2 reactVolume,
    IEntityManager entityManager,
    List<ReagentData>? data = null);
}
