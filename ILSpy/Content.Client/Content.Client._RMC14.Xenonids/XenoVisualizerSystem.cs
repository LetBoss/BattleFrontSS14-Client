using System;
using Content.Client._RMC14.Sprite;
using Content.Shared._RMC14.Sprite;
using Content.Shared._RMC14.Xenonids;
using Content.Shared._RMC14.Xenonids.Charge;
using Content.Shared._RMC14.Xenonids.Egg;
using Content.Shared._RMC14.Xenonids.Leap;
using Content.Shared._RMC14.Xenonids.Movement;
using Content.Shared._RMC14.Xenonids.Parasite;
using Content.Shared._RMC14.Xenonids.Rest;
using Content.Shared.DrawDepth;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.StatusEffect;
using Content.Shared.Stunnable;
using Content.Shared.Throwing;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client._RMC14.Xenonids;

public sealed class XenoVisualizerSystem : VisualizerSystem<XenoComponent>
{
	[Dependency]
	private MobStateSystem _mobState;

	[Dependency]
	private RMCSpriteSystem _rmcSprite;

	[Dependency]
	private SpriteSystem _sprite;

	private EntityQuery<XenoAnimateMovementComponent> _animateQuery;

	public override void Initialize()
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<XenoComponent, KnockedDownEvent>((EntityEventRefHandler<XenoComponent, KnockedDownEvent>)OnXenoKnockedDown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoComponent, StatusEffectEndedEvent>((EntityEventRefHandler<XenoComponent, StatusEffectEndedEvent>)OnXenoStatusEffectEnded, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoComponent, GetDrawDepthEvent>((EntityEventRefHandler<XenoComponent, GetDrawDepthEvent>)OnXenoGetDrawDepth, (Type[])null, (Type[])null);
		_animateQuery = ((EntitySystem)this).GetEntityQuery<XenoAnimateMovementComponent>();
	}

	private void OnXenoKnockedDown(Entity<XenoComponent> xeno, ref KnockedDownEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		UpdateSprite(Entity<SpriteComponent, MobStateComponent, AppearanceComponent, InputMoverComponent, ThrownItemComponent, XenoLeapingComponent, KnockedDownComponent>.op_Implicit(xeno.Owner));
	}

	private void OnXenoStatusEffectEnded(Entity<XenoComponent> xeno, ref StatusEffectEndedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		UpdateSprite(Entity<SpriteComponent, MobStateComponent, AppearanceComponent, InputMoverComponent, ThrownItemComponent, XenoLeapingComponent, KnockedDownComponent>.op_Implicit(xeno.Owner));
	}

	private void OnXenoGetDrawDepth(Entity<XenoComponent> ent, ref GetDrawDepthEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		if (_mobState.IsDead(Entity<XenoComponent>.op_Implicit(ent)) && args.DrawDepth > DrawDepth.DeadMobs)
		{
			args.DrawDepth = DrawDepth.DeadMobs;
		}
	}

	protected override void OnAppearanceChange(EntityUid uid, XenoComponent component, ref AppearanceChangeEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent sprite = args.Sprite;
		UpdateSprite(Entity<SpriteComponent, MobStateComponent, AppearanceComponent, InputMoverComponent, ThrownItemComponent, XenoLeapingComponent, KnockedDownComponent>.op_Implicit((ValueTuple<EntityUid, SpriteComponent, MobStateComponent, AppearanceComponent, InputMoverComponent, ThrownItemComponent>)(uid, sprite, null, args.Component, null, null)));
		_rmcSprite.UpdateDrawDepth(uid);
	}

	public void UpdateSprite(Entity<SpriteComponent?, MobStateComponent?, AppearanceComponent?, InputMoverComponent?, ThrownItemComponent?, XenoLeapingComponent?, KnockedDownComponent?> entity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_043b: Unknown result type (might be due to invalid IL or missing references)
		//IL_043c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0447: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_04eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0467: Unknown result type (might be due to invalid IL or missing references)
		//IL_0468: Unknown result type (might be due to invalid IL or missing references)
		//IL_0473: Unknown result type (might be due to invalid IL or missing references)
		//IL_0486: Unknown result type (might be due to invalid IL or missing references)
		//IL_0487: Unknown result type (might be due to invalid IL or missing references)
		//IL_0492: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		//IL_0413: Unknown result type (might be due to invalid IL or missing references)
		//IL_0414: Unknown result type (might be due to invalid IL or missing references)
		//IL_041f: Unknown result type (might be due to invalid IL or missing references)
		//IL_042b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
		Entity<SpriteComponent, MobStateComponent, AppearanceComponent, InputMoverComponent, ThrownItemComponent, XenoLeapingComponent, KnockedDownComponent> val = entity;
		EntityUid val2 = default(EntityUid);
		SpriteComponent val3 = default(SpriteComponent);
		MobStateComponent mobStateComponent = default(MobStateComponent);
		AppearanceComponent val4 = default(AppearanceComponent);
		InputMoverComponent inputMoverComponent = default(InputMoverComponent);
		ThrownItemComponent thrownItemComponent = default(ThrownItemComponent);
		XenoLeapingComponent xenoLeapingComponent = default(XenoLeapingComponent);
		KnockedDownComponent knockedDownComponent = default(KnockedDownComponent);
		val.Deconstruct(ref val2, ref val3, ref mobStateComponent, ref val4, ref inputMoverComponent, ref thrownItemComponent, ref xenoLeapingComponent, ref knockedDownComponent);
		SpriteComponent val5 = val3;
		MobStateComponent mobStateComponent2 = mobStateComponent;
		AppearanceComponent val6 = val4;
		InputMoverComponent inputMoverComponent2 = inputMoverComponent;
		ThrownItemComponent item = thrownItemComponent;
		XenoLeapingComponent item2 = xenoLeapingComponent;
		KnockedDownComponent knockedDownComponent2 = knockedDownComponent;
		if (!((EntitySystem)this).Resolve<SpriteComponent, AppearanceComponent>(Entity<SpriteComponent, MobStateComponent, AppearanceComponent, InputMoverComponent, ThrownItemComponent, XenoLeapingComponent, KnockedDownComponent>.op_Implicit(entity), ref val5, ref val6, false))
		{
			return;
		}
		MobState mobState = MobState.Alive;
		if (((EntitySystem)this).Resolve<MobStateComponent>(Entity<SpriteComponent, MobStateComponent, AppearanceComponent, InputMoverComponent, ThrownItemComponent, XenoLeapingComponent, KnockedDownComponent>.op_Implicit(entity), ref mobStateComponent2, false))
		{
			mobState = mobStateComponent2.CurrentState;
		}
		((EntitySystem)this).Resolve<InputMoverComponent, ThrownItemComponent, XenoLeapingComponent, KnockedDownComponent>(Entity<SpriteComponent, MobStateComponent, AppearanceComponent, InputMoverComponent, ThrownItemComponent, XenoLeapingComponent, KnockedDownComponent>.op_Implicit(entity), ref inputMoverComponent2, ref item, ref item2, ref knockedDownComponent2, false);
		if (knockedDownComponent2 != null && mobState != MobState.Dead)
		{
			mobState = MobState.Critical;
		}
		if (val5 == null)
		{
			return;
		}
		RSI baseRSI = val5.BaseRSI;
		int num = default(int);
		if (baseRSI == null || !_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((entity.Owner, val5)), (Enum)XenoVisualLayers.Base, ref num, false))
		{
			return;
		}
		string text = null;
		State val7 = default(State);
		switch (mobState)
		{
		case MobState.Critical:
			if (baseRSI.TryGetState(StateId.op_Implicit("crit"), ref val7))
			{
				_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((entity.Owner, val5)), num, StateId.op_Implicit("crit"));
			}
			break;
		case MobState.Dead:
			if (((EntitySystem)this).HasComp<ParasiteSpentComponent>(Entity<SpriteComponent, MobStateComponent, AppearanceComponent, InputMoverComponent, ThrownItemComponent, XenoLeapingComponent, KnockedDownComponent>.op_Implicit(entity)))
			{
				_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((entity.Owner, val5)), num, StateId.op_Implicit("impregnated"));
			}
			else if (baseRSI.TryGetState(StateId.op_Implicit("dead"), ref val7))
			{
				_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((entity.Owner, val5)), num, StateId.op_Implicit("dead"));
			}
			break;
		default:
		{
			XenoOvipositorCapableComponent xenoOvipositorCapableComponent = default(XenoOvipositorCapableComponent);
			XenoRestState xenoRestState = default(XenoRestState);
			bool flag = default(bool);
			bool flag2 = default(bool);
			bool flag3 = default(bool);
			if (((EntitySystem)this).HasComp<XenoAttachedOvipositorComponent>(Entity<SpriteComponent, MobStateComponent, AppearanceComponent, InputMoverComponent, ThrownItemComponent, XenoLeapingComponent, KnockedDownComponent>.op_Implicit(entity)) && ((EntitySystem)this).TryComp<XenoOvipositorCapableComponent>(Entity<SpriteComponent, MobStateComponent, AppearanceComponent, InputMoverComponent, ThrownItemComponent, XenoLeapingComponent, KnockedDownComponent>.op_Implicit(entity), ref xenoOvipositorCapableComponent))
			{
				text = xenoOvipositorCapableComponent.AttachedState;
			}
			else if (((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<XenoRestState>(Entity<SpriteComponent, MobStateComponent, AppearanceComponent, InputMoverComponent, ThrownItemComponent, XenoLeapingComponent, KnockedDownComponent>.op_Implicit(entity), (Enum)XenoVisualLayers.Base, ref xenoRestState, val6) && xenoRestState == XenoRestState.Resting && baseRSI.TryGetState(StateId.op_Implicit("sleeping"), ref val7))
			{
				_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((entity.Owner, val5)), num, StateId.op_Implicit("sleeping"));
			}
			else if (baseRSI.TryGetState(StateId.op_Implicit("thrown"), ref val7) && IsThrown(Entity<XenoLeapingComponent, ThrownItemComponent, ActiveXenoToggleChargingComponent>.op_Implicit((ValueTuple<EntityUid, XenoLeapingComponent, ThrownItemComponent, ActiveXenoToggleChargingComponent>)(Entity<SpriteComponent, MobStateComponent, AppearanceComponent, InputMoverComponent, ThrownItemComponent, XenoLeapingComponent, KnockedDownComponent>.op_Implicit(entity), item2, item, null))))
			{
				_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((entity.Owner, val5)), num, StateId.op_Implicit("thrown"));
			}
			else if (((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<bool>(Entity<SpriteComponent, MobStateComponent, AppearanceComponent, InputMoverComponent, ThrownItemComponent, XenoLeapingComponent, KnockedDownComponent>.op_Implicit(entity), (Enum)XenoVisualLayers.Fortify, ref flag, val6) && flag && baseRSI.TryGetState(StateId.op_Implicit("fortify"), ref val7))
			{
				_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((entity.Owner, val5)), num, StateId.op_Implicit("fortify"));
			}
			else if (((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<bool>(Entity<SpriteComponent, MobStateComponent, AppearanceComponent, InputMoverComponent, ThrownItemComponent, XenoLeapingComponent, KnockedDownComponent>.op_Implicit(entity), (Enum)XenoVisualLayers.Crest, ref flag2, val6) && flag2 && baseRSI.TryGetState(StateId.op_Implicit("crest"), ref val7))
			{
				_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((entity.Owner, val5)), num, StateId.op_Implicit("crest"));
			}
			else if (((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<bool>(Entity<SpriteComponent, MobStateComponent, AppearanceComponent, InputMoverComponent, ThrownItemComponent, XenoLeapingComponent, KnockedDownComponent>.op_Implicit(entity), (Enum)XenoVisualLayers.Burrow, ref flag3, val6) && flag3 && baseRSI.TryGetState(StateId.op_Implicit("burrowed"), ref val7))
			{
				_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((entity.Owner, val5)), num, StateId.op_Implicit("burrowed"));
			}
			else if (inputMoverComponent2 != null && (int)inputMoverComponent2.HeldMoveButtons > 0 && inputMoverComponent2.HeldMoveButtons != MoveButtons.Walk && baseRSI.TryGetState(StateId.op_Implicit("running"), ref val7))
			{
				_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((entity.Owner, val5)), num, StateId.op_Implicit("running"));
			}
			else if (baseRSI.TryGetState(StateId.op_Implicit("alive"), ref val7))
			{
				_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((entity.Owner, val5)), num, StateId.op_Implicit("alive"));
			}
			break;
		}
		}
		int num2 = default(int);
		if (_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((entity.Owner, val5)), (Enum)XenoVisualLayers.Ovipositor, ref num2, false))
		{
			if (text == null)
			{
				_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((entity.Owner, val5)), num2, false);
				_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((entity.Owner, val5)), num, true);
			}
			else
			{
				_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((entity.Owner, val5)), num2, StateId.op_Implicit(text));
				_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((entity.Owner, val5)), num2, true);
				_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((entity.Owner, val5)), num, false);
			}
		}
	}

	private bool IsThrown(Entity<XenoLeapingComponent?, ThrownItemComponent?, ActiveXenoToggleChargingComponent?> xeno)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		if (xeno.Comp1 == null && xeno.Comp2 == null)
		{
			if (((EntitySystem)this).Resolve<ActiveXenoToggleChargingComponent>(Entity<XenoLeapingComponent, ThrownItemComponent, ActiveXenoToggleChargingComponent>.op_Implicit(xeno), ref xeno.Comp3, false))
			{
				return xeno.Comp3.Stage > 0;
			}
			return false;
		}
		return true;
	}

	public override void Update(float frameTime)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<XenoComponent> val = ((EntitySystem)this).EntityQueryEnumerator<XenoComponent>();
		EntityUid val2 = default(EntityUid);
		XenoComponent xenoComponent = default(XenoComponent);
		while (val.MoveNext(ref val2, ref xenoComponent))
		{
			if (!_animateQuery.HasComp(val2))
			{
				UpdateSprite(Entity<SpriteComponent, MobStateComponent, AppearanceComponent, InputMoverComponent, ThrownItemComponent, XenoLeapingComponent, KnockedDownComponent>.op_Implicit(val2));
			}
		}
	}

	public override void FrameUpdate(float frameTime)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<XenoAnimateMovementComponent> val = ((EntitySystem)this).EntityQueryEnumerator<XenoAnimateMovementComponent>();
		EntityUid val2 = default(EntityUid);
		XenoAnimateMovementComponent xenoAnimateMovementComponent = default(XenoAnimateMovementComponent);
		while (val.MoveNext(ref val2, ref xenoAnimateMovementComponent))
		{
			UpdateSprite(Entity<SpriteComponent, MobStateComponent, AppearanceComponent, InputMoverComponent, ThrownItemComponent, XenoLeapingComponent, KnockedDownComponent>.op_Implicit(val2));
		}
	}
}
