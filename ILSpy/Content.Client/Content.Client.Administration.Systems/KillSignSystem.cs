using System;
using System.Numerics;
using Content.Client.Administration.Components;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Utility;

namespace Content.Client.Administration.Systems;

public sealed class KillSignSystem : EntitySystem
{
	private enum KillSignKey
	{
		Key
	}

	[Dependency]
	private SpriteSystem _sprite;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<KillSignComponent, ComponentStartup>((ComponentEventHandler<KillSignComponent, ComponentStartup>)KillSignAdded, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<KillSignComponent, ComponentShutdown>((ComponentEventHandler<KillSignComponent, ComponentShutdown>)KillSignRemoved, (Type[])null, (Type[])null);
	}

	private void KillSignRemoved(EntityUid uid, KillSignComponent component, ComponentShutdown args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent item = default(SpriteComponent);
		int num = default(int);
		if (((EntitySystem)this).TryComp<SpriteComponent>(uid, ref item) && _sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, item)), (Enum)KillSignKey.Key, ref num, false))
		{
			_sprite.RemoveLayer(Entity<SpriteComponent>.op_Implicit((uid, item)), num, true);
		}
	}

	private void KillSignAdded(EntityUid uid, KillSignComponent component, ComponentStartup args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Expected O, but got Unknown
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent val = default(SpriteComponent);
		int num = default(int);
		if (((EntitySystem)this).TryComp<SpriteComponent>(uid, ref val) && !_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, val)), (Enum)KillSignKey.Key, ref num, false))
		{
			Box2 localBounds = _sprite.GetLocalBounds(Entity<SpriteComponent>.op_Implicit((uid, val)));
			float y = ((Box2)(ref localBounds)).Height / 2f + 0.1875f;
			int num2 = _sprite.AddLayer(Entity<SpriteComponent>.op_Implicit((uid, val)), (SpriteSpecifier)new Rsi(new ResPath("Objects/Misc/killsign.rsi"), "sign"), (int?)null);
			_sprite.LayerMapSet(Entity<SpriteComponent>.op_Implicit((uid, val)), (Enum)KillSignKey.Key, num2);
			_sprite.LayerSetOffset(Entity<SpriteComponent>.op_Implicit((uid, val)), num2, new Vector2(0f, y));
			val.LayerSetShader(num2, "unshaded");
		}
	}
}
