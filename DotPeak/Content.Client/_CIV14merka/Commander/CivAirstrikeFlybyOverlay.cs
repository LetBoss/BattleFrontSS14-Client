// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Commander.CivAirstrikeFlybyOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._CIV14merka.Commander;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.Enums;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client._CIV14merka.Commander;

public sealed class CivAirstrikeFlybyOverlay : Overlay
{
  [Dependency]
  private IResourceCache _resourceCache;
  [Dependency]
  private IGameTiming _timing;
  private Texture? _usaTex;
  private Texture? _ruTex;

  public virtual OverlaySpace Space => (OverlaySpace) 4;

  public List<CivAirstrikeFlybyInstance> Instances { get; } = new List<CivAirstrikeFlybyInstance>();

  public CivAirstrikeFlybyOverlay()
  {
    IoCManager.InjectDependencies<CivAirstrikeFlybyOverlay>(this);
    RSI rsi = this._resourceCache.GetResource<RSIResource>(new ResPath("/Textures/_CIV14merka/Effects/jet_shadows.rsi"), true).RSI;
    RSI.State state1;
    if (rsi.TryGetState(RSI.StateId.op_Implicit("usa"), ref state1))
      this._usaTex = state1.Frame0;
    RSI.State state2;
    if (rsi.TryGetState(RSI.StateId.op_Implicit("ru"), ref state2))
      this._ruTex = state2.Frame0;
    this.ZIndex = new int?(-10);
  }

  protected virtual bool BeforeDraw(in OverlayDrawArgs args) => this.Instances.Count > 0;

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    TimeSpan realTime = this._timing.RealTime;
    foreach (CivAirstrikeFlybyInstance instance in this.Instances)
    {
      if (!MapId.op_Inequality(instance.MapId, args.MapId))
      {
        Texture texture = instance.Side == CivAirstrikeSide.Ru ? this._ruTex : this._usaTex;
        if (texture != null)
        {
          float totalSeconds = (float) (realTime - instance.StartTime).TotalSeconds;
          if ((double) totalSeconds >= 0.0)
          {
            float num1 = totalSeconds * instance.Speed;
            if ((double) num1 <= (double) instance.Total)
            {
              Vector2 zero = Vector2.Zero;
              Vector2 unitX = Vector2.UnitX;
              Vector2 pos;
              Vector2 vector2_1;
              if ((double) num1 <= (double) instance.EntryLineLen)
              {
                float amount = (double) instance.EntryLineLen > 0.0 ? num1 / instance.EntryLineLen : 1f;
                pos = Vector2.Lerp(instance.Origin, instance.Entry, amount);
                vector2_1 = instance.Entry - instance.Origin;
              }
              else if ((double) num1 <= (double) instance.EntryTurnEnd)
              {
                float dist = num1 - instance.EntryLineLen;
                pos = CivAirstrikeFlybyMath.ArcPos(instance.EntryCtr, instance.Entry, instance.Approach, instance.EntryCcw, dist);
                vector2_1 = CivAirstrikeFlybyMath.ArcTangent(instance.EntryCtr, pos, instance.EntryCcw);
              }
              else if ((double) num1 <= (double) instance.ToTarget)
              {
                float num2 = num1 - instance.EntryTurnEnd;
                float amount = (double) instance.RunInLen > 0.0 ? num2 / instance.RunInLen : 1f;
                pos = Vector2.Lerp(instance.Approach, instance.Target, amount);
                vector2_1 = instance.Target - instance.Approach;
              }
              else if ((double) num1 <= (double) instance.RunEndDist)
              {
                float num3 = num1 - instance.ToTarget;
                float amount = (double) instance.RunOutLen > 0.0 ? num3 / instance.RunOutLen : 1f;
                pos = Vector2.Lerp(instance.Target, instance.RunEnd, amount);
                vector2_1 = instance.RunEnd - instance.Target;
              }
              else if ((double) num1 <= (double) instance.ExitTurnEnd)
              {
                float dist = num1 - instance.RunEndDist;
                pos = CivAirstrikeFlybyMath.ArcPos(instance.ExitCtr, instance.RunEnd, instance.ExitTurn, instance.ExitCcw, dist);
                vector2_1 = CivAirstrikeFlybyMath.ArcTangent(instance.ExitCtr, pos, instance.ExitCcw);
              }
              else
              {
                float num4 = num1 - instance.ExitTurnEnd;
                float amount = (double) instance.ExitLineLen > 0.0 ? num4 / instance.ExitLineLen : 1f;
                pos = Vector2.Lerp(instance.ExitTurn, instance.Exit, amount);
                vector2_1 = instance.Exit - instance.ExitTurn;
              }
              float num5;
              if ((double) num1 <= (double) instance.ToTarget)
              {
                float num6 = (double) instance.ToTarget > 0.0 ? num1 / instance.ToTarget : 1f;
                num5 = MathHelper.Lerp(instance.ScaleMin, instance.ScaleMax, num6);
              }
              else
              {
                float num7 = instance.Total - instance.ToTarget;
                float num8 = (double) num7 > 0.0 ? (num1 - instance.ToTarget) / num7 : 1f;
                num5 = MathHelper.Lerp(instance.ScaleMax, instance.ScaleMin, num8);
              }
              float num9 = 1f;
              float num10 = (double) instance.Total > 0.0 ? num1 / instance.Total : 1f;
              if ((double) num10 < 0.079999998211860657)
                num9 = num10 / 0.08f;
              else if ((double) num10 > 0.92000001668930054)
                num9 = (float) ((1.0 - (double) num10) / 0.079999998211860657);
              Color white = Color.White;
              Color color = ((Color) ref white).WithAlpha(instance.Alpha * num9);
              if ((double) vector2_1.LengthSquared() < 9.9999997473787516E-05)
                vector2_1 = Vector2.UnitX;
              Matrix3x2 rotation = Matrix3Helpers.CreateRotation((double) MathF.Atan2(vector2_1.Y, vector2_1.X) + 1.5707963705062866);
              Vector2 vector2_2 = new Vector2(num5, num5);
              Matrix3x2 scale = Matrix3Helpers.CreateScale(ref vector2_2);
              Vector2 vector2_3 = new Vector2(-vector2_1.Y, vector2_1.X);
              Vector2 vector2_4 = (double) vector2_3.LengthSquared() > 9.9999997473787516E-05 ? Vector2.Normalize(vector2_3) : Vector2.UnitY;
              Vector2 vector2_5 = new Vector2((float) texture.Width, (float) texture.Height) / 32f / 2f;
              for (int index = 0; index < instance.Count; ++index)
              {
                float num11 = ((float) index - (float) (instance.Count - 1) / 2f) * instance.Spacing;
                Matrix3x2 translation = Matrix3Helpers.CreateTranslation(pos + vector2_4 * num11);
                Matrix3x2 matrix3x2 = Matrix3x2.Multiply(Matrix3x2.Multiply(scale, rotation), translation);
                ((DrawingHandleBase) worldHandle).SetTransform(ref matrix3x2);
                worldHandle.DrawTextureRect(texture, new Box2(-vector2_5, vector2_5), new Color?(color));
              }
            }
          }
        }
      }
    }
    DrawingHandleWorld drawingHandleWorld = worldHandle;
    Matrix3x2 identity = Matrix3x2.Identity;
    ref Matrix3x2 local = ref identity;
    ((DrawingHandleBase) drawingHandleWorld).SetTransform(ref local);
  }
}
