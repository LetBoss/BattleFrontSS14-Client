using System;
using System.Collections.Generic;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Lathe;
using Content.Shared.NameModifier.Components;
using Content.Shared.NameModifier.EntitySystems;
using Content.Shared.Popups;
using Content.Shared.Random;
using Content.Shared.Random.Helpers;
using Content.Shared.Research.Components;
using Content.Shared.Research.Prototypes;
using Content.Shared.Research.Systems;
using Content.Shared.Research.TechnologyDisk.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Shared.Research.TechnologyDisk.Systems;

public sealed class TechnologyDiskSystem : EntitySystem
{
	[Dependency]
	private IPrototypeManager _protoMan;

	[Dependency]
	private IRobustRandom _random;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedResearchSystem _research;

	[Dependency]
	private SharedLatheSystem _lathe;

	[Dependency]
	private NameModifierSystem _nameModifier;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<TechnologyDiskComponent, MapInitEvent>((EntityEventRefHandler<TechnologyDiskComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TechnologyDiskComponent, AfterInteractEvent>((EntityEventRefHandler<TechnologyDiskComponent, AfterInteractEvent>)OnAfterInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TechnologyDiskComponent, ExaminedEvent>((EntityEventRefHandler<TechnologyDiskComponent, ExaminedEvent>)OnExamine, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TechnologyDiskComponent, RefreshNameModifiersEvent>((EntityEventRefHandler<TechnologyDiskComponent, RefreshNameModifiersEvent>)OnRefreshNameModifiers, (Type[])null, (Type[])null);
	}

	private void OnMapInit(Entity<TechnologyDiskComponent> ent, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.Recipes != null)
		{
			return;
		}
		int tier = int.Parse(_protoMan.Index<WeightedRandomPrototype>(ent.Comp.TierWeightPrototype).Pick(_random));
		HashSet<ProtoId<LatheRecipePrototype>> techs = new HashSet<ProtoId<LatheRecipePrototype>>();
		foreach (TechnologyPrototype tech in _protoMan.EnumeratePrototypes<TechnologyPrototype>())
		{
			if (tech.Tier == tier)
			{
				techs.UnionWith(tech.RecipeUnlocks);
			}
		}
		if (techs.Count != 0)
		{
			ent.Comp.Recipes = new List<ProtoId<LatheRecipePrototype>>();
			ent.Comp.Recipes.Add(RandomExtensions.Pick<ProtoId<LatheRecipePrototype>>(_random, (IReadOnlyCollection<ProtoId<LatheRecipePrototype>>)techs));
			((EntitySystem)this).Dirty<TechnologyDiskComponent>(ent, (MetaDataComponent)null);
			_nameModifier.RefreshNameModifiers(Entity<NameModifierComponent>.op_Implicit(ent.Owner));
		}
	}

	private void OnAfterInteract(Entity<TechnologyDiskComponent> ent, ref AfterInteractEvent args)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled || !args.CanReach)
		{
			return;
		}
		EntityUid? target = args.Target;
		if (!target.HasValue)
		{
			return;
		}
		EntityUid target2 = target.GetValueOrDefault();
		TechnologyDatabaseComponent database = default(TechnologyDatabaseComponent);
		if (!((EntitySystem)this).HasComp<ResearchServerComponent>(target2) || !((EntitySystem)this).TryComp<TechnologyDatabaseComponent>(target2, ref database))
		{
			return;
		}
		if (ent.Comp.Recipes != null)
		{
			foreach (ProtoId<LatheRecipePrototype> recipe in ent.Comp.Recipes)
			{
				_research.AddLatheRecipe(target2, ProtoId<LatheRecipePrototype>.op_Implicit(recipe), database);
			}
		}
		_popup.PopupClient(base.Loc.GetString("tech-disk-inserted"), target2, args.User);
		((EntitySystem)this).PredictedQueueDel(ent.Owner);
		((HandledEntityEventArgs)args).Handled = true;
	}

	private void OnExamine(Entity<TechnologyDiskComponent> ent, ref ExaminedEvent args)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		string message = base.Loc.GetString("tech-disk-examine-none");
		if (ent.Comp.Recipes != null && ent.Comp.Recipes.Count > 0)
		{
			LatheRecipePrototype prototype = _protoMan.Index<LatheRecipePrototype>(ent.Comp.Recipes[0]);
			message = base.Loc.GetString("tech-disk-examine", (ValueTuple<string, object>)("result", _lathe.GetRecipeName(prototype)));
			if (ent.Comp.Recipes.Count > 1)
			{
				message = message + " " + base.Loc.GetString("tech-disk-examine-more");
			}
		}
		args.PushMarkup(message);
	}

	private void OnRefreshNameModifiers(Entity<TechnologyDiskComponent> entity, ref RefreshNameModifiersEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		if (entity.Comp.Recipes == null)
		{
			return;
		}
		foreach (ProtoId<LatheRecipePrototype> recipe in entity.Comp.Recipes)
		{
			LatheRecipePrototype proto = _protoMan.Index<LatheRecipePrototype>(recipe);
			args.AddModifier(LocId.op_Implicit("tech-disk-name-format"), 0, ("technology", _lathe.GetRecipeName(proto)));
		}
	}
}
