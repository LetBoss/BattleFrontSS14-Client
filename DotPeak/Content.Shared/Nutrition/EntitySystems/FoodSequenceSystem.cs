// Decompiled with JetBrains decompiler
// Type: Content.Shared.Nutrition.EntitySystems.FoodSequenceSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Utility;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

#nullable enable
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
    base.Initialize();
    this.SubscribeLocalEvent<FoodSequenceStartPointComponent, InteractUsingEvent>(new EntityEventRefHandler<FoodSequenceStartPointComponent, InteractUsingEvent>(this.OnInteractUsing));
    this.SubscribeLocalEvent<FoodMetamorphableByAddingComponent, FoodSequenceIngredientAddedEvent>(new EntityEventRefHandler<FoodMetamorphableByAddingComponent, FoodSequenceIngredientAddedEvent>(this.OnIngredientAdded));
  }

  private void OnInteractUsing(
    Entity<FoodSequenceStartPointComponent> ent,
    ref InteractUsingEvent args)
  {
    FoodSequenceElementComponent comp;
    if (!this.TryComp<FoodSequenceElementComponent>(args.Used, out comp))
      return;
    args.Handled = this.TryAddFoodElement(ent, (Entity<FoodSequenceElementComponent>) (args.Used, comp), new EntityUid?(args.User));
  }

  private void OnIngredientAdded(
    Entity<FoodMetamorphableByAddingComponent> ent,
    ref FoodSequenceIngredientAddedEvent args)
  {
    FoodSequenceStartPointComponent comp;
    FoodSequenceElementPrototype prototype;
    if (!this.TryComp<FoodSequenceStartPointComponent>(args.Start, out comp) || !this._proto.TryIndex<FoodSequenceElementPrototype>(args.Proto, out prototype) || ent.Comp.OnlyFinal && !prototype.Final && comp.FoodLayers.Count != comp.MaxLayers)
      return;
    this.TryMetamorph((Entity<FoodSequenceStartPointComponent>) ((EntityUid) ent, comp));
  }

  private bool TryMetamorph(Entity<FoodSequenceStartPointComponent> start)
  {
    List<MetamorphRecipePrototype> list = new List<MetamorphRecipePrototype>();
    foreach (MetamorphRecipePrototype enumeratePrototype in this._proto.EnumeratePrototypes<MetamorphRecipePrototype>())
    {
      if (!(enumeratePrototype.Key != start.Comp.Key))
      {
        bool flag = true;
        foreach (FoodMetamorphRule rule in enumeratePrototype.Rules)
        {
          if (!rule.Check(this._proto, this.EntityManager, (EntityUid) start, start.Comp.FoodLayers))
          {
            flag = false;
            break;
          }
        }
        if (flag)
          list.Add(enumeratePrototype);
      }
    }
    if (list.Count <= 0)
      return true;
    this.Metamorf(start, RandomExtensions.Pick<MetamorphRecipePrototype>(this._random, (IReadOnlyList<MetamorphRecipePrototype>) list));
    this.PredictedQueueDel(start.Owner);
    return true;
  }

  private void Metamorf(
    Entity<FoodSequenceStartPointComponent> start,
    MetamorphRecipePrototype recipe)
  {
    EntityUid orDrop = this.PredictedSpawnNextToOrDrop((string) recipe.Result, (EntityUid) start);
    this._transform.DropNextTo((Entity<TransformComponent>) orDrop, (Entity<TransformComponent>) ((EntityUid) start, this.Transform((EntityUid) start)));
    Entity<SolutionComponent>? entity1;
    Entity<SolutionComponent>? entity2;
    Solution solution;
    if (!this._solutionContainer.TryGetSolution((Entity<SolutionContainerManagerComponent>) orDrop, start.Comp.Solution, out entity1, out Solution _) || !this._solutionContainer.TryGetSolution((Entity<SolutionContainerManagerComponent>) start.Owner, start.Comp.Solution, out entity2, out solution))
      return;
    this._solutionContainer.RemoveAllSolution(entity1.Value);
    entity1.Value.Comp.Solution.MaxVolume = entity2.Value.Comp.Solution.MaxVolume;
    this._solutionContainer.TryAddSolution(entity1.Value, solution);
    this.MergeFlavorProfiles((EntityUid) start, orDrop);
    this.MergeTrash((EntityUid) start, orDrop);
    this.MergeTags((EntityUid) start, orDrop);
  }

  private bool TryAddFoodElement(
    Entity<FoodSequenceStartPointComponent> start,
    Entity<FoodSequenceElementComponent> element,
    EntityUid? user = null)
  {
    FoodComponent comp1;
    ProtoId<FoodSequenceElementPrototype> protoId;
    FoodSequenceElementPrototype prototype;
    if (!this.TryComp<FoodComponent>((EntityUid) element, out comp1) || comp1.RequireDead && this._mobState.IsAlive((EntityUid) element) || !element.Comp.Entries.TryGetValue(start.Comp.Key, out protoId) || !this._proto.TryIndex<FoodSequenceElementPrototype>(protoId, out prototype))
      return false;
    if (start.Comp.FoodLayers.Count >= start.Comp.MaxLayers && !prototype.Final || start.Comp.Finished)
    {
      if (user.HasValue)
        this._popup.PopupClient(this.Loc.GetString("food-sequence-no-space"), (EntityUid) start, new EntityUid?(user.Value));
      return false;
    }
    SecretStashComponent comp2;
    if (this.TryComp<SecretStashComponent>((EntityUid) element, out comp2) && comp2.ItemContainer.Count != 0)
      return false;
    bool flag = start.Comp.AllowHorizontalFlip && this._random.Prob(0.5f);
    FoodSequenceVisualLayer sequenceVisualLayer = new FoodSequenceVisualLayer((ProtoId<FoodSequenceElementPrototype>) prototype, RandomExtensions.Pick<SpriteSpecifier>(this._random, (IReadOnlyList<SpriteSpecifier>) prototype.Sprites), new Vector2(flag ? -prototype.Scale.X : prototype.Scale.X, prototype.Scale.Y), new Vector2(this._random.NextFloat(start.Comp.MinLayerOffset.X, start.Comp.MaxLayerOffset.X), this._random.NextFloat(start.Comp.MinLayerOffset.Y, start.Comp.MaxLayerOffset.Y)));
    start.Comp.FoodLayers.Add(sequenceVisualLayer);
    this.Dirty<FoodSequenceStartPointComponent>(start);
    if (prototype.Final)
      start.Comp.Finished = true;
    this.UpdateFoodName(start);
    this.MergeFoodSolutions(start.Owner, element.Owner);
    this.MergeFlavorProfiles((EntityUid) start, (EntityUid) element);
    this.MergeTrash(start.Owner, element.Owner);
    this.MergeTags((EntityUid) start, (EntityUid) element);
    FoodSequenceIngredientAddedEvent args = new FoodSequenceIngredientAddedEvent((EntityUid) start, (EntityUid) element, protoId, user);
    this.RaiseLocalEvent<FoodSequenceIngredientAddedEvent>((EntityUid) start, args);
    this.PredictedQueueDel(element.Owner);
    return true;
  }

  private void UpdateFoodName(Entity<FoodSequenceStartPointComponent> start)
  {
    if (!start.Comp.NameGeneration.HasValue)
      return;
    StringBuilder stringBuilder1 = new StringBuilder();
    string str1 = "";
    if (start.Comp.ContentSeparator != null)
      str1 = this.Loc.GetString(start.Comp.ContentSeparator);
    HashSet<ProtoId<FoodSequenceElementPrototype>> protoIdSet = new HashSet<ProtoId<FoodSequenceElementPrototype>>();
    foreach (FoodSequenceVisualLayer foodLayer in start.Comp.FoodLayers)
    {
      if (!protoIdSet.Contains(foodLayer.Proto))
        protoIdSet.Add(foodLayer.Proto);
    }
    int num = 1;
    foreach (ProtoId<FoodSequenceElementPrototype> id in protoIdSet)
    {
      FoodSequenceElementPrototype prototype;
      if (this._proto.TryIndex<FoodSequenceElementPrototype>(id, out prototype))
      {
        LocId? name = prototype.Name;
        if (name.HasValue)
        {
          StringBuilder stringBuilder2 = stringBuilder1;
          ILocalizationManager loc = this.Loc;
          name = prototype.Name;
          string messageId = (string) name.Value;
          string str2 = loc.GetString(messageId);
          stringBuilder2.Append(str2);
          if (num < protoIdSet.Count)
            stringBuilder1.Append(str1);
          ++num;
        }
      }
    }
    ILocalizationManager loc1 = this.Loc;
    string messageId1 = (string) start.Comp.NameGeneration.Value;
    (string, object)[] valueTupleArray = new (string, object)[3];
    LocId? nullable = start.Comp.NamePrefix;
    string str3;
    if (!nullable.HasValue)
    {
      str3 = "";
    }
    else
    {
      ILocalizationManager loc2 = this.Loc;
      nullable = start.Comp.NamePrefix;
      string valueOrDefault = nullable.HasValue ? (string) nullable.GetValueOrDefault() : (string) null;
      str3 = loc2.GetString(valueOrDefault);
    }
    valueTupleArray[0] = ("prefix", (object) str3);
    valueTupleArray[1] = ("content", (object) stringBuilder1);
    nullable = start.Comp.NameSuffix;
    string str4;
    if (!nullable.HasValue)
    {
      str4 = "";
    }
    else
    {
      ILocalizationManager loc3 = this.Loc;
      nullable = start.Comp.NameSuffix;
      string valueOrDefault = nullable.HasValue ? (string) nullable.GetValueOrDefault() : (string) null;
      str4 = loc3.GetString(valueOrDefault);
    }
    valueTupleArray[2] = ("suffix", (object) str4);
    string str5 = loc1.GetString(messageId1, valueTupleArray);
    this._metaData.SetEntityName((EntityUid) start, str5);
  }

  private void MergeFoodSolutions(EntityUid start, EntityUid element)
  {
    FoodComponent comp1;
    FoodComponent comp2;
    Entity<SolutionComponent>? entity;
    Solution solution1;
    Solution solution2;
    if (!this.TryComp<FoodComponent>(start, out comp1) || !this.TryComp<FoodComponent>(element, out comp2) || !this._solutionContainer.TryGetSolution((Entity<SolutionContainerManagerComponent>) start, comp1.Solution, out entity, out solution1) || !this._solutionContainer.TryGetSolution((Entity<SolutionContainerManagerComponent>) element, comp2.Solution, out Entity<SolutionComponent>? _, out solution2))
      return;
    solution1.MaxVolume += solution2.MaxVolume;
    this._solutionContainer.TryAddSolution(entity.Value, solution2);
  }

  private void MergeFlavorProfiles(EntityUid start, EntityUid element)
  {
    FlavorProfileComponent comp1;
    FlavorProfileComponent comp2;
    if (!this.TryComp<FlavorProfileComponent>(start, out comp1) || !this.TryComp<FlavorProfileComponent>(element, out comp2))
      return;
    foreach (string flavor in comp2.Flavors)
    {
      if (comp1 != null && !comp1.Flavors.Contains(flavor))
        comp1.Flavors.Add(flavor);
    }
  }

  private void MergeTrash(EntityUid start, EntityUid element)
  {
    FoodComponent comp1;
    FoodComponent comp2;
    if (!this.TryComp<FoodComponent>(start, out comp1) || !this.TryComp<FoodComponent>(element, out comp2))
      return;
    foreach (EntProtoId entProtoId in comp2.Trash)
      comp1.Trash.Add(entProtoId);
  }

  private void MergeTags(EntityUid start, EntityUid element)
  {
    TagComponent comp;
    if (!this.TryComp<TagComponent>(element, out comp))
      return;
    this.EnsureComp<TagComponent>(start);
    this._tag.TryAddTags(start, (IEnumerable<ProtoId<TagPrototype>>) comp.Tags);
  }
}
