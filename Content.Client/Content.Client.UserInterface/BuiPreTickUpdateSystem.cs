using System;
using System.Collections.Generic;
using Robust.Client.GameObjects;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Client.UserInterface;

public sealed class BuiPreTickUpdateSystem : EntitySystem
{
	[Dependency]
	private IPlayerManager _playerManager;

	[Dependency]
	private UserInterfaceSystem _uiSystem;

	[Dependency]
	private IGameTiming _gameTiming;

	private EntityQuery<UserInterfaceUserComponent> _userQuery;

	public override void Initialize()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Initialize();
		_userQuery = ((EntitySystem)this).GetEntityQuery<UserInterfaceUserComponent>();
	}

	public void RunUpdates()
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		if (!_gameTiming.IsFirstTimePredicted)
		{
			return;
		}
		ICommonSession localSession = ((ISharedPlayerManager)_playerManager).LocalSession;
		EntityUid? val = ((localSession != null) ? localSession.AttachedEntity : ((EntityUid?)null));
		if (!val.HasValue)
		{
			return;
		}
		EntityUid valueOrDefault = val.GetValueOrDefault();
		UserInterfaceUserComponent val2 = default(UserInterfaceUserComponent);
		if (!_userQuery.TryGetComponent(valueOrDefault, ref val2))
		{
			return;
		}
		BoundUserInterface val5 = default(BoundUserInterface);
		foreach (var (val4, list2) in val2.OpenInterfaces)
		{
			foreach (Enum item in list2)
			{
				if (((SharedUserInterfaceSystem)_uiSystem).TryGetOpenUi(Entity<UserInterfaceComponent>.op_Implicit(val4), item, ref val5) && val5 is IBuiPreTickUpdate buiPreTickUpdate)
				{
					buiPreTickUpdate.PreTickUpdate();
				}
			}
		}
	}
}
