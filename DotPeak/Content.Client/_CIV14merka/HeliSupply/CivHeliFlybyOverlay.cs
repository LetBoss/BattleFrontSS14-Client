// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.HeliSupply.CivHeliFlybyOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._CIV14merka.Commander;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.Enums;
using Robust.Shared.Graphics.RSI;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client._CIV14merka.HeliSupply;

public sealed class CivHeliFlybyOverlay : Overlay
{
  [Dependency]
  private IResourceCache _resourceCache;
  [Dependency]
  private IGameTiming _timing;
  private const float HeadingLookahead = 2.5f;
  private static readonly Color DustColor = Color.FromHex((ReadOnlySpan<char>) "#C2B280", new Color?());
  private Texture[] _usaFrames = Array.Empty<Texture>();
  private Texture[] _ruFrames = Array.Empty<Texture>();
  private Texture[] _dustFrames = Array.Empty<Texture>();
  private float _frameDelay = 0.1f;

  public virtual OverlaySpace Space => (OverlaySpace) 4;

  public List<CivHeliFlybyInstance> Instances { get; } = new List<CivHeliFlybyInstance>();

  public CivHeliFlybyOverlay()
  {
    IoCManager.InjectDependencies<CivHeliFlybyOverlay>(this);
    Robust.Client.Graphics.RSI rsi = this._resourceCache.GetResource<RSIResource>(new ResPath("/Textures/_CIV14merka/Effects/heli_shadows.rsi"), true).RSI;
    Robust.Client.Graphics.RSI.State state1;
    if (rsi.TryGetState(Robust.Client.Graphics.RSI.StateId.op_Implicit("usa"), ref state1))
    {
      this._usaFrames = state1.GetFrames((RsiDirection) 0);
      float[] delays = state1.GetDelays();
      if (delays.Length != 0 && (double) delays[0] > 0.0)
        this._frameDelay = delays[0];
    }
    Robust.Client.Graphics.RSI.State state2;
    if (rsi.TryGetState(Robust.Client.Graphics.RSI.StateId.op_Implicit("ru"), ref state2))
      this._ruFrames = state2.GetFrames((RsiDirection) 0);
    Robust.Client.Graphics.RSI.State state3;
    if (this._resourceCache.GetResource<RSIResource>(new ResPath("/Textures/Effects/chemsmoke.rsi"), true).RSI.TryGetState(Robust.Client.Graphics.RSI.StateId.op_Implicit("chemsmoke_white"), ref state3))
      this._dustFrames = state3.GetFrames((RsiDirection) 0);
    this.ZIndex = new int?(-10);
  }

  protected virtual bool BeforeDraw(in OverlayDrawArgs args) => this.Instances.Count > 0;

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    TimeSpan realTime = this._timing.RealTime;
    foreach (CivHeliFlybyInstance instance in this.Instances)
    {
      if (!MapId.op_Inequality(instance.MapId, args.MapId))
      {
        DrawingHandleWorld drawingHandleWorld = worldHandle;
        Matrix3x2 identity = Matrix3x2.Identity;
        ref Matrix3x2 local = ref identity;
        ((DrawingHandleBase) drawingHandleWorld).SetTransform(ref local);
        this.DrawDust(worldHandle, instance);
        Texture[] textureArray = instance.Side == CivAirstrikeSide.Ru ? this._ruFrames : this._usaFrames;
        if (textureArray.Length != 0)
        {
          float totalSeconds = (float) (realTime - instance.StartTime).TotalSeconds;
          if ((double) totalSeconds >= 0.0)
          {
            float cost = totalSeconds * instance.Speed;
            if ((double) cost <= (double) instance.Path.TotalCost)
            {
              Vector2 pos1;
              Vector2 tangent1;
              instance.Path.SampleByCost(cost, out pos1, out tangent1);
              Vector2 pos2;
              Vector2 tangent2;
              instance.Path.SampleByCost(MathF.Max(0.0f, cost - 2.5f), out pos2, out tangent2);
              Vector2 pos3;
              instance.Path.SampleByCost(MathF.Min(instance.Path.TotalCost, cost + 2.5f), out pos3, out tangent2);
              Vector2 vector2_1 = pos3 - pos2;
              if ((double) vector2_1.LengthSquared() < 9.9999997473787516E-05)
                vector2_1 = tangent1;
              if ((double) vector2_1.LengthSquared() < 9.9999997473787516E-05)
                vector2_1 = Vector2.UnitX;
              double scale1 = (double) CivHeliFlybyOverlay.ComputeScale(instance, cost);
              float num1 = 1f;
              float num2 = (double) instance.Path.TotalCost > 0.0 ? cost / instance.Path.TotalCost : 1f;
              if ((double) num2 < 0.05000000074505806)
                num1 = num2 / 0.05f;
              else if ((double) num2 > 0.949999988079071)
                num1 = (float) ((1.0 - (double) num2) / 0.05000000074505806);
              Color white = Color.White;
              Color color = ((Color) ref white).WithAlpha(instance.Alpha * num1);
              Matrix3x2 rotation = Matrix3Helpers.CreateRotation((double) MathF.Atan2(vector2_1.Y, vector2_1.X) + (double) instance.AngleOffset);
              tangent2 = new Vector2((float) scale1, (float) scale1);
              Matrix3x2 scale2 = Matrix3Helpers.CreateScale(ref tangent2);
              Texture texture = textureArray[(int) ((double) totalSeconds / (double) this._frameDelay) % textureArray.Length];
              Vector2 vector2_2 = new Vector2((float) texture.Width, (float) texture.Height) / 32f / 2f;
              Matrix3x2 translation = Matrix3Helpers.CreateTranslation(pos1);
              Matrix3x2 matrix3x2_1 = rotation;
              Matrix3x2 matrix3x2_2 = Matrix3x2.Multiply(Matrix3x2.Multiply(scale2, matrix3x2_1), translation);
              ((DrawingHandleBase) worldHandle).SetTransform(ref matrix3x2_2);
              worldHandle.DrawTextureRect(texture, new Box2(-vector2_2, vector2_2), new Color?(color));
            }
          }
        }
      }
    }
    DrawingHandleWorld drawingHandleWorld1 = worldHandle;
    Matrix3x2 identity1 = Matrix3x2.Identity;
    ref Matrix3x2 local1 = ref identity1;
    ((DrawingHandleBase) drawingHandleWorld1).SetTransform(ref local1);
  }

  private static float ComputeScale(CivHeliFlybyInstance inst, float cost)
  {
    float x = inst.Scale;
    float num1 = inst.Path.DistAtCost(cost);
    if ((double) inst.TakeoffZone > 0.0099999997764825821 && (double) num1 < (double) inst.TakeoffZone)
      x = MathF.Max(x, MathHelper.Lerp(inst.TakeoffScale, inst.Scale, num1 / inst.TakeoffZone));
    if ((double) num1 < (double) inst.DropDist)
    {
      float num2 = inst.DropDist - num1;
      if ((double) inst.DropZone > 0.0099999997764825821 && (double) num2 < (double) inst.DropZone)
        x = MathF.Max(x, MathHelper.Lerp(inst.DropScale, inst.Scale, num2 / inst.DropZone));
    }
    else
    {
      float num3 = num1 - inst.DropDist;
      if ((double) inst.ClimbZone > 0.0099999997764825821 && (double) num3 < (double) inst.ClimbZone)
        x = MathF.Max(x, MathHelper.Lerp(inst.DropScale, inst.Scale, num3 / inst.ClimbZone));
    }
    return x;
  }

  private void DrawDust(DrawingHandleWorld handle, CivHeliFlybyInstance inst)
  {
    if (this._dustFrames.Length == 0 || inst.Dust.Count == 0)
      return;
    foreach (CivHeliDustParticle heliDustParticle in inst.Dust)
    {
      float num1 = Math.Clamp(heliDustParticle.Age / heliDustParticle.Life, 0.0f, 1f);
      Texture dustFrame = this._dustFrames[Math.Min(this._dustFrames.Length - 1, (int) ((double) num1 * (double) this._dustFrames.Length))];
      double num2 = (double) heliDustParticle.Size * (0.699999988079071 + (double) num1 * 0.800000011920929);
      float num3 = (float) (0.34999999403953552 * (1.0 - (double) num1));
      Vector2 vector2 = new Vector2((float) num2, (float) num2) / 2f;
      handle.DrawTextureRect(dustFrame, new Box2(heliDustParticle.Pos - vector2, heliDustParticle.Pos + vector2), new Color?(((Color) ref CivHeliFlybyOverlay.DustColor).WithAlpha(num3)));
    }
  }
}
