using System;
using Content.Shared._RMC14.Standing;
using Content.Shared.CCVar;
using Content.Shared.Movement.Components;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

namespace Content.Shared.Movement.Systems;

public sealed class MovementSpeedModifierSystem : EntitySystem
{
	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private IConfigurationManager _configManager;

	private float _frictionModifier;

	private float _airDamping;

	private float _offGridDamping;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<MovementSpeedModifierComponent, MapInitEvent>((EntityEventRefHandler<MovementSpeedModifierComponent, MapInitEvent>)OnModMapInit, (Type[])null, (Type[])null);
		EntitySystemSubscriptionExt.CVar<float>(((EntitySystem)this).Subs, _configManager, CCVars.TileFrictionModifier, (Action<float>)delegate(float value)
		{
			_frictionModifier = value;
		}, true);
		EntitySystemSubscriptionExt.CVar<float>(((EntitySystem)this).Subs, _configManager, CCVars.AirFriction, (Action<float>)delegate(float value)
		{
			_airDamping = value;
		}, true);
		EntitySystemSubscriptionExt.CVar<float>(((EntitySystem)this).Subs, _configManager, CCVars.OffgridFriction, (Action<float>)delegate(float value)
		{
			_offGridDamping = value;
		}, true);
	}

	private void OnModMapInit(Entity<MovementSpeedModifierComponent> ent, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.WeightlessAcceleration = ent.Comp.BaseWeightlessAcceleration;
		ent.Comp.WeightlessModifier = ent.Comp.BaseWeightlessModifier;
		ent.Comp.WeightlessFriction = _airDamping * ent.Comp.BaseWeightlessFriction;
		ent.Comp.WeightlessFrictionNoInput = _airDamping * ent.Comp.BaseWeightlessFriction;
		ent.Comp.OffGridFriction = _offGridDamping * ent.Comp.BaseWeightlessFriction;
		ent.Comp.Acceleration = ent.Comp.BaseAcceleration;
		ent.Comp.Friction = _frictionModifier * 2.5f;
		ent.Comp.FrictionNoInput = _frictionModifier * 2.5f;
		((EntitySystem)this).Dirty<MovementSpeedModifierComponent>(ent, (MetaDataComponent)null);
	}

	public void RefreshWeightlessModifiers(EntityUid uid, MovementSpeedModifierComponent? move = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<MovementSpeedModifierComponent>(uid, ref move, false) && !_timing.ApplyingState)
		{
			RefreshWeightlessModifiersEvent ev = new RefreshWeightlessModifiersEvent
			{
				WeightlessAcceleration = move.BaseWeightlessAcceleration,
				WeightlessAccelerationMod = 1f,
				WeightlessModifier = move.BaseWeightlessModifier,
				WeightlessFriction = move.BaseWeightlessFriction,
				WeightlessFrictionMod = 1f,
				WeightlessFrictionNoInput = move.BaseWeightlessFriction,
				WeightlessFrictionNoInputMod = 1f
			};
			((EntitySystem)this).RaiseLocalEvent<RefreshWeightlessModifiersEvent>(uid, ref ev, false);
			if (!MathHelper.CloseTo(ev.WeightlessAcceleration, move.WeightlessAcceleration, 1E-07f) || !MathHelper.CloseTo(ev.WeightlessModifier, move.WeightlessModifier, 1E-07f) || !MathHelper.CloseTo(ev.WeightlessFriction, move.WeightlessFriction, 1E-07f) || !MathHelper.CloseTo(ev.WeightlessFrictionNoInput, move.WeightlessFrictionNoInput, 1E-07f))
			{
				move.WeightlessAcceleration = ev.WeightlessAcceleration * ev.WeightlessAccelerationMod;
				move.WeightlessModifier = ev.WeightlessModifier;
				move.WeightlessFriction = _airDamping * ev.WeightlessFriction * ev.WeightlessFrictionMod;
				move.WeightlessFrictionNoInput = _airDamping * ev.WeightlessFrictionNoInput * ev.WeightlessFrictionNoInputMod;
				((EntitySystem)this).Dirty(uid, (IComponent)(object)move, (MetaDataComponent)null);
			}
		}
	}

	public void RefreshMovementSpeedModifiers(EntityUid uid, MovementSpeedModifierComponent? move = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<MovementSpeedModifierComponent>(uid, ref move, false) || _timing.ApplyingState)
		{
			return;
		}
		RefreshMovementSpeedModifiersEvent ev = new RefreshMovementSpeedModifiersEvent();
		((EntitySystem)this).RaiseLocalEvent<RefreshMovementSpeedModifiersEvent>(uid, ev, false);
		RMCRestComponent rest = default(RMCRestComponent);
		if (((EntitySystem)this).TryComp<RMCRestComponent>(uid, ref rest) && rest.Resting)
		{
			float walk = rest.RestingSpeed;
			if (ev.WalkSpeedModifier != 0f)
			{
				walk = rest.RestingSpeed / ev.WalkSpeedModifier;
			}
			float sprint = rest.RestingSpeed;
			if (ev.SprintSpeedModifier != 0f)
			{
				sprint = rest.RestingSpeed / ev.SprintSpeedModifier;
			}
			ev.ModifySpeed(walk, sprint);
		}
		if (!MathHelper.CloseTo(ev.WalkSpeedModifier, move.WalkSpeedModifier, 1E-07f) || !MathHelper.CloseTo(ev.SprintSpeedModifier, move.SprintSpeedModifier, 1E-07f))
		{
			move.WalkSpeedModifier = ev.WalkSpeedModifier;
			move.SprintSpeedModifier = ev.SprintSpeedModifier;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)move, (MetaDataComponent)null);
		}
	}

	public void ChangeBaseSpeed(EntityUid uid, float baseWalkSpeed, float baseSprintSpeed, float acceleration, MovementSpeedModifierComponent? move = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<MovementSpeedModifierComponent>(uid, ref move, false))
		{
			move.BaseWalkSpeed = baseWalkSpeed;
			move.BaseSprintSpeed = baseSprintSpeed;
			move.Acceleration = acceleration;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)move, (MetaDataComponent)null);
		}
	}

	public void RefreshFrictionModifiers(EntityUid uid, MovementSpeedModifierComponent? move = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<MovementSpeedModifierComponent>(uid, ref move, false) && !_timing.ApplyingState)
		{
			RefreshFrictionModifiersEvent ev = new RefreshFrictionModifiersEvent
			{
				Friction = move.BaseFriction,
				FrictionNoInput = move.BaseFriction,
				Acceleration = move.BaseAcceleration
			};
			((EntitySystem)this).RaiseLocalEvent<RefreshFrictionModifiersEvent>(uid, ref ev, false);
			if (!MathHelper.CloseTo(ev.Friction, move.Friction, 1E-07f) || !MathHelper.CloseTo(ev.FrictionNoInput, move.FrictionNoInput, 1E-07f) || !MathHelper.CloseTo(ev.Acceleration, move.Acceleration, 1E-07f))
			{
				move.Friction = _frictionModifier * ev.Friction;
				move.FrictionNoInput = _frictionModifier * ev.FrictionNoInput;
				move.Acceleration = ev.Acceleration;
				((EntitySystem)this).Dirty(uid, (IComponent)(object)move, (MetaDataComponent)null);
			}
		}
	}

	public void ChangeBaseFriction(EntityUid uid, float friction, float frictionNoInput, float acceleration, MovementSpeedModifierComponent? move = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<MovementSpeedModifierComponent>(uid, ref move, false))
		{
			move.BaseFriction = friction;
			move.FrictionNoInput = frictionNoInput;
			move.BaseAcceleration = acceleration;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)move, (MetaDataComponent)null);
		}
	}
}
