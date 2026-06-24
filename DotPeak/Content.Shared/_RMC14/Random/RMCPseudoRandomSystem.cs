// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Random.RMCPseudoRandomSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

#nullable enable
namespace Content.Shared._RMC14.Random;

public sealed class RMCPseudoRandomSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _timing;

  public Xoroshiro64S GetXoroshiro64S(EntityUid ent)
  {
    return new Xoroshiro64S((long) this._timing.CurTick.Value << 32 /*0x20*/ | (long) (uint) this.GetNetEntity(ent).Id);
  }

  public float NextFloat(EntityUid ent) => this.GetXoroshiro64S(ent).NextFloat();

  public float NextFloat(ref Xoroshiro64S xoroshiro) => xoroshiro.NextFloat();

  public Angle NextAngle(EntityUid ent, Angle minValue, Angle maxValue)
  {
    return Angle.op_Addition(Angle.op_Implicit((double) this.NextFloat(ent) * Angle.op_Implicit(Angle.op_Subtraction(maxValue, minValue))), minValue);
  }

  public Angle NextAngle(ref Xoroshiro64S xoroshiro, Angle minValue, Angle maxValue)
  {
    return Angle.op_Addition(Angle.op_Implicit((double) this.NextFloat(ref xoroshiro) * Angle.op_Implicit(Angle.op_Subtraction(maxValue, minValue))), minValue);
  }
}
