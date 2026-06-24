using System;
using Content.Shared._RMC14.Intel.Tech;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;

namespace Content.Client._RMC14.Intel;

public sealed class TechControlConsoleSystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<TechControlConsoleComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<TechControlConsoleComponent, AfterAutoHandleStateEvent>)OnState, (Type[])null, (Type[])null);
	}

	private void OnState(Entity<TechControlConsoleComponent> ent, ref AfterAutoHandleStateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			UserInterfaceComponent val = default(UserInterfaceComponent);
			if (!((EntitySystem)this).TryComp<UserInterfaceComponent>(Entity<TechControlConsoleComponent>.op_Implicit(ent), ref val))
			{
				return;
			}
			foreach (BoundUserInterface value2 in val.ClientOpenInterfaces.Values)
			{
				if (value2 is TechControlConsoleBui techControlConsoleBui)
				{
					techControlConsoleBui.Refresh();
				}
			}
		}
		catch (Exception value)
		{
			((EntitySystem)this).Log.Error($"Error refreshing {"TechControlConsoleBui"}\n{value}");
		}
	}
}
