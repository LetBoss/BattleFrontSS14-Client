using System;
using System.Numerics;
using Content.Shared.Camera;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.CameraShake;

public sealed class RMCCameraShakeSystem : EntitySystem
{
	[Dependency]
	private SharedCameraRecoilSystem _cameraRecoil;

	[Dependency]
	private IRobustRandom _random;

	[Dependency]
	private IGameTiming _timing;

	public void ShakeCamera(EntityUid user, int shakes, int strength, TimeSpan? spacing = null)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		TimeSpan valueOrDefault = spacing.GetValueOrDefault();
		if (!spacing.HasValue)
		{
			valueOrDefault = TimeSpan.FromSeconds(0.1);
			spacing = valueOrDefault;
		}
		RMCCameraShakingComponent shaking = ((EntitySystem)this).EnsureComp<RMCCameraShakingComponent>(user);
		shaking.Spacing = spacing.Value;
		shaking.Shakes = shakes;
		shaking.Strength = strength;
		((EntitySystem)this).Dirty(user, (IComponent)(object)shaking, (MetaDataComponent)null);
	}

	public void ShakeCamera(Filter filter, int shakes, int strength, TimeSpan? spacing = null)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		foreach (ICommonSession recipient2 in filter.Recipients)
		{
			EntityUid? attachedEntity = recipient2.AttachedEntity;
			if (attachedEntity.HasValue)
			{
				EntityUid recipient = attachedEntity.GetValueOrDefault();
				ShakeCamera(recipient, shakes, strength, spacing);
			}
		}
	}

	public override void Update(float frameTime)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<RMCCameraShakingComponent> shakingQuery = ((EntitySystem)this).EntityQueryEnumerator<RMCCameraShakingComponent>();
		EntityUid uid = default(EntityUid);
		RMCCameraShakingComponent shaking = default(RMCCameraShakingComponent);
		while (shakingQuery.MoveNext(ref uid, ref shaking))
		{
			if (shaking.Shakes <= 0)
			{
				((EntitySystem)this).RemCompDeferred<RMCCameraShakingComponent>(uid);
			}
			else if (!(time < shaking.NextShake))
			{
				shaking.Shakes--;
				shaking.NextShake = time + shaking.Spacing;
				Vector2 kick = _random.NextVector2Box((float)(-shaking.Strength), (float)shaking.Strength);
				_cameraRecoil.KickCamera(uid, kick);
			}
		}
	}
}
