using System;
using Content.Shared._RMC14.Dropship.Fabricator;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;

namespace Content.Client._RMC14.Dropship.Fabricator;

public sealed class DropshipFabricatorUISystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<DropshipFabricatorComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<DropshipFabricatorComponent, AfterAutoHandleStateEvent>)OnFabricatorState, (Type[])null, (Type[])null);
	}

	private void OnFabricatorState(Entity<DropshipFabricatorComponent> ent, ref AfterAutoHandleStateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			UserInterfaceComponent val = default(UserInterfaceComponent);
			if (!((EntitySystem)this).TryComp<UserInterfaceComponent>(Entity<DropshipFabricatorComponent>.op_Implicit(ent), ref val))
			{
				return;
			}
			foreach (BoundUserInterface value2 in val.ClientOpenInterfaces.Values)
			{
				if (value2 is DropshipFabricatorBui dropshipFabricatorBui)
				{
					dropshipFabricatorBui.Refresh();
				}
			}
		}
		catch (Exception value)
		{
			((EntitySystem)this).Log.Error($"Error refreshing {"DropshipFabricatorBui"}:\n{value}");
		}
	}
}
