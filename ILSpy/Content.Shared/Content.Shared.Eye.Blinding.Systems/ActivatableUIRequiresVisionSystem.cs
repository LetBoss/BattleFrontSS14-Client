using System;
using Content.Shared.Eye.Blinding.Components;
using Content.Shared.Popups;
using Content.Shared.UserInterface;
using Robust.Shared.Collections;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Eye.Blinding.Systems;

public sealed class ActivatableUIRequiresVisionSystem : EntitySystem
{
	[Dependency]
	private SharedPopupSystem _popupSystem;

	[Dependency]
	private SharedUserInterfaceSystem _userInterfaceSystem;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ActivatableUIRequiresVisionComponent, ActivatableUIOpenAttemptEvent>((ComponentEventHandler<ActivatableUIRequiresVisionComponent, ActivatableUIOpenAttemptEvent>)OnOpenAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BlindableComponent, BlindnessChangedEvent>((ComponentEventRefHandler<BlindableComponent, BlindnessChangedEvent>)OnBlindnessChanged, (Type[])null, (Type[])null);
	}

	private void OnOpenAttempt(EntityUid uid, ActivatableUIRequiresVisionComponent component, ActivatableUIOpenAttemptEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		BlindableComponent blindable = default(BlindableComponent);
		if (!((CancellableEntityEventArgs)args).Cancelled && ((EntitySystem)this).TryComp<BlindableComponent>(args.User, ref blindable) && blindable.IsBlind)
		{
			_popupSystem.PopupClient(base.Loc.GetString("blindness-fail-attempt"), args.User, PopupType.MediumCaution);
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnBlindnessChanged(EntityUid uid, BlindableComponent component, ref BlindnessChangedEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Blind)
		{
			return;
		}
		ValueList<(EntityUid, Enum)> toClose = default(ValueList<(EntityUid, Enum)>);
		foreach (var bui in _userInterfaceSystem.GetActorUis(Entity<UserInterfaceUserComponent>.op_Implicit(uid)))
		{
			if (((EntitySystem)this).HasComp<ActivatableUIRequiresVisionComponent>(bui.Item1))
			{
				toClose.Add(bui);
			}
		}
		Enumerator<(EntityUid, Enum)> enumerator2 = toClose.GetEnumerator();
		try
		{
			while (enumerator2.MoveNext())
			{
				(EntityUid, Enum) bui2 = enumerator2.Current;
				_userInterfaceSystem.CloseUi(Entity<UserInterfaceComponent>.op_Implicit(bui2.Item1), bui2.Item2, (EntityUid?)uid, false);
			}
		}
		finally
		{
			((IDisposable)enumerator2/*cast due to constrained. prefix*/).Dispose();
		}
	}
}
