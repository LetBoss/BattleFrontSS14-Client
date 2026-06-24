using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Sound;

public abstract class SharedSpamEmitSoundRequirePowerSystem : EntitySystem
{
	[Dependency]
	protected SharedEmitSoundSystem EmitSound;
}
