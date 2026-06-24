// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.AciderGeneration.XenoAciderGenerationSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.TrainingDummy;
using Content.Shared._RMC14.Xenonids.Energy;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.AciderGeneration;

public sealed class XenoAciderGenerationSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private XenoSystem _xeno;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private XenoEnergySystem _energy;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<XenoAciderGenerationComponent, MeleeHitEvent>(new EntityEventRefHandler<XenoAciderGenerationComponent, MeleeHitEvent>(this.OnMeleeHit));
  }

  private void OnMeleeHit(Entity<XenoAciderGenerationComponent> xeno, ref MeleeHitEvent args)
  {
    bool flag = false;
    foreach (EntityUid hitEntity in (IEnumerable<EntityUid>) args.HitEntities)
    {
      if (this._xeno.CanAbilityAttackTarget((EntityUid) xeno, hitEntity))
      {
        if (this.HasComp<RMCTrainingDummyComponent>(hitEntity))
          return;
        flag = true;
        break;
      }
    }
    if (!flag)
      return;
    this._appearance.SetData((EntityUid) xeno, (Enum) XenoAcidGeneratingVisuals.Generating, (object) true);
    xeno.Comp.ExpireAt = new TimeSpan?(this._timing.CurTime + xeno.Comp.ExpireDuration);
    this.Dirty<XenoAciderGenerationComponent>(xeno);
  }

  public override void Update(float frameTime)
  {
    if (this._net.IsClient)
      return;
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<XenoAciderGenerationComponent, XenoEnergyComponent> entityQueryEnumerator = this.EntityQueryEnumerator<XenoAciderGenerationComponent, XenoEnergyComponent>();
    EntityUid uid;
    XenoAciderGenerationComponent comp1;
    XenoEnergyComponent comp2;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1, out comp2))
    {
      if (comp1.ExpireAt.HasValue)
      {
        if (curTime >= comp1.NextIncrease)
        {
          this._energy.AddEnergy((Entity<XenoEnergyComponent>) (uid, comp2), comp1.IncreaseAmount, false);
          comp1.NextIncrease = curTime + comp1.TimeBetweenGeneration;
          this.Dirty(uid, (IComponent) comp1);
        }
        TimeSpan timeSpan = curTime;
        TimeSpan? expireAt = comp1.ExpireAt;
        if ((expireAt.HasValue ? (timeSpan < expireAt.GetValueOrDefault() ? 1 : 0) : 0) == 0)
        {
          comp1.ExpireAt = new TimeSpan?();
          this._appearance.SetData(uid, (Enum) XenoAcidGeneratingVisuals.Generating, (object) false);
          this.Dirty(uid, (IComponent) comp1);
        }
      }
    }
  }
}
