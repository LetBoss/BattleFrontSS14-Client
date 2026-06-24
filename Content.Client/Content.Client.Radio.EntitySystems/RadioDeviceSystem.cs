using System;
using Content.Client.Radio.Ui;
using Content.Shared.Radio;
using Content.Shared.Radio.Components;
using Robust.Client.GameObjects;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Radio.EntitySystems;

public sealed class RadioDeviceSystem : EntitySystem
{
	[Dependency]
	private UserInterfaceSystem _ui;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<IntercomComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<IntercomComponent, AfterAutoHandleStateEvent>)OnAfterHandleState, (Type[])null, (Type[])null);
	}

	private void OnAfterHandleState(Entity<IntercomComponent> ent, ref AfterAutoHandleStateEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		IntercomBoundUserInterface intercomBoundUserInterface = default(IntercomBoundUserInterface);
		if (((SharedUserInterfaceSystem)_ui).TryGetOpenUi<IntercomBoundUserInterface>(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum)IntercomUiKey.Key, ref intercomBoundUserInterface))
		{
			intercomBoundUserInterface.Update(ent);
		}
	}
}
