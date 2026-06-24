using System;
using Content.Shared.Atmos.Piping.Binary.Components;
using Content.Shared.Atmos.Piping.Binary.Systems;
using Robust.Client.GameObjects;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Atmos.Piping.Binary.Systems;

public sealed class GasVolumePumpSystem : SharedGasVolumePumpSystem
{
	[Dependency]
	private UserInterfaceSystem _ui;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<GasVolumePumpComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<GasVolumePumpComponent, AfterAutoHandleStateEvent>)OnPumpState, (Type[])null, (Type[])null);
	}

	protected override void UpdateUi(Entity<GasVolumePumpComponent> entity)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		BoundUserInterface val = default(BoundUserInterface);
		if (((SharedUserInterfaceSystem)_ui).TryGetOpenUi(Entity<UserInterfaceComponent>.op_Implicit(entity.Owner), (Enum)GasVolumePumpUiKey.Key, ref val))
		{
			val.Update();
		}
	}

	private void OnPumpState(Entity<GasVolumePumpComponent> ent, ref AfterAutoHandleStateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateUi(ent);
	}
}
