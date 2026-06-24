// Decompiled with JetBrains decompiler
// Type: Content.Client.Emp.EmpSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Emp;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Random;
using System;

#nullable enable
namespace Content.Client.Emp;

public sealed class EmpSystem : SharedEmpSystem
{
  [Dependency]
  private IRobustRandom _random;

  public virtual void Update(float frameTime)
  {
    base.Update(frameTime);
    EntityQueryEnumerator<EmpDisabledComponent, TransformComponent> entityQueryEnumerator = this.EntityQueryEnumerator<EmpDisabledComponent, TransformComponent>();
    EntityUid entityUid;
    EmpDisabledComponent disabledComponent;
    TransformComponent transformComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref disabledComponent, ref transformComponent))
    {
      if (this.Timing.CurTime > disabledComponent.TargetTime)
      {
        disabledComponent.TargetTime = this.Timing.CurTime + (double) this._random.NextFloat(0.8f, 1.2f) * TimeSpan.FromSeconds((double) disabledComponent.EffectCooldown);
        this.Spawn("EffectEmpDisabled", transformComponent.Coordinates);
      }
    }
  }
}
