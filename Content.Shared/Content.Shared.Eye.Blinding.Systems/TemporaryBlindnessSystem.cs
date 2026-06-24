using System;
using Content.Shared.Eye.Blinding.Components;
using Content.Shared.StatusEffect;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared.Eye.Blinding.Systems;

public sealed class TemporaryBlindnessSystem : EntitySystem
{
	public static readonly ProtoId<StatusEffectPrototype> BlindingStatusEffect = ProtoId<StatusEffectPrototype>.op_Implicit("TemporaryBlindness");

	[Dependency]
	private BlindableSystem _blindableSystem;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<TemporaryBlindnessComponent, ComponentStartup>((ComponentEventHandler<TemporaryBlindnessComponent, ComponentStartup>)OnStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TemporaryBlindnessComponent, ComponentShutdown>((ComponentEventHandler<TemporaryBlindnessComponent, ComponentShutdown>)OnShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TemporaryBlindnessComponent, CanSeeAttemptEvent>((ComponentEventHandler<TemporaryBlindnessComponent, CanSeeAttemptEvent>)OnBlindTrySee, (Type[])null, (Type[])null);
	}

	private void OnStartup(EntityUid uid, TemporaryBlindnessComponent component, ComponentStartup args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_blindableSystem.UpdateIsBlind(Entity<BlindableComponent>.op_Implicit(uid));
	}

	private void OnShutdown(EntityUid uid, TemporaryBlindnessComponent component, ComponentShutdown args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_blindableSystem.UpdateIsBlind(Entity<BlindableComponent>.op_Implicit(uid));
	}

	private void OnBlindTrySee(EntityUid uid, TemporaryBlindnessComponent component, CanSeeAttemptEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		if ((int)((Component)component).LifeStage <= 6)
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}
}
