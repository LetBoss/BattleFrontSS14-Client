using System;
using Content.Shared._RMC14.Marines.Squads;
using Content.Shared._RMC14.Sprite;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Pointing;

public sealed class RMCPointingSystem : EntitySystem
{
	[Dependency]
	private SharedRMCSpriteSystem _rmcSprite;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<RMCPointingComponent, RMCSpawnPointingArrowEvent>((EntityEventRefHandler<RMCPointingComponent, RMCSpawnPointingArrowEvent>)OnGetPointingArrow, (Type[])null, (Type[])null);
	}

	private void OnGetPointingArrow(Entity<RMCPointingComponent> ent, ref RMCSpawnPointingArrowEvent ev)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		SquadMemberComponent member = default(SquadMemberComponent);
		if (!((EntitySystem)this).TryComp<SquadMemberComponent>(Entity<RMCPointingComponent>.op_Implicit(ent), ref member))
		{
			ev.Spawned = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(ent.Comp.Arrow), ev.Coordinates);
			return;
		}
		ev.Spawned = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(ent.Comp.SquadArrow), ev.Coordinates);
		_rmcSprite.SetColor(Entity<SpriteColorComponent>.op_Implicit(ev.Spawned.Value), member.BackgroundColor);
	}
}
