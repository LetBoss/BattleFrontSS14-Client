using System;
using Content.Shared._RMC14.Atmos;
using Content.Shared.ActionBlocker;
using Content.Shared.Atmos.Components;
using Content.Shared.Coordinates;
using Content.Shared.Mobs.Components;
using Content.Shared.Popups;
using Content.Shared.Weapons.Melee;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Xenonids.TailFountain;

public sealed class XenoTailFountainSystem : EntitySystem
{
	[Dependency]
	private SharedRMCFlammableSystem _flame;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private ActionBlockerSystem _actionBlocker;

	[Dependency]
	private IGameTiming _timing;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<XenoTailFountainComponent, XenoTailFountainActionEvent>((EntityEventRefHandler<XenoTailFountainComponent, XenoTailFountainActionEvent>)OnTailFountainAction, (Type[])null, (Type[])null);
	}

	private void OnTailFountainAction(Entity<XenoTailFountainComponent> xeno, ref XenoTailFountainActionEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled || !_actionBlocker.CanAttack(Entity<XenoTailFountainComponent>.op_Implicit(xeno)))
		{
			return;
		}
		if (xeno.Owner == args.Target)
		{
			_popup.PopupClient(base.Loc.GetString("rmc-xeno-tail-fountain-fail-self"), Entity<XenoTailFountainComponent>.op_Implicit(xeno), Entity<XenoTailFountainComponent>.op_Implicit(xeno));
			return;
		}
		if (!((EntitySystem)this).HasComp<MobStateComponent>(args.Target))
		{
			_popup.PopupClient(base.Loc.GetString("rmc-xeno-tail-fountain-fail"), Entity<XenoTailFountainComponent>.op_Implicit(xeno), Entity<XenoTailFountainComponent>.op_Implicit(xeno));
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		_flame.Extinguish(Entity<FlammableComponent>.op_Implicit(args.Target));
		_audio.PlayPredicted(xeno.Comp.ExtinguishSound, args.Target, (EntityUid?)Entity<XenoTailFountainComponent>.op_Implicit(xeno), (AudioParams?)null);
		_popup.PopupPredicted(base.Loc.GetString("rmc-xeno-tail-fountain-self", (ValueTuple<string, object>)("target", args.Target)), base.Loc.GetString("rmc-xeno-tail-fountain-others", (ValueTuple<string, object>)("user", xeno), (ValueTuple<string, object>)("target", args.Target)), Entity<XenoTailFountainComponent>.op_Implicit(xeno), Entity<XenoTailFountainComponent>.op_Implicit(xeno), PopupType.SmallCaution);
		if (_net.IsServer)
		{
			((EntitySystem)this).SpawnAttachedTo(EntProtoId.op_Implicit(xeno.Comp.Acid), args.Target.ToCoordinates(), (ComponentRegistry)null, default(Angle));
		}
		MeleeWeaponComponent melee = default(MeleeWeaponComponent);
		if (((EntitySystem)this).TryComp<MeleeWeaponComponent>(Entity<XenoTailFountainComponent>.op_Implicit(xeno), ref melee))
		{
			if (_timing.CurTime < melee.NextAttack)
			{
				return;
			}
			melee.NextAttack = _timing.CurTime + TimeSpan.FromSeconds(1L);
			((EntitySystem)this).Dirty(Entity<XenoTailFountainComponent>.op_Implicit(xeno), (IComponent)(object)melee, (MetaDataComponent)null);
		}
		MeleeAttackEvent attackEv = new MeleeAttackEvent(Entity<XenoTailFountainComponent>.op_Implicit(xeno));
		((EntitySystem)this).RaiseLocalEvent<MeleeAttackEvent>(Entity<XenoTailFountainComponent>.op_Implicit(xeno), ref attackEv, false);
	}
}
