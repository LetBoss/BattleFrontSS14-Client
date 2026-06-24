// Decompiled with JetBrains decompiler
// Type: Content.Shared.Revenant.EntitySystems.SharedRevenantOverloadedLightsSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Revenant.Components;
using Robust.Shared.GameObjects;

#nullable enable
namespace Content.Shared.Revenant.EntitySystems;

public abstract class SharedRevenantOverloadedLightsSystem : EntitySystem
{
  public override void Update(float frameTime)
  {
    base.Update(frameTime);
    Robust.Shared.GameObjects.EntityQueryEnumerator<RevenantOverloadedLightsComponent> entityQueryEnumerator = this.EntityQueryEnumerator<RevenantOverloadedLightsComponent>();
    EntityUid uid;
    RevenantOverloadedLightsComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      comp1.Accumulator += frameTime;
      if ((double) comp1.Accumulator >= (double) comp1.ZapDelay)
      {
        this.OnZap((Entity<RevenantOverloadedLightsComponent>) (uid, comp1));
        this.RemCompDeferred(uid, (IComponent) comp1);
      }
    }
  }

  protected abstract void OnZap(
    Entity<RevenantOverloadedLightsComponent> component);
}
