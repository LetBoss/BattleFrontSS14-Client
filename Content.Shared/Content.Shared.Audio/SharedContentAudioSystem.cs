using Robust.Shared.Audio.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Audio;

public abstract class SharedContentAudioSystem : EntitySystem
{
	[Dependency]
	protected SharedAudioSystem Audio;

	public const float DefaultVariation = 0.05f;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		Audio.OcclusionCollisionMask = 2;
	}

	protected void SilenceAudio()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		AllEntityQueryEnumerator<AudioComponent> query = ((EntitySystem)this).AllEntityQuery<AudioComponent>();
		EntityUid uid = default(EntityUid);
		AudioComponent comp = default(AudioComponent);
		while (query.MoveNext(ref uid, ref comp))
		{
			Audio.SetGain((EntityUid?)uid, 0f, comp);
		}
	}
}
