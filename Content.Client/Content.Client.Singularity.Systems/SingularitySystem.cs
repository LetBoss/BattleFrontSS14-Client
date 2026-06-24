using System;
using Content.Shared.Singularity.Components;
using Content.Shared.Singularity.EntitySystems;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;

namespace Content.Client.Singularity.Systems;

public sealed class SingularitySystem : SharedSingularitySystem
{
	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<SingularityComponent, ComponentHandleState>((ComponentEventRefHandler<SingularityComponent, ComponentHandleState>)HandleSingularityState, (Type[])null, (Type[])null);
	}

	private void HandleSingularityState(EntityUid uid, SingularityComponent comp, ref ComponentHandleState args)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		if (((ComponentHandleState)(ref args)).Current is SingularityComponentState singularityComponentState)
		{
			SetLevel(uid, singularityComponentState.Level, comp);
		}
	}
}
