using System;
using Content.Shared._RMC14.Camera;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;

namespace Content.Client._RMC14.Camera;

public sealed class RMCCameraSystem : SharedRMCCameraSystem
{
	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<RMCCameraComputerComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<RMCCameraComputerComponent, AfterAutoHandleStateEvent>)OnComputerState, (Type[])null, (Type[])null);
	}

	private void OnComputerState(Entity<RMCCameraComputerComponent> ent, ref AfterAutoHandleStateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			UserInterfaceComponent val = default(UserInterfaceComponent);
			if (!((EntitySystem)this).TryComp<UserInterfaceComponent>(Entity<RMCCameraComputerComponent>.op_Implicit(ent), ref val))
			{
				return;
			}
			foreach (BoundUserInterface value2 in val.ClientOpenInterfaces.Values)
			{
				if (value2 is RMCCameraBui rMCCameraBui)
				{
					rMCCameraBui.Refresh();
				}
			}
		}
		catch (Exception value)
		{
			((EntitySystem)this).Log.Error($"Error refreshing {"RMCCameraBui"}\n{value}");
		}
	}
}
