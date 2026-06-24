using System;
using Content.Shared._RMC14.Ghost;
using Content.Shared.Emoting;
using Content.Shared.Hands;
using Content.Shared.Interaction.Events;
using Content.Shared.Item;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Ghost;

public abstract class SharedGhostSystem : EntitySystem
{
	[Dependency]
	protected SharedPopupSystem Popup;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<GhostComponent, UseAttemptEvent>((ComponentEventHandler<GhostComponent, UseAttemptEvent>)OnAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GhostComponent, InteractionAttemptEvent>((EntityEventRefHandler<GhostComponent, InteractionAttemptEvent>)OnAttemptInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GhostComponent, EmoteAttemptEvent>((ComponentEventHandler<GhostComponent, EmoteAttemptEvent>)OnAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GhostComponent, DropAttemptEvent>((ComponentEventHandler<GhostComponent, DropAttemptEvent>)OnAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GhostComponent, PickupAttemptEvent>((ComponentEventHandler<GhostComponent, PickupAttemptEvent>)OnAttempt, (Type[])null, (Type[])null);
	}

	private void OnAttemptInteract(Entity<GhostComponent> ent, ref InteractionAttemptEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		if (!ent.Comp.CanGhostInteract && !((EntitySystem)this).HasComp<RMCIgnoreGhostInteractionLimitsComponent>(args.Target))
		{
			args.Cancelled = true;
		}
	}

	private void OnAttempt(EntityUid uid, GhostComponent component, CancellableEntityEventArgs args)
	{
		if (!component.CanGhostInteract)
		{
			args.Cancel();
		}
	}

	public void SetTimeOfDeath(Entity<GhostComponent?> entity, TimeSpan value)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<GhostComponent>(Entity<GhostComponent>.op_Implicit(entity), ref entity.Comp, true) && !(entity.Comp.TimeOfDeath == value))
		{
			entity.Comp.TimeOfDeath = value;
			((EntitySystem)this).Dirty<GhostComponent>(entity, (MetaDataComponent)null);
		}
	}

	[Obsolete("Use the Entity<GhostComponent?> overload")]
	public void SetTimeOfDeath(EntityUid uid, TimeSpan value, GhostComponent? component)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		SetTimeOfDeath(Entity<GhostComponent>.op_Implicit((uid, component)), value);
	}

	public void SetCanReturnToBody(Entity<GhostComponent?> entity, bool value)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<GhostComponent>(Entity<GhostComponent>.op_Implicit(entity), ref entity.Comp, true) && entity.Comp.CanReturnToBody != value)
		{
			entity.Comp.CanReturnToBody = value;
			((EntitySystem)this).Dirty<GhostComponent>(entity, (MetaDataComponent)null);
		}
	}

	[Obsolete("Use the Entity<GhostComponent?> overload")]
	public void SetCanReturnToBody(EntityUid uid, bool value, GhostComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		SetCanReturnToBody(Entity<GhostComponent>.op_Implicit((uid, component)), value);
	}

	[Obsolete("Use the Entity<GhostComponent?> overload")]
	public void SetCanReturnToBody(GhostComponent component, bool value)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		SetCanReturnToBody(Entity<GhostComponent>.op_Implicit((((Component)component).Owner, component)), value);
	}

	public void SetCanGhostInteract(Entity<GhostComponent?> entity, bool value)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<GhostComponent>(Entity<GhostComponent>.op_Implicit(entity), ref entity.Comp, true) && entity.Comp.CanGhostInteract != value)
		{
			entity.Comp.CanGhostInteract = value;
			((EntitySystem)this).Dirty<GhostComponent>(entity, (MetaDataComponent)null);
		}
	}
}
