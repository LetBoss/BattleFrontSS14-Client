using System;
using Content.Shared._RMC14.Ghost;
using Content.Shared.Ghost;
using Robust.Client.GameObjects;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;

namespace Content.Client._RMC14.Ghost;

public sealed class RMCVisibleOnlyToGhostsSystem : EntitySystem
{
	[Dependency]
	private IPlayerManager _player;

	[Dependency]
	private SpriteSystem _sprite;

	public override void Update(float frameTime)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		EntityQueryEnumerator<RMCVisibleToGhostsOnlyComponent, SpriteComponent> val = ((EntitySystem)this).EntityQueryEnumerator<RMCVisibleToGhostsOnlyComponent, SpriteComponent>();
		bool flag = ((EntitySystem)this).HasComp<GhostComponent>(localEntity);
		EntityUid item = default(EntityUid);
		RMCVisibleToGhostsOnlyComponent rMCVisibleToGhostsOnlyComponent = default(RMCVisibleToGhostsOnlyComponent);
		SpriteComponent item2 = default(SpriteComponent);
		int num = default(int);
		while (val.MoveNext(ref item, ref rMCVisibleToGhostsOnlyComponent, ref item2))
		{
			if (_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((item, item2)), (Enum)RMCGhostVisibleOnlyVisualLayers.Base, ref num, true))
			{
				_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((item, item2)), num, flag);
			}
		}
	}
}
