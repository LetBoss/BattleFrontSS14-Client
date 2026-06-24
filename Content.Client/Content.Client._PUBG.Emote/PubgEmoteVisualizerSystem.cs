using System;
using System.Numerics;
using Content.Shared._PUBG.Emote;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Client._PUBG.Emote;

public sealed class PubgEmoteVisualizerSystem : EntitySystem
{
	private enum EmoteLayers : byte
	{
		Emote
	}

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SpriteSystem _sprite;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<PlayEmoteEvent>((EntityEventHandler<PlayEmoteEvent>)OnPlayEmote, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActiveEmoteComponent, ComponentShutdown>((ComponentEventHandler<ActiveEmoteComponent, ComponentShutdown>)OnEmoteShutdown, (Type[])null, (Type[])null);
	}

	public override void Update(float frameTime)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		EntityQueryEnumerator<ActiveEmoteComponent, SpriteComponent> val = ((EntitySystem)this).EntityQueryEnumerator<ActiveEmoteComponent, SpriteComponent>();
		EntityUid val2 = default(EntityUid);
		ActiveEmoteComponent activeEmoteComponent = default(ActiveEmoteComponent);
		SpriteComponent sprite = default(SpriteComponent);
		while (val.MoveNext(ref val2, ref activeEmoteComponent, ref sprite))
		{
			TimeSpan timeSpan = _timing.CurTime - activeEmoteComponent.StartTime;
			switch (activeEmoteComponent.PlayMode)
			{
			case EmotePlayMode.Once:
				if (HasAnimationFinished(val2, sprite, activeEmoteComponent))
				{
					((EntitySystem)this).RemCompDeferred<ActiveEmoteComponent>(val2);
				}
				break;
			case EmotePlayMode.Loop:
				if (activeEmoteComponent.Duration.HasValue && timeSpan.TotalSeconds >= (double)activeEmoteComponent.Duration.Value)
				{
					((EntitySystem)this).RemCompDeferred<ActiveEmoteComponent>(val2);
				}
				break;
			case EmotePlayMode.Count:
				if (activeEmoteComponent.RepeatCount.HasValue && GetAnimationCycles(val2, sprite, activeEmoteComponent) >= activeEmoteComponent.RepeatCount.Value)
				{
					((EntitySystem)this).RemCompDeferred<ActiveEmoteComponent>(val2);
				}
				break;
			}
		}
	}

	private void OnPlayEmote(PlayEmoteEvent ev)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		EntityUid entity = ((EntitySystem)this).GetEntity(ev.Entity);
		if (((EntitySystem)this).Exists(entity))
		{
			ActiveEmoteComponent activeEmoteComponent = ((EntitySystem)this).EnsureComp<ActiveEmoteComponent>(entity);
			activeEmoteComponent.PlayMode = ev.PlayMode;
			activeEmoteComponent.StartTime = _timing.CurTime;
			activeEmoteComponent.Duration = ev.Duration;
			activeEmoteComponent.RepeatCount = ev.RepeatCount;
			activeEmoteComponent.CurrentRepeats = 0;
			activeEmoteComponent.RsiPath = ev.RsiPath;
			activeEmoteComponent.StateName = ev.StateName;
			AddEmoteLayer(entity, ev.RsiPath, ev.StateName, ev.Scale);
		}
	}

	private void AddEmoteLayer(EntityUid uid, string rsiPath, string stateName, float scale = 1f)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Expected O, but got Unknown
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent val = default(SpriteComponent);
		if (((EntitySystem)this).TryComp<SpriteComponent>(uid, ref val) && !string.IsNullOrEmpty(rsiPath) && !string.IsNullOrEmpty(stateName))
		{
			int num = default(int);
			if (_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, val)), (Enum)EmoteLayers.Emote, ref num, false))
			{
				_sprite.RemoveLayer(Entity<SpriteComponent>.op_Implicit((uid, val)), num, true);
			}
			ResPath val2 = default(ResPath);
			((ResPath)(ref val2))._002Ector(rsiPath);
			int num2 = _sprite.AddLayer(Entity<SpriteComponent>.op_Implicit((uid, val)), (SpriteSpecifier)new Rsi(val2, stateName), (int?)null);
			_sprite.LayerMapSet(Entity<SpriteComponent>.op_Implicit((uid, val)), (Enum)EmoteLayers.Emote, num2);
			Box2 localBounds = _sprite.GetLocalBounds(Entity<SpriteComponent>.op_Implicit((uid, val)));
			float num3 = ((Box2)(ref localBounds)).Height / 2f + 0.3125f;
			float num4 = num3 * (scale - 1f) * 0.2f;
			_sprite.LayerSetOffset(Entity<SpriteComponent>.op_Implicit((uid, val)), num2, new Vector2(0f, num3 + num4));
			_sprite.LayerSetScale(Entity<SpriteComponent>.op_Implicit((uid, val)), num2, new Vector2(scale, scale));
			val.LayerSetShader(num2, "unshaded");
		}
	}

	private void OnEmoteShutdown(EntityUid uid, ActiveEmoteComponent component, ComponentShutdown args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent item = default(SpriteComponent);
		int num = default(int);
		if (((EntitySystem)this).TryComp<SpriteComponent>(uid, ref item) && _sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, item)), (Enum)EmoteLayers.Emote, ref num, false))
		{
			_sprite.RemoveLayer(Entity<SpriteComponent>.op_Implicit((uid, item)), num, true);
		}
	}

	private bool HasAnimationFinished(EntityUid uid, SpriteComponent sprite, ActiveEmoteComponent activeEmote)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		int num = default(int);
		if (!_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum)EmoteLayers.Emote, ref num, false))
		{
			return true;
		}
		ISpriteLayer val = sprite[num];
		if (val.Rsi == null || val.RsiState == StateId.op_Implicit((string)null))
		{
			return true;
		}
		State val2 = default(State);
		if (!val.Rsi.TryGetState(val.RsiState, ref val2))
		{
			return true;
		}
		if (!val2.IsAnimated)
		{
			return true;
		}
		float num2 = 0f;
		float[] delays = val2.Delays;
		foreach (float num3 in delays)
		{
			num2 += num3;
		}
		return (_timing.CurTime - activeEmote.StartTime).TotalSeconds >= (double)num2;
	}

	private int GetAnimationCycles(EntityUid uid, SpriteComponent sprite, ActiveEmoteComponent activeEmote)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		int num = default(int);
		if (!_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum)EmoteLayers.Emote, ref num, false))
		{
			return 0;
		}
		ISpriteLayer val = sprite[num];
		if (val.Rsi == null || val.RsiState == StateId.op_Implicit((string)null))
		{
			return 0;
		}
		State val2 = default(State);
		if (!val.Rsi.TryGetState(val.RsiState, ref val2))
		{
			return 0;
		}
		if (!val2.IsAnimated)
		{
			return 0;
		}
		float num2 = 0f;
		float[] delays = val2.Delays;
		foreach (float num3 in delays)
		{
			num2 += num3;
		}
		if (num2 <= 0f)
		{
			return 0;
		}
		return (int)((_timing.CurTime - activeEmote.StartTime).TotalSeconds / (double)num2);
	}
}
