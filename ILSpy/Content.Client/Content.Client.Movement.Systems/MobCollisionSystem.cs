using System.Numerics;
using Content.Shared.CCVar;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Components;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Client.Movement.Systems;

public sealed class MobCollisionSystem : SharedMobCollisionSystem
{
	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private IPlayerManager _player;

	public override void Update(float frameTime)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		if (!CfgManager.GetCVar<bool>(CCVars.MovementMobPushing))
		{
			return;
		}
		if (_timing.IsFirstTimePredicted)
		{
			EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
			MobCollisionComponent item = default(MobCollisionComponent);
			PhysicsComponent item2 = default(PhysicsComponent);
			if (MobQuery.TryComp(localEntity, ref item) && PhysicsQuery.TryComp(localEntity, ref item2))
			{
				HandleCollisions(Entity<MobCollisionComponent, PhysicsComponent>.op_Implicit((localEntity.Value, item, item2)), frameTime);
			}
		}
		base.Update(frameTime);
	}

	protected override void RaiseCollisionEvent(EntityUid uid, Vector2 direction, float speedMod)
	{
		((EntitySystem)this).RaisePredictiveEvent<MobCollisionMessage>(new MobCollisionMessage
		{
			Direction = direction,
			SpeedModifier = speedMod
		});
	}
}
