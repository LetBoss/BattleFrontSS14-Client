using System;
using Content.Shared._RMC14.Admin;
using Content.Shared._RMC14.CCVar;
using Content.Shared._RMC14.Marines;
using Content.Shared._RMC14.Requisitions;
using Content.Shared._RMC14.Requisitions.Components;
using Content.Shared._RMC14.Vendors;
using Content.Shared._RMC14.Weapons.Ranged.IFF;
using Content.Shared.GameTicking;
using Content.Shared.Mind;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Roles;
using Content.Shared.Roles.Jobs;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Scaling;

public sealed class ScalingSystem : EntitySystem
{
	[Dependency]
	private IConfigurationManager _config;

	[Dependency]
	private SharedGameTicker _gameTicker;

	[Dependency]
	private SharedJobSystem _job;

	[Dependency]
	private SharedMindSystem _mind;

	[Dependency]
	private IPrototypeManager _prototypes;

	[Dependency]
	private SharedRequisitionsSystem _requisitions;

	[Dependency]
	private SharedCMAutomatedVendorSystem _rmcAutomatedVendor;

	[Dependency]
	private GunIFFSystem _gunIFF;

	private float _marineScalingBonus;

	public float MarineScalingNormal { get; private set; }

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<PlayerSpawnCompleteEvent>((EntityEventHandler<PlayerSpawnCompleteEvent>)OnPlayerSpawnComplete, (Type[])null, (Type[])null);
		EntitySystemSubscriptionExt.CVar<float>(((EntitySystem)this).Subs, _config, RMCCVars.RMCMarineScalingNormal, (Action<float>)delegate(float v)
		{
			MarineScalingNormal = v;
		}, true);
		EntitySystemSubscriptionExt.CVar<float>(((EntitySystem)this).Subs, _config, RMCCVars.RMCMarineScalingBonus, (Action<float>)delegate(float v)
		{
			_marineScalingBonus = v;
		}, true);
	}

	private void OnPlayerSpawnComplete(PlayerSpawnCompleteEvent ev)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		if (!ev.LateJoin || !((EntitySystem)this).HasComp<MarineComponent>(ev.Mob) || ((EntitySystem)this).HasComp<RMCAdminSpawnedComponent>(ev.Mob))
		{
			return;
		}
		string jobId = ev.JobId;
		JobPrototype job = default(JobPrototype);
		if (jobId == null || !_prototypes.TryIndex<JobPrototype>(jobId, ref job) || job.RoleWeight <= 0f)
		{
			return;
		}
		EntityQueryEnumerator<MarineScalingComponent> scalingQuery = ((EntitySystem)this).EntityQueryEnumerator<MarineScalingComponent>();
		EntityUid uid = default(EntityUid);
		MarineScalingComponent scaling = default(MarineScalingComponent);
		while (scalingQuery.MoveNext(ref uid, ref scaling))
		{
			double deciseconds = _gameTicker.RoundDuration().TotalSeconds * 10.0;
			scaling.Scale += (double)job.RoleWeight * (0.25 + 0.75 / (1.0 + deciseconds / 20000.0)) / (double)MarineScalingNormal;
			double delta = scaling.Scale - scaling.MaxScale;
			if (delta > 0.0)
			{
				scaling.MaxScale = scaling.Scale;
				MarineScaleChangedEvent scaleEv = new MarineScaleChangedEvent(scaling.MaxScale, delta);
				((EntitySystem)this).RaiseLocalEvent<MarineScaleChangedEvent>(ref scaleEv);
			}
			((EntitySystem)this).Dirty(uid, (IComponent)(object)scaling, (MetaDataComponent)null);
		}
	}

	public bool TryGetScaling(out Entity<MarineScalingComponent> scaling)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		EntityUid uid = default(EntityUid);
		MarineScalingComponent comp = default(MarineScalingComponent);
		if (((EntitySystem)this).EntityQueryEnumerator<MarineScalingComponent>().MoveNext(ref uid, ref comp))
		{
			scaling = Entity<MarineScalingComponent>.op_Implicit((uid, comp));
			return true;
		}
		scaling = default(Entity<MarineScalingComponent>);
		return false;
	}

	private Entity<MarineScalingComponent> EnsureScaling()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		EntityUid uid = default(EntityUid);
		MarineScalingComponent scaling = default(MarineScalingComponent);
		if (((EntitySystem)this).EntityQueryEnumerator<MarineScalingComponent>().MoveNext(ref uid, ref scaling))
		{
			return Entity<MarineScalingComponent>.op_Implicit((uid, scaling));
		}
		EntityUid scalingId = ((EntitySystem)this).Spawn((string)null, MapCoordinates.Nullspace, (ComponentRegistry)null, default(Angle));
		MarineScalingComponent scalingComp = ((EntitySystem)this).EnsureComp<MarineScalingComponent>(scalingId);
		return Entity<MarineScalingComponent>.op_Implicit((scalingId, scalingComp));
	}

	public void TryStartScaling(EntProtoId<IFFFactionComponent> faction)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		Entity<MarineScalingComponent> scaling = EnsureScaling();
		if (scaling.Comp.Started)
		{
			return;
		}
		scaling.Comp.Started = true;
		((EntitySystem)this).Dirty<MarineScalingComponent>(scaling, (MetaDataComponent)null);
		float marineCount = _marineScalingBonus;
		EntityQueryEnumerator<UserIFFComponent, ActorComponent> marines = ((EntitySystem)this).EntityQueryEnumerator<UserIFFComponent, ActorComponent>();
		EntityUid marineId = default(EntityUid);
		UserIFFComponent userIFF = default(UserIFFComponent);
		ActorComponent val = default(ActorComponent);
		while (marines.MoveNext(ref marineId, ref userIFF, ref val))
		{
			if (_gunIFF.IsInFaction(marineId, faction) && _mind.TryGetMind(marineId, out EntityUid mindId, out MindComponent _) && _job.MindTryGetJob(mindId, out JobPrototype job))
			{
				marineCount += job.RoleWeight;
			}
		}
		scaling.Comp.Scale = Math.Max(1f, marineCount / MarineScalingNormal);
		scaling.Comp.MaxScale = scaling.Comp.Scale;
		EntityQueryEnumerator<RequisitionsAccountComponent> accounts = ((EntitySystem)this).EntityQueryEnumerator<RequisitionsAccountComponent>();
		EntityUid uid = default(EntityUid);
		RequisitionsAccountComponent account = default(RequisitionsAccountComponent);
		while (accounts.MoveNext(ref uid, ref account))
		{
			_requisitions.StartAccount(Entity<RequisitionsAccountComponent>.op_Implicit((uid, account)), scaling.Comp.Scale, marineCount);
		}
		EntityQueryEnumerator<CMAutomatedVendorComponent> vendors = ((EntitySystem)this).EntityQueryEnumerator<CMAutomatedVendorComponent>();
		EntityUid vendorId = default(EntityUid);
		CMAutomatedVendorComponent vendor = default(CMAutomatedVendorComponent);
		while (vendors.MoveNext(ref vendorId, ref vendor))
		{
			double scale = scaling.Comp.Scale;
			if (!vendor.Scaling)
			{
				scale = 1.0;
			}
			foreach (CMVendorSection section in vendor.Sections)
			{
				for (int i = 0; i < section.Entries.Count; i++)
				{
					CMVendorEntry entry = section.Entries[i];
					int? amount = entry.Amount;
					if (amount.HasValue)
					{
						int amount2 = amount.GetValueOrDefault();
						if (!entry.Box.HasValue)
						{
							amount2 = (int)Math.Round((double)amount2 * scale);
							section.Entries[i] = entry with
							{
								Amount = amount2,
								Max = amount2
							};
							_rmcAutomatedVendor.AmountUpdated(Entity<CMAutomatedVendorComponent>.op_Implicit((vendorId, vendor)), entry);
						}
					}
				}
			}
			((EntitySystem)this).Dirty(vendorId, (IComponent)(object)vendor, (MetaDataComponent)null);
		}
	}

	public int GetAliveHumanoids()
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		int alive = 0;
		EntityQueryEnumerator<MarineComponent, MobStateComponent> query = ((EntitySystem)this).EntityQueryEnumerator<MarineComponent, MobStateComponent>();
		MarineComponent marineComponent = default(MarineComponent);
		MobStateComponent mobState = default(MobStateComponent);
		while (query.MoveNext(ref marineComponent, ref mobState))
		{
			if (mobState.CurrentState != MobState.Dead)
			{
				alive++;
			}
		}
		return alive;
	}
}
