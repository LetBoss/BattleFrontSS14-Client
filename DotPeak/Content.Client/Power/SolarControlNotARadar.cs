// Decompiled with JetBrains decompiler
// Type: Content.Client.Power.SolarControlNotARadar
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Solar;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client.Power;

public sealed class SolarControlNotARadar : Control
{
  [Dependency]
  private IGameTiming _gameTiming;
  private SolarControlConsoleBoundInterfaceState _lastState = new SolarControlConsoleBoundInterfaceState(Angle.op_Implicit(0.0f), Angle.op_Implicit(0.0f), 0.0f, Angle.op_Implicit(0.0f));
  private TimeSpan _lastStateTime = TimeSpan.Zero;
  public const int StandardSizeFull = 290;
  public const int StandardRadiusCircle = 140;

  public int SizeFull => (int) (290.0 * (double) this.UIScale);

  public int RadiusCircle => (int) (140.0 * (double) this.UIScale);

  public SolarControlNotARadar()
  {
    IoCManager.InjectDependencies<SolarControlNotARadar>(this);
    this.MinSize = new Vector2((float) this.SizeFull, (float) this.SizeFull);
  }

  public void UpdateState(SolarControlConsoleBoundInterfaceState ls)
  {
    this._lastState = ls;
    this._lastStateTime = this._gameTiming.CurTime;
  }

  public Angle PredictedPanelRotation
  {
    get
    {
      return Angle.op_Addition(this._lastState.Rotation, Angle.op_Implicit(Angle.op_Implicit(this._lastState.AngularVelocity) * (this._gameTiming.CurTime - this._lastStateTime).TotalSeconds));
    }
  }

  protected virtual void Draw(DrawingHandleScreen handle)
  {
    int num1 = this.SizeFull / 2;
    Color color1;
    // ISSUE: explicit constructor call
    ((Color) ref color1).\u002Ector(0.08f, 0.08f, 0.08f, 1f);
    Color color2;
    // ISSUE: explicit constructor call
    ((Color) ref color2).\u002Ector(0.08f, 0.08f, 0.08f, 1f);
    int num2 = 4;
    int num3 = 8;
    int num4 = 8;
    ((DrawingHandleBase) handle).DrawCircle(new Vector2((float) num1, (float) num1), (float) (this.RadiusCircle + 1), color1, true);
    ((DrawingHandleBase) handle).DrawCircle(new Vector2((float) num1, (float) num1), (float) this.RadiusCircle, Color.Black, true);
    for (int index = 0; index < num4; ++index)
      ((DrawingHandleBase) handle).DrawCircle(new Vector2((float) num1, (float) num1), (float) (this.RadiusCircle / num4 * index), color2, false);
    for (int index = 0; index < num3; ++index)
    {
      Angle angle = Angle.op_Implicit(Math.PI / (double) num3 * (double) index);
      Vector2 vector2 = ((Angle) ref angle).ToVec() * (float) this.RadiusCircle;
      ((DrawingHandleBase) handle).DrawLine(new Vector2((float) num1, (float) num1) - vector2, new Vector2((float) num1, (float) num1) + vector2, color2);
    }
    Vector2 vector2_1 = new Vector2(1f, -1f);
    Angle angle1;
    // ISSUE: explicit constructor call
    ((Angle) ref angle1).\u002Ector(-1.0 * Math.PI / 2.0);
    Angle angle2 = Angle.op_Addition(this.PredictedPanelRotation, angle1);
    Vector2 vector2_2 = ((Angle) ref angle2).ToVec() * vector2_1 * (float) this.RadiusCircle;
    Vector2 vector2_3 = new Vector2(vector2_2.Y, -vector2_2.X);
    ((DrawingHandleBase) handle).DrawLine(new Vector2((float) num1, (float) num1) - vector2_3, new Vector2((float) num1, (float) num1) + vector2_3, Color.White);
    ((DrawingHandleBase) handle).DrawLine(new Vector2((float) num1, (float) num1) + vector2_2 / (float) num2, new Vector2((float) num1, (float) num1) + vector2_2 - vector2_2 / (float) num2, Color.DarkGray);
    Angle angle3 = Angle.op_Addition(this._lastState.TowardsSun, angle1);
    Vector2 vector2_4 = ((Angle) ref angle3).ToVec() * vector2_1 * (float) this.RadiusCircle;
    ((DrawingHandleBase) handle).DrawLine(new Vector2((float) num1, (float) num1) + vector2_4, new Vector2((float) num1, (float) num1), Color.Yellow);
  }
}
