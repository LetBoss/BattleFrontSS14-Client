// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Xenonids.HiveLeader.HiveLeaderOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._RMC14.NightVision;
using Content.Shared._RMC14.Mobs;
using Content.Shared._RMC14.Xenonids;
using Content.Shared._RMC14.Xenonids.HiveLeader;
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
namespace Content.Client._RMC14.Xenonids.HiveLeader;

public sealed class HiveLeaderOverlay : Overlay
{
  [Dependency]
  private IEntityManager _entity;
  [Dependency]
  private IOverlayManager _overlay;
  [Dependency]
  private IPlayerManager _players;
  [Dependency]
  private IPrototypeManager _prototype;
  [Dependency]
  private IGameTiming _timing;
  private readonly SpriteSystem _sprite;
  private readonly TransformSystem _transform;
  private readonly EntityQuery<TransformComponent> _xformQuery;
  private readonly ShaderInstance _shader;

  public virtual OverlaySpace Space
  {
    get => !this._overlay.HasOverlay<NightVisionOverlay>() ? (OverlaySpace) 8 : (OverlaySpace) 4;
  }

  public HiveLeaderOverlay()
  {
    IoCManager.InjectDependencies<HiveLeaderOverlay>(this);
    this._sprite = this._entity.System<SpriteSystem>();
    this._transform = this._entity.System<TransformSystem>();
    this._xformQuery = this._entity.GetEntityQuery<TransformComponent>();
    this._shader = this._prototype.Index<ShaderPrototype>(new ProtoId<ShaderPrototype>("unshaded")).Instance();
    this.ZIndex = new int?(1);
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
    SpriteSpecifier.Rsi rsi = new SpriteSpecifier.Rsi(new ResPath("/Textures/_RMC14/Interface/xeno_leader.rsi"), "hudxenoleader");
    ((DrawingHandleBase) worldHandle).UseShader(this._shader);
    EntityQueryEnumerator<HiveLeaderComponent, SpriteComponent, TransformComponent> entityQueryEnumerator = this._entity.EntityQueryEnumerator<HiveLeaderComponent, SpriteComponent, TransformComponent>();
    EntityUid entityUid;
    HiveLeaderComponent hiveLeaderComponent;
    SpriteComponent spriteComponent;
    TransformComponent transformComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref hiveLeaderComponent, ref spriteComponent, ref transformComponent))
    {
      if (!MapId.op_Inequality(transformComponent.MapID, args.MapId))
      {
        Box2 localBounds = this._sprite.GetLocalBounds(Entity<SpriteComponent>.op_Implicit((entityUid, spriteComponent)));
        Vector2 worldPosition = ((SharedTransformSystem) this._transform).GetWorldPosition(transformComponent, this._xformQuery);
        Box2 box2 = ((Box2) ref localBounds).Translated(worldPosition);
        if (((Box2) ref box2).Intersects(ref args.WorldAABB))
        {
          Matrix3x2 translation = Matrix3x2.CreateTranslation(worldPosition);
          Matrix3x2 matrix3x2_1 = Matrix3x2.Multiply(scale, translation);
          Matrix3x2 matrix3x2_2 = Matrix3x2.Multiply(rotation, matrix3x2_1);
          ((DrawingHandleBase) worldHandle).SetTransform(ref matrix3x2_2);
          Texture frame = this._sprite.GetFrame((SpriteSpecifier) rsi, this._timing.CurTime, true);
          float y = (float) (((double) ((Box2) ref localBounds).Height + (double) spriteComponent.Offset.Y) / 2.0 - (double) frame.Height / 32.0 * (double) ((Box2) ref localBounds).Height + 0.15000000596046448);
          Vector2 vector2 = new Vector2((float) (((double) ((Box2) ref localBounds).Width + (double) spriteComponent.Offset.X) / 2.0 - (double) frame.Width / 32.0 - 0.60000002384185791), y);
          ((DrawingHandleBase) worldHandle).DrawTexture(frame, vector2, new Color?());
        }
      }
    }
    ((DrawingHandleBase) worldHandle).UseShader((ShaderInstance) null);
  }
}
