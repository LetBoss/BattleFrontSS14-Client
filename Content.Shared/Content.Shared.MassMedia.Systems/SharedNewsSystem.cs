using Robust.Shared.GameObjects;

namespace Content.Shared.MassMedia.Systems;

public abstract class SharedNewsSystem : EntitySystem
{
	public const int MaxTitleLength = 25;

	public const int MaxContentLength = 2048;
}
