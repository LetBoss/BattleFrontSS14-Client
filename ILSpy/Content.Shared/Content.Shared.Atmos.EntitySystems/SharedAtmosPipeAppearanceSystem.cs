using Content.Shared.Atmos.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.Atmos.EntitySystems;

public abstract class SharedAtmosPipeAppearanceSystem : EntitySystem
{
	protected int GetNumberOfPipeLayers(EntityUid uid, out AtmosPipeLayersComponent? atmosPipeLayers)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).TryComp<AtmosPipeLayersComponent>(uid, ref atmosPipeLayers))
		{
			return 1;
		}
		return atmosPipeLayers.NumberOfPipeLayers;
	}
}
