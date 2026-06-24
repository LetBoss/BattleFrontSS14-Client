using System;
using System.Collections.Generic;
using Content.Shared.Cargo.Components;
using Content.Shared.Cargo.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Shared.Cargo;

public abstract class SharedCargoSystem : EntitySystem
{
	[Dependency]
	protected IGameTiming Timing;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<StationBankAccountComponent, MapInitEvent>((EntityEventRefHandler<StationBankAccountComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
	}

	private void OnMapInit(Entity<StationBankAccountComponent> ent, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.NextIncomeTime = Timing.CurTime + ent.Comp.IncomeDelay;
		((EntitySystem)this).Dirty<StationBankAccountComponent>(ent, (MetaDataComponent)null);
	}

	public int GetBalanceFromAccount(Entity<StationBankAccountComponent?> station, ProtoId<CargoAccountPrototype> account)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<StationBankAccountComponent>(Entity<StationBankAccountComponent>.op_Implicit(station), ref station.Comp, true))
		{
			return 0;
		}
		return station.Comp.Accounts.GetValueOrDefault(account);
	}

	public Dictionary<ProtoId<CargoAccountPrototype>, double> CreateAccountDistribution(Entity<StationBankAccountComponent> stationBank)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<ProtoId<CargoAccountPrototype>, double> distribution = new Dictionary<ProtoId<CargoAccountPrototype>, double> { 
		{
			stationBank.Comp.PrimaryAccount,
			stationBank.Comp.PrimaryCut
		} };
		double remaining = 1.0 - stationBank.Comp.PrimaryCut;
		foreach (KeyValuePair<ProtoId<CargoAccountPrototype>, double> item in stationBank.Comp.RevenueDistribution)
		{
			item.Deconstruct(out var key, out var value);
			ProtoId<CargoAccountPrototype> account = key;
			double percentage = value;
			double existing = Extensions.GetOrNew<ProtoId<CargoAccountPrototype>, double>(distribution, account);
			distribution[account] = existing + remaining * percentage;
		}
		return distribution;
	}
}
