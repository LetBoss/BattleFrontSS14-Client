using System;
using System.Linq;
using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Armor;
using Content.Shared._RMC14.Explosion;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Xenonids.Crest;
using Content.Shared._RMC14.Xenonids.Headbutt;
using Content.Shared._RMC14.Xenonids.Rest;
using Content.Shared._RMC14.Xenonids.Sweep;
using Content.Shared.ActionBlocker;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Explosion;
using Content.Shared.Interaction.Events;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Movement.Events;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.StatusEffectNew;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map.Components;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Xenonids.Fortify;

public sealed class XenoFortifySystem : EntitySystem
{
	[Dependency]
	private ActionBlockerSystem _actionBlocker;

	[Dependency]
	private SharedActionsSystem _actions;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private CMArmorSystem _armor;

	[Dependency]
	private SharedRMCExplosionSystem _explode;

	[Dependency]
	private FixtureSystem _fixtures;

	[Dependency]
	private SharedPhysicsSystem _physics;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedRMCActionsSystem _rmcActions;

	[Dependency]
	private MovementSpeedModifierSystem _speed;

	[Dependency]
	private SharedTransformSystem _transform;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<XenoFortifyComponent, XenoFortifyActionEvent>((EntityEventRefHandler<XenoFortifyComponent, XenoFortifyActionEvent>)OnXenoFortifyAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoFortifyComponent, CMGetArmorEvent>((EntityEventRefHandler<XenoFortifyComponent, CMGetArmorEvent>)OnXenoFortifyGetArmor, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoFortifyComponent, BeforeStatusEffectAddedEvent>((EntityEventRefHandler<XenoFortifyComponent, BeforeStatusEffectAddedEvent>)OnXenoFortifyBeforeStatusAdded, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoFortifyComponent, GetExplosionResistanceEvent>((EntityEventRefHandler<XenoFortifyComponent, GetExplosionResistanceEvent>)OnXenoFortifyGetExplosionResistance, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoFortifyComponent, ChangeDirectionAttemptEvent>((EntityEventRefHandler<XenoFortifyComponent, ChangeDirectionAttemptEvent>)OnXenoFortifyCancel, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoFortifyComponent, UpdateCanMoveEvent>((EntityEventRefHandler<XenoFortifyComponent, UpdateCanMoveEvent>)OnXenoFortifyCancel, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoFortifyComponent, AttackAttemptEvent>((EntityEventRefHandler<XenoFortifyComponent, AttackAttemptEvent>)OnXenoFortifyAttack, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoFortifyComponent, XenoHeadbuttAttemptEvent>((EntityEventRefHandler<XenoFortifyComponent, XenoHeadbuttAttemptEvent>)OnXenoFortifyHeadbuttAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoFortifyComponent, XenoRestAttemptEvent>((EntityEventRefHandler<XenoFortifyComponent, XenoRestAttemptEvent>)OnXenoFortifyRestAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoFortifyComponent, XenoTailSweepAttemptEvent>((EntityEventRefHandler<XenoFortifyComponent, XenoTailSweepAttemptEvent>)OnXenoFortifyTailSweepAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoFortifyComponent, XenoToggleCrestAttemptEvent>((EntityEventRefHandler<XenoFortifyComponent, XenoToggleCrestAttemptEvent>)OnXenoFortifyToggleCrestAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoFortifyComponent, MobStateChangedEvent>((EntityEventRefHandler<XenoFortifyComponent, MobStateChangedEvent>)OnXenoFortifyMobStateChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoFortifyComponent, RefreshMovementSpeedModifiersEvent>((EntityEventRefHandler<XenoFortifyComponent, RefreshMovementSpeedModifiersEvent>)OnXenoFortifyRefreshSpeed, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoFortifyComponent, GetMeleeDamageEvent>((EntityEventRefHandler<XenoFortifyComponent, GetMeleeDamageEvent>)OnXenoFortifyGetMeleeDamage, (Type[])null, (Type[])null);
	}

	private void OnXenoFortifyAction(Entity<XenoFortifyComponent> xeno, ref XenoFortifyActionEvent args)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		XenoFortifyAttemptEvent attempt = default(XenoFortifyAttemptEvent);
		((EntitySystem)this).RaiseLocalEvent<XenoFortifyAttemptEvent>(Entity<XenoFortifyComponent>.op_Implicit(xeno), ref attempt, false);
		if (!attempt.Cancelled)
		{
			((HandledEntityEventArgs)args).Handled = true;
			_audio.PlayPredicted(xeno.Comp.FortifySound, Entity<XenoFortifyComponent>.op_Implicit(xeno), (EntityUid?)Entity<XenoFortifyComponent>.op_Implicit(xeno), (AudioParams?)null);
			if (xeno.Comp.Fortified)
			{
				Unfortify(xeno);
			}
			else
			{
				Fortify(xeno);
			}
		}
	}

	private void OnXenoFortifyGetArmor(Entity<XenoFortifyComponent> xeno, ref CMGetArmorEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if (xeno.Comp.Fortified)
		{
			args.XenoArmor += xeno.Comp.Armor;
			args.FrontalArmor += xeno.Comp.FrontalArmor;
		}
	}

	private void OnXenoFortifyBeforeStatusAdded(Entity<XenoFortifyComponent> xeno, ref BeforeStatusEffectAddedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if (xeno.Comp.Fortified)
		{
			string[] immuneToStatuses = xeno.Comp.ImmuneToStatuses;
			EntProtoId effect = args.Effect;
			if (Enumerable.Contains(immuneToStatuses, ((EntProtoId)(ref effect)).Id))
			{
				args.Cancelled = true;
			}
		}
	}

	private void OnXenoFortifyGetExplosionResistance(Entity<XenoFortifyComponent> xeno, ref GetExplosionResistanceEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		if (xeno.Comp.Fortified)
		{
			int armor = xeno.Comp.ExplosionArmor;
			if (armor > 0)
			{
				float resist = (float)Math.Pow(1.1, (double)armor / 5.0);
				args.DamageCoefficient /= resist;
			}
		}
	}

	private void OnXenoFortifyCancel<T>(Entity<XenoFortifyComponent> xeno, ref T args) where T : CancellableEntityEventArgs
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		if (xeno.Comp.Fortified && !xeno.Comp.CanMoveFortified)
		{
			((CancellableEntityEventArgs)args/*cast due to constrained. prefix*/).Cancel();
		}
	}

	private void OnXenoFortifyAttack(Entity<XenoFortifyComponent> xeno, ref AttackAttemptEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (!xeno.Comp.Fortified)
		{
			return;
		}
		EntityUid? target = args.Target;
		if (target.HasValue)
		{
			EntityUid target2 = target.GetValueOrDefault();
			if (((EntitySystem)this).HasComp<MobStateComponent>(target2))
			{
				((CancellableEntityEventArgs)args).Cancel();
			}
		}
	}

	private void OnXenoFortifyHeadbuttAttempt(Entity<XenoFortifyComponent> xeno, ref XenoHeadbuttAttemptEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if (!xeno.Comp.CanHeadbuttFortified && xeno.Comp.Fortified)
		{
			_popup.PopupClient(base.Loc.GetString("cm-xeno-fortify-cant-headbutt"), Entity<XenoFortifyComponent>.op_Implicit(xeno), Entity<XenoFortifyComponent>.op_Implicit(xeno));
			args.Cancelled = true;
		}
	}

	private void OnXenoFortifyRestAttempt(Entity<XenoFortifyComponent> xeno, ref XenoRestAttemptEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (xeno.Comp.Fortified)
		{
			_popup.PopupClient(base.Loc.GetString("cm-xeno-fortify-cant-rest"), Entity<XenoFortifyComponent>.op_Implicit(xeno), Entity<XenoFortifyComponent>.op_Implicit(xeno));
			args.Cancelled = true;
		}
	}

	private void OnXenoFortifyTailSweepAttempt(Entity<XenoFortifyComponent> xeno, ref XenoTailSweepAttemptEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (xeno.Comp.Fortified)
		{
			_popup.PopupClient(base.Loc.GetString("cm-xeno-fortify-cant-tail-sweep"), Entity<XenoFortifyComponent>.op_Implicit(xeno), Entity<XenoFortifyComponent>.op_Implicit(xeno));
			args.Cancelled = true;
		}
	}

	private void OnXenoFortifyToggleCrestAttempt(Entity<XenoFortifyComponent> xeno, ref XenoToggleCrestAttemptEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (xeno.Comp.Fortified)
		{
			_popup.PopupClient(base.Loc.GetString("cm-xeno-fortify-cant-toggle-crest"), Entity<XenoFortifyComponent>.op_Implicit(xeno), Entity<XenoFortifyComponent>.op_Implicit(xeno));
			args.Cancelled = true;
		}
	}

	private void OnXenoFortifyMobStateChanged(Entity<XenoFortifyComponent> xeno, ref MobStateChangedEvent args)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (args.NewMobState == MobState.Critical || args.NewMobState == MobState.Dead)
		{
			Unfortify(xeno);
		}
	}

	private void OnXenoFortifyRefreshSpeed(Entity<XenoFortifyComponent> xeno, ref RefreshMovementSpeedModifiersEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (xeno.Comp.CanMoveFortified && xeno.Comp.Fortified)
		{
			float modifier = xeno.Comp.MoveSpeedModifier.Float();
			args.ModifySpeed(modifier, modifier);
		}
	}

	private void OnXenoFortifyGetMeleeDamage(Entity<XenoFortifyComponent> xeno, ref GetMeleeDamageEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (xeno.Comp.Fortified)
		{
			args.Damage.ExclusiveAdd(xeno.Comp.DamageAddedFortified);
		}
	}

	private void Fortify(Entity<XenoFortifyComponent> xeno)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		xeno.Comp.Fortified = true;
		RMCSizeComponent size = default(RMCSizeComponent);
		if (((EntitySystem)this).TryComp<RMCSizeComponent>(Entity<XenoFortifyComponent>.op_Implicit(xeno), ref size))
		{
			xeno.Comp.OriginalSize = size.Size;
			size.Size = xeno.Comp.FortifySize;
			((EntitySystem)this).Dirty(xeno.Owner, (IComponent)(object)size, (MetaDataComponent)null);
		}
		StunOnExplosionReceivedComponent explode = default(StunOnExplosionReceivedComponent);
		if (xeno.Comp.ChangeExplosionWeakness && ((EntitySystem)this).TryComp<StunOnExplosionReceivedComponent>(Entity<XenoFortifyComponent>.op_Implicit(xeno), ref explode))
		{
			_explode.ChangeExplosionStunResistance(Entity<XenoFortifyComponent>.op_Implicit(xeno), explode, isStunnable: false);
		}
		if (!xeno.Comp.CanMoveFortified)
		{
			_fixtures.TryCreateFixture(Entity<XenoFortifyComponent>.op_Implicit(xeno), xeno.Comp.Shape, "cm-xeno-fortify", 1f, true, 223, 0, 0.4f, 0f, true, (FixturesComponent)null, (PhysicsComponent)null, (TransformComponent)null);
			_transform.AnchorEntity(Entity<TransformComponent>.op_Implicit((Entity<XenoFortifyComponent>.op_Implicit(xeno), ((EntitySystem)this).Transform(Entity<XenoFortifyComponent>.op_Implicit(xeno)))), (Entity<MapGridComponent>?)null);
		}
		else
		{
			_speed.RefreshMovementSpeedModifiers(Entity<XenoFortifyComponent>.op_Implicit(xeno));
		}
		FortifyUpdated(xeno);
	}

	private void Unfortify(Entity<XenoFortifyComponent> xeno)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		xeno.Comp.Fortified = false;
		RMCSizeComponent size = default(RMCSizeComponent);
		if (((EntitySystem)this).TryComp<RMCSizeComponent>(Entity<XenoFortifyComponent>.op_Implicit(xeno), ref size))
		{
			size.Size = xeno.Comp.OriginalSize ?? RMCSizes.Xeno;
			((EntitySystem)this).Dirty(xeno.Owner, (IComponent)(object)size, (MetaDataComponent)null);
		}
		StunOnExplosionReceivedComponent explode = default(StunOnExplosionReceivedComponent);
		if (xeno.Comp.ChangeExplosionWeakness && ((EntitySystem)this).TryComp<StunOnExplosionReceivedComponent>(Entity<XenoFortifyComponent>.op_Implicit(xeno), ref explode))
		{
			_explode.ChangeExplosionStunResistance(Entity<XenoFortifyComponent>.op_Implicit(xeno), explode, xeno.Comp.BaseWeakToExplosionStuns);
		}
		if (!xeno.Comp.CanMoveFortified)
		{
			_fixtures.DestroyFixture(Entity<XenoFortifyComponent>.op_Implicit(xeno), "cm-xeno-fortify", true, (PhysicsComponent)null, (FixturesComponent)null, (TransformComponent)null);
			_transform.Unanchor(Entity<XenoFortifyComponent>.op_Implicit(xeno), ((EntitySystem)this).Transform(Entity<XenoFortifyComponent>.op_Implicit(xeno)), true);
			_physics.TrySetBodyType(Entity<XenoFortifyComponent>.op_Implicit(xeno), (BodyType)2, (FixturesComponent)null, (PhysicsComponent)null, (TransformComponent)null);
		}
		else
		{
			_speed.RefreshMovementSpeedModifiers(Entity<XenoFortifyComponent>.op_Implicit(xeno));
		}
		FortifyUpdated(xeno);
	}

	private void FortifyUpdated(Entity<XenoFortifyComponent> xeno)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		_actionBlocker.UpdateCanMove(Entity<XenoFortifyComponent>.op_Implicit(xeno));
		_appearance.SetData(Entity<XenoFortifyComponent>.op_Implicit(xeno), (Enum)XenoVisualLayers.Fortify, (object)xeno.Comp.Fortified, (AppearanceComponent)null);
		foreach (Entity<ActionComponent> action in _rmcActions.GetActionsWithEvent<XenoFortifyActionEvent>(Entity<XenoFortifyComponent>.op_Implicit(xeno)))
		{
			_actions.SetToggled(action.AsNullable(), xeno.Comp.Fortified);
		}
		_armor.UpdateArmorValue(Entity<CMArmorComponent>.op_Implicit((ValueTuple<EntityUid, CMArmorComponent>)(Entity<XenoFortifyComponent>.op_Implicit(xeno), null)));
		((EntitySystem)this).Dirty<XenoFortifyComponent>(xeno, (MetaDataComponent)null);
		XenoFortifiedEvent ev = new XenoFortifiedEvent(xeno.Comp.Fortified);
		((EntitySystem)this).RaiseLocalEvent<XenoFortifiedEvent>(Entity<XenoFortifyComponent>.op_Implicit(xeno), ref ev, false);
	}
}
