// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityEffects.EntityEffectExt
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.IoC;
using Robust.Shared.Random;

#nullable enable
namespace Content.Shared.EntityEffects;

public static class EntityEffectExt
{
  public static bool ShouldApply(
    this EntityEffect effect,
    EntityEffectBaseArgs args,
    IRobustRandom? random = null)
  {
    if (random == null)
      random = IoCManager.Resolve<IRobustRandom>();
    if ((double) effect.Probability < 1.0 && !random.Prob(effect.Probability))
      return false;
    if (effect.Conditions != null)
    {
      foreach (EntityEffectCondition condition in effect.Conditions)
      {
        if (!condition.Condition(args))
          return false;
      }
    }
    return true;
  }
}
