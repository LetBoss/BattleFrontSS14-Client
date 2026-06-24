// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Xenonids.Pheromones.XenoPheromonesOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Mobs;
using Content.Shared._RMC14.Xenonids;
using Content.Shared._RMC14.Xenonids.HiveLeader;
using Content.Shared._RMC14.Xenonids.Pheromones;
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
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Numerics;

#nullable enable
namespace Content.Client._RMC14.Xenonids.Pheromones;

public sealed class XenoPheromonesOverlay : Overlay
{
  [Dependency]
  private IEntityManager _entity;
  [Dependency]
  private IPlayerManager _players;
  [Dependency]
  private IPrototypeManager _prototype;
  [Dependency]
  private IGameTiming _timing;
  private static readonly ImmutableArray<XenoPheromones> AllPheromones = ((IEnumerable<XenoPheromones>) Enum.GetValues<XenoPheromones>()).ToImmutableArray<XenoPheromones>();
  private readonly ContainerSystem _container;
  private readonly SpriteSystem _sprite;
  private readonly TransformSystem _transform;
  private readonly EntityQuery<TransformComponent> _xformQuery;
  private readonly ShaderInstance _shader;

  public virtual OverlaySpace Space => (OverlaySpace) 4;

  public XenoPheromonesOverlay()
  {
    IoCManager.InjectDependencies<XenoPheromonesOverlay>(this);
    this._container = this._entity.System<ContainerSystem>();
    this._sprite = this._entity.System<SpriteSystem>();
    this._transform = this._entity.System<TransformSystem>();
    this._xformQuery = this._entity.GetEntityQuery<TransformComponent>();
    this._shader = this._prototype.Index<ShaderPrototype>(new ProtoId<ShaderPrototype>("unshaded")).Instance();
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    if (!this._entity.HasComponent<XenoComponent>(((ISharedPlayerManager) this._players).LocalEntity) && !this._entity.HasComponent<CMGhostXenoHudComponent>(((ISharedPlayerManager) this._players).LocalEntity))
      return;
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    IEye eye = args.Viewport.Eye;
    Angle angle = eye != null ? eye.Rotation : new Angle();
    Matrix3x2 scale = Matrix3x2.CreateScale(new Vector2(1f, 1f));
    Matrix3x2 rotation = Matrix3Helpers.CreateRotation(Angle.op_Implicit(Angle.op_UnaryNegation(angle)));
    ((DrawingHandleBase) worldHandle).UseShader(this._shader);
    AllEntityQueryEnumerator<XenoRecoveryPheromonesComponent, SpriteComponent, TransformComponent> entityQueryEnumerator1 = this._entity.AllEntityQueryEnumerator<XenoRecoveryPheromonesComponent, SpriteComponent, TransformComponent>();
    EntityUid entityUid1;
    XenoRecoveryPheromonesComponent pheromonesComponent1;
    SpriteComponent spriteComponent1;
    TransformComponent transformComponent1;
    while (entityQueryEnumerator1.MoveNext(ref entityUid1, ref pheromonesComponent1, ref spriteComponent1, ref transformComponent1))
      this.DrawIcon(Entity<SpriteComponent, TransformComponent>.op_Implicit((entityUid1, spriteComponent1, transformComponent1)), in args, pheromonesComponent1.Icon, scale, rotation);
    AllEntityQueryEnumerator<XenoWardingPheromonesComponent, SpriteComponent, TransformComponent> entityQueryEnumerator2 = this._entity.AllEntityQueryEnumerator<XenoWardingPheromonesComponent, SpriteComponent, TransformComponent>();
    EntityUid entityUid2;
    XenoWardingPheromonesComponent pheromonesComponent2;
    SpriteComponent spriteComponent2;
    TransformComponent transformComponent2;
    while (entityQueryEnumerator2.MoveNext(ref entityUid2, ref pheromonesComponent2, ref spriteComponent2, ref transformComponent2))
      this.DrawIcon(Entity<SpriteComponent, TransformComponent>.op_Implicit((entityUid2, spriteComponent2, transformComponent2)), in args, pheromonesComponent2.Icon, scale, rotation);
    AllEntityQueryEnumerator<XenoFrenzyPheromonesComponent, SpriteComponent, TransformComponent> entityQueryEnumerator3 = this._entity.AllEntityQueryEnumerator<XenoFrenzyPheromonesComponent, SpriteComponent, TransformComponent>();
    EntityUid entityUid3;
    XenoFrenzyPheromonesComponent pheromonesComponent3;
    SpriteComponent spriteComponent3;
    TransformComponent transformComponent3;
    while (entityQueryEnumerator3.MoveNext(ref entityUid3, ref pheromonesComponent3, ref spriteComponent3, ref transformComponent3))
      this.DrawIcon(Entity<SpriteComponent, TransformComponent>.op_Implicit((entityUid3, spriteComponent3, transformComponent3)), in args, pheromonesComponent3.Icon, scale, rotation);
    AllEntityQueryEnumerator<XenoActivePheromonesComponent, SpriteComponent, TransformComponent> entityQueryEnumerator4 = this._entity.AllEntityQueryEnumerator<XenoActivePheromonesComponent, SpriteComponent, TransformComponent>();
    EntityUid entityUid4;
    XenoActivePheromonesComponent pheromonesComponent4;
    SpriteComponent spriteComponent4;
    TransformComponent transformComponent4;
    while (entityQueryEnumerator4.MoveNext(ref entityUid4, ref pheromonesComponent4, ref spriteComponent4, ref transformComponent4))
    {
      SpriteSpecifier.Rsi icon = new SpriteSpecifier.Rsi(new ResPath("/Textures/_RMC14/Interface/xeno_pheromones_hud.rsi"), "aura_" + pheromonesComponent4.Pheromones.ToString().ToLowerInvariant());
      this.DrawIcon(Entity<SpriteComponent, TransformComponent>.op_Implicit((entityUid4, spriteComponent4, transformComponent4)), in args, (SpriteSpecifier) icon, scale, rotation);
    }
    AllEntityQueryEnumerator<HiveLeaderComponent, SpriteComponent, TransformComponent> entityQueryEnumerator5 = this._entity.AllEntityQueryEnumerator<HiveLeaderComponent, SpriteComponent, TransformComponent>();
    EntityUid entityUid5;
    HiveLeaderComponent hiveLeaderComponent;
    SpriteComponent spriteComponent5;
    TransformComponent transformComponent5;
    while (entityQueryEnumerator5.MoveNext(ref entityUid5, ref hiveLeaderComponent, ref spriteComponent5, ref transformComponent5))
    {
      BaseContainer baseContainer;
      EntityUid? nullable;
      XenoActivePheromonesComponent pheromonesComponent5;
      if (((SharedContainerSystem) this._container).TryGetContainer(entityUid5, hiveLeaderComponent.PheromonesContainerId, ref baseContainer, (ContainerManagerComponent) null) && Extensions.TryFirstOrNull<EntityUid>((IEnumerable<EntityUid>) baseContainer.ContainedEntities, ref nullable) && this._entity.TryGetComponent<XenoActivePheromonesComponent>(nullable, ref pheromonesComponent5))
      {
        SpriteSpecifier.Rsi icon = new SpriteSpecifier.Rsi(new ResPath("/Textures/_RMC14/Interface/xeno_pheromones_hud.rsi"), "aura_" + pheromonesComponent5.Pheromones.ToString().ToLowerInvariant());
        this.DrawIcon(Entity<SpriteComponent, TransformComponent>.op_Implicit((entityUid5, spriteComponent5, transformComponent5)), in args, (SpriteSpecifier) icon, scale, rotation);
      }
    }
    ((DrawingHandleBase) worldHandle).UseShader((ShaderInstance) null);
  }

  private void DrawIcon(
    Entity<SpriteComponent, TransformComponent> ent,
    in OverlayDrawArgs args,
    SpriteSpecifier icon,
    Matrix3x2 scaleMatrix,
    Matrix3x2 rotationMatrix)
  {
    EntityUid entityUid;
    SpriteComponent spriteComponent1;
    TransformComponent transformComponent1;
    ent.Deconstruct(ref entityUid, ref spriteComponent1, ref transformComponent1);
    SpriteComponent spriteComponent2 = spriteComponent1;
    TransformComponent transformComponent2 = transformComponent1;
    if (MapId.op_Inequality(transformComponent2.MapID, args.MapId))
      return;
    Box2 localBounds = this._sprite.GetLocalBounds(Entity<SpriteComponent>.op_Implicit((ent.Owner, spriteComponent2)));
    Vector2 worldPosition = ((SharedTransformSystem) this._transform).GetWorldPosition(transformComponent2, this._xformQuery);
    Box2 box2 = ((Box2) ref localBounds).Translated(worldPosition);
    if (!((Box2) ref box2).Intersects(ref args.WorldAABB))
      return;
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    Matrix3x2 translation = Matrix3x2.CreateTranslation(worldPosition);
    Matrix3x2 matrix3x2_1 = Matrix3x2.Multiply(scaleMatrix, translation);
    Matrix3x2 matrix3x2_2 = Matrix3x2.Multiply(rotationMatrix, matrix3x2_1);
    ((DrawingHandleBase) worldHandle).SetTransform(ref matrix3x2_2);
    Texture frame = this._sprite.GetFrame(icon, this._timing.CurTime, true);
    float y = (float) (((double) ((Box2) ref localBounds).Height + (double) spriteComponent2.Offset.Y) / 2.0 - (double) frame.Height / 32.0 * (double) ((Box2) ref localBounds).Height);
    Vector2 vector2 = new Vector2((float) (((double) ((Box2) ref localBounds).Width + (double) spriteComponent2.Offset.X) / 2.0 - (double) frame.Width / 32.0 - 0.25), y);
    ((DrawingHandleBase) worldHandle).DrawTexture(frame, vector2, new Color?());
  }
}
