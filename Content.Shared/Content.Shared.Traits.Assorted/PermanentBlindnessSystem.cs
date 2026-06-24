using System;
using Content.Shared.Examine;
using Content.Shared.Eye.Blinding.Components;
using Content.Shared.Eye.Blinding.Systems;
using Content.Shared.IdentityManagement;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Traits.Assorted;

public sealed class PermanentBlindnessSystem : EntitySystem
{
	[Dependency]
	private BlindableSystem _blinding;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<PermanentBlindnessComponent, MapInitEvent>((EntityEventRefHandler<PermanentBlindnessComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PermanentBlindnessComponent, ComponentShutdown>((EntityEventRefHandler<PermanentBlindnessComponent, ComponentShutdown>)OnShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PermanentBlindnessComponent, ExaminedEvent>((EntityEventRefHandler<PermanentBlindnessComponent, ExaminedEvent>)OnExamined, (Type[])null, (Type[])null);
	}

	private void OnExamined(Entity<PermanentBlindnessComponent> blindness, ref ExaminedEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		if (args.IsInDetailsRange && blindness.Comp.Blindness == 0)
		{
			args.PushMarkup(base.Loc.GetString("permanent-blindness-trait-examined", (ValueTuple<string, object>)("target", Identity.Entity(Entity<PermanentBlindnessComponent>.op_Implicit(blindness), (IEntityManager)(object)base.EntityManager))));
		}
	}

	private void OnShutdown(Entity<PermanentBlindnessComponent> blindness, ref ComponentShutdown args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		BlindableComponent blindable = default(BlindableComponent);
		if (((EntitySystem)this).TryComp<BlindableComponent>(blindness.Owner, ref blindable) && blindable.MinDamage != 0)
		{
			_blinding.SetMinDamage(Entity<BlindableComponent>.op_Implicit((blindness.Owner, blindable)), 0);
		}
	}

	private void OnMapInit(Entity<PermanentBlindnessComponent> blindness, ref MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		BlindableComponent blindable = default(BlindableComponent);
		if (((EntitySystem)this).TryComp<BlindableComponent>(blindness.Owner, ref blindable))
		{
			if (blindness.Comp.Blindness != 0)
			{
				_blinding.SetMinDamage(Entity<BlindableComponent>.op_Implicit((blindness.Owner, blindable)), blindness.Comp.Blindness);
				return;
			}
			int maxMagnitudeInt = 6;
			_blinding.SetMinDamage(Entity<BlindableComponent>.op_Implicit((blindness.Owner, blindable)), maxMagnitudeInt);
		}
	}
}
