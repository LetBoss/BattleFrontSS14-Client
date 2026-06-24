using System;
using Content.Shared.Weapons.Ranged.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Physics.Events;

namespace Content.Shared._CIV14merka.Weapons;

public sealed class CivTargetedShotSystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<CivPhantomShotComponent, PreventCollideEvent>((EntityEventRefHandler<CivPhantomShotComponent, PreventCollideEvent>)OnPhantomPreventCollide, (Type[])null, (Type[])null);
	}

	private void OnPhantomPreventCollide(Entity<CivPhantomShotComponent> ent, ref PreventCollideEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		if (args.OtherEntity != ent.Comp.Target)
		{
			args.Cancelled = true;
		}
	}

	public void SetPhantomTarget(EntityUid projectile, EntityUid target)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		TargetedProjectileComponent targeted = ((EntitySystem)this).EnsureComp<TargetedProjectileComponent>(projectile);
		targeted.Target = target;
		((EntitySystem)this).Dirty(projectile, (IComponent)(object)targeted, (MetaDataComponent)null);
		CivPhantomShotComponent phantom = ((EntitySystem)this).EnsureComp<CivPhantomShotComponent>(projectile);
		phantom.Target = target;
		((EntitySystem)this).Dirty(projectile, (IComponent)(object)phantom, (MetaDataComponent)null);
	}
}
