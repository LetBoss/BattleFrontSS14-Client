// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Aura.SharedAuraSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Stealth;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared._RMC14.Aura;

public abstract class SharedAuraSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private INetManager _net;

  public void GiveAura(EntityUid ent, Color auraColor, TimeSpan? duration, float outlineWidth = 2f)
  {
    if (this.HasComp<EntityActiveInvisibleComponent>(ent))
      return;
    AuraComponent auraComponent1 = this.EnsureComp<AuraComponent>(ent);
    auraComponent1.Color = auraColor;
    AuraComponent auraComponent2 = auraComponent1;
    TimeSpan curTime = this._timing.CurTime;
    TimeSpan? nullable1 = duration;
    TimeSpan? nullable2 = nullable1.HasValue ? new TimeSpan?(curTime + nullable1.GetValueOrDefault()) : new TimeSpan?();
    auraComponent2.ExpiresAt = nullable2;
    auraComponent1.OutlineWidth = outlineWidth;
    this.Dirty(ent, (IComponent) auraComponent1);
  }

  public override void Update(float frameTime)
  {
    if (this._net.IsClient)
      return;
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<AuraComponent> entityQueryEnumerator = this.EntityQueryEnumerator<AuraComponent>();
    EntityUid uid;
    AuraComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (comp1.ExpiresAt.HasValue)
      {
        TimeSpan timeSpan = curTime;
        TimeSpan? expiresAt = comp1.ExpiresAt;
        if ((expiresAt.HasValue ? (timeSpan < expiresAt.GetValueOrDefault() ? 1 : 0) : 0) == 0)
          this.RemCompDeferred<AuraComponent>(uid);
      }
    }
  }
}
