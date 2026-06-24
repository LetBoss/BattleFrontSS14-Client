// Decompiled with JetBrains decompiler
// Type: Content.Shared.Cargo.SharedCargoSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Cargo.Components;
using Content.Shared.Cargo.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Cargo;

public abstract class SharedCargoSystem : EntitySystem
{
  [Dependency]
  protected IGameTiming Timing;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<StationBankAccountComponent, MapInitEvent>(new EntityEventRefHandler<StationBankAccountComponent, MapInitEvent>((object) this, __methodptr(OnMapInit)), (Type[]) null, (Type[]) null);
  }

  private void OnMapInit(Entity<StationBankAccountComponent> ent, ref MapInitEvent args)
  {
    ent.Comp.NextIncomeTime = this.Timing.CurTime + ent.Comp.IncomeDelay;
    this.Dirty<StationBankAccountComponent>(ent, (MetaDataComponent) null);
  }

  public int GetBalanceFromAccount(
    Entity<StationBankAccountComponent?> station,
    ProtoId<CargoAccountPrototype> account)
  {
    return !this.Resolve<StationBankAccountComponent>(Entity<StationBankAccountComponent>.op_Implicit(station), ref station.Comp, true) ? 0 : station.Comp.Accounts.GetValueOrDefault<ProtoId<CargoAccountPrototype>, int>(account);
  }

  public Dictionary<ProtoId<CargoAccountPrototype>, double> CreateAccountDistribution(
    Entity<StationBankAccountComponent> stationBank)
  {
    Dictionary<ProtoId<CargoAccountPrototype>, double> accountDistribution = new Dictionary<ProtoId<CargoAccountPrototype>, double>()
    {
      {
        stationBank.Comp.PrimaryAccount,
        stationBank.Comp.PrimaryCut
      }
    };
    double num1 = 1.0 - stationBank.Comp.PrimaryCut;
    foreach ((ProtoId<CargoAccountPrototype> key, double num2) in stationBank.Comp.RevenueDistribution)
    {
      double orNew = Extensions.GetOrNew<ProtoId<CargoAccountPrototype>, double>(accountDistribution, key);
      accountDistribution[key] = orNew + num1 * num2;
    }
    return accountDistribution;
  }
}
