// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chemistry.Reaction.ChemicalReactionSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Administration.Logs;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Database;
using Content.Shared.EntityEffects;
using Content.Shared.FixedPoint;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared.Chemistry.Reaction;

public sealed class ChemicalReactionSystem : EntitySystem
{
  public static readonly ProtoId<ReactionPrototype> FoamReaction = ProtoId<ReactionPrototype>.op_Implicit("Foam");
  private const int MaxReactionIterations = 20;
  [Dependency]
  private IPrototypeManager _prototypeManager;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private ISharedAdminLogManager _adminLogger;
  [Dependency]
  private SharedTransformSystem _transformSystem;
  private FrozenDictionary<string, List<ReactionPrototype>> _reactionsSingle;
  private FrozenDictionary<string, List<ReactionPrototype>> _reactions;

  public virtual void Initialize()
  {
    base.Initialize();
    this.InitializeReactionCache();
    this.SubscribeLocalEvent<PrototypesReloadedEventArgs>(new EntityEventHandler<PrototypesReloadedEventArgs>(this.OnPrototypesReloaded), (Type[]) null, (Type[]) null);
  }

  private void InitializeReactionCache()
  {
    Dictionary<string, List<ReactionPrototype>> source = new Dictionary<string, List<ReactionPrototype>>();
    foreach (ReactionPrototype enumeratePrototype in this._prototypeManager.EnumeratePrototypes<ReactionPrototype>())
    {
      string str = enumeratePrototype.Reactants.Keys.First<string>();
      Extensions.GetOrNew<string, List<ReactionPrototype>>(source, str).Add(enumeratePrototype);
    }
    this._reactionsSingle = source.ToFrozenDictionary<string, List<ReactionPrototype>>();
    source.Clear();
    foreach (ReactionPrototype enumeratePrototype in this._prototypeManager.EnumeratePrototypes<ReactionPrototype>())
    {
      foreach (string key in enumeratePrototype.Reactants.Keys)
        Extensions.GetOrNew<string, List<ReactionPrototype>>(source, key).Add(enumeratePrototype);
    }
    this._reactions = source.ToFrozenDictionary<string, List<ReactionPrototype>>();
  }

  private void OnPrototypesReloaded(PrototypesReloadedEventArgs eventArgs)
  {
    if (!eventArgs.WasModified<ReactionPrototype>())
      return;
    this.InitializeReactionCache();
  }

  private bool CanReact(
    Entity<SolutionComponent> soln,
    ReactionPrototype reaction,
    ReactionMixerComponent? mixerComponent,
    out FixedPoint2 lowestUnitReactions)
  {
    Solution solution = soln.Comp.Solution;
    lowestUnitReactions = FixedPoint2.MaxValue;
    if ((double) solution.Temperature < (double) reaction.MinimumTemperature)
    {
      lowestUnitReactions = FixedPoint2.Zero;
      return false;
    }
    if ((double) solution.Temperature > (double) reaction.MaximumTemperature)
    {
      lowestUnitReactions = FixedPoint2.Zero;
      return false;
    }
    if (mixerComponent == null && reaction.MixingCategories != null || mixerComponent != null && reaction.MixingCategories != null && reaction.MixingCategories.Except<ProtoId<MixingCategoryPrototype>>((IEnumerable<ProtoId<MixingCategoryPrototype>>) mixerComponent.ReactionTypes).Any<ProtoId<MixingCategoryPrototype>>())
    {
      lowestUnitReactions = FixedPoint2.Zero;
      return false;
    }
    ReactionAttemptEvent reactionAttemptEvent = new ReactionAttemptEvent(reaction, soln);
    this.RaiseLocalEvent<ReactionAttemptEvent>(Entity<SolutionComponent>.op_Implicit(soln), ref reactionAttemptEvent, false);
    if (reactionAttemptEvent.Cancelled)
    {
      lowestUnitReactions = FixedPoint2.Zero;
      return false;
    }
    foreach (KeyValuePair<string, ReactantPrototype> reactant in reaction.Reactants)
    {
      string key = reactant.Key;
      FixedPoint2 amount = reactant.Value.Amount;
      FixedPoint2 prototypeQuantity = solution.GetTotalPrototypeQuantity(key);
      if (prototypeQuantity <= FixedPoint2.Zero)
        return false;
      if (reactant.Value.Catalyst)
      {
        if (prototypeQuantity == FixedPoint2.Zero || reaction.Quantized && prototypeQuantity < amount)
          return false;
      }
      else
      {
        FixedPoint2 fixedPoint2 = prototypeQuantity / amount;
        if (fixedPoint2 < lowestUnitReactions)
          lowestUnitReactions = fixedPoint2;
      }
    }
    if (reaction.Quantized)
      lowestUnitReactions = (FixedPoint2) (int) lowestUnitReactions;
    return lowestUnitReactions > 0;
  }

  private List<string> PerformReaction(
    Entity<SolutionComponent> soln,
    ReactionPrototype reaction,
    FixedPoint2 unitReactions)
  {
    EntityUid entityUid;
    SolutionComponent solutionComponent;
    soln.Deconstruct(ref entityUid, ref solutionComponent);
    Solution solution = solutionComponent.Solution;
    float num = reaction.ConserveEnergy ? solution.GetThermalEnergy(this._prototypeManager) : 0.0f;
    foreach (KeyValuePair<string, ReactantPrototype> reactant in reaction.Reactants)
    {
      if (!reactant.Value.Catalyst)
      {
        FixedPoint2 quantity = unitReactions * reactant.Value.Amount;
        solution.RemoveReagent(reactant.Key, quantity, ignoreReagentData: true);
      }
    }
    List<string> stringList = new List<string>();
    foreach (KeyValuePair<string, FixedPoint2> product in reaction.Products)
    {
      stringList.Add(product.Key);
      solution.AddReagent(product.Key, product.Value * unitReactions);
    }
    if (reaction.ConserveEnergy)
    {
      float heatCapacity = solution.GetHeatCapacity(this._prototypeManager);
      if ((double) heatCapacity > 0.0)
        solution.Temperature = num / heatCapacity;
    }
    this.OnReaction(soln, reaction, (ReagentPrototype) null, unitReactions);
    return stringList;
  }

  private void OnReaction(
    Entity<SolutionComponent> soln,
    ReactionPrototype reaction,
    ReagentPrototype? reagent,
    FixedPoint2 unitReactions)
  {
    EntityEffectReagentArgs args = new EntityEffectReagentArgs(Entity<SolutionComponent>.op_Implicit(soln), (IEntityManager) this.EntityManager, new EntityUid?(), soln.Comp.Solution, unitReactions, reagent, new ReactionMethod?(), (FixedPoint2) 1f);
    EntityCoordinates? nullable;
    bool orGridCoordinates = this._transformSystem.TryGetMapOrGridCoordinates(Entity<SolutionComponent>.op_Implicit(soln), ref nullable, (TransformComponent) null);
    ISharedAdminLogManager adminLogger1 = this._adminLogger;
    int impact = (int) reaction.Impact;
    LogStringHandler logStringHandler1 = new LogStringHandler(61, 4);
    logStringHandler1.AppendLiteral("Chemical reaction ");
    logStringHandler1.AppendFormatted(reaction.ID, format: nameof (reaction));
    logStringHandler1.AppendLiteral(" occurred with strength ");
    logStringHandler1.AppendFormatted<FixedPoint2>(unitReactions, "strength", nameof (unitReactions));
    logStringHandler1.AppendLiteral(" on entity ");
    logStringHandler1.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?(Entity<SolutionComponent>.op_Implicit(soln)), (MetaDataComponent) null), "metabolizer", "ToPrettyString(soln)");
    logStringHandler1.AppendLiteral(" at Pos:");
    ref LogStringHandler local1 = ref logStringHandler1;
    string str1;
    if (!orGridCoordinates)
      str1 = "[Grid or Map not Found]";
    else
      str1 = $"{nullable:coordinates}";
    local1.AppendFormatted(str1);
    ref LogStringHandler local2 = ref logStringHandler1;
    adminLogger1.Add(LogType.ChemicalReaction, (LogImpact) impact, ref local2);
    foreach (EntityEffect effect in reaction.Effects)
    {
      if (effect.ShouldApply((EntityEffectBaseArgs) args))
      {
        if (effect.ShouldLog)
        {
          EntityUid targetEntity = args.TargetEntity;
          ISharedAdminLogManager adminLogger2 = this._adminLogger;
          int logImpact = (int) effect.LogImpact;
          LogStringHandler logStringHandler2 = new LogStringHandler(56, 4);
          logStringHandler2.AppendLiteral("Reaction effect ");
          logStringHandler2.AppendFormatted(effect.GetType().Name, format: "effect");
          logStringHandler2.AppendLiteral(" of reaction ");
          logStringHandler2.AppendFormatted(reaction.ID, format: nameof (reaction));
          logStringHandler2.AppendLiteral(" applied on entity ");
          logStringHandler2.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(targetEntity)), "entity", "ToPrettyString(entity)");
          logStringHandler2.AppendLiteral(" at Pos:");
          ref LogStringHandler local3 = ref logStringHandler2;
          string str2;
          if (!orGridCoordinates)
            str2 = "[Grid or Map not Found";
          else
            str2 = $"{nullable:coordinates}";
          local3.AppendFormatted(str2);
          ref LogStringHandler local4 = ref logStringHandler2;
          adminLogger2.Add(LogType.ReagentEffect, (LogImpact) logImpact, ref local4);
        }
        effect.Effect((EntityEffectBaseArgs) args);
      }
    }
    this._audio.PlayPvs(reaction.Sound, Entity<SolutionComponent>.op_Implicit(soln), new AudioParams?());
  }

  private bool ProcessReactions(
    Entity<SolutionComponent> soln,
    SortedSet<ReactionPrototype> reactions,
    ReactionMixerComponent? mixerComponent)
  {
    HashSet<ReactionPrototype> reactionPrototypeSet = new HashSet<ReactionPrototype>();
    List<string> stringList = (List<string>) null;
    foreach (ReactionPrototype reaction in reactions)
    {
      FixedPoint2 lowestUnitReactions;
      if (!this.CanReact(soln, reaction, mixerComponent, out lowestUnitReactions))
      {
        reactionPrototypeSet.Add(reaction);
      }
      else
      {
        stringList = this.PerformReaction(soln, reaction, lowestUnitReactions);
        break;
      }
    }
    if (stringList == null)
      return false;
    if (stringList.Count == 0)
      return true;
    foreach (string key in stringList)
    {
      List<ReactionPrototype> other;
      if (this._reactions.TryGetValue(key, out other))
        reactions.UnionWith((IEnumerable<ReactionPrototype>) other);
    }
    return true;
  }

  public void FullyReactSolution(
    Entity<SolutionComponent> soln,
    ReactionMixerComponent? mixerComponent = null)
  {
    SortedSet<ReactionPrototype> reactions = new SortedSet<ReactionPrototype>();
    foreach (ReagentQuantity content in soln.Comp.Solution.Contents)
    {
      List<ReactionPrototype> other;
      if (this._reactionsSingle.TryGetValue(content.Reagent.Prototype, out other))
        reactions.UnionWith((IEnumerable<ReactionPrototype>) other);
    }
    for (int index = 0; index < 20; ++index)
    {
      if (!this.ProcessReactions(soln, reactions, mixerComponent))
        return;
    }
    this.Log.Error($"{"Solution"} {soln.Owner} could not finish reacting in under {20} loops.");
  }
}
