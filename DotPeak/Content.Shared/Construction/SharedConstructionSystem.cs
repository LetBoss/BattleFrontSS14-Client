// Decompiled with JetBrains decompiler
// Type: Content.Shared.Construction.SharedConstructionSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Construction.Components;
using Content.Shared.Interaction;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Prototypes;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared.Construction;

public abstract class SharedConstructionSystem : EntitySystem
{
  [Dependency]
  private IMapManager _mapManager;
  [Dependency]
  private SharedMapSystem _map;
  [Dependency]
  protected IPrototypeManager PrototypeManager;
  [Dependency]
  protected SharedTransformSystem TransformSystem;

  public SharedInteractionSystem.Ignored? GetPredicate(
    bool canBuildInImpassable,
    MapCoordinates coords)
  {
    if (!canBuildInImpassable)
      return (SharedInteractionSystem.Ignored) null;
    EntityUid entityUid;
    MapGridComponent mapGridComponent;
    if (!this._mapManager.TryFindGridAt(coords, ref entityUid, ref mapGridComponent))
      return (SharedInteractionSystem.Ignored) null;
    HashSet<EntityUid> ignored = this._map.GetAnchoredEntities(Entity<MapGridComponent>.op_Implicit((entityUid, mapGridComponent)), coords).ToHashSet<EntityUid>();
    return (SharedInteractionSystem.Ignored) (e => ignored.Contains(e));
  }

  public string GetExamineName(GenericPartInfo info)
  {
    return info.ExamineName.HasValue ? this.Loc.GetString(LocId.op_Implicit(info.ExamineName.Value)) : this.PrototypeManager.Index(info.DefaultPrototype).Name;
  }
}
