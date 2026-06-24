// Decompiled with JetBrains decompiler
// Type: Content.Client.Singularity.SingularityOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Singularity.Components;
using Robust.Client.Graphics;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client.Singularity;

public sealed class SingularityOverlay : Overlay, IEntityEventSubscriber
{
  private static readonly ProtoId<ShaderPrototype> Shader = ProtoId<ShaderPrototype>.op_Implicit("Singularity");
  [Dependency]
  private IEntityManager _entMan;
  [Dependency]
  private IPrototypeManager _prototypeManager;
  private SharedTransformSystem? _xformSystem;
  public const int MaxCount = 5;
  private const float MaxDistance = 20f;
  private readonly ShaderInstance _shader;
  private readonly Vector2[] _positions = new Vector2[5];
  private readonly float[] _intensities = new float[5];
  private readonly float[] _falloffPowers = new float[5];
  private int _count;

  public virtual OverlaySpace Space => (OverlaySpace) 4;

  public virtual bool RequestScreenTexture => true;

  public SingularityOverlay()
  {
    IoCManager.InjectDependencies<SingularityOverlay>(this);
    this._shader = this._prototypeManager.Index<ShaderPrototype>(SingularityOverlay.Shader).Instance().Duplicate();
    this._shader.SetParameter("maxDistance", 640f);
    // ISSUE: method pointer
    ((IBroadcastEventBus) this._entMan.EventBus).SubscribeEvent<PixelToMapEvent>((EventSource) 1, (IEntityEventSubscriber) this, new EntityEventRefHandler<PixelToMapEvent>((object) this, __methodptr(OnProjectFromScreenToMap)));
    this.ZIndex = new int?(101);
  }

  protected virtual bool BeforeDraw(in OverlayDrawArgs args)
  {
    if (args.Viewport.Eye == null || this._xformSystem == null && !this._entMan.TrySystem<SharedTransformSystem>(ref this._xformSystem))
      return false;
    this._count = 0;
    EntityQueryEnumerator<SingularityDistortionComponent, TransformComponent> entityQueryEnumerator = this._entMan.EntityQueryEnumerator<SingularityDistortionComponent, TransformComponent>();
    EntityUid entityUid;
    SingularityDistortionComponent distortionComponent;
    TransformComponent transformComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref distortionComponent, ref transformComponent))
    {
      if (!MapId.op_Inequality(transformComponent.MapID, args.MapId))
      {
        Vector2 worldPosition = this._xformSystem.GetWorldPosition(entityUid);
        if ((double) (worldPosition - ((Box2) ref args.WorldAABB).ClosestPoint(ref worldPosition)).LengthSquared() <= 400.0)
        {
          Vector2 local = args.Viewport.WorldToLocal(worldPosition);
          local.Y = (float) args.Viewport.Size.Y - local.Y;
          this._positions[this._count] = local;
          this._intensities[this._count] = distortionComponent.Intensity;
          this._falloffPowers[this._count] = distortionComponent.FalloffPower;
          ++this._count;
          if (this._count == 5)
            break;
        }
      }
    }
    return this._count > 0;
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    if (this.ScreenTexture == null || args.Viewport.Eye == null)
      return;
    this._shader?.SetParameter("renderScale", args.Viewport.RenderScale * args.Viewport.Eye.Scale);
    this._shader?.SetParameter("count", this._count);
    this._shader?.SetParameter("position", this._positions);
    this._shader?.SetParameter("intensity", this._intensities);
    this._shader?.SetParameter("falloffPower", this._falloffPowers);
    this._shader?.SetParameter("SCREEN_TEXTURE", this.ScreenTexture);
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    ((DrawingHandleBase) worldHandle).UseShader(this._shader);
    worldHandle.DrawRect(args.WorldAABB, Color.White, true);
    ((DrawingHandleBase) worldHandle).UseShader((ShaderInstance) null);
  }

  private void OnProjectFromScreenToMap(ref PixelToMapEvent args)
  {
    if (args.Viewport.Eye == null)
      return;
    float num1 = 640f;
    Vector2 visiblePosition = args.VisiblePosition;
    for (int index = 0; index < 5 && index < this._count; ++index)
    {
      Vector2 position = this._positions[index];
      position.Y = (float) args.Viewport.Size.Y - position.Y;
      Vector2 vector2 = args.VisiblePosition - position;
      float x1 = (vector2 / (args.Viewport.RenderScale * args.Viewport.Eye.Scale)).Length();
      float num2 = this._intensities[index] / MathF.Pow(x1, this._falloffPowers[index]);
      float x2 = (double) x1 < (double) num1 ? num2 * (1f - MathF.Pow(x1 / num1, 4f)) : 0.0f;
      if ((double) x2 > 0.8)
        x2 = MathF.Pow(x2, 0.3f);
      visiblePosition -= vector2 * x2;
    }
    visiblePosition.X -= (float) ((double) MathF.Floor(visiblePosition.X / (float) (args.Viewport.Size.X * 2)) * (double) args.Viewport.Size.X * 2.0);
    visiblePosition.Y -= (float) ((double) MathF.Floor(visiblePosition.Y / (float) (args.Viewport.Size.Y * 2)) * (double) args.Viewport.Size.Y * 2.0);
    visiblePosition.X = (double) visiblePosition.X >= (double) args.Viewport.Size.X ? (float) (args.Viewport.Size.X * 2) - visiblePosition.X : visiblePosition.X;
    visiblePosition.Y = (double) visiblePosition.Y >= (double) args.Viewport.Size.Y ? (float) (args.Viewport.Size.Y * 2) - visiblePosition.Y : visiblePosition.Y;
    args.VisiblePosition = visiblePosition;
  }
}
