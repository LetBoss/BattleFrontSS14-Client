using Content.Shared._RMC14.NightVision;
using Content.Shared._RMC14.Xenonids.Burrow;
using Robust.Client.GameObjects;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;

namespace Content.Client._RMC14.Xenonids.Burrow;

public sealed class XenoBurrowSystem : SharedXenoBurrowSystem
{
	[Dependency]
	private IPlayerManager _player;

	[Dependency]
	private SpriteSystem _sprite;

	public override void Update(float frameTime)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		base.Update(frameTime);
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		EntityQueryEnumerator<XenoBurrowComponent, SpriteComponent, RMCNightVisionVisibleComponent> val = ((EntitySystem)this).EntityQueryEnumerator<XenoBurrowComponent, SpriteComponent, RMCNightVisionVisibleComponent>();
		EntityUid val2 = default(EntityUid);
		XenoBurrowComponent xenoBurrowComponent = default(XenoBurrowComponent);
		SpriteComponent item = default(SpriteComponent);
		RMCNightVisionVisibleComponent rMCNightVisionVisibleComponent = default(RMCNightVisionVisibleComponent);
		while (val.MoveNext(ref val2, ref xenoBurrowComponent, ref item, ref rMCNightVisionVisibleComponent))
		{
			EntityUid? val3 = localEntity;
			EntityUid val4 = val2;
			if (!val3.HasValue || val3.GetValueOrDefault() != val4)
			{
				_sprite.SetVisible(Entity<SpriteComponent>.op_Implicit((val2, item)), !xenoBurrowComponent.Active);
			}
			else
			{
				SpriteSystem sprite = _sprite;
				Entity<SpriteComponent> val5 = Entity<SpriteComponent>.op_Implicit((val2, item));
				Color val6;
				if (!xenoBurrowComponent.Active)
				{
					val6 = Color.White;
				}
				else
				{
					Color white = Color.White;
					val6 = ((Color)(ref white)).WithAlpha(0.4f);
				}
				sprite.SetColor(val5, val6);
			}
			if (xenoBurrowComponent.Active)
			{
				rMCNightVisionVisibleComponent.Transparency = 0.4f;
			}
			else
			{
				rMCNightVisionVisibleComponent.Transparency = null;
			}
		}
	}
}
