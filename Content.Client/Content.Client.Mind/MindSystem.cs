using System;
using System.Collections.Generic;
using Content.Shared.Mind;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;

namespace Content.Client.Mind;

public sealed class MindSystem : SharedMindSystem
{
	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<MindComponent, AfterAutoHandleStateEvent>((ComponentEventRefHandler<MindComponent, AfterAutoHandleStateEvent>)OnHandleState, (Type[])null, (Type[])null);
	}

	private void OnHandleState(EntityUid uid, MindComponent component, ref AfterAutoHandleStateEvent args)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		foreach (var (key, val3) in UserMinds)
		{
			if (val3 == uid)
			{
				UserMinds.Remove(key);
			}
		}
		if (component.UserId.HasValue)
		{
			UserMinds[component.UserId.Value] = uid;
		}
	}
}
