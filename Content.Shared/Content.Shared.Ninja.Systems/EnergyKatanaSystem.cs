using System;
using Content.Shared.Inventory.Events;
using Content.Shared.Ninja.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Ninja.Systems;

public sealed class EnergyKatanaSystem : EntitySystem
{
	[Dependency]
	private SharedSpaceNinjaSystem _ninja;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<EnergyKatanaComponent, GotEquippedEvent>((EntityEventRefHandler<EnergyKatanaComponent, GotEquippedEvent>)OnEquipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EnergyKatanaComponent, CheckDashEvent>((EntityEventRefHandler<EnergyKatanaComponent, CheckDashEvent>)OnCheckDash, (Type[])null, (Type[])null);
	}

	private void OnEquipped(Entity<EnergyKatanaComponent> ent, ref GotEquippedEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		_ninja.BindKatana(Entity<SpaceNinjaComponent>.op_Implicit(args.Equipee), Entity<EnergyKatanaComponent>.op_Implicit(ent));
	}

	private void OnCheckDash(Entity<EnergyKatanaComponent> ent, ref CheckDashEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		if (!_ninja.IsNinja(args.User))
		{
			args.Cancelled = true;
		}
	}
}
