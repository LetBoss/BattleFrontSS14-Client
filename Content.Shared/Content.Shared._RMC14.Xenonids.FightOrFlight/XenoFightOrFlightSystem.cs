using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Xenonids.Energy;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared.Coordinates;
using Content.Shared.Jittering;
using Content.Shared.Popups;
using Content.Shared.StatusEffect;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Xenonids.FightOrFlight;

public sealed class XenoFightOrFlightSystem : EntitySystem
{
	[Dependency]
	private SharedRMCActionsSystem _rmcActions;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private XenoEnergySystem _energy;

	[Dependency]
	private EntityLookupSystem _entityLookup;

	[Dependency]
	private SharedXenoHiveSystem _hive;

	[Dependency]
	private StatusEffectsSystem _status;

	[Dependency]
	private SharedJitteringSystem _jitter;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedPopupSystem _popup;

	private readonly HashSet<Entity<XenoComponent>> _xenos = new HashSet<Entity<XenoComponent>>();

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<XenoFightOrFlightComponent, XenoFightOrFlightActionEvent>((EntityEventRefHandler<XenoFightOrFlightComponent, XenoFightOrFlightActionEvent>)OnFightOrFlightAction, (Type[])null, (Type[])null);
	}

	private void OnFightOrFlightAction(Entity<XenoFightOrFlightComponent> xeno, ref XenoFightOrFlightActionEvent args)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		XenoEnergyComponent energy = default(XenoEnergyComponent);
		if (((HandledEntityEventArgs)args).Handled || !_rmcActions.TryUseAction(args) || !((EntitySystem)this).TryComp<XenoEnergyComponent>(Entity<XenoFightOrFlightComponent>.op_Implicit(xeno), ref energy))
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		_audio.PlayPredicted(xeno.Comp.RoarSound, Entity<XenoFightOrFlightComponent>.op_Implicit(xeno), (EntityUid?)Entity<XenoFightOrFlightComponent>.op_Implicit(xeno), (AudioParams?)null);
		bool highFury = _energy.HasEnergy(Entity<XenoEnergyComponent>.op_Implicit((xeno.Owner, energy)), xeno.Comp.FuryThreshold);
		_xenos.Clear();
		_entityLookup.GetEntitiesInRange<XenoComponent>(xeno.Owner.ToCoordinates(), (float)(highFury ? xeno.Comp.HighRange : xeno.Comp.LowRange), _xenos, (LookupFlags)110);
		if (_net.IsServer)
		{
			((EntitySystem)this).SpawnAttachedTo(EntProtoId.op_Implicit(highFury ? xeno.Comp.RoarEffect : xeno.Comp.WeakRoarEffect), xeno.Owner.ToCoordinates(), (ComponentRegistry)null, default(Angle));
		}
		foreach (Entity<XenoComponent> otherXeno in _xenos)
		{
			if (_hive.FromSameHive(Entity<HiveMemberComponent>.op_Implicit(xeno.Owner), Entity<HiveMemberComponent>.op_Implicit(otherXeno.Owner)))
			{
				ProtoId<StatusEffectPrototype>[] ailmentsRemove = xeno.Comp.AilmentsRemove;
				foreach (ProtoId<StatusEffectPrototype> status in ailmentsRemove)
				{
					_status.TryRemoveStatusEffect(Entity<XenoComponent>.op_Implicit(otherXeno), ProtoId<StatusEffectPrototype>.op_Implicit(status));
				}
				base.EntityManager.RemoveComponents(Entity<XenoComponent>.op_Implicit(otherXeno), xeno.Comp.ComponentsRemove);
				_jitter.DoJitter(Entity<XenoComponent>.op_Implicit(otherXeno), xeno.Comp.Jitter, refresh: true, 80f, 8f, forceValueChange: true);
				if (_net.IsServer)
				{
					((EntitySystem)this).SpawnAttachedTo(EntProtoId.op_Implicit(xeno.Comp.HealEffect), otherXeno.Owner.ToCoordinates(), (ComponentRegistry)null, default(Angle));
					_popup.PopupEntity(base.Loc.GetString("rmc-xeno-fof-effect"), Entity<XenoComponent>.op_Implicit(otherXeno), Entity<XenoComponent>.op_Implicit(otherXeno), PopupType.SmallCaution);
				}
			}
		}
	}
}
