using System;
using Content.Shared.Atmos.Components;
using Content.Shared.Body.Components;
using Content.Shared.Body.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Body.Systems;

public sealed class InternalsSystem : SharedInternalsSystem
{
	[Dependency]
	private SharedUserInterfaceSystem _ui;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<InternalsComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<InternalsComponent, AfterAutoHandleStateEvent>)OnInternalsAfterState, (Type[])null, (Type[])null);
	}

	private void OnInternalsAfterState(Entity<InternalsComponent> ent, ref AfterAutoHandleStateEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		BoundUserInterface val = default(BoundUserInterface);
		if (ent.Comp.GasTankEntity.HasValue && _ui.TryGetOpenUi(Entity<UserInterfaceComponent>.op_Implicit(ent.Comp.GasTankEntity.Value), (Enum)SharedGasTankUiKey.Key, ref val))
		{
			val.Update();
		}
	}
}
