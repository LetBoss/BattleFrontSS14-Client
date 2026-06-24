using System;
using Content.Shared._RMC14.Xenonids.Construction;
using Content.Shared._RMC14.Xenonids.Construction.Events;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Client._RMC14.Xenonids.Construction;

public sealed class XenoConstructionAnimationVisualizerSystem : EntitySystem
{
	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SpriteSystem _sprite;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<XenoConstructionAnimationStartEvent>((EntityEventHandler<XenoConstructionAnimationStartEvent>)OnAnimateResinBuilding, (Type[])null, (Type[])null);
	}

	private void OnAnimateResinBuilding(XenoConstructionAnimationStartEvent ev)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? val = default(EntityUid?);
		EntityUid? val2 = default(EntityUid?);
		XenoConstructionAnimationComponent xenoConstructionAnimationComponent = default(XenoConstructionAnimationComponent);
		SpriteComponent item = default(SpriteComponent);
		if (((EntitySystem)this).TryGetEntity(ev.Effect, ref val) && ((EntitySystem)this).TryGetEntity(ev.Xeno, ref val2) && ((EntitySystem)this).TryComp<XenoConstructionAnimationComponent>(val, ref xenoConstructionAnimationComponent) && ((EntitySystem)this).TryComp<SpriteComponent>(val, ref item))
		{
			int num = default(int);
			_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((val.Value, item)), (Enum)XenoConstructionVisualLayers.Animation, ref num, false);
			_sprite.LayerGetRsiState(Entity<SpriteComponent>.op_Implicit((val.Value, item)), num);
			xenoConstructionAnimationComponent.AnimationTime = ev.BuildTime;
			xenoConstructionAnimationComponent.AnimationTimeFinished = _timing.CurTime + ev.BuildTime;
			Layer val3 = default(Layer);
			if (_sprite.TryGetLayer(Entity<SpriteComponent>.op_Implicit((val.Value, item)), num, ref val3, false) && val3.ActualState != null)
			{
				xenoConstructionAnimationComponent.TotalFrames = val3.ActualState.DelayCount;
			}
		}
	}

	private void Animate(EntityUid uid, SpriteComponent sprite, Enum layerKey, int frame)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		if (_sprite.LayerExists(Entity<SpriteComponent>.op_Implicit((uid, sprite)), layerKey))
		{
			ISpriteLayer obj = sprite[(object)layerKey];
			Layer val = (Layer)(object)((obj is Layer) ? obj : null);
			if (val != null)
			{
				_sprite.LayerSetAutoAnimated(val, val.AnimationFrame < frame);
			}
		}
	}

	public override void FrameUpdate(float frameTime)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).FrameUpdate(frameTime);
		EntityQueryEnumerator<XenoConstructionAnimationComponent, SpriteComponent> val = ((EntitySystem)this).EntityQueryEnumerator<XenoConstructionAnimationComponent, SpriteComponent>();
		EntityUid uid = default(EntityUid);
		XenoConstructionAnimationComponent xenoConstructionAnimationComponent = default(XenoConstructionAnimationComponent);
		SpriteComponent sprite = default(SpriteComponent);
		while (val.MoveNext(ref uid, ref xenoConstructionAnimationComponent, ref sprite))
		{
			double num = (xenoConstructionAnimationComponent.AnimationTimeFinished - _timing.CurTime) / xenoConstructionAnimationComponent.AnimationTime;
			if (num < 0.0)
			{
				num = 0.0;
			}
			int frame = (int)Math.Min((double)xenoConstructionAnimationComponent.TotalFrames * (1.0 - num), xenoConstructionAnimationComponent.TotalFrames - 1);
			Animate(uid, sprite, XenoConstructionVisualLayers.Animation, frame);
		}
	}
}
