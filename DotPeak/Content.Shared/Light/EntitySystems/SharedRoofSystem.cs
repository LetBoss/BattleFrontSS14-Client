// Decompiled with JetBrains decompiler
// Type: Content.Shared.Light.EntitySystems.SharedRoofSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Areas;
using Content.Shared.Light.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Light.EntitySystems;

public abstract class SharedRoofSystem : EntitySystem
{
  [Dependency]
  private EntityLookupSystem _lookup;
  [Dependency]
  private AreaSystem _area;
  private HashSet<Entity<IsRoofComponent>> _roofSet = new HashSet<Entity<IsRoofComponent>>();

  public bool IsRooved(Entity<MapGridComponent, RoofComponent> grid, Vector2i index)
  {
    ulong num1;
    if (grid.Comp2.Data.TryGetValue(SharedMapSystem.GetChunkIndices(index, 8), out num1))
    {
      Vector2i chunkRelative = SharedMapSystem.GetChunkRelative(index, 8);
      ulong num2 = 1UL << chunkRelative.X + chunkRelative.Y * 8;
      if (((long) num1 & (long) num2) == (long) num2)
        return true;
    }
    this._roofSet.Clear();
    this._lookup.GetLocalEntitiesIntersecting<IsRoofComponent>(grid.Owner, index, this._roofSet);
    foreach (Entity<IsRoofComponent> roof in this._roofSet)
    {
      if (roof.Comp.Enabled)
        return true;
    }
    return false;
  }

  public Color? GetColor(Entity<MapGridComponent, RoofComponent> grid, Vector2i index)
  {
    RoofComponent comp2 = grid.Comp2;
    Vector2i chunkIndices = SharedMapSystem.GetChunkIndices(index, 8);
    ulong num1;
    if (comp2.Data.TryGetValue(chunkIndices, out num1))
    {
      Vector2i chunkRelative = SharedMapSystem.GetChunkRelative(index, 8);
      ulong num2 = 1UL << chunkRelative.X + chunkRelative.Y * 8;
      if (((long) num1 & (long) num2) == (long) num2)
        return new Color?(comp2.Color);
    }
    this._roofSet.Clear();
    this._lookup.GetLocalEntitiesIntersecting<IsRoofComponent>(grid.Owner, index, this._roofSet);
    foreach (Entity<IsRoofComponent> roof in this._roofSet)
    {
      if (roof.Comp.Enabled)
      {
        Color? color = roof.Comp.Color;
        color = new Color?(color ?? comp2.Color);
        return color;
      }
    }
    return this._area.IsLightBlocked((Entity<MapGridComponent>) grid, index) ? new Color?(comp2.Color) : new Color?();
  }

  public void SetRoof(Entity<MapGridComponent?, RoofComponent?> grid, Vector2i index, bool value)
  {
    if (!this.Resolve<MapGridComponent, RoofComponent>((EntityUid) grid, ref grid.Comp1, ref grid.Comp2, false))
      return;
    Vector2i chunkIndices = SharedMapSystem.GetChunkIndices(index, 8);
    RoofComponent comp2 = grid.Comp2;
    ulong num1;
    if (!comp2.Data.TryGetValue(chunkIndices, out num1))
    {
      if (!value)
        return;
      num1 = 0UL;
    }
    Vector2i chunkRelative = SharedMapSystem.GetChunkRelative(index, 8);
    ulong num2 = 1UL << chunkRelative.X + chunkRelative.Y * 8;
    if (value)
    {
      if (((long) num1 & (long) num2) == (long) num2)
        return;
      num1 |= num2;
    }
    else
    {
      if (((long) num1 & (long) num2) == 0L)
        return;
      num1 &= ~num2;
    }
    comp2.Data[chunkIndices] = num1;
    this.Dirty(grid.Owner, (IComponent) comp2);
  }
}
