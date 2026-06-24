// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Systems.DamageOverlays.Overlays.DamageOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Mobs;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client.UserInterface.Systems.DamageOverlays.Overlays;

public sealed class DamageOverlay : Overlay
{
  private static readonly ProtoId<ShaderPrototype> CircleMaskShader = ProtoId<ShaderPrototype>.op_Implicit("GradientCircleMask");
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private IPrototypeManager _prototypeManager;
  [Dependency]
  private IEntityManager _entityManager;
  [Dependency]
  private IPlayerManager _playerManager;
  private readonly ShaderInstance _critShader;
  private readonly ShaderInstance _oxygenShader;
  private readonly ShaderInstance _bruteShader;
  public MobState State = MobState.Alive;
  public float PainLevel;
  private float _oldPainLevel;
  public float OxygenLevel;
  private float _oldOxygenLevel;
  public float CritLevel;
  private float _oldCritLevel;
  public float DeadLevel = 1f;

  public virtual OverlaySpace Space => (OverlaySpace) 4;

  public DamageOverlay()
  {
    IoCManager.InjectDependencies<DamageOverlay>(this);
    this._oxygenShader = this._prototypeManager.Index<ShaderPrototype>(DamageOverlay.CircleMaskShader).InstanceUnique();
    this._critShader = this._prototypeManager.Index<ShaderPrototype>(DamageOverlay.CircleMaskShader).InstanceUnique();
    this._bruteShader = this._prototypeManager.Index<ShaderPrototype>(DamageOverlay.CircleMaskShader).InstanceUnique();
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    EyeComponent eyeComponent;
    if (!this._entityManager.TryGetComponent<EyeComponent>(((ISharedPlayerManager) this._playerManager).LocalEntity, ref eyeComponent) || args.Viewport.Eye != eyeComponent.Eye)
      return;
    Box2 worldAabb = args.WorldAABB;
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    int width = ((UIBox2i) ref args.ViewportBounds).Width;
    TimeSpan timeSpan = this._timing.RealTime;
    float totalSeconds1 = (float) timeSpan.TotalSeconds;
    timeSpan = this._timing.FrameTime;
    float totalSeconds2 = (float) timeSpan.TotalSeconds;
    if (this.State != MobState.Dead)
      this.DeadLevel = 1f;
    else if (!MathHelper.CloseTo(0.0f, this.DeadLevel, 1f / 1000f))
      this.DeadLevel += this.GetDiff(-this.DeadLevel, totalSeconds2);
    else
      this.DeadLevel = 0.0f;
    if (!MathHelper.CloseTo(this._oldPainLevel, this.PainLevel, 1f / 1000f))
      this._oldPainLevel += this.GetDiff(this.PainLevel - this._oldPainLevel, totalSeconds2);
    else
      this._oldPainLevel = this.PainLevel;
    if (!MathHelper.CloseTo(this._oldOxygenLevel, this.OxygenLevel, 1f / 1000f))
      this._oldOxygenLevel += this.GetDiff(this.OxygenLevel - this._oldOxygenLevel, totalSeconds2);
    else
      this._oldOxygenLevel = this.OxygenLevel;
    if (!MathHelper.CloseTo(this._oldCritLevel, this.CritLevel, 1f / 1000f))
      this._oldCritLevel += this.GetDiff(this.CritLevel - this._oldCritLevel, totalSeconds2);
    else
      this._oldCritLevel = this.CritLevel;
    float oldPainLevel = this._oldPainLevel;
    if ((double) oldPainLevel > 0.0 && (double) this._oldCritLevel <= 0.0)
    {
      float num1 = 3f;
      float x = totalSeconds1 * num1;
      float num2 = 2f * (float) width;
      float num3 = 0.8f * (float) width;
      float num4 = 0.6f * (float) width;
      float num5 = 0.2f * (float) width;
      float num6 = num2 - oldPainLevel * (num2 - num3);
      float num7 = num4 - oldPainLevel * (num4 - num5);
      this._bruteShader.SetParameter("time", MathF.Max(0.0f, MathF.Sin(x)));
      this._bruteShader.SetParameter("color", new Vector3(1f, 0.0f, 0.0f));
      this._bruteShader.SetParameter("darknessAlphaOuter", 0.8f);
      this._bruteShader.SetParameter("outerCircleRadius", num6);
      this._bruteShader.SetParameter("outerCircleMaxRadius", num6 + 0.2f * (float) width);
      this._bruteShader.SetParameter("innerCircleRadius", num7);
      this._bruteShader.SetParameter("innerCircleMaxRadius", num7 + 0.02f * (float) width);
      ((DrawingHandleBase) worldHandle).UseShader(this._bruteShader);
      worldHandle.DrawRect(worldAabb, Color.White, true);
    }
    else
      this._oldPainLevel = this.PainLevel;
    float x1 = this.State != MobState.Critical ? this._oldOxygenLevel : 1f;
    if ((double) x1 > 0.0)
    {
      float num8 = 0.6f * (float) width;
      float num9 = 0.06f * (float) width;
      float num10 = 0.02f * (float) width;
      float num11 = 0.02f * (float) width;
      float num12 = num8 - x1 * (num8 - num9);
      float num13 = num10 - x1 * (num10 - num11);
      float num14;
      if ((double) this._oldCritLevel > 0.0)
      {
        float x2 = totalSeconds1 * 2f;
        float num15 = MathF.Max(0.0f, (float) ((double) MathF.Sin(x2) + 2.0 * (double) MathF.Sin((float) (2.0 * (double) x2 / 4.0)) + (double) MathF.Sin(x2 / 4f) - 3.0));
        num14 = (double) num15 <= 0.0 ? 1f : (float) (1.0 - (double) num15 / 1.5);
      }
      else
        num14 = MathF.Min(0.98f, (float) (0.30000001192092896 * (double) MathF.Log(x1) + 1.0));
      this._oxygenShader.SetParameter("time", 0.0f);
      this._oxygenShader.SetParameter("color", new Vector3(0.0f, 0.0f, 0.0f));
      this._oxygenShader.SetParameter("darknessAlphaOuter", num14);
      this._oxygenShader.SetParameter("innerCircleRadius", num13);
      this._oxygenShader.SetParameter("innerCircleMaxRadius", num13);
      this._oxygenShader.SetParameter("outerCircleRadius", num12);
      this._oxygenShader.SetParameter("outerCircleMaxRadius", num12 + 0.2f * (float) width);
      ((DrawingHandleBase) worldHandle).UseShader(this._oxygenShader);
      worldHandle.DrawRect(worldAabb, Color.White, true);
    }
    float num16 = this.State != MobState.Dead ? this._oldCritLevel : this.DeadLevel;
    if ((double) num16 > 0.0)
    {
      float num17 = 2f * (float) width;
      float num18 = 1f * (float) width;
      float num19 = 0.6f * (float) width;
      float num20 = 0.02f * (float) width;
      float num21 = num17 - num16 * (num17 - num18);
      float num22 = num19 - num16 * (num19 - num20);
      this._critShader.SetParameter("time", MathF.Max(0.0f, MathF.Sin(totalSeconds1)));
      this._critShader.SetParameter("color", new Vector3(1f, 1f, 1f));
      this._critShader.SetParameter("darknessAlphaOuter", 1f);
      this._critShader.SetParameter("innerCircleRadius", num22);
      this._critShader.SetParameter("innerCircleMaxRadius", num22 + 0.005f * (float) width);
      this._critShader.SetParameter("outerCircleRadius", num21);
      this._critShader.SetParameter("outerCircleMaxRadius", num21 + 0.2f * (float) width);
      ((DrawingHandleBase) worldHandle).UseShader(this._critShader);
      worldHandle.DrawRect(worldAabb, Color.White, true);
    }
    ((DrawingHandleBase) worldHandle).UseShader((ShaderInstance) null);
  }

  private float GetDiff(float value, float lastFrameTime)
  {
    float num = value * 5f * lastFrameTime;
    return (double) value >= 0.0 ? Math.Clamp(num, -value, value) : Math.Clamp(num, value, -value);
  }
}
