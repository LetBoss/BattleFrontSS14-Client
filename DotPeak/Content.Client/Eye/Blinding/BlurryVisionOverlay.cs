// Decompiled with JetBrains decompiler
// Type: Content.Client.Eye.Blinding.BlurryVisionOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.CCVar;
using Content.Shared.Eye.Blinding.Components;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Configuration;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using System;

#nullable enable
namespace Content.Client.Eye.Blinding;

public sealed class BlurryVisionOverlay : Overlay
{
  private static readonly ProtoId<ShaderPrototype> CataractsShader = ProtoId<ShaderPrototype>.op_Implicit("Cataracts");
  private static readonly ProtoId<ShaderPrototype> CircleShader = ProtoId<ShaderPrototype>.op_Implicit("CircleMask");
  [Dependency]
  private IEntityManager _entityManager;
  [Dependency]
  private IPlayerManager _playerManager;
  [Dependency]
  private IPrototypeManager _prototypeManager;
  [Dependency]
  private IConfigurationManager _configManager;
  private readonly ShaderInstance _cataractsShader;
  private readonly ShaderInstance _circleMaskShader;
  private float _magnitude;
  private float _correctionPower = 2f;
  private const float Distortion_Pow = 2f;
  private const float Cloudiness_Pow = 1f;
  private const float NoMotion_Radius = 30f;
  private const float NoMotion_Pow = 0.2f;
  private const float NoMotion_Max = 8f;
  private const float NoMotion_Mult = 0.75f;

  public virtual bool RequestScreenTexture => true;

  public virtual OverlaySpace Space => (OverlaySpace) 4;

  public BlurryVisionOverlay()
  {
    IoCManager.InjectDependencies<BlurryVisionOverlay>(this);
    this._cataractsShader = this._prototypeManager.Index<ShaderPrototype>(BlurryVisionOverlay.CataractsShader).InstanceUnique();
    this._circleMaskShader = this._prototypeManager.Index<ShaderPrototype>(BlurryVisionOverlay.CircleShader).InstanceUnique();
    this._circleMaskShader.SetParameter("CircleMinDist", 0.0f);
    this._circleMaskShader.SetParameter("CirclePow", 0.2f);
    this._circleMaskShader.SetParameter("CircleMax", 8f);
    this._circleMaskShader.SetParameter("CircleMult", 0.75f);
  }

  protected virtual bool BeforeDraw(in OverlayDrawArgs args)
  {
    EyeComponent eyeComponent;
    if (!this._entityManager.TryGetComponent<EyeComponent>((EntityUid?) ((ISharedPlayerManager) this._playerManager).LocalSession?.AttachedEntity, ref eyeComponent) || args.Viewport.Eye != eyeComponent.Eye)
      return false;
    EntityUid? attachedEntity = (EntityUid?) ((ISharedPlayerManager) this._playerManager).LocalSession?.AttachedEntity;
    BlurryVisionComponent blurryVisionComponent;
    BlindableComponent blindableComponent;
    if (!attachedEntity.HasValue || !this._entityManager.TryGetComponent<BlurryVisionComponent>(attachedEntity, ref blurryVisionComponent) || (double) blurryVisionComponent.Magnitude <= 0.0 || this._entityManager.TryGetComponent<BlindableComponent>(attachedEntity, ref blindableComponent) && blindableComponent.IsBlind)
      return false;
    this._magnitude = blurryVisionComponent.Magnitude;
    this._correctionPower = blurryVisionComponent.CorrectionPower;
    return true;
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    if (this.ScreenTexture == null)
      return;
    EntityUid? attachedEntity = (EntityUid?) ((ISharedPlayerManager) this._playerManager).LocalSession?.AttachedEntity;
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    Box2Rotated worldBounds = args.WorldBounds;
    float x = (float) Math.Pow((double) Math.Min(this._magnitude / 6f, 1f), (double) this._correctionPower);
    float num = 1f;
    EyeComponent eyeComponent;
    if (this._entityManager.TryGetComponent<EyeComponent>(attachedEntity, ref eyeComponent))
      num = eyeComponent.Zoom.X;
    if (this._configManager.GetCVar<bool>(CCVars.ReducedMotion))
    {
      this._circleMaskShader.SetParameter("SCREEN_TEXTURE", this.ScreenTexture);
      this._circleMaskShader.SetParameter("Zoom", num);
      this._circleMaskShader.SetParameter("CircleRadius", 30f / x);
      ((DrawingHandleBase) worldHandle).UseShader(this._circleMaskShader);
      worldHandle.DrawRect(ref worldBounds, Color.White, true);
      ((DrawingHandleBase) worldHandle).UseShader((ShaderInstance) null);
    }
    else
    {
      this._cataractsShader.SetParameter("SCREEN_TEXTURE", this.ScreenTexture);
      this._cataractsShader.SetParameter("LIGHT_TEXTURE", args.Viewport.LightRenderTarget.Texture);
      this._cataractsShader.SetParameter("Zoom", num);
      this._cataractsShader.SetParameter("DistortionScalar", (float) Math.Pow((double) x, 2.0));
      this._cataractsShader.SetParameter("CloudinessScalar", (float) Math.Pow((double) x, 1.0));
      ((DrawingHandleBase) worldHandle).UseShader(this._cataractsShader);
      worldHandle.DrawRect(ref worldBounds, Color.White, true);
      ((DrawingHandleBase) worldHandle).UseShader((ShaderInstance) null);
    }
  }
}
