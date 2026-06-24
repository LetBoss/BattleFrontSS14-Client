using System;
using System.Collections.Generic;
using Content.Shared.UserInterface;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared._RMC14.UserInterface;

public sealed class RMCUserInterfaceSystem : EntitySystem
{
	[Dependency]
	private EntityWhitelistSystem _whitelist;

	private readonly List<(Entity<UserInterfaceComponent?> Ent, Action<Entity<UserInterfaceComponent?>, RMCUserInterfaceSystem> Act)> _toRefresh = new List<(Entity<UserInterfaceComponent>, Action<Entity<UserInterfaceComponent>, RMCUserInterfaceSystem>)>();

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<ActivatableUIBlacklistComponent, ActivatableUIOpenAttemptEvent>((EntityEventRefHandler<ActivatableUIBlacklistComponent, ActivatableUIOpenAttemptEvent>)OnUIBlacklistAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<UserBlacklistActivatableUIComponent, UserOpenActivatableUIAttemptEvent>((EntityEventRefHandler<UserBlacklistActivatableUIComponent, UserOpenActivatableUIAttemptEvent>)OnUIBlacklistUserAttempt, (Type[])null, (Type[])null);
	}

	private void OnUIBlacklistAttempt(Entity<ActivatableUIBlacklistComponent> ent, ref ActivatableUIOpenAttemptEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if (!((CancellableEntityEventArgs)args).Cancelled && !CanOpenUI(Entity<ActivatableUIBlacklistComponent>.op_Implicit((Entity<ActivatableUIBlacklistComponent>.op_Implicit(ent), Entity<ActivatableUIBlacklistComponent>.op_Implicit(ent))), Entity<UserBlacklistActivatableUIComponent>.op_Implicit(args.User), null))
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnUIBlacklistUserAttempt(Entity<UserBlacklistActivatableUIComponent> ent, ref UserOpenActivatableUIAttemptEvent args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		ActivatableUIComponent activatable = default(ActivatableUIComponent);
		if (!((CancellableEntityEventArgs)args).Cancelled && ((EntitySystem)this).TryComp<ActivatableUIComponent>(args.Target, ref activatable))
		{
			Enum key = activatable.Key;
			if (key != null && ent.Comp.Keys.Contains(key))
			{
				((CancellableEntityEventArgs)args).Cancel();
			}
		}
	}

	public bool CanOpenUI(Entity<ActivatableUIBlacklistComponent?> ent, Entity<UserBlacklistActivatableUIComponent?> user, Enum? key)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<ActivatableUIBlacklistComponent>(Entity<ActivatableUIBlacklistComponent>.op_Implicit(ent), ref ent.Comp, false) && _whitelist.IsBlacklistPass(ent.Comp.Blacklist, Entity<UserBlacklistActivatableUIComponent>.op_Implicit(user)))
		{
			return false;
		}
		if (key != null && ((EntitySystem)this).Resolve<UserBlacklistActivatableUIComponent>(Entity<UserBlacklistActivatableUIComponent>.op_Implicit(user), ref user.Comp, false) && user.Comp.Keys.Contains(key))
		{
			return false;
		}
		return true;
	}

	public void RefreshUIs<T>(Entity<UserInterfaceComponent?> uiEnt) where T : BoundUserInterface, IRefreshableBui
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		_toRefresh.Add((uiEnt, delegate(Entity<UserInterfaceComponent?> val, RMCUserInterfaceSystem system)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (((EntitySystem)system).TerminatingOrDeleted(Entity<UserInterfaceComponent>.op_Implicit(val), (MetaDataComponent)null) || !((EntitySystem)system).Resolve<UserInterfaceComponent>(Entity<UserInterfaceComponent>.op_Implicit(val), ref val.Comp, true))
				{
					return;
				}
				foreach (BoundUserInterface value2 in val.Comp.ClientOpenInterfaces.Values)
				{
					T val2 = (T)(object)((value2 is T) ? value2 : null);
					if (val2 != null)
					{
						val2.Refresh();
					}
				}
			}
			catch (Exception value)
			{
				((EntitySystem)system).Log.Error($"Error refreshing {"T"}\n{value}");
			}
		}));
	}

	public void TryBui<T>(Entity<UserInterfaceComponent?> ent, Action<T> action) where T : BoundUserInterface
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (!((EntitySystem)this).Resolve<UserInterfaceComponent>(Entity<UserInterfaceComponent>.op_Implicit(ent), ref ent.Comp, false))
			{
				return;
			}
			foreach (BoundUserInterface value2 in ent.Comp.ClientOpenInterfaces.Values)
			{
				T dialogUi = (T)(object)((value2 is T) ? value2 : null);
				if (dialogUi != null)
				{
					action(dialogUi);
				}
			}
		}
		catch (Exception value)
		{
			((EntitySystem)this).Log.Error($"Error getting {"T"}:\n{value}");
		}
	}

	public override void Update(float frameTime)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			foreach (var refresh in _toRefresh)
			{
				refresh.Act(refresh.Ent, this);
			}
		}
		finally
		{
			_toRefresh.Clear();
		}
	}
}
