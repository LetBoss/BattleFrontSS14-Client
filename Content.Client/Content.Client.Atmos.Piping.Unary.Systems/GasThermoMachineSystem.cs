using System;
using Content.Client.Atmos.UI;
using Content.Shared.Atmos.Piping.Unary.Components;
using Content.Shared.Atmos.Piping.Unary.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Atmos.Piping.Unary.Systems;

public sealed class GasThermoMachineSystem : SharedGasThermoMachineSystem
{
	[Dependency]
	private SharedUserInterfaceSystem _ui;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<GasThermoMachineComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<GasThermoMachineComponent, AfterAutoHandleStateEvent>)OnGasAfterState, (Type[])null, (Type[])null);
	}

	private void OnGasAfterState(Entity<GasThermoMachineComponent> ent, ref AfterAutoHandleStateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		DirtyUI(ent.Owner, ent.Comp);
	}

	protected override void DirtyUI(EntityUid uid, GasThermoMachineComponent? thermoMachine, UserInterfaceComponent? ui = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		GasThermomachineBoundUserInterface gasThermomachineBoundUserInterface = default(GasThermomachineBoundUserInterface);
		if (_ui.TryGetOpenUi<GasThermomachineBoundUserInterface>(Entity<UserInterfaceComponent>.op_Implicit(uid), (Enum)ThermomachineUiKey.Key, ref gasThermomachineBoundUserInterface))
		{
			((BoundUserInterface)gasThermomachineBoundUserInterface).Update();
		}
	}
}
