using System;
using Content.Client.Popups;
using Content.Shared._RMC14.Input;
using Content.Shared._RMC14.Weapons.Ranged.Flamer;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Client.Input;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Client._RMC14.Weapons.Ranged.Flamer;

public sealed class RMCFlamerSystem : SharedRMCFlamerSystem
{
	[Dependency]
	private IInputManager _input;

	[Dependency]
	private PopupSystem _popup;

	protected override void OnIgniterAttemptShoot(Entity<RMCIgniterComponent> ent, ref AttemptShootEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled)
		{
			base.OnIgniterAttemptShoot(ent, ref args);
			if (!ent.Comp.Enabled)
			{
				IKeyBinding val = default(IKeyBinding);
				string message = (_input.TryGetKeyBinding(CMKeyFunctions.CMUniqueAction, ref val) ? ((EntitySystem)this).Loc.GetString(LocId.op_Implicit(ent.Comp.PopupKey), (ValueTuple<string, object>)("key", val.GetKeyString())) : ((EntitySystem)this).Loc.GetString(LocId.op_Implicit(ent.Comp.Popup)));
				_popup.PopupClient(message, args.User, args.User);
			}
		}
	}
}
