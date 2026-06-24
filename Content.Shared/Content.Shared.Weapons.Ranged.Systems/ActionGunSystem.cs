using System;
using Content.Shared.Actions;
using Content.Shared.Weapons.Ranged.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared.Weapons.Ranged.Systems;

public sealed class ActionGunSystem : EntitySystem
{
	[Dependency]
	private SharedActionsSystem _actions;

	[Dependency]
	private SharedGunSystem _gun;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ActionGunComponent, MapInitEvent>((EntityEventRefHandler<ActionGunComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActionGunComponent, ComponentShutdown>((EntityEventRefHandler<ActionGunComponent, ComponentShutdown>)OnShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActionGunComponent, ActionGunShootEvent>((EntityEventRefHandler<ActionGunComponent, ActionGunShootEvent>)OnShoot, (Type[])null, (Type[])null);
	}

	private void OnMapInit(Entity<ActionGunComponent> ent, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		if (!string.IsNullOrEmpty(EntProtoId.op_Implicit(ent.Comp.Action)))
		{
			_actions.AddAction(Entity<ActionGunComponent>.op_Implicit(ent), ref ent.Comp.ActionEntity, EntProtoId.op_Implicit(ent.Comp.Action));
			ent.Comp.Gun = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(ent.Comp.GunProto), (ComponentRegistry)null, true);
		}
	}

	private void OnShutdown(Entity<ActionGunComponent> ent, ref ComponentShutdown args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? gun = ent.Comp.Gun;
		if (gun.HasValue)
		{
			EntityUid gun2 = gun.GetValueOrDefault();
			((EntitySystem)this).QueueDel((EntityUid?)gun2);
		}
	}

	private void OnShoot(Entity<ActionGunComponent> ent, ref ActionGunShootEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		GunComponent gun = default(GunComponent);
		if (((EntitySystem)this).TryComp<GunComponent>(ent.Comp.Gun, ref gun))
		{
			_gun.AttemptShoot(Entity<ActionGunComponent>.op_Implicit(ent), ent.Comp.Gun.Value, gun, args.Target);
		}
	}
}
