// Decompiled with JetBrains decompiler
// Type: Content.Client.Chemistry.EntitySystems.ChemistryGuideDataSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Atmos.Prototypes;
using Content.Shared.Body.Part;
using Content.Shared.Chemistry;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Reaction;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Content.Shared.Kitchen.Components;
using Content.Shared.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.Chemistry.EntitySystems;

public sealed class ChemistryGuideDataSystem : SharedChemistryGuideDataSystem
{
  [Dependency]
  private SharedSolutionContainerSystem _solutionContainer;
  private static readonly ProtoId<MixingCategoryPrototype> DefaultMixingCategory = ProtoId<MixingCategoryPrototype>.op_Implicit("DummyMix");
  private static readonly ProtoId<MixingCategoryPrototype> DefaultGrindCategory = ProtoId<MixingCategoryPrototype>.op_Implicit("DummyGrind");
  private static readonly ProtoId<MixingCategoryPrototype> DefaultJuiceCategory = ProtoId<MixingCategoryPrototype>.op_Implicit("DummyJuice");
  private static readonly ProtoId<MixingCategoryPrototype> DefaultCondenseCategory = ProtoId<MixingCategoryPrototype>.op_Implicit("DummyCondense");
  private readonly Dictionary<string, List<ReagentSourceData>> _reagentSources = new Dictionary<string, List<ReagentSourceData>>();

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<ReagentGuideRegistryChangedEvent>(new EntityEventHandler<ReagentGuideRegistryChangedEvent>(this.OnReceiveRegistryUpdate), (Type[]) null, (Type[]) null);
    this.SubscribeLocalEvent<PrototypesReloadedEventArgs>(new EntityEventHandler<PrototypesReloadedEventArgs>(this.OnPrototypesReloaded), (Type[]) null, (Type[]) null);
    this.OnPrototypesReloaded((PrototypesReloadedEventArgs) null);
  }

  private void OnReceiveRegistryUpdate(ReagentGuideRegistryChangedEvent message)
  {
    ReagentGuideChangeset changeset = message.Changeset;
    foreach (string key in changeset.Removed)
      this.Registry.Remove(key);
    foreach (KeyValuePair<string, ReagentGuideEntry> guideEntry in changeset.GuideEntries)
    {
      string key;
      (key, this.Registry[key]) = guideEntry;
    }
  }

  private void OnPrototypesReloaded(PrototypesReloadedEventArgs? ev)
  {
    this._reagentSources.Clear();
    foreach (ReagentPrototype enumeratePrototype in this.PrototypeManager.EnumeratePrototypes<ReagentPrototype>())
      this._reagentSources.Add(enumeratePrototype.ID, new List<ReagentSourceData>());
    foreach (ReactionPrototype enumeratePrototype in this.PrototypeManager.EnumeratePrototypes<ReactionPrototype>())
    {
      if (enumeratePrototype.Source)
      {
        List<ProtoId<MixingCategoryPrototype>> mixingType = enumeratePrototype.MixingCategories;
        if (mixingType == null)
          mixingType = new List<ProtoId<MixingCategoryPrototype>>()
          {
            ChemistryGuideDataSystem.DefaultMixingCategory
          };
        ReagentReactionSourceData reactionSourceData = new ReagentReactionSourceData(mixingType, enumeratePrototype);
        foreach (string key in enumeratePrototype.Products.Keys)
          this._reagentSources[key].Add((ReagentSourceData) reactionSourceData);
      }
    }
    foreach (GasPrototype enumeratePrototype in this.PrototypeManager.EnumeratePrototypes<GasPrototype>())
    {
      if (enumeratePrototype.Reagent != null)
      {
        ReagentGasSourceData reagentGasSourceData = new ReagentGasSourceData(new List<ProtoId<MixingCategoryPrototype>>()
        {
          ChemistryGuideDataSystem.DefaultCondenseCategory
        }, enumeratePrototype);
        this._reagentSources[enumeratePrototype.Reagent].Add((ReagentSourceData) reagentGasSourceData);
      }
    }
    List<string> stringList = new List<string>();
    foreach (EntityPrototype enumeratePrototype in this.PrototypeManager.EnumeratePrototypes<EntityPrototype>())
    {
      ExtractableComponent extractableComponent;
      if (!enumeratePrototype.Abstract && !stringList.Contains(enumeratePrototype.Name) && enumeratePrototype.TryGetComponent<ExtractableComponent>(ref extractableComponent, this.EntityManager.ComponentFactory) && !enumeratePrototype.HasComponent<BodyPartComponent>() && !enumeratePrototype.HasComponent<PillComponent>())
      {
        Solution juiceSolution = extractableComponent.JuiceSolution;
        ReagentId id3;
        FixedPoint2 quantity3;
        if (juiceSolution != null)
        {
          ReagentEntitySourceData entitySourceData = new ReagentEntitySourceData(new List<ProtoId<MixingCategoryPrototype>>()
          {
            ChemistryGuideDataSystem.DefaultJuiceCategory
          }, enumeratePrototype, juiceSolution);
          foreach ((id3, quantity3) in juiceSolution.Contents)
            this._reagentSources[id3.Prototype].Add((ReagentSourceData) entitySourceData);
          stringList.Add(enumeratePrototype.Name);
        }
        string grindableSolution = extractableComponent.GrindableSolution;
        SolutionContainerManagerComponent container;
        Solution solution;
        if (grindableSolution != null && enumeratePrototype.TryGetComponent<SolutionContainerManagerComponent>(ref container, this.EntityManager.ComponentFactory) && this._solutionContainer.TryGetSolution(container, grindableSolution, out solution))
        {
          ReagentEntitySourceData entitySourceData = new ReagentEntitySourceData(new List<ProtoId<MixingCategoryPrototype>>()
          {
            ChemistryGuideDataSystem.DefaultGrindCategory
          }, enumeratePrototype, solution);
          foreach ((id3, quantity3) in solution.Contents)
            this._reagentSources[id3.Prototype].Add((ReagentSourceData) entitySourceData);
          stringList.Add(enumeratePrototype.Name);
        }
      }
    }
  }

  public List<ReagentSourceData> GetReagentSources(string id)
  {
    return this._reagentSources.GetValueOrDefault<string, List<ReagentSourceData>>(id) ?? new List<ReagentSourceData>();
  }

  public override void ReloadAllReagentPrototypes()
  {
  }
}
