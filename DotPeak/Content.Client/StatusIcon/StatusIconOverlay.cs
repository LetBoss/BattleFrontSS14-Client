// Decompiled with JetBrains decompiler
// Type: Content.Client.StatusIcon.StatusIconOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.CrashLand;
using Content.Shared.ParaDrop;
using Content.Shared.StatusIcon;
using Content.Shared.StatusIcon.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client.StatusIcon;

public sealed class StatusIconOverlay : Overlay
{
  private static readonly ProtoId<ShaderPrototype> UnshadedShader = ProtoId<ShaderPrototype>.op_Implicit("unshaded");
  [Dependency]
  private IEntityManager _entity;
  [Dependency]
  private IPrototypeManager _prototype;
  [Dependency]
  private IGameTiming _timing;
  private readonly SpriteSystem _sprite;
  private readonly TransformSystem _transform;
  private readonly StatusIconSystem _statusIcon;
  private readonly ShaderInstance _unshadedShader;
  private readonly EntityQuery<TransformComponent> _xformQuery;

  public virtual OverlaySpace Space => (OverlaySpace) 8;

  internal StatusIconOverlay()
  {
    IoCManager.InjectDependencies<StatusIconOverlay>(this);
    this._sprite = this._entity.System<SpriteSystem>();
    this._transform = this._entity.System<TransformSystem>();
    this._statusIcon = this._entity.System<StatusIconSystem>();
    this._unshadedShader = this._prototype.Index<ShaderPrototype>(StatusIconOverlay.UnshadedShader).Instance();
    this._xformQuery = this._entity.GetEntityQuery<TransformComponent>();
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    IEye eye = args.Viewport.Eye;
    Matrix3x2 rotation = Matrix3Helpers.CreateRotation(Angle.op_Implicit(Angle.op_UnaryNegation(eye != null ? eye.Rotation : new Angle())));
    AllEntityQueryEnumerator<StatusIconComponent, SpriteComponent, TransformComponent, MetaDataComponent> entityQueryEnumerator = this._entity.AllEntityQueryEnumerator<StatusIconComponent, SpriteComponent, TransformComponent, MetaDataComponent>();
    EntityUid uid;
    StatusIconComponent statusIconComponent;
    SpriteComponent spriteComponent;
    TransformComponent transformComponent;
    MetaDataComponent meta;
    while (entityQueryEnumerator.MoveNext(ref uid, ref statusIconComponent, ref spriteComponent, ref transformComponent, ref meta))
    {
      if (!MapId.op_Inequality(transformComponent.MapID, args.MapId) && spriteComponent.Visible)
      {
        Box2 box2_1 = statusIconComponent.Bounds ?? this._sprite.GetLocalBounds(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)));
        Vector2 worldPosition = ((SharedTransformSystem) this._transform).GetWorldPosition(transformComponent, this._xformQuery);
        Box2 box2_2 = ((Box2) ref box2_1).Translated(worldPosition);
        if (((Box2) ref box2_2).Intersects(ref args.WorldAABB))
        {
          List<StatusIconData> statusIcons = this._statusIcon.GetStatusIcons(uid, meta);
          if (statusIcons.Count != 0)
          {
            Matrix3x2 translation = Matrix3Helpers.CreateTranslation(worldPosition);
            Matrix3x2 matrix3x2 = Matrix3x2.Multiply(rotation, translation);
            ((DrawingHandleBase) worldHandle).SetTransform(ref matrix3x2);
            int num1 = 0;
            int num2 = 0;
            int num3 = 0;
            int num4 = 0;
            statusIcons.Sort();
            foreach (StatusIconData data in statusIcons)
            {
              if (this._statusIcon.IsVisible(Entity<MetaDataComponent>.op_Implicit((uid, meta)), data))
              {
                TimeSpan realTime = this._timing.RealTime;
                Texture frame = this._sprite.GetFrame(data.Icon, realTime, true);
                float y;
                float x;
                if (data.LocationPreference == StatusIconLocationPreference.Left || data.LocationPreference == StatusIconLocationPreference.None && num1 <= num2)
                {
                  double num5 = (double) (num3 + frame.Height);
                  box2_2 = this._sprite.GetLocalBounds(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)));
                  double num6 = (double) ((Box2) ref box2_2).Height * 32.0;
                  if (num5 <= num6)
                  {
                    if (data.Layer == StatusIconLayer.Base)
                    {
                      num3 += frame.Height;
                      ++num1;
                    }
                    y = (float) (((double) ((Box2) ref box2_1).Height + (double) spriteComponent.Offset.Y) / 2.0 - (double) (num3 - data.Offset) / 32.0);
                    x = (float) (-((double) ((Box2) ref box2_1).Width + (double) spriteComponent.Offset.X) / 2.0);
                    if (this._entity.HasComponent<CrashLandingComponent>(uid) || this._entity.HasComponent<ParaDroppingComponent>(uid))
                      y = 0.25f + spriteComponent.Offset.Y;
                  }
                  else
                    break;
                }
                else
                {
                  double num7 = (double) (num4 + frame.Height);
                  box2_2 = this._sprite.GetLocalBounds(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)));
                  double num8 = (double) ((Box2) ref box2_2).Height * 32.0;
                  if (num7 <= num8)
                  {
                    if (data.Layer == StatusIconLayer.Base)
                    {
                      num4 += frame.Height;
                      ++num2;
                    }
                    y = (float) (((double) ((Box2) ref box2_1).Height + (double) spriteComponent.Offset.Y) / 2.0 - (double) (num4 - data.Offset) / 32.0);
                    x = (float) (((double) ((Box2) ref box2_1).Width + (double) spriteComponent.Offset.X) / 2.0 - (double) frame.Width / 32.0);
                    if (this._entity.HasComponent<CrashLandingComponent>(uid) || this._entity.HasComponent<ParaDroppingComponent>(uid))
                      y = 0.25f + spriteComponent.Offset.Y;
                  }
                  else
                    break;
                }
                if (data.IsShaded)
                  ((DrawingHandleBase) worldHandle).UseShader((ShaderInstance) null);
                else
                  ((DrawingHandleBase) worldHandle).UseShader(this._unshadedShader);
                Vector2 vector2 = new Vector2(x, y);
                ((DrawingHandleBase) worldHandle).DrawTexture(frame, vector2, new Color?());
              }
            }
            ((DrawingHandleBase) worldHandle).UseShader((ShaderInstance) null);
            DrawingHandleWorld drawingHandleWorld = worldHandle;
            Matrix3x2 identity = Matrix3x2.Identity;
            ref Matrix3x2 local = ref identity;
            ((DrawingHandleBase) drawingHandleWorld).SetTransform(ref local);
          }
        }
      }
    }
  }
}
