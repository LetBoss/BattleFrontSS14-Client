using System;
using Content.Shared.Access.Components;
using Content.Shared.Popups;
using Content.Shared.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Shared.Access.Systems;

public sealed class ActivatableUIRequiresAccessSystem : EntitySystem
{
	[Dependency]
	private AccessReaderSystem _access;

	[Dependency]
	private SharedPopupSystem _popup;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ActivatableUIRequiresAccessComponent, ActivatableUIOpenAttemptEvent>((EntityEventRefHandler<ActivatableUIRequiresAccessComponent, ActivatableUIOpenAttemptEvent>)OnUIOpenAttempt, (Type[])null, (Type[])null);
	}

	private void OnUIOpenAttempt(Entity<ActivatableUIRequiresAccessComponent> activatableUI, ref ActivatableUIOpenAttemptEvent args)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		if (!((CancellableEntityEventArgs)args).Cancelled && !_access.IsAllowed(args.User, Entity<ActivatableUIRequiresAccessComponent>.op_Implicit(activatableUI)))
		{
			((CancellableEntityEventArgs)args).Cancel();
			if (activatableUI.Comp.PopupMessage.HasValue)
			{
				SharedPopupSystem popup = _popup;
				ILocalizationManager loc = base.Loc;
				LocId? popupMessage = activatableUI.Comp.PopupMessage;
				popup.PopupClient(loc.GetString(popupMessage.HasValue ? LocId.op_Implicit(popupMessage.GetValueOrDefault()) : null), Entity<ActivatableUIRequiresAccessComponent>.op_Implicit(activatableUI), args.User);
			}
		}
	}
}
