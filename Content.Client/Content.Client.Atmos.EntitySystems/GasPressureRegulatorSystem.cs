using System;
using Content.Shared.Atmos.EntitySystems;
using Content.Shared.Atmos.Piping.Binary.Components;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;

namespace Content.Client.Atmos.EntitySystems;

public sealed class GasPressureRegulatorSystem : SharedGasPressureRegulatorSystem
{
	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<GasPressureRegulatorComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<GasPressureRegulatorComponent, AfterAutoHandleStateEvent>)OnValveUpdate, (Type[])null, (Type[])null);
	}

	private void OnValveUpdate(Entity<GasPressureRegulatorComponent> ent, ref AfterAutoHandleStateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateUi(ent);
	}

	protected override void UpdateUi(Entity<GasPressureRegulatorComponent> ent)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		BoundUserInterface val = default(BoundUserInterface);
		if (UserInterfaceSystem.TryGetOpenUi(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum)GasPressureRegulatorUiKey.Key, ref val))
		{
			val.Update();
		}
	}
}
