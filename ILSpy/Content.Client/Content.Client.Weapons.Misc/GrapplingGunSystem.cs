using Content.Client.Hands.Systems;
using Content.Shared.CombatMode;
using Content.Shared.Weapons.Misc;
using Content.Shared.Weapons.Ranged.Components;
using Robust.Client.GameObjects;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Dynamics.Joints;
using Robust.Shared.Player;

namespace Content.Client.Weapons.Misc;

public sealed class GrapplingGunSystem : SharedGrapplingGunSystem
{
	[Dependency]
	private HandsSystem _hands;

	[Dependency]
	private InputSystem _input;

	[Dependency]
	private IPlayerManager _player;

	public override void Update(float frameTime)
	{
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Invalid comparison between Unknown and I4
		base.Update(frameTime);
		if (!Timing.IsFirstTimePredicted)
		{
			return;
		}
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		EntityUid? activeHandEntity = _hands.GetActiveHandEntity();
		GrapplingGunComponent grapplingGunComponent = default(GrapplingGunComponent);
		JointComponent val = default(JointComponent);
		if (!((EntitySystem)this).TryComp<GrapplingGunComponent>(activeHandEntity, ref grapplingGunComponent) || !((EntitySystem)this).TryComp<JointComponent>(activeHandEntity, ref val) || !val.GetJoints.TryGetValue("grappling", out var value))
		{
			return;
		}
		DistanceJoint val2 = (DistanceJoint)(object)((value is DistanceJoint) ? value : null);
		if (val2 != null && !(val2.MaxLength <= val2.MinLength))
		{
			bool flag = (int)_input.CmdStates.GetState(EngineKeyFunctions.UseSecondary) == 1;
			CombatModeComponent combatModeComponent = default(CombatModeComponent);
			if (!((EntitySystem)this).TryComp<CombatModeComponent>(localEntity, ref combatModeComponent) || !combatModeComponent.IsInCombatMode)
			{
				flag = false;
			}
			if (grapplingGunComponent.Reeling != flag)
			{
				((EntitySystem)this).RaisePredictiveEvent<RequestGrapplingReelMessage>(new RequestGrapplingReelMessage(flag));
			}
		}
	}
}
