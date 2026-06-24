using System;
using Content.Shared.Alert;
using Content.Shared.Damage.Events;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Events;
using Content.Shared.Mobs.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared.Traits.Assorted;

public sealed class PainNumbnessSystem : EntitySystem
{
	[Dependency]
	private MobThresholdSystem _mobThresholdSystem;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<PainNumbnessComponent, ComponentInit>((ComponentEventHandler<PainNumbnessComponent, ComponentInit>)OnComponentInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PainNumbnessComponent, ComponentRemove>((ComponentEventHandler<PainNumbnessComponent, ComponentRemove>)OnComponentRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PainNumbnessComponent, BeforeForceSayEvent>((EntityEventRefHandler<PainNumbnessComponent, BeforeForceSayEvent>)OnChangeForceSay, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PainNumbnessComponent, BeforeAlertSeverityCheckEvent>((EntityEventRefHandler<PainNumbnessComponent, BeforeAlertSeverityCheckEvent>)OnAlertSeverityCheck, (Type[])null, (Type[])null);
	}

	private void OnComponentRemove(EntityUid uid, PainNumbnessComponent component, ComponentRemove args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<MobThresholdsComponent>(uid))
		{
			_mobThresholdSystem.VerifyThresholds(uid);
		}
	}

	private void OnComponentInit(EntityUid uid, PainNumbnessComponent component, ComponentInit args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<MobThresholdsComponent>(uid))
		{
			_mobThresholdSystem.VerifyThresholds(uid);
		}
	}

	private void OnChangeForceSay(Entity<PainNumbnessComponent> ent, ref BeforeForceSayEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		args.Prefix = ent.Comp.ForceSayNumbDataset;
	}

	private void OnAlertSeverityCheck(Entity<PainNumbnessComponent> ent, ref BeforeAlertSeverityCheckEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		if (args.CurrentAlert == ProtoId<AlertPrototype>.op_Implicit("HumanHealth"))
		{
			args.CancelUpdate = true;
		}
	}
}
