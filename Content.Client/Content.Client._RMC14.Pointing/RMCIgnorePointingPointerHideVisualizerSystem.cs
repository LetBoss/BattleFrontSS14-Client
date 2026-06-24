using System;
using Content.Shared._RMC14.Pointing;
using Content.Shared.Ghost;
using Content.Shared.Mobs.Components;
using Robust.Client.GameObjects;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;

namespace Content.Client._RMC14.Pointing;

public sealed class RMCIgnorePointingPointerHideVisualizerSystem : VisualizerSystem<RMCPointingArrowComponent>
{
	[Dependency]
	private IPlayerManager _player;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<RMCPointingArrowComponent, ComponentStartup>((EntityEventRefHandler<RMCPointingArrowComponent, ComponentStartup>)OnPointSpawn, (Type[])null, (Type[])null);
	}

	private void OnPointSpawn(Entity<RMCPointingArrowComponent> arrow, ref ComponentStartup args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent item = default(SpriteComponent);
		if (!((EntitySystem)this).TryComp<SpriteComponent>(Entity<RMCPointingArrowComponent>.op_Implicit(arrow), ref item))
		{
			return;
		}
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		RMCIgnorePointingComponent rMCIgnorePointingComponent = default(RMCIgnorePointingComponent);
		if (((EntitySystem)this).TryComp<RMCIgnorePointingComponent>(localEntity, ref rMCIgnorePointingComponent) && arrow.Comp.Source.HasValue)
		{
			EntityUid? entity = ((EntitySystem)this).GetEntity(arrow.Comp.Source);
			EntityUid? val = localEntity;
			EntityUid? val2 = entity;
			if ((val.HasValue != val2.HasValue || (val.HasValue && !(val.GetValueOrDefault() == val2.GetValueOrDefault()))) && ((rMCIgnorePointingComponent.IgnoreMobs && ((EntitySystem)this).HasComp<MobStateComponent>(entity)) || (rMCIgnorePointingComponent.IgnoreGhosts && ((EntitySystem)this).HasComp<GhostComponent>(entity))))
			{
				base.SpriteSystem.SetVisible(Entity<SpriteComponent>.op_Implicit((arrow.Owner, item)), false);
			}
		}
	}
}
