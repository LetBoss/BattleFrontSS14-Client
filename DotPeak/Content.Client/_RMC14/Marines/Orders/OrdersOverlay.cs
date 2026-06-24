// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Marines.Orders.OrdersOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Marines;
using Content.Shared._RMC14.Marines.Orders;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
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
using System.Numerics;

#nullable enable
namespace Content.Client._RMC14.Marines.Orders;

public sealed class OrdersOverlay : Overlay
{
  [Dependency]
  private IEntityManager _entity;
  [Dependency]
  private IPlayerManager _players;
  [Dependency]
  private IPrototypeManager _prototype;
  [Dependency]
  private IGameTiming _timing;
  private readonly SpriteSystem _sprite;
  private readonly TransformSystem _transform;
  private readonly ShaderInstance _shader;

  public virtual OverlaySpace Space => (OverlaySpace) 8;

  public OrdersOverlay()
  {
    IoCManager.InjectDependencies<OrdersOverlay>(this);
    this._sprite = this._entity.System<SpriteSystem>();
    this._transform = this._entity.System<TransformSystem>();
    this._shader = this._prototype.Index<ShaderPrototype>(new ProtoId<ShaderPrototype>("unshaded")).Instance();
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    if (!this._entity.HasComponent<MarineComponent>(((ISharedPlayerManager) this._players).LocalEntity))
      return;
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    IEye eye = args.Viewport.Eye;
    Angle angle = eye != null ? eye.Rotation : new Angle();
    Matrix3x2 scale = Matrix3x2.CreateScale(new Vector2(1f, 1f));
    Matrix3x2 rotation = Matrix3Helpers.CreateRotation(Angle.op_Implicit(Angle.op_UnaryNegation(angle)));
    ((DrawingHandleBase) worldHandle).UseShader(this._shader);
    AllEntityQueryEnumerator<MoveOrderComponent, SpriteComponent, TransformComponent> entityQueryEnumerator1 = this._entity.AllEntityQueryEnumerator<MoveOrderComponent, SpriteComponent, TransformComponent>();
    EntityUid entityUid1;
    MoveOrderComponent moveOrderComponent;
    SpriteComponent spriteComponent1;
    TransformComponent transformComponent1;
    while (entityQueryEnumerator1.MoveNext(ref entityUid1, ref moveOrderComponent, ref spriteComponent1, ref transformComponent1))
      this.DrawIcon(Entity<SpriteComponent, TransformComponent>.op_Implicit((entityUid1, spriteComponent1, transformComponent1)), in args, moveOrderComponent.Icon, scale, rotation);
    AllEntityQueryEnumerator<HoldOrderComponent, SpriteComponent, TransformComponent> entityQueryEnumerator2 = this._entity.AllEntityQueryEnumerator<HoldOrderComponent, SpriteComponent, TransformComponent>();
    EntityUid entityUid2;
    HoldOrderComponent holdOrderComponent;
    SpriteComponent spriteComponent2;
    TransformComponent transformComponent2;
    while (entityQueryEnumerator2.MoveNext(ref entityUid2, ref holdOrderComponent, ref spriteComponent2, ref transformComponent2))
      this.DrawIcon(Entity<SpriteComponent, TransformComponent>.op_Implicit((entityUid2, spriteComponent2, transformComponent2)), in args, holdOrderComponent.Icon, scale, rotation);
    AllEntityQueryEnumerator<FocusOrderComponent, SpriteComponent, TransformComponent> entityQueryEnumerator3 = this._entity.AllEntityQueryEnumerator<FocusOrderComponent, SpriteComponent, TransformComponent>();
    EntityUid entityUid3;
    FocusOrderComponent focusOrderComponent;
    SpriteComponent spriteComponent3;
    TransformComponent transformComponent3;
    while (entityQueryEnumerator3.MoveNext(ref entityUid3, ref focusOrderComponent, ref spriteComponent3, ref transformComponent3))
      this.DrawIcon(Entity<SpriteComponent, TransformComponent>.op_Implicit((entityUid3, spriteComponent3, transformComponent3)), in args, focusOrderComponent.Icon, scale, rotation);
    DrawingHandleWorld drawingHandleWorld = worldHandle;
    Matrix3x2 identity = Matrix3x2.Identity;
    ref Matrix3x2 local = ref identity;
    ((DrawingHandleBase) drawingHandleWorld).SetTransform(ref local);
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
    Vector2 worldPosition = ((SharedTransformSystem) this._transform).GetWorldPosition(transformComponent2);
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
