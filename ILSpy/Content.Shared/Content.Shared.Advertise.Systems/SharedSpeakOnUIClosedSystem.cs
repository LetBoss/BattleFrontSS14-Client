using Content.Shared.Advertise.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.Advertise.Systems;

public abstract class SharedSpeakOnUIClosedSystem : EntitySystem
{
	public bool TrySetFlag(Entity<SpeakOnUIClosedComponent?> entity, bool value = true)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<SpeakOnUIClosedComponent>(Entity<SpeakOnUIClosedComponent>.op_Implicit(entity), ref entity.Comp, true))
		{
			return false;
		}
		entity.Comp.Flag = value;
		((EntitySystem)this).Dirty<SpeakOnUIClosedComponent>(entity, (MetaDataComponent)null);
		return true;
	}
}
