using System;
using Content.Shared._RMC14.Effect;
using Content.Shared._RMC14.Stealth;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

namespace Content.Client._RMC14.Effect;

public sealed class RMCEffectSystem : SharedRMCEffectSystem
{
	private const int OpacityDivider = 3;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SpriteSystem _sprite;

	public override void FrameUpdate(float frameTime)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		TimeSpan curTime = _timing.CurTime;
		EntityQueryEnumerator<EffectAlphaAnimationComponent, SpriteComponent> val = ((EntitySystem)this).EntityQueryEnumerator<EffectAlphaAnimationComponent, SpriteComponent>();
		EntityUid item = default(EntityUid);
		EffectAlphaAnimationComponent effectAlphaAnimationComponent = default(EffectAlphaAnimationComponent);
		SpriteComponent val2 = default(SpriteComponent);
		Color color;
		while (val.MoveNext(ref item, ref effectAlphaAnimationComponent, ref val2))
		{
			TimeSpan? spawnedAt = effectAlphaAnimationComponent.SpawnedAt;
			if (spawnedAt.HasValue)
			{
				TimeSpan valueOrDefault = spawnedAt.GetValueOrDefault();
				double num = MathHelper.Lerp((valueOrDefault + effectAlphaAnimationComponent.Delay).TotalSeconds, valueOrDefault.TotalSeconds, curTime.TotalSeconds);
				SpriteSystem sprite = _sprite;
				Entity<SpriteComponent> val3 = Entity<SpriteComponent>.op_Implicit((item, val2));
				color = val2.Color;
				sprite.SetColor(val3, ((Color)(ref color)).WithAlpha((float)num));
			}
		}
		EntityQueryEnumerator<RMCEffectComponent> val4 = ((EntitySystem)this).EntityQueryEnumerator<RMCEffectComponent>();
		EntityUid val5 = default(EntityUid);
		RMCEffectComponent rMCEffectComponent = default(RMCEffectComponent);
		SpriteComponent val6 = default(SpriteComponent);
		SpriteComponent val7 = default(SpriteComponent);
		EntityActiveInvisibleComponent entityActiveInvisibleComponent = default(EntityActiveInvisibleComponent);
		while (val4.MoveNext(ref val5, ref rMCEffectComponent))
		{
			EntityUid parentUid = ((EntitySystem)this).Transform(val5).ParentUid;
			if (!((EntitySystem)this).TryComp<SpriteComponent>(parentUid, ref val6) || !((EntitySystem)this).TryComp<SpriteComponent>(val5, ref val7))
			{
				break;
			}
			if (((EntitySystem)this).TryComp<EntityActiveInvisibleComponent>(parentUid, ref entityActiveInvisibleComponent) && entityActiveInvisibleComponent.Opacity < 1f)
			{
				SpriteSystem sprite2 = _sprite;
				Entity<SpriteComponent> val8 = Entity<SpriteComponent>.op_Implicit((val5, val7));
				color = val7.Color;
				sprite2.SetColor(val8, ((Color)(ref color)).WithAlpha(entityActiveInvisibleComponent.Opacity / 3f));
			}
			else if (val7.Color.A < 1f)
			{
				SpriteSystem sprite3 = _sprite;
				Entity<SpriteComponent> val9 = Entity<SpriteComponent>.op_Implicit((val5, val7));
				color = val7.Color;
				sprite3.SetColor(val9, ((Color)(ref color)).WithAlpha(val6.Color.A / 3f));
			}
		}
	}
}
