// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.HoloTargeting.RMCHoloTargetedSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Shared._RMC14.Weapons.Ranged.HoloTargeting;

public sealed class RMCHoloTargetedSystem : EntitySystem
{
  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<HoloTargetedComponent, DamageModifyEvent>(new ComponentEventRefHandler<HoloTargetedComponent, DamageModifyEvent>(this.OnDamageModify));
  }

  public void ApplyHoloStacks(EntityUid uid, float decay, float stacks, float maxStacks)
  {
    HoloTargetedComponent targetedComponent = this.EnsureComp<HoloTargetedComponent>(uid);
    targetedComponent.Decay = decay;
    float num = targetedComponent.Stacks + stacks;
    targetedComponent.Stacks = Math.Clamp(num, 0.0f, maxStacks);
    targetedComponent.DecayDelay = 0.0f;
    targetedComponent.DecayTimer = 0.0f;
    this.Dirty(uid, (IComponent) targetedComponent);
  }

  private void OnDamageModify(
    EntityUid uid,
    HoloTargetedComponent component,
    ref DamageModifyEvent args)
  {
    float num = (float) (1.0 + (double) component.Stacks / 1000.0);
    args.Damage *= num;
  }

  public override void Update(float frameTime)
  {
    Robust.Shared.GameObjects.EntityQueryEnumerator<HoloTargetedComponent> entityQueryEnumerator = this.EntityQueryEnumerator<HoloTargetedComponent>();
    EntityUid uid;
    HoloTargetedComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      comp1.DecayDelay += frameTime;
      if ((double) comp1.DecayDelay >= 5.0)
      {
        comp1.DecayTimer += frameTime;
        if ((double) comp1.DecayTimer >= 1.0)
        {
          comp1.DecayTimer = 0.0f;
          comp1.Stacks -= comp1.Decay;
          this.Dirty(uid, (IComponent) comp1);
          if ((double) comp1.Stacks <= 0.0)
            this.RemCompDeferred<HoloTargetedComponent>(uid);
        }
      }
    }
  }
}
