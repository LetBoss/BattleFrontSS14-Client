using System;
using Content.Shared._CIV14merka.Teams;
using Content.Shared.Damage;
using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

namespace Content.Shared._CIV14merka.Damage;

public sealed class CivDamageSlowdownSystem : EntitySystem
{
	private const float SlowdownFactor = 0.5f;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<CivTeamMemberComponent, ModifySlowOnDamageSpeedEvent>((EntityEventRefHandler<CivTeamMemberComponent, ModifySlowOnDamageSpeedEvent>)OnModifySlowOnDamageSpeed, (Type[])null, new Type[1] { typeof(InventorySystem) });
	}

	private void OnModifySlowOnDamageSpeed(Entity<CivTeamMemberComponent> ent, ref ModifySlowOnDamageSpeedEvent args)
	{
		if (!(args.Speed <= 0f) && !(args.Speed >= 1f))
		{
			args.Speed = 1f - (1f - args.Speed) * 0.5f;
		}
	}
}
