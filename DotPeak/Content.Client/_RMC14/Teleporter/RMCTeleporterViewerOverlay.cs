// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Teleporter.RMCTeleporterViewerOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._RMC14.NightVision;
using Content.Shared._RMC14.Atmos;
using Content.Shared._RMC14.Teleporter;
using Content.Shared._RMC14.Xenonids;
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
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;

#nullable enable
namespace Content.Client._RMC14.Teleporter;

public sealed class RMCTeleporterViewerOverlay : Overlay
{
  [Dependency]
  private IEntityManager _entity;
  [Dependency]
  private IPlayerManager _player;
  [Dependency]
  private IOverlayManager _overlay;
  private readonly SharedContainerSystem _container;
  private readonly EntityLookupSystem _entityLookup;
  private readonly SharedPhysicsSystem _physics;
  private readonly SpriteSystem _sprite;
  private readonly SharedRMCTeleporterSystem _teleporter;
  private readonly SharedTransformSystem _transform;
  private readonly EntityQuery<SpriteComponent> _spriteQuery;
  private readonly EntityQuery<RMCTeleporterViewerComponent> _teleporterViewerQuery;
  private readonly EntityQuery<TileFireComponent> _tileFireQuery;
  private readonly EntityQuery<TransformComponent> _transformQuery;
  private readonly EntityQuery<XenoComponent> _xenoQuery;
  private readonly List<(Entity<SpriteComponent> Ent, Vector2 Position, Angle Rotation)> _toDraw = new List<(Entity<SpriteComponent>, Vector2, Angle)>();

  public virtual OverlaySpace Space
  {
    get => !this._overlay.HasOverlay<NightVisionOverlay>() ? (OverlaySpace) 8 : (OverlaySpace) 4;
  }

  public RMCTeleporterViewerOverlay()
  {
    IoCManager.InjectDependencies<RMCTeleporterViewerOverlay>(this);
    this._container = this._entity.System<SharedContainerSystem>();
    this._entityLookup = this._entity.System<EntityLookupSystem>();
    this._physics = this._entity.System<SharedPhysicsSystem>();
    this._sprite = this._entity.System<SpriteSystem>();
    this._teleporter = this._entity.System<SharedRMCTeleporterSystem>();
    this._transform = this._entity.System<SharedTransformSystem>();
    this._spriteQuery = this._entity.GetEntityQuery<SpriteComponent>();
    this._teleporterViewerQuery = this._entity.GetEntityQuery<RMCTeleporterViewerComponent>();
    this._tileFireQuery = this._entity.GetEntityQuery<TileFireComponent>();
    this._transformQuery = this._entity.GetEntityQuery<TransformComponent>();
    this._xenoQuery = this._entity.GetEntityQuery<XenoComponent>();
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    if (!localEntity.HasValue)
      return;
    EntityUid valueOrDefault = localEntity.GetValueOrDefault();
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    IEye eye = args.Viewport.Eye;
    Angle angle1 = eye != null ? eye.Rotation : new Angle();
    foreach (EntityUid entityUid1 in this._physics.GetEntitiesIntersectingBody(valueOrDefault, 65, true, (PhysicsComponent) null, (FixturesComponent) null, (TransformComponent) null))
    {
      RMCTeleporterViewerComponent teleporterViewerComponent;
      if (this._teleporterViewerQuery.TryComp(entityUid1, ref teleporterViewerComponent))
      {
        Vector2 worldPosition = this._transform.GetWorldPosition(entityUid1);
        foreach (Entity<RMCTeleporterViewerComponent> teleporterViewer in this._teleporter.GetMatchingTeleporterViewers(Entity<RMCTeleporterViewerComponent>.op_Implicit((entityUid1, teleporterViewerComponent))))
        {
          MapCoordinates mapCoordinates = this._transform.GetMapCoordinates(Entity<RMCTeleporterViewerComponent>.op_Implicit(teleporterViewer), (TransformComponent) null);
          Vector2 vector2_1 = mapCoordinates.Position - worldPosition;
          Box2 worldAabb = this._physics.GetWorldAABB(Entity<RMCTeleporterViewerComponent>.op_Implicit(teleporterViewer), (FixturesComponent) null, (PhysicsComponent) null, (TransformComponent) null);
          this._toDraw.Clear();
          foreach (EntityUid entityUid2 in this._entityLookup.GetEntitiesIntersecting(mapCoordinates.MapId, worldAabb, (LookupFlags) 78))
          {
            SpriteComponent spriteComponent;
            TransformComponent transformComponent;
            if (this._spriteQuery.TryComp(entityUid2, ref spriteComponent) && spriteComponent.Visible && this._transformQuery.TryComp(entityUid2, ref transformComponent) && !this._container.IsEntityInContainer(entityUid2, (MetaDataComponent) null) && (!transformComponent.Anchored || this._xenoQuery.HasComp(entityUid2) || this._tileFireQuery.HasComp(entityUid2)))
            {
              (Vector2 vector2_2, Angle angle2) = this._transform.GetWorldPositionRotation(transformComponent);
              this._toDraw.Add((Entity<SpriteComponent>.op_Implicit((entityUid2, spriteComponent)), vector2_2, angle2));
            }
          }
          this._toDraw.Sort((Comparison<(Entity<SpriteComponent>, Vector2, Angle)>) ((a, b) => a.Ent.Comp.DrawDepth.CompareTo(b.Ent.Comp.DrawDepth)));
          Span<(Entity<SpriteComponent>, Vector2, Angle)> span = CollectionsMarshal.AsSpan<(Entity<SpriteComponent>, Vector2, Angle)>(this._toDraw);
          for (int index = 0; index < span.Length; ++index)
          {
            ref (Entity<SpriteComponent>, Vector2, Angle) local = ref span[index];
            local.Item2 -= vector2_1;
            this._sprite.RenderSprite(local.Item1, worldHandle, angle1, local.Item3, local.Item2, new Direction?());
          }
        }
      }
    }
    DrawingHandleWorld drawingHandleWorld = worldHandle;
    Matrix3x2 identity = Matrix3x2.Identity;
    ref Matrix3x2 local1 = ref identity;
    ((DrawingHandleBase) drawingHandleWorld).SetTransform(ref local1);
  }
}
