using Content.Shared.Shuttles.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.Shuttles.Systems;

public abstract class SharedRadarConsoleSystem : EntitySystem
{
	public const float DefaultMaxRange = 256f;

	protected virtual void UpdateState(EntityUid uid, RadarConsoleComponent component)
	{
	}

	public void SetRange(EntityUid uid, float value, RadarConsoleComponent component)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (!component.MaxRange.Equals(value))
		{
			component.MaxRange = value;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
			UpdateState(uid, component);
		}
	}
}
