// Decompiled with JetBrains decompiler
// Type: Content.Client.Construction.ConstructionPlacementHijack
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Construction.Prototypes;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Placement;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using System.Linq;

#nullable enable
namespace Content.Client.Construction;

public sealed class ConstructionPlacementHijack : PlacementHijack
{
  private readonly ConstructionSystem _constructionSystem;
  private readonly ConstructionPrototype? _prototype;

  public ConstructionSystem? CurrentConstructionSystem => this._constructionSystem;

  public ConstructionPrototype? CurrentPrototype => this._prototype;

  public virtual bool CanRotate { get; }

  public ConstructionPlacementHijack(
    ConstructionSystem constructionSystem,
    ConstructionPrototype? prototype)
  {
    this._constructionSystem = constructionSystem;
    this._prototype = prototype;
    this.CanRotate = prototype == null || prototype.CanRotate;
  }

  public virtual bool HijackPlacementRequest(EntityCoordinates coordinates)
  {
    if (this._prototype != null)
    {
      Direction direction = this.Manager.Direction;
      this._constructionSystem.SpawnGhost(this._prototype, coordinates, direction);
    }
    return true;
  }

  public virtual bool HijackDeletion(EntityUid entity)
  {
    if (IoCManager.Resolve<IEntityManager>().HasComponent<ConstructionGhostComponent>(entity))
      this._constructionSystem.ClearGhost(entity.GetHashCode());
    return true;
  }

  public virtual void StartHijack(PlacementManager manager)
  {
    base.StartHijack(manager);
    string targetProtoId;
    EntityPrototype entityPrototype;
    if (this._prototype == null || !this._constructionSystem.TryGetRecipePrototype(this._prototype.ID, out targetProtoId) || !IoCManager.Resolve<IPrototypeManager>().TryIndex<EntityPrototype>(targetProtoId, ref entityPrototype))
      return;
    manager.CurrentTextures = IoCManager.Resolve<IEntityManager>().System<SpriteSystem>().GetPrototypeTextures(entityPrototype).ToList<IDirectionalTextureProvider>();
  }
}
