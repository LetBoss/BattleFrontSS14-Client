using System;
using Content.Shared._RMC14.Input;
using Content.Shared.Vehicle.Components;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Vehicle;

public sealed class VehicleOverchargeSystem : EntitySystem
{
	[Dependency]
	private readonly IGameTiming _timing;

	[Dependency]
	private readonly INetManager _net;

	[Dependency]
	private readonly SharedAudioSystem _audio;

	[Dependency]
	private readonly VehicleSystem _rmcVehicles;

	public override void Initialize()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Expected O, but got Unknown
		if (_net.IsClient)
		{
			CommandBinds.Builder.Bind(CMKeyFunctions.CMUniqueAction, InputCmdHandler.FromDelegate((StateInputCmdDelegate)delegate(ICommonSession? session)
			{
				//IL_0020: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_002f: Unknown result type (might be due to invalid IL or missing references)
				//IL_003c: Unknown result type (might be due to invalid IL or missing references)
				//IL_003e: Unknown result type (might be due to invalid IL or missing references)
				EntityUid? val = ((session != null) ? session.AttachedEntity : ((EntityUid?)null));
				if (val.HasValue)
				{
					EntityUid valueOrDefault = val.GetValueOrDefault();
					if (_rmcVehicles.TryResolveControlledVehicle(valueOrDefault, out var vehicle))
					{
						((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new VehicleOverchargeRequestEvent(((EntitySystem)this).GetNetEntity(vehicle, (MetaDataComponent)null)));
					}
				}
			}, (StateInputCmdDelegate)null, false, true)).Register<VehicleOverchargeSystem>();
		}
		((EntitySystem)this).SubscribeNetworkEvent<VehicleOverchargeRequestEvent>((EntitySessionEventHandler<VehicleOverchargeRequestEvent>)OnOverchargeRequest, (Type[])null, (Type[])null);
	}

	private void OnOverchargeRequest(VehicleOverchargeRequestEvent ev, EntitySessionEventArgs args)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsServer)
		{
			return;
		}
		EntityUid? attachedEntity = ((EntitySessionEventArgs)(ref args)).SenderSession.AttachedEntity;
		if (!attachedEntity.HasValue)
		{
			return;
		}
		EntityUid user = attachedEntity.GetValueOrDefault();
		EntityUid vehicle = ((EntitySystem)this).GetEntity(ev.Vehicle);
		VehicleComponent vehicleComp = default(VehicleComponent);
		if (!((EntitySystem)this).TryComp<VehicleComponent>(vehicle, ref vehicleComp))
		{
			return;
		}
		attachedEntity = vehicleComp.Operator;
		EntityUid val = user;
		VehicleOverchargeComponent overcharge = default(VehicleOverchargeComponent);
		if (!attachedEntity.HasValue || attachedEntity.GetValueOrDefault() != val || !((EntitySystem)this).TryComp<VehicleOverchargeComponent>(vehicle, ref overcharge))
		{
			return;
		}
		TimeSpan now = _timing.CurTime;
		if (!(overcharge.CooldownUntil > now) && !(overcharge.ActiveUntil > now))
		{
			overcharge.ActiveUntil = now + TimeSpan.FromSeconds(overcharge.Duration);
			overcharge.CooldownUntil = now + TimeSpan.FromSeconds(overcharge.Cooldown);
			if (overcharge.OverchargeSound != null)
			{
				_audio.PlayPvs(overcharge.OverchargeSound, vehicle, (AudioParams?)null);
			}
			((EntitySystem)this).Dirty(vehicle, (IComponent)(object)overcharge, (MetaDataComponent)null);
		}
	}
}
