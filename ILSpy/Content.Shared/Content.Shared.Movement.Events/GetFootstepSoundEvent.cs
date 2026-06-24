using System.Runtime.CompilerServices;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;

namespace Content.Shared.Movement.Events;

[ByRefEvent]
public record struct GetFootstepSoundEvent(EntityUid User)
{
	public readonly EntityUid User = User;

	public SoundSpecifier? Sound = null;

	[CompilerGenerated]
	public readonly void Deconstruct(out EntityUid User)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		User = this.User;
	}
}
