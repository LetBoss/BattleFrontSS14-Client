using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Lathe;
using Content.Shared.Popups;
using Content.Shared.Research.Components;
using Content.Shared.Research.Prototypes;
using Content.Shared.Whitelist;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Components;
using Robust.Shared.Prototypes;

namespace Content.Shared.Research.Systems;

public sealed class BlueprintSystem : EntitySystem
{
	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private EntityWhitelistSystem _entityWhitelist;

	[Dependency]
	private SharedPopupSystem _popup;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<BlueprintReceiverComponent, ComponentStartup>((EntityEventRefHandler<BlueprintReceiverComponent, ComponentStartup>)OnStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BlueprintReceiverComponent, AfterInteractUsingEvent>((EntityEventRefHandler<BlueprintReceiverComponent, AfterInteractUsingEvent>)OnAfterInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BlueprintReceiverComponent, LatheGetRecipesEvent>((EntityEventRefHandler<BlueprintReceiverComponent, LatheGetRecipesEvent>)OnGetRecipes, (Type[])null, (Type[])null);
	}

	private void OnStartup(Entity<BlueprintReceiverComponent> ent, ref ComponentStartup args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		_container.EnsureContainer<Container>(Entity<BlueprintReceiverComponent>.op_Implicit(ent), ent.Comp.ContainerId, (ContainerManagerComponent)null);
	}

	private void OnAfterInteract(Entity<BlueprintReceiverComponent> ent, ref AfterInteractUsingEvent args)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		BlueprintComponent blueprintComponent = default(BlueprintComponent);
		if (!((HandledEntityEventArgs)args).Handled && args.CanReach && ((EntitySystem)this).TryComp<BlueprintComponent>(args.Used, ref blueprintComponent))
		{
			((HandledEntityEventArgs)args).Handled = TryInsertBlueprint(ent, Entity<BlueprintComponent>.op_Implicit((args.Used, blueprintComponent)), args.User);
		}
	}

	private void OnGetRecipes(Entity<BlueprintReceiverComponent> ent, ref LatheGetRecipesEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		foreach (ProtoId<LatheRecipePrototype> recipe in GetBlueprintRecipes(ent))
		{
			args.Recipes.Add(recipe);
		}
	}

	public bool TryInsertBlueprint(Entity<BlueprintReceiverComponent> ent, Entity<BlueprintComponent> blueprint, EntityUid? user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		if (!CanInsertBlueprint(ent, blueprint, user))
		{
			return false;
		}
		if (user.HasValue)
		{
			EntityUid userId = Identity.Entity(user.Value, (IEntityManager)(object)base.EntityManager);
			EntityUid bpId = Identity.Entity(Entity<BlueprintComponent>.op_Implicit(blueprint), (IEntityManager)(object)base.EntityManager);
			EntityUid machineId = Identity.Entity(Entity<BlueprintReceiverComponent>.op_Implicit(ent), (IEntityManager)(object)base.EntityManager);
			string msg = base.Loc.GetString("blueprint-receiver-popup-insert", new(string, object)[3]
			{
				("user", userId),
				("blueprint", bpId),
				("receiver", machineId)
			});
			_popup.PopupPredicted(msg, Entity<BlueprintReceiverComponent>.op_Implicit(ent), user);
		}
		_container.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(blueprint.Owner), _container.GetContainer(Entity<BlueprintReceiverComponent>.op_Implicit(ent), ent.Comp.ContainerId, (ContainerManagerComponent)null), (TransformComponent)null, false);
		TechnologyDatabaseModifiedEvent ev = new TechnologyDatabaseModifiedEvent(blueprint.Comp.ProvidedRecipes.Select<ProtoId<LatheRecipePrototype>, string>((ProtoId<LatheRecipePrototype> it) => it.Id).ToList());
		((EntitySystem)this).RaiseLocalEvent<TechnologyDatabaseModifiedEvent>(Entity<BlueprintReceiverComponent>.op_Implicit(ent), ref ev, false);
		return true;
	}

	public bool CanInsertBlueprint(Entity<BlueprintReceiverComponent> ent, Entity<BlueprintComponent> blueprint, EntityUid? user)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		if (_entityWhitelist.IsWhitelistFail(ent.Comp.Whitelist, Entity<BlueprintComponent>.op_Implicit(blueprint)))
		{
			return false;
		}
		if (blueprint.Comp.ProvidedRecipes.Count == 0)
		{
			((EntitySystem)this).Log.Error($"Attempted to insert blueprint {((EntitySystem)this).ToPrettyString((EntityUid?)Entity<BlueprintComponent>.op_Implicit(blueprint), (MetaDataComponent)null)} with no recipes.");
			return false;
		}
		HashSet<ProtoId<LatheRecipePrototype>> currentRecipes = GetBlueprintRecipes(ent);
		if (currentRecipes.Count != 0 && currentRecipes.IsSupersetOf(blueprint.Comp.ProvidedRecipes))
		{
			_popup.PopupPredicted(base.Loc.GetString("blueprint-receiver-popup-recipe-exists"), Entity<BlueprintReceiverComponent>.op_Implicit(ent), user);
			return false;
		}
		return _container.CanInsert(Entity<BlueprintComponent>.op_Implicit(blueprint), _container.GetContainer(Entity<BlueprintReceiverComponent>.op_Implicit(ent), ent.Comp.ContainerId, (ContainerManagerComponent)null), false, (TransformComponent)null);
	}

	public HashSet<ProtoId<LatheRecipePrototype>> GetBlueprintRecipes(Entity<BlueprintReceiverComponent> ent)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		BaseContainer container = _container.GetContainer(Entity<BlueprintReceiverComponent>.op_Implicit(ent), ent.Comp.ContainerId, (ContainerManagerComponent)null);
		HashSet<ProtoId<LatheRecipePrototype>> recipes = new HashSet<ProtoId<LatheRecipePrototype>>();
		BlueprintComponent blueprintComponent = default(BlueprintComponent);
		foreach (EntityUid blueprint in container.ContainedEntities)
		{
			if (!((EntitySystem)this).TryComp<BlueprintComponent>(blueprint, ref blueprintComponent))
			{
				continue;
			}
			foreach (ProtoId<LatheRecipePrototype> provided in blueprintComponent.ProvidedRecipes)
			{
				recipes.Add(provided);
			}
		}
		return recipes;
	}
}
