// Decompiled with JetBrains decompiler
// Type: Content.Client.DoAfter.DoAfterOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.UserInterface.Systems;
using Content.Shared._RMC14.Stealth;
using Content.Shared.DoAfter;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Containers;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client.DoAfter;

public sealed class DoAfterOverlay : Overlay
{
  private static readonly ProtoId<ShaderPrototype> UnshadedShader = ProtoId<ShaderPrototype>.op_Implicit("unshaded");
  private readonly IEntityManager _entManager;
  private readonly IGameTiming _timing;
  private readonly IPlayerManager _player;
  private readonly SharedTransformSystem _transform;
  private readonly MetaDataSystem _meta;
  private readonly ProgressColorSystem _progressColor;
  private readonly SharedContainerSystem _container;
  private readonly SpriteSystem _sprite;
  private readonly Texture _barTexture;
  private readonly ShaderInstance _unshadedShader;
  private const float FlashTime = 0.125f;
  private const float StartX = 2f;
  private const float EndX = 22f;

  public virtual OverlaySpace Space => (OverlaySpace) 8;

  public DoAfterOverlay(
    IEntityManager entManager,
    IPrototypeManager protoManager,
    IGameTiming timing,
    IPlayerManager player)
  {
    this._entManager = entManager;
    this._timing = timing;
    this._player = player;
    this._transform = this._entManager.EntitySysManager.GetEntitySystem<SharedTransformSystem>();
    this._meta = this._entManager.EntitySysManager.GetEntitySystem<MetaDataSystem>();
    this._container = this._entManager.EntitySysManager.GetEntitySystem<SharedContainerSystem>();
    this._progressColor = this._entManager.System<ProgressColorSystem>();
    this._sprite = this._entManager.System<SpriteSystem>();
    SpriteSpecifier.Rsi rsi = new SpriteSpecifier.Rsi(new ResPath("/Textures/Interface/Misc/progress_bar.rsi"), "icon");
    this._barTexture = this._entManager.EntitySysManager.GetEntitySystem<SpriteSystem>().Frame0((SpriteSpecifier) rsi);
    this._unshadedShader = protoManager.Index<ShaderPrototype>(DoAfterOverlay.UnshadedShader).Instance();
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    IEye eye = args.Viewport.Eye;
    Angle angle = eye != null ? eye.Rotation : Angle.Zero;
    EntityQuery<TransformComponent> entityQuery1 = this._entManager.GetEntityQuery<TransformComponent>();
    Vector2 vector2_1 = new Vector2(1f, 1f);
    Matrix3x2 scale = Matrix3Helpers.CreateScale(ref vector2_1);
    Matrix3x2 rotation = Matrix3Helpers.CreateRotation(Angle.op_Implicit(Angle.op_UnaryNegation(angle)));
    TimeSpan curTime = this._timing.CurTime;
    Box2 box2_1 = ((Box2) ref args.WorldAABB).Enlarged(5f);
    EntityUid? attachedEntity = (EntityUid?) ((ISharedPlayerManager) this._player).LocalSession?.AttachedEntity;
    EntityQuery<MetaDataComponent> entityQuery2 = this._entManager.GetEntityQuery<MetaDataComponent>();
    AllEntityQueryEnumerator<ActiveDoAfterComponent, DoAfterComponent, SpriteComponent, TransformComponent> entityQueryEnumerator = this._entManager.AllEntityQueryEnumerator<ActiveDoAfterComponent, DoAfterComponent, SpriteComponent, TransformComponent>();
    EntityUid entityUid1;
    ActiveDoAfterComponent doAfterComponent1;
    DoAfterComponent doAfterComponent2;
    SpriteComponent spriteComponent;
    TransformComponent transformComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid1, ref doAfterComponent1, ref doAfterComponent2, ref spriteComponent, ref transformComponent))
    {
      if (!MapId.op_Inequality(transformComponent.MapID, args.MapId) && doAfterComponent2.DoAfters.Count != 0)
      {
        Vector2 worldPosition = this._transform.GetWorldPosition(transformComponent, entityQuery1);
        if (((Box2) ref box2_1).Contains(worldPosition, true))
        {
          EntityUid entityUid2 = entityUid1;
          EntityUid? nullable1 = attachedEntity;
          if ((nullable1.HasValue ? (EntityUid.op_Inequality(entityUid2, nullable1.GetValueOrDefault()) ? 1 : 0) : 1) != 0)
            ((DrawingHandleBase) worldHandle).UseShader((ShaderInstance) null);
          else
            ((DrawingHandleBase) worldHandle).UseShader(this._unshadedShader);
          MetaDataComponent component = entityQuery2.GetComponent(entityUid1);
          TimeSpan timeSpan = component.EntityPaused ? curTime - this._meta.GetPauseTime(entityUid1, component) : curTime;
          Matrix3x2 translation = Matrix3Helpers.CreateTranslation(worldPosition);
          Matrix3x2 matrix3x2_1 = Matrix3x2.Multiply(scale, translation);
          Matrix3x2 matrix3x2_2 = Matrix3x2.Multiply(rotation, matrix3x2_1);
          ((DrawingHandleBase) worldHandle).SetTransform(ref matrix3x2_2);
          float num = 0.0f;
          bool flag = this._container.IsEntityOrParentInContainer(entityUid1, component, transformComponent);
          foreach (Content.Shared.DoAfter.DoAfter doAfter in doAfterComponent2.DoAfters.Values)
          {
            float alpha = 1f;
            if (doAfter.Args.Hidden | flag)
            {
              EntityUid entityUid3 = entityUid1;
              nullable1 = attachedEntity;
              if ((nullable1.HasValue ? (EntityUid.op_Inequality(entityUid3, nullable1.GetValueOrDefault()) ? 1 : 0) : 1) == 0)
                alpha = 0.5f;
              else
                continue;
            }
            if (!doAfter.Args.ForceVisible)
            {
              if (!spriteComponent.Visible)
              {
                EntityUid entityUid4 = entityUid1;
                nullable1 = attachedEntity;
                if ((nullable1.HasValue ? (EntityUid.op_Inequality(entityUid4, nullable1.GetValueOrDefault()) ? 1 : 0) : 1) != 0)
                  continue;
              }
              alpha = spriteComponent.Color.A;
              EntityActiveInvisibleComponent invisibleComponent;
              this._entManager.GetEntityQuery<EntityActiveInvisibleComponent>().TryGetComponent(entityUid1, ref invisibleComponent);
              if (invisibleComponent != null)
                alpha = invisibleComponent.Opacity;
            }
            Box2 localBounds = this._sprite.GetLocalBounds(Entity<SpriteComponent>.op_Implicit((entityUid1, spriteComponent)));
            Vector2 vector2_2 = new Vector2((float) ((double) -this._barTexture.Width / 2.0 / 32.0), (float) (((double) ((Box2) ref localBounds).Height / 2.0 + 0.05000000074505806) / 1.0 + (double) num / 32.0 * 1.0));
            DrawingHandleWorld drawingHandleWorld = worldHandle;
            Texture barTexture = this._barTexture;
            Vector2 vector2_3 = vector2_2;
            Color white = Color.White;
            Color? nullable2 = new Color?(((Color) ref white).WithAlpha(alpha));
            ((DrawingHandleBase) drawingHandleWorld).DrawTexture(barTexture, vector2_3, nullable2);
            float progress;
            Color progressColor;
            if (doAfter.CancelledTime.HasValue)
            {
              progress = (float) Math.Min(1.0, (doAfter.CancelledTime.Value - doAfter.StartTime).TotalSeconds / doAfter.Args.Delay.TotalSeconds);
              progressColor = this.GetProgressColor(0.0f, Math.Floor((timeSpan - doAfter.CancelledTime.Value).TotalSeconds / 0.125) % 2.0 == 0.0 ? alpha : 0.0f);
            }
            else
            {
              progress = (float) Math.Min(1.0, (timeSpan - doAfter.StartTime).TotalSeconds / doAfter.Args.Delay.TotalSeconds);
              progressColor = this.GetProgressColor(progress, alpha);
            }
            float x = (float) (20.0 * (double) progress + 2.0);
            Box2 box2_2;
            // ISSUE: explicit constructor call
            ((Box2) ref box2_2).\u002Ector(new Vector2(2f, 3f) / 32f, new Vector2(x, 4f) / 32f);
            box2_2 = ((Box2) ref box2_2).Translated(vector2_2);
            worldHandle.DrawRect(box2_2, progressColor, true);
            num += (float) this._barTexture.Height / 1f;
          }
        }
      }
    }
    ((DrawingHandleBase) worldHandle).UseShader((ShaderInstance) null);
    DrawingHandleWorld drawingHandleWorld1 = worldHandle;
    Matrix3x2 identity = Matrix3x2.Identity;
    ref Matrix3x2 local = ref identity;
    ((DrawingHandleBase) drawingHandleWorld1).SetTransform(ref local);
  }

  public Color GetProgressColor(float progress, float alpha = 1f)
  {
    Color progressColor = this._progressColor.GetProgressColor(progress);
    return ((Color) ref progressColor).WithAlpha(alpha);
  }
}
