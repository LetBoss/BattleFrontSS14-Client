// Decompiled with JetBrains decompiler
// Type: Content.Client.Cooldown.CooldownGraphic
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client.Cooldown;

public sealed class CooldownGraphic : Control
{
  private static readonly ProtoId<ShaderPrototype> Shader = ProtoId<ShaderPrototype>.op_Implicit("CooldownAnimation");
  [Dependency]
  private IGameTiming _gameTiming;
  [Dependency]
  private IPrototypeManager _protoMan;
  private readonly ShaderInstance _shader;

  public CooldownGraphic()
  {
    IoCManager.InjectDependencies<CooldownGraphic>(this);
    this._shader = this._protoMan.Index<ShaderPrototype>(CooldownGraphic.Shader).InstanceUnique();
  }

  public float Progress { get; set; }

  protected virtual void Draw(DrawingHandleScreen handle)
  {
    Span<float> span = (Span<float>) new float[10];
    float num1 = 1f - MathF.Abs(this.Progress);
    Color color;
    if ((double) this.Progress >= 0.0)
    {
      color = Color.FromHsv(new Vector4(0.2777778f * num1, 0.75f, 0.75f, 0.5f));
    }
    else
    {
      float num2 = MathHelper.Clamp(0.5f * num1, 0.0f, 0.5f);
      // ISSUE: explicit constructor call
      ((Color) ref color).\u002Ector(1f, 1f, 1f, num2);
    }
    this._shader.SetParameter("progress", this.Progress);
    ((DrawingHandleBase) handle).UseShader(this._shader);
    handle.DrawRect(UIBox2i.op_Implicit(this.PixelSizeBox), color, true);
    ((DrawingHandleBase) handle).UseShader((ShaderInstance) null);
  }

  public void FromTime(TimeSpan start, TimeSpan end)
  {
    TimeSpan timeSpan = end - start;
    TimeSpan curTime = this._gameTiming.CurTime;
    double totalSeconds = timeSpan.TotalSeconds;
    double num1 = (curTime - start).TotalSeconds / totalSeconds;
    double num2 = num1 <= 1.0 ? 1.0 - num1 : (curTime - end).TotalSeconds * -5.0;
    this.Progress = MathHelper.Clamp((float) num2, -1f, 1f);
    this.Visible = num2 > -1.0;
  }
}
