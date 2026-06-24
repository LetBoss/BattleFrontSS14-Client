using System;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Doors.Components;
using Content.Shared.Weapons.Melee;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Xenonids.ClawSharpness;

public sealed class XenoClawsSystem : EntitySystem
{
	[Dependency]
	private IPrototypeManager _protoManager;

	private EntityQuery<MeleeWeaponComponent> _meleeWeaponQuery;

	private EntityQuery<XenoClawsComponent> _xenoClawsQuery;

	private EntityQuery<XenoComponent> _xenoQuery;

	private readonly ProtoId<DamageGroupPrototype> _clawsDamageGroup = ProtoId<DamageGroupPrototype>.op_Implicit("Brute");

	public override void Initialize()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		_meleeWeaponQuery = ((EntitySystem)this).GetEntityQuery<MeleeWeaponComponent>();
		_xenoClawsQuery = ((EntitySystem)this).GetEntityQuery<XenoClawsComponent>();
		_xenoQuery = ((EntitySystem)this).GetEntityQuery<XenoComponent>();
		((EntitySystem)this).SubscribeLocalEvent<ReceiverXenoClawsComponent, DamageModifyEvent>((EntityEventRefHandler<ReceiverXenoClawsComponent, DamageModifyEvent>)OnReceiverDamageModify, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AirlockReceiverXenoClawsComponent, DamageModifyEvent>((EntityEventRefHandler<AirlockReceiverXenoClawsComponent, DamageModifyEvent>)OnAirlockReceiverDamageModify, (Type[])null, (Type[])null);
	}

	private void OnReceiverDamageModify(Entity<ReceiverXenoClawsComponent> ent, ref DamageModifyEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? xeno = args.Tool;
		ReceiverXenoClawsComponent receiver = ent.Comp;
		XenoClawsComponent claws = default(XenoClawsComponent);
		if (_meleeWeaponQuery.HasComp(xeno) && _xenoClawsQuery.TryComp(xeno, ref claws))
		{
			bool num = claws.ClawType.CompareTo(receiver.MinimumClawStrength) >= 0;
			bool hasRequiredTier = false;
			if (receiver.MinimumXenoTier.HasValue)
			{
				XenoComponent xenoComp = default(XenoComponent);
				hasRequiredTier = _xenoQuery.TryComp(xeno, ref xenoComp) && xenoComp.Tier >= receiver.MinimumXenoTier;
			}
			if (num || hasRequiredTier)
			{
				args.Damage = new DamageSpecifier(_protoManager.Index<DamageGroupPrototype>(_clawsDamageGroup), receiver.MaxHealth / (float)receiver.HitsToDestroy);
			}
			else
			{
				args.Damage = new DamageSpecifier(_protoManager.Index<DamageGroupPrototype>(_clawsDamageGroup), 0);
			}
		}
	}

	private void OnAirlockReceiverDamageModify(Entity<AirlockReceiverXenoClawsComponent> ent, ref DamageModifyEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? xeno = args.Tool;
		AirlockReceiverXenoClawsComponent receiver = ent.Comp;
		DoorComponent door = default(DoorComponent);
		DoorBoltComponent bolt = default(DoorBoltComponent);
		if (!_meleeWeaponQuery.HasComp(xeno) || !((EntitySystem)this).TryComp<DoorComponent>(Entity<AirlockReceiverXenoClawsComponent>.op_Implicit(ent), ref door) || !((EntitySystem)this).TryComp<DoorBoltComponent>(Entity<AirlockReceiverXenoClawsComponent>.op_Implicit(ent), ref bolt))
		{
			return;
		}
		DamageSpecifier damage = new DamageSpecifier(_protoManager.Index<DamageGroupPrototype>(_clawsDamageGroup), 0);
		XenoClawsComponent claws = default(XenoClawsComponent);
		if (_xenoClawsQuery.TryComp(xeno, ref claws) && claws.ClawType.CompareTo(receiver.MinimumClawStrength) >= 0)
		{
			if (bolt.BoltsDown)
			{
				damage = new DamageSpecifier(_protoManager.Index<DamageGroupPrototype>(_clawsDamageGroup), receiver.MaxHealth / (float)receiver.HitsToDestroyBolted);
			}
			if (door.State == DoorState.Welded)
			{
				damage = new DamageSpecifier(_protoManager.Index<DamageGroupPrototype>(_clawsDamageGroup), receiver.MaxHealth / (float)receiver.HitsToDestroyWelded);
			}
		}
		args.Damage = damage;
	}
}
