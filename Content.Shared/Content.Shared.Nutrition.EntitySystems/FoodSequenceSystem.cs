using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Mobs.Systems;
using Content.Shared.Nutrition.Components;
using Content.Shared.Nutrition.FoodMetamorphRules;
using Content.Shared.Nutrition.Prototypes;
using Content.Shared.Popups;
using Content.Shared.Storage.Components;
using Content.Shared.Tag;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Utility;

namespace Content.Shared.Nutrition.EntitySystems;

public sealed class FoodSequenceSystem : SharedFoodSequenceSystem
{
	[Dependency]
	private SharedSolutionContainerSystem _solutionContainer;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private MetaDataSystem _metaData;

	[Dependency]
	private MobStateSystem _mobState;

	[Dependency]
	private TagSystem _tag;

	[Dependency]
	private IRobustRandom _random;

	[Dependency]
	private IPrototypeManager _proto;

	[Dependency]
	private SharedTransformSystem _transform;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<FoodSequenceStartPointComponent, InteractUsingEvent>((EntityEventRefHandler<FoodSequenceStartPointComponent, InteractUsingEvent>)OnInteractUsing, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FoodMetamorphableByAddingComponent, FoodSequenceIngredientAddedEvent>((EntityEventRefHandler<FoodMetamorphableByAddingComponent, FoodSequenceIngredientAddedEvent>)OnIngredientAdded, (Type[])null, (Type[])null);
	}

	private void OnInteractUsing(Entity<FoodSequenceStartPointComponent> ent, ref InteractUsingEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		FoodSequenceElementComponent sequenceElement = default(FoodSequenceElementComponent);
		if (((EntitySystem)this).TryComp<FoodSequenceElementComponent>(args.Used, ref sequenceElement))
		{
			((HandledEntityEventArgs)args).Handled = TryAddFoodElement(ent, Entity<FoodSequenceElementComponent>.op_Implicit((args.Used, sequenceElement)), args.User);
		}
	}

	private void OnIngredientAdded(Entity<FoodMetamorphableByAddingComponent> ent, ref FoodSequenceIngredientAddedEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		FoodSequenceStartPointComponent start = default(FoodSequenceStartPointComponent);
		FoodSequenceElementPrototype elementProto = default(FoodSequenceElementPrototype);
		if (((EntitySystem)this).TryComp<FoodSequenceStartPointComponent>(args.Start, ref start) && _proto.TryIndex<FoodSequenceElementPrototype>(args.Proto, ref elementProto) && (!ent.Comp.OnlyFinal || elementProto.Final || start.FoodLayers.Count == start.MaxLayers))
		{
			TryMetamorph(Entity<FoodSequenceStartPointComponent>.op_Implicit((Entity<FoodMetamorphableByAddingComponent>.op_Implicit(ent), start)));
		}
	}

	private bool TryMetamorph(Entity<FoodSequenceStartPointComponent> start)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		List<MetamorphRecipePrototype> availableRecipes = new List<MetamorphRecipePrototype>();
		foreach (MetamorphRecipePrototype recipe in _proto.EnumeratePrototypes<MetamorphRecipePrototype>())
		{
			if (recipe.Key != start.Comp.Key)
			{
				continue;
			}
			bool allowed = true;
			foreach (FoodMetamorphRule rule in recipe.Rules)
			{
				if (!rule.Check(_proto, ((EntitySystem)this).EntityManager, Entity<FoodSequenceStartPointComponent>.op_Implicit(start), start.Comp.FoodLayers))
				{
					allowed = false;
					break;
				}
			}
			if (allowed)
			{
				availableRecipes.Add(recipe);
			}
		}
		if (availableRecipes.Count <= 0)
		{
			return true;
		}
		Metamorf(start, RandomExtensions.Pick<MetamorphRecipePrototype>(_random, (IReadOnlyList<MetamorphRecipePrototype>)availableRecipes));
		((EntitySystem)this).PredictedQueueDel(start.Owner);
		return true;
	}

	private void Metamorf(Entity<FoodSequenceStartPointComponent> start, MetamorphRecipePrototype recipe)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		EntityUid result = ((EntitySystem)this).PredictedSpawnNextToOrDrop(EntProtoId.op_Implicit(recipe.Result), Entity<FoodSequenceStartPointComponent>.op_Implicit(start), (TransformComponent)null, (ComponentRegistry)null);
		_transform.DropNextTo(Entity<TransformComponent>.op_Implicit(result), Entity<TransformComponent>.op_Implicit((Entity<FoodSequenceStartPointComponent>.op_Implicit(start), ((EntitySystem)this).Transform(Entity<FoodSequenceStartPointComponent>.op_Implicit(start)))));
		if (_solutionContainer.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(result), start.Comp.Solution, out Entity<SolutionComponent>? resultSoln, out Solution _) && _solutionContainer.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(start.Owner), start.Comp.Solution, out Entity<SolutionComponent>? startSoln, out Solution startSolution))
		{
			_solutionContainer.RemoveAllSolution(resultSoln.Value);
			resultSoln.Value.Comp.Solution.MaxVolume = startSoln.Value.Comp.Solution.MaxVolume;
			_solutionContainer.TryAddSolution(resultSoln.Value, startSolution);
			MergeFlavorProfiles(Entity<FoodSequenceStartPointComponent>.op_Implicit(start), result);
			MergeTrash(Entity<FoodSequenceStartPointComponent>.op_Implicit(start), result);
			MergeTags(Entity<FoodSequenceStartPointComponent>.op_Implicit(start), result);
		}
	}

	private bool TryAddFoodElement(Entity<FoodSequenceStartPointComponent> start, Entity<FoodSequenceElementComponent> element, EntityUid? user = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		FoodComponent elementFood = default(FoodComponent);
		if (!((EntitySystem)this).TryComp<FoodComponent>(Entity<FoodSequenceElementComponent>.op_Implicit(element), ref elementFood))
		{
			return false;
		}
		if (elementFood.RequireDead && _mobState.IsAlive(Entity<FoodSequenceElementComponent>.op_Implicit(element)))
		{
			return false;
		}
		if (!element.Comp.Entries.TryGetValue(start.Comp.Key, out ProtoId<FoodSequenceElementPrototype> elementProto))
		{
			return false;
		}
		FoodSequenceElementPrototype elementIndexed = default(FoodSequenceElementPrototype);
		if (!_proto.TryIndex<FoodSequenceElementPrototype>(elementProto, ref elementIndexed))
		{
			return false;
		}
		if ((start.Comp.FoodLayers.Count >= start.Comp.MaxLayers && !elementIndexed.Final) || start.Comp.Finished)
		{
			if (user.HasValue)
			{
				_popup.PopupClient(((EntitySystem)this).Loc.GetString("food-sequence-no-space"), Entity<FoodSequenceStartPointComponent>.op_Implicit(start), user.Value);
			}
			return false;
		}
		SecretStashComponent stashComponent = default(SecretStashComponent);
		if (((EntitySystem)this).TryComp<SecretStashComponent>(Entity<FoodSequenceElementComponent>.op_Implicit(element), ref stashComponent) && ((BaseContainer)stashComponent.ItemContainer).Count != 0)
		{
			return false;
		}
		bool flip = start.Comp.AllowHorizontalFlip && RandomExtensions.Prob(_random, 0.5f);
		FoodSequenceVisualLayer layer = new FoodSequenceVisualLayer(ProtoId<FoodSequenceElementPrototype>.op_Implicit(elementIndexed), RandomExtensions.Pick<SpriteSpecifier>(_random, (IReadOnlyList<SpriteSpecifier>)elementIndexed.Sprites), new Vector2(flip ? (0f - elementIndexed.Scale.X) : elementIndexed.Scale.X, elementIndexed.Scale.Y), new Vector2(_random.NextFloat(start.Comp.MinLayerOffset.X, start.Comp.MaxLayerOffset.X), _random.NextFloat(start.Comp.MinLayerOffset.Y, start.Comp.MaxLayerOffset.Y)));
		start.Comp.FoodLayers.Add(layer);
		((EntitySystem)this).Dirty<FoodSequenceStartPointComponent>(start, (MetaDataComponent)null);
		if (elementIndexed.Final)
		{
			start.Comp.Finished = true;
		}
		UpdateFoodName(start);
		MergeFoodSolutions(start.Owner, element.Owner);
		MergeFlavorProfiles(Entity<FoodSequenceStartPointComponent>.op_Implicit(start), Entity<FoodSequenceElementComponent>.op_Implicit(element));
		MergeTrash(start.Owner, element.Owner);
		MergeTags(Entity<FoodSequenceStartPointComponent>.op_Implicit(start), Entity<FoodSequenceElementComponent>.op_Implicit(element));
		FoodSequenceIngredientAddedEvent ev = new FoodSequenceIngredientAddedEvent(Entity<FoodSequenceStartPointComponent>.op_Implicit(start), Entity<FoodSequenceElementComponent>.op_Implicit(element), elementProto, user);
		((EntitySystem)this).RaiseLocalEvent<FoodSequenceIngredientAddedEvent>(Entity<FoodSequenceStartPointComponent>.op_Implicit(start), ev, false);
		((EntitySystem)this).PredictedQueueDel(element.Owner);
		return true;
	}

	private void UpdateFoodName(Entity<FoodSequenceStartPointComponent> start)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		LocId? nameGeneration = start.Comp.NameGeneration;
		if (!nameGeneration.HasValue)
		{
			return;
		}
		StringBuilder content = new StringBuilder();
		string separator = "";
		if (start.Comp.ContentSeparator != null)
		{
			separator = ((EntitySystem)this).Loc.GetString(start.Comp.ContentSeparator);
		}
		HashSet<ProtoId<FoodSequenceElementPrototype>> existedContentNames = new HashSet<ProtoId<FoodSequenceElementPrototype>>();
		foreach (FoodSequenceVisualLayer layer in start.Comp.FoodLayers)
		{
			if (!existedContentNames.Contains(layer.Proto))
			{
				existedContentNames.Add(layer.Proto);
			}
		}
		int nameCounter = 1;
		FoodSequenceElementPrototype protoIndexed = default(FoodSequenceElementPrototype);
		foreach (ProtoId<FoodSequenceElementPrototype> proto in existedContentNames)
		{
			if (_proto.TryIndex<FoodSequenceElementPrototype>(proto, ref protoIndexed) && protoIndexed.Name.HasValue)
			{
				content.Append(((EntitySystem)this).Loc.GetString(LocId.op_Implicit(protoIndexed.Name.Value)));
				if (nameCounter < existedContentNames.Count)
				{
					content.Append(separator);
				}
				nameCounter++;
			}
		}
		ILocalizationManager loc = ((EntitySystem)this).Loc;
		string text = LocId.op_Implicit(start.Comp.NameGeneration.Value);
		(string, object)[] array = new(string, object)[3];
		nameGeneration = start.Comp.NamePrefix;
		object item;
		if (!nameGeneration.HasValue)
		{
			item = "";
		}
		else
		{
			ILocalizationManager loc2 = ((EntitySystem)this).Loc;
			nameGeneration = start.Comp.NamePrefix;
			item = loc2.GetString(nameGeneration.HasValue ? LocId.op_Implicit(nameGeneration.GetValueOrDefault()) : null);
		}
		array[0] = ("prefix", item);
		array[1] = ("content", content);
		nameGeneration = start.Comp.NameSuffix;
		object item2;
		if (!nameGeneration.HasValue)
		{
			item2 = "";
		}
		else
		{
			ILocalizationManager loc3 = ((EntitySystem)this).Loc;
			nameGeneration = start.Comp.NameSuffix;
			item2 = loc3.GetString(nameGeneration.HasValue ? LocId.op_Implicit(nameGeneration.GetValueOrDefault()) : null);
		}
		array[2] = ("suffix", item2);
		string newName = loc.GetString(text, array);
		_metaData.SetEntityName(Entity<FoodSequenceStartPointComponent>.op_Implicit(start), newName, (MetaDataComponent)null, true);
	}

	private void MergeFoodSolutions(EntityUid start, EntityUid element)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		FoodComponent startFood = default(FoodComponent);
		FoodComponent elementFood = default(FoodComponent);
		if (((EntitySystem)this).TryComp<FoodComponent>(start, ref startFood) && ((EntitySystem)this).TryComp<FoodComponent>(element, ref elementFood) && _solutionContainer.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(start), startFood.Solution, out Entity<SolutionComponent>? startSolutionEntity, out Solution startSolution) && _solutionContainer.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(element), elementFood.Solution, out Entity<SolutionComponent>? _, out Solution elementSolution))
		{
			startSolution.MaxVolume += elementSolution.MaxVolume;
			_solutionContainer.TryAddSolution(startSolutionEntity.Value, elementSolution);
		}
	}

	private void MergeFlavorProfiles(EntityUid start, EntityUid element)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		FlavorProfileComponent startProfile = default(FlavorProfileComponent);
		FlavorProfileComponent elementProfile = default(FlavorProfileComponent);
		if (!((EntitySystem)this).TryComp<FlavorProfileComponent>(start, ref startProfile) || !((EntitySystem)this).TryComp<FlavorProfileComponent>(element, ref elementProfile))
		{
			return;
		}
		foreach (string flavor in elementProfile.Flavors)
		{
			if (startProfile != null && !startProfile.Flavors.Contains(flavor))
			{
				startProfile.Flavors.Add(flavor);
			}
		}
	}

	private void MergeTrash(EntityUid start, EntityUid element)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		FoodComponent startFood = default(FoodComponent);
		FoodComponent elementFood = default(FoodComponent);
		if (!((EntitySystem)this).TryComp<FoodComponent>(start, ref startFood) || !((EntitySystem)this).TryComp<FoodComponent>(element, ref elementFood))
		{
			return;
		}
		foreach (EntProtoId trash in elementFood.Trash)
		{
			startFood.Trash.Add(trash);
		}
	}

	private void MergeTags(EntityUid start, EntityUid element)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		TagComponent elementTags = default(TagComponent);
		if (((EntitySystem)this).TryComp<TagComponent>(element, ref elementTags))
		{
			((EntitySystem)this).EnsureComp<TagComponent>(start);
			_tag.TryAddTags(start, (IEnumerable<ProtoId<TagPrototype>>)elementTags.Tags);
		}
	}
}
