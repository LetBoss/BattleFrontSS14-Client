using System;
using Content.Shared._RMC14.Input;
using Content.Shared._RMC14.Weapons.Ranged;
using Content.Shared.Examine;
using Content.Shared.Popups;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Client.Input;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Client._RMC14.Weapons.Ranged;

public sealed class PumpActionSystem : SharedPumpActionSystem
{
	[Dependency]
	private IInputManager _input;

	[Dependency]
	private SharedPopupSystem _popup;

	protected override void OnExamined(Entity<PumpActionComponent> ent, ref ExaminedEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		IKeyBinding val = default(IKeyBinding);
		if (_input.TryGetKeyBinding(CMKeyFunctions.CMUniqueAction, ref val))
		{
			args.PushMarkup(((EntitySystem)this).Loc.GetString(LocId.op_Implicit(ent.Comp.Examine)), 1);
		}
	}

	protected override void OnAttemptShoot(Entity<PumpActionComponent> ent, ref AttemptShootEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled)
		{
			base.OnAttemptShoot(ent, ref args);
			GunComponent gunComponent = default(GunComponent);
			if (!ent.Comp.Pumped && ((EntitySystem)this).TryComp<GunComponent>(ent.Owner, ref gunComponent) && !gunComponent.BurstActivated)
			{
				IKeyBinding val = default(IKeyBinding);
				string message = (_input.TryGetKeyBinding(CMKeyFunctions.CMUniqueAction, ref val) ? ((EntitySystem)this).Loc.GetString(LocId.op_Implicit(ent.Comp.PopupKey), (ValueTuple<string, object>)("key", val.GetKeyString())) : ((EntitySystem)this).Loc.GetString(LocId.op_Implicit(ent.Comp.Popup)));
				_popup.PopupClient(message, args.User, args.User);
			}
		}
	}
}
