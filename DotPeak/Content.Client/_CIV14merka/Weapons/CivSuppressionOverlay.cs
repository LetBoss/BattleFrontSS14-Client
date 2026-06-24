// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Weapons.CivSuppressionOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._CIV14merka.Weapons;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client._CIV14merka.Weapons;

public sealed class CivSuppressionOverlay : Overlay
{
  private static readonly ProtoId<ShaderPrototype> SuppressionShader = ProtoId<ShaderPrototype>.op_Implicit("CivSuppression");
  private static readonly ProtoId<ShaderPrototype> VignetteShader = ProtoId<ShaderPrototype>.op_Implicit("GradientCircleMask");
  [Dependency]
  private IPrototypeManager _prototypeManager;
  private readonly CivSuppressionSystem _system;
  private readonly ShaderInstance _suppressionShader;
  private readonly ShaderInstance _vignetteShader;
  private float _strength;
  private float _stress;
  private float _pulse;
  private CivSuppressionVisualProfile _profile;

  public virtual OverlaySpace Space => (OverlaySpace) 4;

  public virtual bool RequestScreenTexture => true;

  public CivSuppressionOverlay(CivSuppressionSystem system)
  {
    IoCManager.InjectDependencies<CivSuppressionOverlay>(this);
    this._system = system;
    this._suppressionShader = this._prototypeManager.Index<ShaderPrototype>(CivSuppressionOverlay.SuppressionShader).InstanceUnique();
    this._vignetteShader = this._prototypeManager.Index<ShaderPrototype>(CivSuppressionOverlay.VignetteShader).InstanceUnique();
    this.ZIndex = new int?(500);
  }

  protected virtual void DisposeBehavior()
  {
    base.DisposeBehavior();
    this._suppressionShader.Dispose();
    this._vignetteShader.Dispose();
  }

  protected virtual bool BeforeDraw(in OverlayDrawArgs args)
  {
    this._strength = this._system.CurrentIntensity;
    this._stress = this._system.Stress;
    this._pulse = this._system.Pulse;
    this._profile = this._system.VisualProfile;
    return this._system.Active;
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    Box2 worldAabb = args.WorldAABB;
    if (this.ScreenTexture != null)
    {
      this._suppressionShader.SetParameter("SCREEN_TEXTURE", this.ScreenTexture);
      this._suppressionShader.SetParameter("strength", this._strength);
      this._suppressionShader.SetParameter("stress", this._stress);
      this._suppressionShader.SetParameter("pulse", this._pulse);
      this._suppressionShader.SetParameter("profile", (float) this._profile);
      ((DrawingHandleBase) worldHandle).UseShader(this._suppressionShader);
      worldHandle.DrawRect(worldAabb, Color.White, true);
      ((DrawingHandleBase) worldHandle).UseShader((ShaderInstance) null);
    }
    float num1;
    Color color;
    float num2;
    float num3;
    float num4;
    float num5;
    Vector3 vector3;
    switch (this._profile)
    {
      case CivSuppressionVisualProfile.Explosion:
        num1 = Math.Clamp((float) ((double) this._pulse * 0.14000000059604645 + (double) this._strength * 0.10000000149011612 + (double) this._stress * 0.11999999731779099), 0.0f, 0.32f);
        // ISSUE: explicit constructor call
        ((Color) ref color).\u002Ector((byte) 118, (byte) 120, (byte) 124, (byte) ((double) num1 * (double) byte.MaxValue));
        num2 = Math.Clamp((float) (0.86000001430511475 - (double) this._pulse * 0.10000000149011612 - (double) this._strength * 0.059999998658895493), 0.58f, 0.92f);
        num3 = Math.Clamp((float) (0.984000027179718 - (double) this._pulse * 0.019999999552965164 - (double) this._stress * 0.017999999225139618), 0.9f, 1f);
        num4 = Math.Clamp((float) ((double) this._pulse * 0.2199999988079071 + (double) this._strength * 0.14000000059604645 + (double) this._stress * 0.11999999731779099), 0.0f, 0.52f);
        num5 = Math.Clamp((float) ((double) this._pulse * 0.029999999329447746 + (double) this._strength * 0.019999999552965164), 0.0f, 0.08f);
        vector3 = new Vector3(0.02f, 0.021f, 23f / 1000f);
        break;
      case CivSuppressionVisualProfile.Mortar:
        num1 = Math.Clamp((float) ((double) this._stress * 0.23999999463558197 + (double) this._strength * 0.14000000059604645 + (double) this._pulse * 0.039999999105930328), 0.0f, 0.36f);
        // ISSUE: explicit constructor call
        ((Color) ref color).\u002Ector((byte) 112 /*0x70*/, (byte) 106, (byte) 98, (byte) ((double) num1 * (double) byte.MaxValue));
        num2 = Math.Clamp((float) (0.74000000953674316 - (double) this._stress * 0.20000000298023224 - (double) this._strength * 0.079999998211860657 - (double) this._pulse * 0.019999999552965164), 0.42f, 0.82f);
        num3 = Math.Clamp((float) (0.95800000429153442 - (double) this._stress * 0.05000000074505806 - (double) this._pulse * 0.016000000759959221), 0.82f, 0.99f);
        num4 = Math.Clamp((float) ((double) this._stress * 0.74000000953674316 + (double) this._strength * 0.14000000059604645 + (double) this._pulse * 0.059999998658895493), 0.0f, 0.84f);
        num5 = Math.Clamp((float) ((double) this._stress * 0.079999998211860657 + (double) this._strength * 0.02500000037252903), 0.0f, 0.14f);
        vector3 = new Vector3(0.021f, 0.018f, 0.014f);
        break;
      default:
        num1 = Math.Clamp((float) ((double) this._stress * 0.34000000357627869 + (double) this._strength * 0.11999999731779099 + (double) this._pulse * 0.029999999329447746), 0.0f, 0.5f);
        // ISSUE: explicit constructor call
        ((Color) ref color).\u002Ector((byte) 80 /*0x50*/, (byte) 85, (byte) 92, (byte) ((double) num1 * (double) byte.MaxValue));
        num2 = Math.Clamp((float) (0.75999999046325684 - (double) this._stress * 0.2800000011920929 - (double) this._strength * 0.18000000715255737 - (double) this._pulse * 0.05000000074505806), 0.3f, 0.82f);
        num3 = Math.Clamp((float) (0.972000002861023 - (double) this._stress * 0.059999998658895493 - (double) this._strength * 0.039999999105930328), 0.8f, 1f);
        num4 = Math.Clamp((float) ((double) this._stress * 0.89999997615814209 + (double) this._strength * 0.2199999988079071 + (double) this._pulse * 0.059999998658895493), 0.0f, 0.995f);
        num5 = Math.Clamp((float) ((double) this._stress * 0.14000000059604645 + (double) this._strength * 0.039999999105930328), 0.0f, 0.22f);
        vector3 = new Vector3(0.005f, 3f / 500f, 0.008f);
        break;
    }
    if ((double) num1 > 1.0 / 1000.0)
      worldHandle.DrawRect(worldAabb, color, true);
    float num6 = MathF.Max((float) ((UIBox2i) ref args.ViewportBounds).Width, (float) ((UIBox2i) ref args.ViewportBounds).Height);
    float num7 = (float) ((double) num2 * (double) num6 * 0.5);
    float num8 = (float) ((double) num3 * (double) num6 * 0.5);
    this._vignetteShader.SetParameter("color", vector3);
    this._vignetteShader.SetParameter("darknessAlphaOuter", num4);
    this._vignetteShader.SetParameter("darknessAlphaInner", num5);
    this._vignetteShader.SetParameter("innerCircleRadius", num7);
    this._vignetteShader.SetParameter("innerCircleMaxRadius", num7);
    this._vignetteShader.SetParameter("outerCircleRadius", num8);
    this._vignetteShader.SetParameter("outerCircleMaxRadius", num8);
    ((DrawingHandleBase) worldHandle).UseShader(this._vignetteShader);
    worldHandle.DrawRect(worldAabb, Color.White, true);
    ((DrawingHandleBase) worldHandle).UseShader((ShaderInstance) null);
  }
}
