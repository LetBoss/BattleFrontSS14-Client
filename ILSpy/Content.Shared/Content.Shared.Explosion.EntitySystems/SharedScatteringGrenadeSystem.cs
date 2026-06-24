using System;
using Content.Shared.Explosion.Components;
using Content.Shared.Interaction;
using Content.Shared.Whitelist;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Components;

namespace Content.Shared.Explosion.EntitySystems;

public abstract class SharedScatteringGrenadeSystem : EntitySystem
{
	[Dependency]
	private EntityWhitelistSystem _whitelistSystem;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SharedContainerSystem _container;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ScatteringGrenadeComponent, ComponentInit>((EntityEventRefHandler<ScatteringGrenadeComponent, ComponentInit>)OnScatteringInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ScatteringGrenadeComponent, ComponentStartup>((EntityEventRefHandler<ScatteringGrenadeComponent, ComponentStartup>)OnScatteringStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ScatteringGrenadeComponent, InteractUsingEvent>((EntityEventRefHandler<ScatteringGrenadeComponent, InteractUsingEvent>)OnScatteringInteractUsing, (Type[])null, (Type[])null);
	}

	private void OnScatteringInit(Entity<ScatteringGrenadeComponent> entity, ref ComponentInit args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		entity.Comp.Container = _container.EnsureContainer<Container>(entity.Owner, "cluster-payload", (ContainerManagerComponent)null);
	}

	private void OnScatteringStartup(Entity<ScatteringGrenadeComponent> entity, ref ComponentStartup args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		if (entity.Comp.FillPrototype.HasValue)
		{
			entity.Comp.UnspawnedCount = Math.Max(0, entity.Comp.Capacity - ((BaseContainer)entity.Comp.Container).ContainedEntities.Count);
			UpdateAppearance(entity);
			((EntitySystem)this).Dirty(Entity<ScatteringGrenadeComponent>.op_Implicit(entity), (IComponent)(object)entity.Comp, (MetaDataComponent)null);
		}
	}

	private void OnScatteringInteractUsing(Entity<ScatteringGrenadeComponent> entity, ref InteractUsingEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		if (entity.Comp.Whitelist != null && entity.Comp.Count < entity.Comp.Capacity && !((HandledEntityEventArgs)args).Handled && _whitelistSystem.IsValid(entity.Comp.Whitelist, args.Used))
		{
			_container.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(args.Used), (BaseContainer)(object)entity.Comp.Container, (TransformComponent)null, false);
			UpdateAppearance(entity);
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void UpdateAppearance(Entity<ScatteringGrenadeComponent> entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		AppearanceComponent appearanceComponent = default(AppearanceComponent);
		if (((EntitySystem)this).TryComp<AppearanceComponent>(Entity<ScatteringGrenadeComponent>.op_Implicit(entity), ref appearanceComponent))
		{
			_appearance.SetData(Entity<ScatteringGrenadeComponent>.op_Implicit(entity), (Enum)ClusterGrenadeVisuals.GrenadesCounter, (object)entity.Comp.Count, appearanceComponent);
		}
	}
}
