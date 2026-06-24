using System;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Events;
using Content.Shared.Movement.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared._CIV14merka.Teams;

public sealed class CivTeamMovementSystem : EntitySystem
{
	[Dependency]
	private MovementSpeedModifierSystem _movementSpeed;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<CivTeamMemberComponent, RefreshMovementSpeedModifiersEvent>((EntityEventRefHandler<CivTeamMemberComponent, RefreshMovementSpeedModifiersEvent>)OnRefreshSpeed, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CivTeamMemberComponent, MoveInputEvent>((EntityEventRefHandler<CivTeamMemberComponent, MoveInputEvent>)OnMoveInput, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CivTeamMemberComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<CivTeamMemberComponent, AfterAutoHandleStateEvent>)OnAfterAutoHandleState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MovementSpeedModifierComponent, ComponentStartup>((ComponentEventRefHandler<MovementSpeedModifierComponent, ComponentStartup>)OnMovementStartup, (Type[])null, (Type[])null);
	}

	private void OnRefreshSpeed(Entity<CivTeamMemberComponent> ent, ref RefreshMovementSpeedModifiersEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		float sprint = ((ent.Comp.SprintSpeedModifier > 0f) ? ent.Comp.SprintSpeedModifier : ent.Comp.WalkSpeedModifier);
		if (!(sprint <= 0f))
		{
			bool runActive = ent.Comp.RunActive;
			InputMoverComponent mover = default(InputMoverComponent);
			CivStaminaComponent stamina = default(CivStaminaComponent);
			if (((EntitySystem)this).TryComp<InputMoverComponent>(Entity<CivTeamMemberComponent>.op_Implicit(ent), ref mover) && ((EntitySystem)this).TryComp<CivStaminaComponent>(Entity<CivTeamMemberComponent>.op_Implicit(ent), ref stamina))
			{
				runActive = CivTeamRunHelper.ShouldRun(ent.Comp, mover, stamina);
			}
			float walkBase = 2.5f;
			float sprintBase = 4.5f;
			MovementSpeedModifierComponent movement = default(MovementSpeedModifierComponent);
			if (((EntitySystem)this).TryComp<MovementSpeedModifierComponent>(Entity<CivTeamMemberComponent>.op_Implicit(ent), ref movement) && movement.BaseWalkSpeed > 0f)
			{
				walkBase = movement.BaseWalkSpeed;
				sprintBase = movement.BaseSprintSpeed;
			}
			float walk = ((runActive && ent.Comp.RunSpeedModifier > 0f) ? ent.Comp.RunSpeedModifier : sprint) * sprintBase / walkBase;
			args.ModifySpeed(walk, sprint);
		}
	}

	private void OnAfterAutoHandleState(Entity<CivTeamMemberComponent> ent, ref AfterAutoHandleStateEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_movementSpeed.RefreshMovementSpeedModifiers(Entity<CivTeamMemberComponent>.op_Implicit(ent));
	}

	private void OnMoveInput(Entity<CivTeamMemberComponent> ent, ref MoveInputEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_movementSpeed.RefreshMovementSpeedModifiers(Entity<CivTeamMemberComponent>.op_Implicit(ent));
	}

	private void OnMovementStartup(EntityUid uid, MovementSpeedModifierComponent comp, ref ComponentStartup args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<CivTeamMemberComponent>(uid))
		{
			_movementSpeed.RefreshMovementSpeedModifiers(uid, comp);
		}
	}
}
