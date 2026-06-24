using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Content.Shared._RMC14.Chemistry.Reagent;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Emag.Systems;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Lathe.Prototypes;
using Content.Shared.Localizations;
using Content.Shared.Materials;
using Content.Shared.Research.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared.Lathe;

public abstract class SharedLatheSystem : EntitySystem
{
	[Dependency]
	private IPrototypeManager _proto;

	[Dependency]
	private SharedMaterialStorageSystem _materialStorage;

	[Dependency]
	private EmagSystem _emag;

	[Dependency]
	private RMCReagentSystem _reagents;

	public readonly Dictionary<string, List<LatheRecipePrototype>> InverseRecipes = new Dictionary<string, List<LatheRecipePrototype>>();

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<EmagLatheRecipesComponent, GotEmaggedEvent>((ComponentEventRefHandler<EmagLatheRecipesComponent, GotEmaggedEvent>)OnEmagged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<LatheComponent, ExaminedEvent>((EntityEventRefHandler<LatheComponent, ExaminedEvent>)OnExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PrototypesReloadedEventArgs>((EntityEventHandler<PrototypesReloadedEventArgs>)OnPrototypesReloaded, (Type[])null, (Type[])null);
		BuildInverseRecipeDictionary();
	}

	public HashSet<ProtoId<LatheRecipePrototype>> GetAllPossibleRecipes(LatheComponent component)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		HashSet<ProtoId<LatheRecipePrototype>> recipes = new HashSet<ProtoId<LatheRecipePrototype>>();
		foreach (ProtoId<LatheRecipePackPrototype> pack in component.StaticPacks)
		{
			recipes.UnionWith(_proto.Index<LatheRecipePackPrototype>(pack).Recipes);
		}
		foreach (ProtoId<LatheRecipePackPrototype> pack2 in component.DynamicPacks)
		{
			recipes.UnionWith(_proto.Index<LatheRecipePackPrototype>(pack2).Recipes);
		}
		return recipes;
	}

	public void AddRecipesFromPacks(HashSet<ProtoId<LatheRecipePrototype>> recipes, IEnumerable<ProtoId<LatheRecipePackPrototype>> packs)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		foreach (ProtoId<LatheRecipePackPrototype> id in packs)
		{
			LatheRecipePackPrototype pack = _proto.Index<LatheRecipePackPrototype>(id);
			recipes.UnionWith(pack.Recipes);
		}
	}

	private void OnExamined(Entity<LatheComponent> ent, ref ExaminedEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (args.IsInDetailsRange && ent.Comp.ReagentOutputSlotId != null)
		{
			args.PushMarkup(base.Loc.GetString("lathe-menu-reagent-slot-examine"));
		}
	}

	public bool CanProduce(EntityUid uid, string recipe, int amount = 1, LatheComponent? component = null)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		LatheRecipePrototype proto = default(LatheRecipePrototype);
		if (_proto.TryIndex<LatheRecipePrototype>(recipe, ref proto))
		{
			return CanProduce(uid, proto, amount, component);
		}
		return false;
	}

	public bool CanProduce(EntityUid uid, LatheRecipePrototype recipe, int amount = 1, LatheComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<LatheComponent>(uid, ref component, true))
		{
			return false;
		}
		if (!HasRecipe(uid, recipe, component))
		{
			return false;
		}
		foreach (KeyValuePair<ProtoId<MaterialPrototype>, int> material2 in recipe.Materials)
		{
			material2.Deconstruct(out var key, out var value);
			ProtoId<MaterialPrototype> material = key;
			int adjustedAmount = AdjustMaterial(value, recipe.ApplyMaterialDiscount, component.MaterialUseMultiplier);
			if (_materialStorage.GetMaterialAmount(uid, ProtoId<MaterialPrototype>.op_Implicit(material)) < adjustedAmount * amount)
			{
				return false;
			}
		}
		return true;
	}

	private void OnEmagged(EntityUid uid, EmagLatheRecipesComponent component, ref GotEmaggedEvent args)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (_emag.CompareFlag(args.Type, EmagType.Interaction) && !_emag.CheckFlag(uid, EmagType.Interaction))
		{
			args.Handled = true;
		}
	}

	public static int AdjustMaterial(int original, bool reduce, float multiplier)
	{
		if (!reduce)
		{
			return original;
		}
		return (int)MathF.Ceiling((float)original * multiplier);
	}

	protected abstract bool HasRecipe(EntityUid uid, LatheRecipePrototype recipe, LatheComponent component);

	private void OnPrototypesReloaded(PrototypesReloadedEventArgs obj)
	{
		if (obj.WasModified<LatheRecipePrototype>())
		{
			BuildInverseRecipeDictionary();
		}
	}

	private void BuildInverseRecipeDictionary()
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		InverseRecipes.Clear();
		foreach (LatheRecipePrototype latheRecipe in _proto.EnumeratePrototypes<LatheRecipePrototype>())
		{
			EntProtoId? result = latheRecipe.Result;
			if (result.HasValue)
			{
				EntProtoId result2 = result.GetValueOrDefault();
				Extensions.GetOrNew<string, List<LatheRecipePrototype>>(InverseRecipes, EntProtoId.op_Implicit(result2)).Add(latheRecipe);
			}
		}
	}

	public bool TryGetRecipesFromEntity(string prototype, [NotNullWhen(true)] out List<LatheRecipePrototype>? recipes)
	{
		recipes = new List<LatheRecipePrototype>();
		if (InverseRecipes.TryGetValue(prototype, out List<LatheRecipePrototype> r))
		{
			recipes.AddRange(r);
		}
		return recipes.Count != 0;
	}

	public string GetRecipeName(ProtoId<LatheRecipePrototype> proto)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return GetRecipeName(_proto.Index<LatheRecipePrototype>(proto));
	}

	public string GetRecipeName(LatheRecipePrototype proto)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		LocId? name = proto.Name;
		if (!string.IsNullOrWhiteSpace(name.HasValue ? LocId.op_Implicit(name.GetValueOrDefault()) : null))
		{
			ILocalizationManager loc = base.Loc;
			name = proto.Name;
			return loc.GetString(name.HasValue ? LocId.op_Implicit(name.GetValueOrDefault()) : null);
		}
		EntProtoId? result = proto.Result;
		if (result.HasValue)
		{
			EntProtoId result2 = result.GetValueOrDefault();
			return _proto.Index(result2).Name;
		}
		Dictionary<ProtoId<ReagentPrototype>, FixedPoint2> resultReagents = proto.ResultReagents;
		if (resultReagents != null)
		{
			return ContentLocalizationManager.FormatList(resultReagents.Select((KeyValuePair<ProtoId<ReagentPrototype>, FixedPoint2> p) => base.Loc.GetString("lathe-menu-result-reagent-display", (ValueTuple<string, object>)("reagent", _reagents.Index(p.Key).LocalizedName), (ValueTuple<string, object>)("amount", p.Value))).ToList());
		}
		return string.Empty;
	}

	public string GetRecipeDescription(ProtoId<LatheRecipePrototype> proto)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return GetRecipeDescription(_proto.Index<LatheRecipePrototype>(proto));
	}

	public string GetRecipeDescription(LatheRecipePrototype proto)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		LocId? description = proto.Description;
		if (!string.IsNullOrWhiteSpace(description.HasValue ? LocId.op_Implicit(description.GetValueOrDefault()) : null))
		{
			ILocalizationManager loc = base.Loc;
			description = proto.Description;
			return loc.GetString(description.HasValue ? LocId.op_Implicit(description.GetValueOrDefault()) : null);
		}
		EntProtoId? result = proto.Result;
		if (result.HasValue)
		{
			EntProtoId result2 = result.GetValueOrDefault();
			return _proto.Index(result2).Description;
		}
		Dictionary<ProtoId<ReagentPrototype>, FixedPoint2> resultReagents = proto.ResultReagents;
		if (resultReagents != null)
		{
			ProtoId<ReagentPrototype> reagent = resultReagents.First().Key;
			return _reagents.Index(reagent).LocalizedDescription;
		}
		return string.Empty;
	}
}
