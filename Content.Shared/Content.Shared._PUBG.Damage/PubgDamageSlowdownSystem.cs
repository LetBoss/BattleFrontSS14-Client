using System;
using Content.Shared.Damage;
using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

namespace Content.Shared._PUBG.Damage;

public sealed class PubgDamageSlowdownSystem : EntitySystem
{
	private const float SlowdownFactor = 0.5f;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<PubgCharacterComponent, ModifySlowOnDamageSpeedEvent>((EntityEventRefHandler<PubgCharacterComponent, ModifySlowOnDamageSpeedEvent>)OnModifySlowOnDamageSpeed, (Type[])null, new Type[1] { typeof(InventorySystem) });
	}

	private void OnModifySlowOnDamageSpeed(Entity<PubgCharacterComponent> ent, ref ModifySlowOnDamageSpeedEvent args)
	{
		if (!(args.Speed <= 0f) && !(args.Speed >= 1f))
		{
			args.Speed = 1f - (1f - args.Speed) * 0.5f;
		}
	}
}
