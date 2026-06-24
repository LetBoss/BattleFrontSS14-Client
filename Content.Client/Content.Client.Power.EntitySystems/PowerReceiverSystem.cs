using System;
using System.Diagnostics.CodeAnalysis;
using Content.Client.Power.Components;
using Content.Shared.Examine;
using Content.Shared.Power;
using Content.Shared.Power.Components;
using Content.Shared.Power.EntitySystems;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;

namespace Content.Client.Power.EntitySystems;

public sealed class PowerReceiverSystem : SharedPowerReceiverSystem
{
	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ApcPowerReceiverComponent, ExaminedEvent>((EntityEventRefHandler<ApcPowerReceiverComponent, ExaminedEvent>)OnExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ApcPowerReceiverComponent, ComponentHandleState>((ComponentEventRefHandler<ApcPowerReceiverComponent, ComponentHandleState>)OnHandleState, (Type[])null, (Type[])null);
	}

	private void OnExamined(Entity<ApcPowerReceiverComponent> ent, ref ExaminedEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		args.PushMarkup(GetExamineText(ent.Comp.Powered));
	}

	private void OnHandleState(EntityUid uid, ApcPowerReceiverComponent component, ref ComponentHandleState args)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if (((ComponentHandleState)(ref args)).Current is ApcPowerReceiverComponentState apcPowerReceiverComponentState)
		{
			bool num = component.Powered != apcPowerReceiverComponentState.Powered;
			component.Powered = apcPowerReceiverComponentState.Powered;
			component.NeedsPower = apcPowerReceiverComponentState.NeedsPower;
			component.PowerDisabled = apcPowerReceiverComponentState.PowerDisabled;
			if (num)
			{
				RaisePower(Entity<SharedApcPowerReceiverComponent>.op_Implicit((ValueTuple<EntityUid, SharedApcPowerReceiverComponent>)(uid, component)));
			}
		}
	}

	protected override void RaisePower(Entity<SharedApcPowerReceiverComponent> entity)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		PowerChangedEvent powerChangedEvent = new PowerChangedEvent(entity.Comp.Powered, 0f);
		((EntitySystem)this).RaiseLocalEvent<PowerChangedEvent>(entity.Owner, ref powerChangedEvent, false);
	}

	public override bool ResolveApc(EntityUid entity, [NotNullWhen(true)] ref SharedApcPowerReceiverComponent? component)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		if (component != null)
		{
			return true;
		}
		ApcPowerReceiverComponent apcPowerReceiverComponent = default(ApcPowerReceiverComponent);
		if (!((EntitySystem)this).TryComp<ApcPowerReceiverComponent>(entity, ref apcPowerReceiverComponent))
		{
			return false;
		}
		component = apcPowerReceiverComponent;
		return true;
	}
}
