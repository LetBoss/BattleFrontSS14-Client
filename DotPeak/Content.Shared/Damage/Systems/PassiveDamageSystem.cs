// Decompiled with JetBrains decompiler
// Type: Content.Shared.Damage.PassiveDamageSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage.Components;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared.Damage;

public sealed class PassiveDamageSystem : EntitySystem
{
  [Dependency]
  private DamageableSystem _damageable;
  [Dependency]
  private IGameTiming _timing;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<PassiveDamageComponent, MapInitEvent>(new ComponentEventHandler<PassiveDamageComponent, MapInitEvent>((object) this, __methodptr(OnPendingMapInit)), (Type[]) null, (Type[]) null);
  }

  private void OnPendingMapInit(EntityUid uid, PassiveDamageComponent component, MapInitEvent args)
  {
    component.NextDamage = this._timing.CurTime + TimeSpan.FromSeconds(1.0);
  }

  public virtual void Update(float frameTime)
  {
    base.Update(frameTime);
    TimeSpan curTime = this._timing.CurTime;
    EntityQueryEnumerator<PassiveDamageComponent, DamageableComponent, MobStateComponent> entityQueryEnumerator = this.EntityQueryEnumerator<PassiveDamageComponent, DamageableComponent, MobStateComponent>();
    EntityUid entityUid;
    PassiveDamageComponent passiveDamageComponent;
    DamageableComponent damageable;
    MobStateComponent mobStateComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref passiveDamageComponent, ref damageable, ref mobStateComponent))
    {
      if (!(passiveDamageComponent.NextDamage > curTime) && (!(passiveDamageComponent.DamageCap != 0) || !(damageable.TotalDamage >= passiveDamageComponent.DamageCap)))
      {
        passiveDamageComponent.NextDamage = curTime + TimeSpan.FromSeconds(1.0);
        foreach (MobState allowedState in passiveDamageComponent.AllowedStates)
        {
          if (allowedState == mobStateComponent.CurrentState)
            this._damageable.TryChangeDamage(new EntityUid?(entityUid), passiveDamageComponent.Damage, true, false, damageable);
        }
      }
    }
  }
}
