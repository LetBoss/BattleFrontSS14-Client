using System;
using Content.Shared._RMC14.TrainingDummy;
using Content.Shared._RMC14.Xenonids.Energy;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Timing;

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
		((EntitySystem)this).SubscribeLocalEvent<XenoAciderGenerationComponent, MeleeHitEvent>((EntityEventRefHandler<XenoAciderGenerationComponent, MeleeHitEvent>)OnMeleeHit, (Type[])null, (Type[])null);
	}

	private void OnMeleeHit(Entity<XenoAciderGenerationComponent> xeno, ref MeleeHitEvent args)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		bool startGenerating = false;
		foreach (EntityUid hit in args.HitEntities)
		{
			if (_xeno.CanAbilityAttackTarget(Entity<XenoAciderGenerationComponent>.op_Implicit(xeno), hit))
			{
				if (((EntitySystem)this).HasComp<RMCTrainingDummyComponent>(hit))
				{
					return;
				}
				startGenerating = true;
				break;
			}
		}
		if (startGenerating)
		{
			_appearance.SetData(Entity<XenoAciderGenerationComponent>.op_Implicit(xeno), (Enum)XenoAcidGeneratingVisuals.Generating, (object)true, (AppearanceComponent)null);
			xeno.Comp.ExpireAt = _timing.CurTime + xeno.Comp.ExpireDuration;
			((EntitySystem)this).Dirty<XenoAciderGenerationComponent>(xeno, (MetaDataComponent)null);
		}
	}

	public override void Update(float frameTime)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<XenoAciderGenerationComponent, XenoEnergyComponent> query = ((EntitySystem)this).EntityQueryEnumerator<XenoAciderGenerationComponent, XenoEnergyComponent>();
		EntityUid uid = default(EntityUid);
		XenoAciderGenerationComponent acid = default(XenoAciderGenerationComponent);
		XenoEnergyComponent energy = default(XenoEnergyComponent);
		while (query.MoveNext(ref uid, ref acid, ref energy))
		{
			if (acid.ExpireAt.HasValue)
			{
				if (time >= acid.NextIncrease)
				{
					_energy.AddEnergy(Entity<XenoEnergyComponent>.op_Implicit((uid, energy)), acid.IncreaseAmount, popup: false);
					acid.NextIncrease = time + acid.TimeBetweenGeneration;
					((EntitySystem)this).Dirty(uid, (IComponent)(object)acid, (MetaDataComponent)null);
				}
				TimeSpan value = time;
				TimeSpan? expireAt = acid.ExpireAt;
				if (!(value < expireAt))
				{
					acid.ExpireAt = null;
					_appearance.SetData(uid, (Enum)XenoAcidGeneratingVisuals.Generating, (object)false, (AppearanceComponent)null);
					((EntitySystem)this).Dirty(uid, (IComponent)(object)acid, (MetaDataComponent)null);
				}
			}
		}
	}
}
