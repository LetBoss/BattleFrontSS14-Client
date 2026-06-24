using System;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Shared.UserInterface;

public sealed class ActivatableUIRequiresAnchorSystem : EntitySystem
{
	[Dependency]
	private SharedPopupSystem _popup;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ActivatableUIRequiresAnchorComponent, ActivatableUIOpenAttemptEvent>((EntityEventRefHandler<ActivatableUIRequiresAnchorComponent, ActivatableUIOpenAttemptEvent>)OnActivatableUIOpenAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActivatableUIRequiresAnchorComponent, BoundUserInterfaceCheckRangeEvent>((EntityEventRefHandler<ActivatableUIRequiresAnchorComponent, BoundUserInterfaceCheckRangeEvent>)OnUICheck, (Type[])null, (Type[])null);
	}

	private void OnUICheck(Entity<ActivatableUIRequiresAnchorComponent> ent, ref BoundUserInterfaceCheckRangeEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if ((int)args.Result != 2 && !((EntitySystem)this).Transform(ent.Owner).Anchored)
		{
			args.Result = (BoundUserInterfaceRangeResult)2;
		}
	}

	private void OnActivatableUIOpenAttempt(Entity<ActivatableUIRequiresAnchorComponent> ent, ref ActivatableUIOpenAttemptEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		if (!((CancellableEntityEventArgs)args).Cancelled && !((EntitySystem)this).Transform(ent.Owner).Anchored)
		{
			if (ent.Comp.Popup.HasValue)
			{
				SharedPopupSystem popup = _popup;
				ILocalizationManager loc = base.Loc;
				LocId? popup2 = ent.Comp.Popup;
				popup.PopupClient(loc.GetString(popup2.HasValue ? LocId.op_Implicit(popup2.GetValueOrDefault()) : null), args.User);
			}
			((CancellableEntityEventArgs)args).Cancel();
		}
	}
}
