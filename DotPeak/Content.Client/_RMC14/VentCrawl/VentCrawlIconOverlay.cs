// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.VentCrawl.VentCrawlIconOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.SubFloor;
using Content.Shared._RMC14.Vents;
using Content.Shared.SubFloor;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Containers;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System.Numerics;

#nullable enable
namespace Content.Client._RMC14.VentCrawl;

public sealed class VentCrawlIconOverlay : Overlay
{
  [Dependency]
  private IEntityManager _entity;
  [Dependency]
  private IPlayerManager _players;
  [Dependency]
  private IGameTiming _timing;
  private readonly SpriteSystem _sprite;
  private readonly TransformSystem _transform;
  private readonly ContainerSystem _container;
  private readonly EntityQuery<TransformComponent> _xformQuery;
  private readonly ResPath _rsiPath = new ResPath("/Textures/_RMC14/Interface/vent_crawl.rsi");

  public virtual OverlaySpace Space => (OverlaySpace) 16 /*0x10*/;

  public VentCrawlIconOverlay()
  {
    IoCManager.InjectDependencies<VentCrawlIconOverlay>(this);
    this._container = this._entity.System<ContainerSystem>();
    this._sprite = this._entity.System<SpriteSystem>();
    this._transform = this._entity.System<TransformSystem>();
    this._xformQuery = this._entity.GetEntityQuery<TransformComponent>();
    this.ZIndex = new int?(-5);
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    if (!this._entity.HasComponent<VentSightComponent>(((ISharedPlayerManager) this._players).LocalEntity))
      return;
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    IEye eye = args.Viewport.Eye;
    Angle angle = eye != null ? eye.Rotation : new Angle();
    Matrix3x2 scale = Matrix3x2.CreateScale(new Vector2(1f, 1f));
    Matrix3x2 rotation = Matrix3Helpers.CreateRotation(Angle.op_Implicit(Angle.op_UnaryNegation(angle)));
    AllEntityQueryEnumerator<VentCrawlingComponent, VentCrawlerComponent, TransformComponent, SpriteComponent> entityQueryEnumerator = this._entity.AllEntityQueryEnumerator<VentCrawlingComponent, VentCrawlerComponent, TransformComponent, SpriteComponent>();
    EntityUid ent;
    VentCrawlingComponent crawlingComponent;
    VentCrawlerComponent crawler;
    TransformComponent transform;
    SpriteComponent sprite;
    while (entityQueryEnumerator.MoveNext(ref ent, ref crawlingComponent, ref crawler, ref transform, ref sprite))
    {
      EntityUid entityUid = ent;
      EntityUid? localEntity = ((ISharedPlayerManager) this._players).LocalEntity;
      BaseContainer baseContainer;
      SubFloorHideComponent floorHideComponent;
      if ((localEntity.HasValue ? (EntityUid.op_Inequality(entityUid, localEntity.GetValueOrDefault()) ? 1 : 0) : 1) == 0 || ((SharedContainerSystem) this._container).TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit(ent), ref baseContainer) && (!this._entity.TryGetComponent<SubFloorHideComponent>(ent, ref floorHideComponent) || !floorHideComponent.IsUnderCover || this._entity.HasComponent<TrayRevealedComponent>(baseContainer.Owner)))
        this.DrawIcon(args, scale, rotation, ent, crawler, transform, sprite);
    }
    DrawingHandleWorld drawingHandleWorld = worldHandle;
    Matrix3x2 identity = Matrix3x2.Identity;
    ref Matrix3x2 local = ref identity;
    ((DrawingHandleBase) drawingHandleWorld).SetTransform(ref local);
  }

  private void DrawIcon(
    OverlayDrawArgs args,
    Matrix3x2 scale,
    Matrix3x2 rotate,
    EntityUid ent,
    VentCrawlerComponent crawler,
    TransformComponent transform,
    SpriteComponent sprite)
  {
    if (MapId.op_Inequality(transform.MapID, args.MapId))
      return;
    Box2 localBounds = this._sprite.GetLocalBounds(Entity<SpriteComponent>.op_Implicit((ent, sprite)));
    Vector2 worldPosition = ((SharedTransformSystem) this._transform).GetWorldPosition(transform, this._xformQuery);
    Box2 box2 = ((Box2) ref localBounds).Translated(worldPosition);
    if (!((Box2) ref box2).Intersects(ref args.WorldAABB))
      return;
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    Matrix3x2 translation = Matrix3x2.CreateTranslation(worldPosition);
    Matrix3x2 matrix3x2_1 = Matrix3x2.Multiply(scale, translation);
    Matrix3x2 matrix3x2_2 = Matrix3x2.Multiply(rotate, matrix3x2_1);
    ((DrawingHandleBase) worldHandle).SetTransform(ref matrix3x2_2);
    ((DrawingHandleBase) worldHandle).DrawTexture(this._sprite.GetFrame((SpriteSpecifier) new SpriteSpecifier.Rsi(this._rsiPath, crawler.VentCrawlIcon), this._timing.CurTime, true), ((Box2) ref localBounds).Center + sprite.Offset + new Vector2(-0.5f, -0.5f), new Color?());
  }
}
