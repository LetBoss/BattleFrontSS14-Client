using System;
using Content.Client.Power.EntitySystems;
using Content.Shared.Popups;
using Content.Shared.Power.Components;
using Content.Shared.Power.EntitySystems;
using Content.Shared.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Power;

public sealed class ActivatableUIRequiresPowerSystem : SharedActivatableUIRequiresPowerSystem
{
	[Dependency]
	private SharedPopupSystem _popup;

	protected override void OnActivate(Entity<ActivatableUIRequiresPowerComponent> ent, ref ActivatableUIOpenAttemptEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		if (!((CancellableEntityEventArgs)args).Cancelled && !((EntitySystem)(object)this).IsPowered(ent.Owner, (IEntityManager)(object)((EntitySystem)this).EntityManager))
		{
			_popup.PopupClient(((EntitySystem)this).Loc.GetString("base-computer-ui-component-not-powered", (ValueTuple<string, object>)("machine", ent.Owner)), args.User, args.User);
			((CancellableEntityEventArgs)args).Cancel();
		}
	}
}
