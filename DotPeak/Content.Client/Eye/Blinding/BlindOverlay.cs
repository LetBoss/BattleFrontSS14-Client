// Decompiled with JetBrains decompiler
// Type: Content.Client.Eye.Blinding.BlindOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Eye.Blinding.Components;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

#nullable enable
namespace Content.Client.Eye.Blinding;

public sealed class BlindOverlay : Overlay
{
  private static readonly ProtoId<ShaderPrototype> GreyscaleShader = ProtoId<ShaderPrototype>.op_Implicit("GreyscaleFullscreen");
  private static readonly ProtoId<ShaderPrototype> CircleShader = ProtoId<ShaderPrototype>.op_Implicit("CircleMask");
  [Dependency]
  private IPrototypeManager _prototypeManager;
  [Dependency]
  private IPlayerManager _playerManager;
  [Dependency]
  private IEntityManager _entityManager;
  [Dependency]
  private ILightManager _lightManager;
  private readonly ShaderInstance _greyscaleShader;
  private readonly ShaderInstance _circleMaskShader;
  private BlindableComponent _blindableComponent;

  public virtual bool RequestScreenTexture => true;

  public virtual OverlaySpace Space => (OverlaySpace) 4;

  public BlindOverlay()
  {
    IoCManager.InjectDependencies<BlindOverlay>(this);
    this._greyscaleShader = this._prototypeManager.Index<ShaderPrototype>(BlindOverlay.GreyscaleShader).InstanceUnique();
    this._circleMaskShader = this._prototypeManager.Index<ShaderPrototype>(BlindOverlay.CircleShader).InstanceUnique();
  }

  protected virtual bool BeforeDraw(in OverlayDrawArgs args)
  {
    EyeComponent eyeComponent;
    if (!this._entityManager.TryGetComponent<EyeComponent>((EntityUid?) ((ISharedPlayerManager) this._playerManager).LocalSession?.AttachedEntity, ref eyeComponent) || args.Viewport.Eye != eyeComponent.Eye)
      return false;
    EntityUid? attachedEntity = (EntityUid?) ((ISharedPlayerManager) this._playerManager).LocalSession?.AttachedEntity;
    BlindableComponent blindableComponent;
    if (!attachedEntity.HasValue || !this._entityManager.TryGetComponent<BlindableComponent>(attachedEntity, ref blindableComponent))
      return false;
    this._blindableComponent = blindableComponent;
    bool isBlind = this._blindableComponent.IsBlind;
    if (isBlind || !this._blindableComponent.LightSetup)
      return isBlind;
    this._lightManager.Enabled = true;
    this._blindableComponent.LightSetup = false;
    this._blindableComponent.GraceFrame = true;
    return true;
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    if (this.ScreenTexture == null)
      return;
    EntityUid? attachedEntity = (EntityUid?) ((ISharedPlayerManager) this._playerManager).LocalSession?.AttachedEntity;
    if (!attachedEntity.HasValue)
      return;
    if (!this._blindableComponent.GraceFrame)
    {
      this._blindableComponent.LightSetup = true;
      this._lightManager.Enabled = false;
    }
    else
      this._blindableComponent.GraceFrame = false;
    EyeComponent eyeComponent;
    if (this._entityManager.TryGetComponent<EyeComponent>(attachedEntity, ref eyeComponent))
      this._circleMaskShader?.SetParameter("Zoom", eyeComponent.Zoom.X);
    this._greyscaleShader?.SetParameter("SCREEN_TEXTURE", this.ScreenTexture);
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    Box2Rotated worldBounds = args.WorldBounds;
    ((DrawingHandleBase) worldHandle).UseShader(this._greyscaleShader);
    worldHandle.DrawRect(ref worldBounds, Color.White, true);
    ((DrawingHandleBase) worldHandle).UseShader(this._circleMaskShader);
    worldHandle.DrawRect(ref worldBounds, Color.White, true);
    ((DrawingHandleBase) worldHandle).UseShader((ShaderInstance) null);
  }
}
