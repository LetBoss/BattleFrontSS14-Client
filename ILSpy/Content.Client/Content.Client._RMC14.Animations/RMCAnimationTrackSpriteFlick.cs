using System;
using System.Collections.Generic;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Utility;

namespace Content.Client._RMC14.Animations;

public sealed class RMCAnimationTrackSpriteFlick : AnimationTrack
{
	public readonly record struct KeyFrame(Rsi Rsi, float KeyTime);

	public required List<KeyFrame> KeyFrames { get; init; }

	public required string LayerKey { get; init; }

	public override (int KeyFrameIndex, float FramePlayingTime) InitPlayback()
	{
		return (KeyFrameIndex: -1, FramePlayingTime: 0f);
	}

	public override (int KeyFrameIndex, float FramePlayingTime) AdvancePlayback(object context, int prevKeyFrameIndex, float prevPlayingTime, float frameTime)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		EntityUid val = (EntityUid)context;
		IEntityManager obj = IoCManager.Resolve<IEntityManager>();
		SpriteComponent component = obj.GetComponent<SpriteComponent>(val);
		Entity<SpriteComponent> val2 = default(Entity<SpriteComponent>);
		val2._002Ector(val, component);
		SpriteSystem val3 = obj.System<SpriteSystem>();
		float num = prevPlayingTime + frameTime;
		int i;
		for (i = prevKeyFrameIndex; i != KeyFrames.Count - 1 && KeyFrames[i + 1].KeyTime < num; i++)
		{
			num -= KeyFrames[i + 1].KeyTime;
		}
		if (i >= 0)
		{
			KeyFrame keyFrame = KeyFrames[i];
			Layer val4 = default(Layer);
			if (!val3.TryGetLayer(val2.AsNullable(), LayerKey, ref val4, false))
			{
				return (KeyFrameIndex: i, FramePlayingTime: num);
			}
			RSI actualRsi = val4.ActualRsi;
			State val5 = default(State);
			if (actualRsi != null && actualRsi.TryGetState(StateId.op_Implicit(keyFrame.Rsi.RsiState), ref val5))
			{
				float num2 = Math.Min(val5.AnimationLength - 0.01f, num);
				val3.LayerSetAutoAnimated(val4, false);
				val3.LayerSetSprite(val4, (SpriteSpecifier)(object)keyFrame.Rsi);
				val3.LayerSetRsiState(val4, StateId.op_Implicit(keyFrame.Rsi.RsiState), false);
				val3.LayerSetAnimationTime(val4, num2);
			}
		}
		return (KeyFrameIndex: i, FramePlayingTime: num);
	}
}
