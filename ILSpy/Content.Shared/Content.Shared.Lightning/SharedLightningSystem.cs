using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Random;

namespace Content.Shared.Lightning;

public abstract class SharedLightningSystem : EntitySystem
{
	[Dependency]
	private IRobustRandom _random;

	public string LightningRandomizer()
	{
		return "lightning_" + _random.Next(1, 12);
	}
}
