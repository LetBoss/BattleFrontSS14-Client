using System;
using Content.Shared.Popups;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Lock;

public sealed class LockingWhitelistSystem : EntitySystem
{
	[Dependency]
	private EntityWhitelistSystem _whitelistSystem;

	[Dependency]
	private SharedPopupSystem _popupSystem;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<LockingWhitelistComponent, UserLockToggleAttemptEvent>((EntityEventRefHandler<LockingWhitelistComponent, UserLockToggleAttemptEvent>)OnUserLockToggleAttempt, (Type[])null, (Type[])null);
	}

	private void OnUserLockToggleAttempt(Entity<LockingWhitelistComponent> ent, ref UserLockToggleAttemptEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		if (!_whitelistSystem.CheckBoth(args.Target, ent.Comp.Blacklist, ent.Comp.Whitelist))
		{
			if (!args.Silent)
			{
				_popupSystem.PopupClient(base.Loc.GetString("locking-whitelist-component-lock-toggle-deny"), ent.Owner);
			}
			args.Cancelled = true;
		}
	}
}
