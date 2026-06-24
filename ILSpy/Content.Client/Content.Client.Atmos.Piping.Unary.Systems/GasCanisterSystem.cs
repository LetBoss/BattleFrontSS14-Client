using System;
using Content.Client.Atmos.UI;
using Content.Shared.Atmos.Piping.Binary.Components;
using Content.Shared.Atmos.Piping.Unary.Components;
using Content.Shared.Atmos.Piping.Unary.Systems;
using Content.Shared.NodeContainer;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;

namespace Content.Client.Atmos.Piping.Unary.Systems;

public sealed class GasCanisterSystem : SharedGasCanisterSystem
{
	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<GasCanisterComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<GasCanisterComponent, AfterAutoHandleStateEvent>)OnGasState, (Type[])null, (Type[])null);
	}

	private void OnGasState(Entity<GasCanisterComponent> ent, ref AfterAutoHandleStateEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		GasCanisterBoundUserInterface gasCanisterBoundUserInterface = default(GasCanisterBoundUserInterface);
		if (UI.TryGetOpenUi<GasCanisterBoundUserInterface>(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum)GasCanisterUiKey.Key, ref gasCanisterBoundUserInterface))
		{
			((BoundUserInterface)gasCanisterBoundUserInterface).Update<GasCanisterBoundUserInterfaceState>();
		}
	}

	protected override void DirtyUI(EntityUid uid, GasCanisterComponent? component = null, NodeContainerComponent? nodes = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		GasCanisterBoundUserInterface gasCanisterBoundUserInterface = default(GasCanisterBoundUserInterface);
		if (UI.TryGetOpenUi<GasCanisterBoundUserInterface>(Entity<UserInterfaceComponent>.op_Implicit(uid), (Enum)GasCanisterUiKey.Key, ref gasCanisterBoundUserInterface))
		{
			((BoundUserInterface)gasCanisterBoundUserInterface).Update<GasCanisterBoundUserInterfaceState>();
		}
	}
}
