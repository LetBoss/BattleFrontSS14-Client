using System;
using Content.Shared.Atmos.Components;
using Content.Shared.Atmos.EntitySystems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;

namespace Content.Client.Atmos.EntitySystems;

public sealed class GasTankSystem : SharedGasTankSystem
{
	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<GasTankComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<GasTankComponent, AfterAutoHandleStateEvent>)OnGasTankState, (Type[])null, (Type[])null);
	}

	private void OnGasTankState(Entity<GasTankComponent> ent, ref AfterAutoHandleStateEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		BoundUserInterface val = default(BoundUserInterface);
		if (UI.TryGetOpenUi(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum)SharedGasTankUiKey.Key, ref val))
		{
			val.Update<GasTankBoundUserInterfaceState>();
		}
	}

	public override void UpdateUserInterface(Entity<GasTankComponent> ent)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		BoundUserInterface val = default(BoundUserInterface);
		if (UI.TryGetOpenUi(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum)SharedGasTankUiKey.Key, ref val))
		{
			val.Update<GasTankBoundUserInterfaceState>();
		}
	}
}
