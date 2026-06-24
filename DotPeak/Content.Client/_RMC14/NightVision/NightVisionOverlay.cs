// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.NightVision.NightVisionOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Examine;
using Content.Shared._RMC14.NightVision;
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
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client._RMC14.NightVision;

public sealed class NightVisionOverlay : Overlay
{
  [Dependency]
  private IEntityManager _entity;
  [Dependency]
  private IPlayerManager _players;
  [Dependency]
  private IPrototypeManager _prototype;
  private readonly ContainerSystem _container;
  private readonly ExamineSystem _examine;
  private readonly SpriteSystem _sprite;
  private readonly TransformSystem _transform;
  private readonly EntityQuery<XenoComponent> _xenoQuery;
  private readonly ShaderInstance _shader;
  private readonly List<NightVisionRenderEntry> _entries = new List<NightVisionRenderEntry>();

  public virtual OverlaySpace Space => (OverlaySpace) 4;

  public NightVisionOverlay()
  {
    IoCManager.InjectDependencies<NightVisionOverlay>(this);
    this._container = this._entity.System<ContainerSystem>();
    this._examine = this._entity.System<ExamineSystem>();
    this._sprite = this._entity.System<SpriteSystem>();
    this._transform = this._entity.System<TransformSystem>();
    this._xenoQuery = this._entity.GetEntityQuery<XenoComponent>();
    this._shader = this._prototype.Index<ShaderPrototype>(new ProtoId<ShaderPrototype>("RMCNightVision")).Instance().Duplicate();
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    NightVisionComponent nightVisionComponent;
    if (!this._entity.TryGetComponent<NightVisionComponent>(((ISharedPlayerManager) this._players).LocalEntity, ref nightVisionComponent) || nightVisionComponent.State == NightVisionState.Off)
      return;
    DrawingHandleWorld worldHandle1 = ((OverlayDrawArgs) ref args).WorldHandle;
    IEye eye = args.Viewport.Eye;
    Angle eyeRot = eye != null ? eye.Rotation : new Angle();
    this._entries.Clear();
    EntityQueryEnumerator<RMCNightVisionVisibleComponent, SpriteComponent, TransformComponent> entityQueryEnumerator1 = this._entity.EntityQueryEnumerator<RMCNightVisionVisibleComponent, SpriteComponent, TransformComponent>();
    EntityUid entityUid;
    RMCNightVisionVisibleComponent visibleComponent;
    SpriteComponent spriteComponent1;
    TransformComponent transformComponent1;
    while (entityQueryEnumerator1.MoveNext(ref entityUid, ref visibleComponent, ref spriteComponent1, ref transformComponent1))
      this._entries.Add(new NightVisionRenderEntry((entityUid, spriteComponent1, transformComponent1), eye?.Position.MapId, nightVisionComponent.SeeThroughContainers, visibleComponent.Priority, visibleComponent.Transparency));
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    this._entries.Sort(NightVisionOverlay.\u003C\u003EO.\u003C0\u003E__SortPriority ?? (NightVisionOverlay.\u003C\u003EO.\u003C0\u003E__SortPriority = new Comparison<NightVisionRenderEntry>(NightVisionOverlay.SortPriority)));
    foreach (NightVisionRenderEntry entry in this._entries)
      this.Render(Entity<SpriteComponent, TransformComponent>.op_Implicit(entry.Ent), entry.Map, worldHandle1, eyeRot, entry.NightVisionSeeThroughContainers, entry.Transparency);
    EntityUid? localEntity = ((ISharedPlayerManager) this._players).LocalEntity;
    if (localEntity.HasValue)
    {
      EntityUid valueOrDefault = localEntity.GetValueOrDefault();
      EntityQueryEnumerator<RMCNightVisionVisibleInViewComponent, SpriteComponent, TransformComponent> entityQueryEnumerator2 = this._entity.EntityQueryEnumerator<RMCNightVisionVisibleInViewComponent, SpriteComponent, TransformComponent>();
      EntityUid origin;
      RMCNightVisionVisibleInViewComponent visibleInViewComponent;
      SpriteComponent spriteComponent2;
      TransformComponent transformComponent2;
      while (entityQueryEnumerator2.MoveNext(ref origin, ref visibleInViewComponent, ref spriteComponent2, ref transformComponent2))
      {
        if (this._examine.InRangeUnOccluded(origin, valueOrDefault))
          this.Render(Entity<SpriteComponent, TransformComponent>.op_Implicit((origin, spriteComponent2, transformComponent2)), eye?.Position.MapId, worldHandle1, eyeRot, false, new float?());
      }
    }
    DrawingHandleWorld drawingHandleWorld = worldHandle1;
    Matrix3x2 identity = Matrix3x2.Identity;
    ref Matrix3x2 local = ref identity;
    ((DrawingHandleBase) drawingHandleWorld).SetTransform(ref local);
    if (!nightVisionComponent.Green || this.ScreenTexture == null || args.Viewport.Eye == null)
      return;
    this._shader.SetParameter("renderScale", args.Viewport.RenderScale * args.Viewport.Eye.Scale);
    this._shader.SetParameter("SCREEN_TEXTURE", this.ScreenTexture);
    DrawingHandleWorld worldHandle2 = ((OverlayDrawArgs) ref args).WorldHandle;
    ((DrawingHandleBase) worldHandle2).UseShader(this._shader);
    worldHandle2.DrawRect(ref args.WorldBounds, Color.White, true);
    ((DrawingHandleBase) worldHandle2).UseShader((ShaderInstance) null);
  }

  private static int SortPriority(NightVisionRenderEntry x, NightVisionRenderEntry y)
  {
    return x.Priority.CompareTo(y.Priority);
  }

  private void Render(
    Entity<SpriteComponent, TransformComponent> ent,
    MapId? map,
    DrawingHandleWorld handle,
    Angle eyeRot,
    bool seeThroughContainers,
    float? transparency)
  {
    EntityUid entityUid1;
    SpriteComponent spriteComponent1;
    TransformComponent transformComponent1;
    ent.Deconstruct(ref entityUid1, ref spriteComponent1, ref transformComponent1);
    EntityUid entityUid2 = entityUid1;
    SpriteComponent spriteComponent2 = spriteComponent1;
    TransformComponent transformComponent2 = transformComponent1;
    MapId mapId = transformComponent2.MapID;
    MapId? nullable = map;
    if ((nullable.HasValue ? (MapId.op_Inequality(mapId, nullable.GetValueOrDefault()) ? 1 : 0) : 1) != 0 || (!seeThroughContainers ? 0 : (!this._xenoQuery.HasComp(entityUid2) ? 1 : 0)) == 0 && ((SharedContainerSystem) this._container).IsEntityOrParentInContainer(entityUid2, (MetaDataComponent) null, transformComponent2))
      return;
    (Vector2 vector2, Angle angle) = ((SharedTransformSystem) this._transform).GetWorldPositionRotation(transformComponent2);
    Color color1 = spriteComponent2.Color;
    if (transparency.HasValue)
    {
      Color color2 = spriteComponent2.Color;
      ref Color local1 = ref color2;
      Color color3 = Color.White;
      color3 = ((Color) ref color3).WithAlpha(transparency.Value);
      ref Color local2 = ref color3;
      Color color4 = Color.op_Multiply(ref local1, ref local2);
      this._sprite.SetColor(Entity<SpriteComponent>.op_Implicit((entityUid2, spriteComponent2)), color4);
    }
    this._sprite.RenderSprite(Entity<SpriteComponent>.op_Implicit((entityUid2, spriteComponent2)), handle, eyeRot, angle, vector2, new Direction?());
    if (!transparency.HasValue)
      return;
    this._sprite.SetColor(Entity<SpriteComponent>.op_Implicit((entityUid2, spriteComponent2)), color1);
  }
}
