using System;
using System.Numerics;
using Content.Client.Gravity;
using Content.Shared.Anomaly;
using Content.Shared.Anomaly.Components;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

namespace Content.Client.Anomaly;

public sealed class AnomalySystem : SharedAnomalySystem
{
	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private FloatingVisualizerSystem _floating;

	[Dependency]
	private SpriteSystem _sprite;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<AnomalyComponent, AppearanceChangeEvent>((ComponentEventRefHandler<AnomalyComponent, AppearanceChangeEvent>)OnAppearanceChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AnomalyComponent, ComponentStartup>((ComponentEventHandler<AnomalyComponent, ComponentStartup>)OnStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AnomalyComponent, AnimationCompletedEvent>((ComponentEventHandler<AnomalyComponent, AnimationCompletedEvent>)OnAnimationComplete, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AnomalySupercriticalComponent, ComponentShutdown>((EntityEventRefHandler<AnomalySupercriticalComponent, ComponentShutdown>)OnShutdown, (Type[])null, (Type[])null);
	}

	private void OnStartup(EntityUid uid, AnomalyComponent component, ComponentStartup args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		_floating.FloatAnimation(uid, component.FloatingOffset, component.AnimationKey, component.AnimationTime);
	}

	private void OnAnimationComplete(EntityUid uid, AnomalyComponent component, AnimationCompletedEvent args)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (!(args.Key != component.AnimationKey))
		{
			_floating.FloatAnimation(uid, component.FloatingOffset, component.AnimationKey, component.AnimationTime);
		}
	}

	private void OnAppearanceChanged(EntityUid uid, AnomalyComponent component, ref AppearanceChangeEvent args)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent sprite = args.Sprite;
		if (sprite != null)
		{
			bool flag = default(bool);
			if (!Appearance.TryGetData<bool>(uid, (Enum)AnomalyVisuals.IsPulsing, ref flag, args.Component))
			{
				flag = false;
			}
			bool flag2 = default(bool);
			if (Appearance.TryGetData<bool>(uid, (Enum)AnomalyVisuals.Supercritical, ref flag2, args.Component) && flag2)
			{
				flag = flag2;
			}
			if (((EntitySystem)this).HasComp<AnomalySupercriticalComponent>(uid))
			{
				flag = true;
			}
			int num = default(int);
			int num2 = default(int);
			if (_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum)AnomalyVisualLayers.Base, ref num, false) && _sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum)AnomalyVisualLayers.Animated, ref num2, false))
			{
				_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num, !flag);
				_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num2, flag);
			}
		}
	}

	public override void Update(float frameTime)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		base.Update(frameTime);
		EntityQueryEnumerator<AnomalySupercriticalComponent, SpriteComponent> val = ((EntitySystem)this).EntityQueryEnumerator<AnomalySupercriticalComponent, SpriteComponent>();
		EntityUid item = default(EntityUid);
		AnomalySupercriticalComponent anomalySupercriticalComponent = default(AnomalySupercriticalComponent);
		SpriteComponent val2 = default(SpriteComponent);
		while (val.MoveNext(ref item, ref anomalySupercriticalComponent, ref val2))
		{
			float num = 1f - (float)((anomalySupercriticalComponent.EndTime - _timing.CurTime) / anomalySupercriticalComponent.SupercriticalDuration);
			float num2 = num * (anomalySupercriticalComponent.MaxScaleAmount - 1f) + 1f;
			_sprite.SetScale(Entity<SpriteComponent>.op_Implicit((item, val2)), new Vector2(num2, num2));
			byte b = (byte)(65f * (1f - num) + 190f);
			Color color = val2.Color;
			if (b < ((Color)(ref color)).AByte)
			{
				SpriteSystem sprite = _sprite;
				Entity<SpriteComponent> val3 = Entity<SpriteComponent>.op_Implicit((item, val2));
				color = val2.Color;
				sprite.SetColor(val3, ((Color)(ref color)).WithAlpha(b));
			}
		}
	}

	private void OnShutdown(Entity<AnomalySupercriticalComponent> ent, ref ComponentShutdown args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent val = default(SpriteComponent);
		if (((EntitySystem)this).TryComp<SpriteComponent>(Entity<AnomalySupercriticalComponent>.op_Implicit(ent), ref val))
		{
			_sprite.SetScale(Entity<SpriteComponent>.op_Implicit((ent.Owner, val)), Vector2.One);
			SpriteSystem sprite = _sprite;
			Entity<SpriteComponent> val2 = Entity<SpriteComponent>.op_Implicit((ent.Owner, val));
			Color color = val.Color;
			sprite.SetColor(val2, ((Color)(ref color)).WithAlpha(1f));
		}
	}
}
