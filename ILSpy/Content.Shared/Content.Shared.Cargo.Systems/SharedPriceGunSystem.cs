using System;
using Content.Shared.Cargo.Components;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;

namespace Content.Shared.Cargo.Systems;

public abstract class SharedPriceGunSystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<PriceGunComponent, GetVerbsEvent<UtilityVerb>>((ComponentEventHandler<PriceGunComponent, GetVerbsEvent<UtilityVerb>>)OnUtilityVerb, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PriceGunComponent, AfterInteractEvent>((EntityEventRefHandler<PriceGunComponent, AfterInteractEvent>)OnAfterInteract, (Type[])null, (Type[])null);
	}

	private void OnUtilityVerb(EntityUid uid, PriceGunComponent component, GetVerbsEvent<UtilityVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		if (args.CanAccess && args.CanInteract && args.Using.HasValue)
		{
			UtilityVerb verb = new UtilityVerb
			{
				Act = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_0017: Unknown result type (might be due to invalid IL or missing references)
					//IL_0022: Unknown result type (might be due to invalid IL or missing references)
					//IL_002d: Unknown result type (might be due to invalid IL or missing references)
					GetPriceOrBounty(Entity<PriceGunComponent>.op_Implicit((uid, component)), args.Target, args.User);
				},
				Text = base.Loc.GetString("price-gun-verb-text"),
				Message = base.Loc.GetString("price-gun-verb-message", (ValueTuple<string, object>)("object", Identity.Entity(args.Target, (IEntityManager)(object)base.EntityManager)))
			};
			args.Verbs.Add(verb);
		}
	}

	private void OnAfterInteract(Entity<PriceGunComponent> entity, ref AfterInteractEvent args)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if (args.CanReach && args.Target.HasValue && !((HandledEntityEventArgs)args).Handled)
		{
			AfterInteractEvent obj = args;
			((HandledEntityEventArgs)obj).Handled = ((HandledEntityEventArgs)obj).Handled | GetPriceOrBounty(entity, args.Target.Value, args.User);
		}
	}

	protected abstract bool GetPriceOrBounty(Entity<PriceGunComponent> entity, EntityUid target, EntityUid user);
}
