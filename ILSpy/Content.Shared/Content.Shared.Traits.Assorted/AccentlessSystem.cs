using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;

namespace Content.Shared.Traits.Assorted;

public sealed class AccentlessSystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<AccentlessComponent, ComponentStartup>((ComponentEventHandler<AccentlessComponent, ComponentStartup>)RemoveAccents, (Type[])null, (Type[])null);
	}

	private void RemoveAccents(EntityUid uid, AccentlessComponent component, ComponentStartup args)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		foreach (ComponentRegistryEntry value in ((Dictionary<string, ComponentRegistryEntry>)(object)component.RemovedAccents).Values)
		{
			IComponent accentComponent = value.Component;
			((EntitySystem)this).RemComp(uid, ((object)accentComponent).GetType());
		}
	}
}
