using System;
using Content.Shared.Movement.Components;

namespace Content.Shared._CIV14merka.Teams;

public static class CivTeamRunHelper
{
	public static bool WantsRun(CivTeamMemberComponent member, InputMoverComponent mover)
	{
		if (!member.RunHeld)
		{
			return !mover.Sprinting;
		}
		return true;
	}

	public static bool ShouldRun(CivTeamMemberComponent member, InputMoverComponent mover, CivStaminaComponent stamina)
	{
		float threshold = Math.Clamp(member.RunStaminaThresholdRatio, 0f, 1f);
		bool canStartRun = stamina.MaxStamina > 0f && stamina.Stamina > stamina.MaxStamina * threshold;
		if (WantsRun(member, mover) && mover.HasDirectionalMovement && member.RunSpeedModifier > member.SprintSpeedModifier && stamina.Stamina > 0f)
		{
			return member.RunActive || canStartRun;
		}
		return false;
	}
}
