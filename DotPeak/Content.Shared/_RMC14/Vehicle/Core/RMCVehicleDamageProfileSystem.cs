// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.RMCVehicleDamageProfileSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Vehicle;

public sealed class RMCVehicleDamageProfileSystem : EntitySystem
{
  public override void Initialize()
  {
    this.SubscribeLocalEvent<RMCVehicleDamageProfileComponent, BeforeDamageChangedEvent>(new EntityEventRefHandler<RMCVehicleDamageProfileComponent, BeforeDamageChangedEvent>(this.OnBeforeDamageChanged), new Type[1]
    {
      typeof (VehicleSystem)
    });
  }

  private void OnBeforeDamageChanged(
    Entity<RMCVehicleDamageProfileComponent> ent,
    ref BeforeDamageChangedEvent args)
  {
    if (args.Cancelled || ent.Comp.Rules.Count == 0 || !args.Damage.AnyPositive())
      return;
    foreach (string key in new List<string>((IEnumerable<string>) args.Damage.DamageDict.Keys))
    {
      FixedPoint2 fixedPoint2;
      if (args.Damage.DamageDict.TryGetValue(key, out fixedPoint2) && !(fixedPoint2 <= 0))
      {
        foreach (RMCVehicleDamageScaleRule rule in ent.Comp.Rules)
        {
          if (rule.DamageTypes.Contains(key) && !(fixedPoint2 > rule.MaxDamage))
          {
            args.Damage.DamageDict[key] = fixedPoint2 * rule.Multiplier;
            break;
          }
        }
      }
    }
  }
}
