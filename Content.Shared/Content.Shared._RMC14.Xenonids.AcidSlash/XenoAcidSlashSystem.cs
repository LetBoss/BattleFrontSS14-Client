using System;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Xenonids.AcidSlash;

public sealed class XenoAcidSlashSystem : EntitySystem
{
	[Dependency]
	private XenoSystem _xeno;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<XenoAcidSlashComponent, MeleeHitEvent>((EntityEventRefHandler<XenoAcidSlashComponent, MeleeHitEvent>)OnMeleeHit, (Type[])null, (Type[])null);
	}

	private void OnMeleeHit(Entity<XenoAcidSlashComponent> xeno, ref MeleeHitEvent args)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		if (!args.IsHit)
		{
			return;
		}
		foreach (EntityUid hit in args.HitEntities)
		{
			if (_xeno.CanAbilityAttackTarget(xeno.Owner, hit) && !((EntitySystem)this).HasComp<XenoComponent>(hit))
			{
				ComponentRegistry add = xeno.Comp.Acid;
				if (add != null)
				{
					base.EntityManager.AddComponents(hit, add, true);
				}
				break;
			}
		}
	}
}
