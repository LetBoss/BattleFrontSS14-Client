// Decompiled with JetBrains decompiler
// Type: Content.Shared.Body.Systems.SharedBloodstreamSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Damage;
using Content.Shared._RMC14.Medical.Stasis;
using Content.Shared._RMC14.Medical.Wounds;
using Content.Shared.Alert;
using Content.Shared.Body.Components;
using Content.Shared.Body.Events;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Reaction;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Drunk;
using Content.Shared.EntityEffects;
using Content.Shared.EntityEffects.Effects;
using Content.Shared.FixedPoint;
using Content.Shared.Fluids;
using Content.Shared.Forensics.Components;
using Content.Shared.HealthExaminable;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Random.Helpers;
using Content.Shared.Rejuvenate;
using Content.Shared.Speech.EntitySystems;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Body.Systems;

public abstract class SharedBloodstreamSystem : EntitySystem
{
  private static readonly bool DisableBleedingExamineText = true;
  [Dependency]
  protected SharedSolutionContainerSystem SolutionContainer;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private IPrototypeManager _prototypeManager;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedPuddleSystem _puddle;
  [Dependency]
  private AlertsSystem _alertsSystem;
  [Dependency]
  private MobStateSystem _mobStateSystem;
  [Dependency]
  private DamageableSystem _damageableSystem;
  [Dependency]
  private SharedDrunkSystem _drunkSystem;
  [Dependency]
  private SharedStutteringSystem _stutteringSystem;
  [Dependency]
  private CMStasisBagSystem _cmStasisBag;
  [Dependency]
  private SharedRMCDamageableSystem _rmcDamageable;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BloodstreamComponent, MapInitEvent>(new EntityEventRefHandler<BloodstreamComponent, MapInitEvent>((object) this, __methodptr(OnMapInit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BloodstreamComponent, EntRemovedFromContainerMessage>(new EntityEventRefHandler<BloodstreamComponent, EntRemovedFromContainerMessage>((object) this, __methodptr(OnEntRemoved)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BloodstreamComponent, ReactionAttemptEvent>(new EntityEventRefHandler<BloodstreamComponent, ReactionAttemptEvent>((object) this, __methodptr(OnReactionAttempt)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BloodstreamComponent, SolutionRelayEvent<ReactionAttemptEvent>>(new EntityEventRefHandler<BloodstreamComponent, SolutionRelayEvent<ReactionAttemptEvent>>((object) this, __methodptr(OnReactionAttempt)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BloodstreamComponent, DamageChangedEvent>(new EntityEventRefHandler<BloodstreamComponent, DamageChangedEvent>((object) this, __methodptr(OnDamageChanged)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BloodstreamComponent, HealthBeingExaminedEvent>(new EntityEventRefHandler<BloodstreamComponent, HealthBeingExaminedEvent>((object) this, __methodptr(OnHealthBeingExamined)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BloodstreamComponent, BeingGibbedEvent>(new EntityEventRefHandler<BloodstreamComponent, BeingGibbedEvent>((object) this, __methodptr(OnBeingGibbed)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BloodstreamComponent, ApplyMetabolicMultiplierEvent>(new EntityEventRefHandler<BloodstreamComponent, ApplyMetabolicMultiplierEvent>((object) this, __methodptr(OnApplyMetabolicMultiplier)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BloodstreamComponent, RejuvenateEvent>(new EntityEventRefHandler<BloodstreamComponent, RejuvenateEvent>((object) this, __methodptr(OnRejuvenate)), (Type[]) null, (Type[]) null);
  }

  public virtual void Update(float frameTime)
  {
    base.Update(frameTime);
    TimeSpan curTime = this._timing.CurTime;
    EntityQueryEnumerator<BloodstreamComponent> entityQueryEnumerator = this.EntityQueryEnumerator<BloodstreamComponent>();
    EntityUid entityUid;
    BloodstreamComponent bloodstreamComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref bloodstreamComponent))
    {
      if (!(curTime < bloodstreamComponent.NextUpdate))
      {
        bloodstreamComponent.NextUpdate += bloodstreamComponent.AdjustedUpdateInterval;
        this.DirtyField<BloodstreamComponent>(entityUid, bloodstreamComponent, "NextUpdate", (MetaDataComponent) null);
        Solution solution;
        if (this._cmStasisBag.CanBodyMetabolize(entityUid) && this.SolutionContainer.ResolveSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(entityUid), bloodstreamComponent.BloodSolutionName, ref bloodstreamComponent.BloodSolution, out solution))
        {
          if (solution.Volume < solution.MaxVolume && !this._mobStateSystem.IsDead(entityUid))
            this.TryModifyBloodLevel(Entity<BloodstreamComponent>.op_Implicit((entityUid, bloodstreamComponent)), bloodstreamComponent.BloodRefreshAmount);
          if ((double) bloodstreamComponent.BleedAmount > 0.0)
          {
            this.TryModifyBloodLevel(Entity<BloodstreamComponent>.op_Implicit((entityUid, bloodstreamComponent)), (FixedPoint2) -bloodstreamComponent.BleedAmount);
            this.TryModifyBleedAmount(Entity<BloodstreamComponent>.op_Implicit((entityUid, bloodstreamComponent)), -bloodstreamComponent.BleedReductionAmount);
          }
          float bloodLevelPercentage = this.GetBloodLevelPercentage(Entity<BloodstreamComponent>.op_Implicit((entityUid, bloodstreamComponent)));
          if ((double) bloodLevelPercentage < (double) bloodstreamComponent.BloodlossThreshold && !this._mobStateSystem.IsDead(entityUid))
          {
            DamageSpecifier damage = bloodstreamComponent.BloodlossDamage / (0.1f + bloodLevelPercentage);
            this._damageableSystem.TryChangeDamage(new EntityUid?(entityUid), damage, interruptsDoAfters: false);
            this._drunkSystem.TryApplyDrunkenness(entityUid, (float) (bloodstreamComponent.AdjustedUpdateInterval.TotalSeconds * 2.0), false);
            this._stutteringSystem.DoStutter(entityUid, bloodstreamComponent.AdjustedUpdateInterval * 2.0, false);
            bloodstreamComponent.StatusTime += bloodstreamComponent.AdjustedUpdateInterval * 2.0;
            this.DirtyField<BloodstreamComponent>(entityUid, bloodstreamComponent, "StatusTime", (MetaDataComponent) null);
          }
          else if (!this._mobStateSystem.IsDead(entityUid))
          {
            if (this._rmcDamageable.HasAnyDamage(Entity<DamageableComponent>.op_Implicit(entityUid), bloodstreamComponent.BloodlossHealDamage))
              this._damageableSystem.TryChangeDamage(new EntityUid?(entityUid), bloodstreamComponent.BloodlossHealDamage * bloodLevelPercentage, true, false);
            this._drunkSystem.TryRemoveDrunkenessTime(entityUid, bloodstreamComponent.StatusTime.TotalSeconds);
            this._stutteringSystem.DoRemoveStutterTime(entityUid, bloodstreamComponent.StatusTime.TotalSeconds);
            bloodstreamComponent.StatusTime = TimeSpan.Zero;
            this.DirtyField<BloodstreamComponent>(entityUid, bloodstreamComponent, "StatusTime", (MetaDataComponent) null);
          }
        }
      }
    }
  }

  private void OnMapInit(Entity<BloodstreamComponent> ent, ref MapInitEvent args)
  {
    ent.Comp.NextUpdate = this._timing.CurTime + ent.Comp.AdjustedUpdateInterval;
    this.DirtyField<BloodstreamComponent>(Entity<BloodstreamComponent>.op_Implicit(ent), ent.Comp, "NextUpdate", (MetaDataComponent) null);
  }

  private void OnEntRemoved(
    Entity<BloodstreamComponent> entity,
    ref EntRemovedFromContainerMessage args)
  {
    EntityUid entity1 = ((ContainerModifiedMessage) args).Entity;
    ref Entity<SolutionComponent>? local1 = ref entity.Comp.BloodSolution;
    EntityUid? nullable1 = local1.HasValue ? new EntityUid?(local1.GetValueOrDefault().Owner) : new EntityUid?();
    if ((nullable1.HasValue ? (EntityUid.op_Equality(entity1, nullable1.GetValueOrDefault()) ? 1 : 0) : 0) != 0)
      entity.Comp.BloodSolution = new Entity<SolutionComponent>?();
    EntityUid entity2 = ((ContainerModifiedMessage) args).Entity;
    ref Entity<SolutionComponent>? local2 = ref entity.Comp.ChemicalSolution;
    EntityUid? nullable2 = local2.HasValue ? new EntityUid?(local2.GetValueOrDefault().Owner) : new EntityUid?();
    if ((nullable2.HasValue ? (EntityUid.op_Equality(entity2, nullable2.GetValueOrDefault()) ? 1 : 0) : 0) != 0)
      entity.Comp.ChemicalSolution = new Entity<SolutionComponent>?();
    EntityUid entity3 = ((ContainerModifiedMessage) args).Entity;
    ref Entity<SolutionComponent>? local3 = ref entity.Comp.TemporarySolution;
    EntityUid? nullable3 = local3.HasValue ? new EntityUid?(local3.GetValueOrDefault().Owner) : new EntityUid?();
    if ((nullable3.HasValue ? (EntityUid.op_Equality(entity3, nullable3.GetValueOrDefault()) ? 1 : 0) : 0) == 0)
      return;
    entity.Comp.TemporarySolution = new Entity<SolutionComponent>?();
  }

  private void OnReactionAttempt(Entity<BloodstreamComponent> ent, ref ReactionAttemptEvent args)
  {
    if (args.Cancelled)
      return;
    foreach (EntityEffect effect in args.Reaction.Effects)
    {
      if (effect is CreateEntityReactionEffect || effect is AreaReactionEffect)
      {
        args.Cancelled = true;
        break;
      }
    }
  }

  private void OnReactionAttempt(
    Entity<BloodstreamComponent> ent,
    ref SolutionRelayEvent<ReactionAttemptEvent> args)
  {
    if (args.Name != ent.Comp.BloodSolutionName && args.Name != ent.Comp.ChemicalSolutionName && args.Name != ent.Comp.BloodTemporarySolutionName)
      return;
    this.OnReactionAttempt(ent, ref args.Event);
  }

  private void OnDamageChanged(Entity<BloodstreamComponent> ent, ref DamageChangedEvent args)
  {
    if (this._timing.ApplyingState)
      return;
    CMBleedEvent cmBleedEvent = new CMBleedEvent(args);
    this.RaiseLocalEvent<CMBleedEvent>(Entity<BloodstreamComponent>.op_Implicit(ent), ref cmBleedEvent, false);
    DamageModifierSetPrototype modifierSet;
    if (cmBleedEvent.Handled || args.DamageDelta == null || !args.DamageIncreased || !this._prototypeManager.TryIndex<DamageModifierSetPrototype>(ent.Comp.DamageBleedModifiers, ref modifierSet))
      return;
    DamageSpecifier damageSpecifier = DamageSpecifier.ApplyModifierSet(DamageSpecifier.GetPositive(args.DamageDelta), (DamageModifierSet) modifierSet);
    if (damageSpecifier.Empty)
      return;
    float bleedAmount = ent.Comp.BleedAmount;
    FixedPoint2 total = damageSpecifier.GetTotal();
    float amount = total.Float();
    this.TryModifyBleedAmount(ent.AsNullable(), amount);
    List<int> values = new List<int>();
    values.Add((int) this._timing.CurTick.Value);
    values.Add(this.GetNetEntity(Entity<BloodstreamComponent>.op_Implicit(ent), (MetaDataComponent) null).Id);
    NetEntity? netEntity = this.GetNetEntity(args.Origin, (MetaDataComponent) null);
    ref NetEntity? local = ref netEntity;
    values.Add(local.HasValue ? local.GetValueOrDefault().Id : 0);
    System.Random random = new System.Random(SharedRandomExtensions.HashCodeCombine(values));
    float num = Math.Clamp(amount / 25f, 0.0f, 1f);
    if ((double) amount > 0.0 && random.NextDouble() < (double) num)
    {
      this.TryModifyBloodLevel(ent.AsNullable(), -total / 5f);
      this._audio.PlayPredicted(ent.Comp.InstantBloodSound, Entity<BloodstreamComponent>.op_Implicit(ent), args.Origin, new AudioParams?());
    }
    else
    {
      if ((double) amount > (double) ent.Comp.BloodHealedSoundThreshold || (double) bleedAmount <= 0.0)
        return;
      this._popup.PopupEntity(this.Loc.GetString("bloodstream-component-wounds-cauterized"), Entity<BloodstreamComponent>.op_Implicit(ent), Entity<BloodstreamComponent>.op_Implicit(ent), PopupType.Medium);
      this._audio.PlayPredicted(ent.Comp.BloodHealedSound, Entity<BloodstreamComponent>.op_Implicit(ent), args.Origin, new AudioParams?());
    }
  }

  private void OnHealthBeingExamined(
    Entity<BloodstreamComponent> ent,
    ref HealthBeingExaminedEvent args)
  {
    if (SharedBloodstreamSystem.DisableBleedingExamineText)
      return;
    if ((double) ent.Comp.BleedAmount > (double) ent.Comp.MaxBleedAmount * 0.75)
    {
      args.Message.PushNewline();
      args.Message.AddMarkupOrThrow(this.Loc.GetString("bloodstream-component-massive-bleeding", ("target", (object) ent.Owner)));
    }
    else if ((double) ent.Comp.BleedAmount > (double) ent.Comp.MaxBleedAmount * 0.5)
    {
      args.Message.PushNewline();
      args.Message.AddMarkupOrThrow(this.Loc.GetString("bloodstream-component-strong-bleeding", ("target", (object) ent.Owner)));
    }
    else if ((double) ent.Comp.BleedAmount > (double) ent.Comp.MaxBleedAmount * 0.25)
    {
      args.Message.PushNewline();
      args.Message.AddMarkupOrThrow(this.Loc.GetString("bloodstream-component-bleeding", ("target", (object) ent.Owner)));
    }
    else if ((double) ent.Comp.BleedAmount > 0.0)
    {
      args.Message.PushNewline();
      args.Message.AddMarkupOrThrow(this.Loc.GetString("bloodstream-component-slight-bleeding", ("target", (object) ent.Owner)));
    }
    if ((double) this.GetBloodLevelPercentage(ent.AsNullable()) >= (double) ent.Comp.BloodlossThreshold)
      return;
    args.Message.PushNewline();
    args.Message.AddMarkupOrThrow(this.Loc.GetString("bloodstream-component-looks-pale", ("target", (object) ent.Owner)));
  }

  private void OnBeingGibbed(Entity<BloodstreamComponent> ent, ref BeingGibbedEvent args)
  {
    this.SpillAllSolutions(ent.AsNullable());
  }

  private void OnApplyMetabolicMultiplier(
    Entity<BloodstreamComponent> ent,
    ref ApplyMetabolicMultiplierEvent args)
  {
    ent.Comp.UpdateIntervalMultiplier = args.Multiplier;
    this.DirtyField<BloodstreamComponent>(Entity<BloodstreamComponent>.op_Implicit(ent), ent.Comp, "UpdateIntervalMultiplier", (MetaDataComponent) null);
  }

  private void OnRejuvenate(Entity<BloodstreamComponent> ent, ref RejuvenateEvent args)
  {
    this.TryModifyBleedAmount(ent.AsNullable(), -ent.Comp.BleedAmount);
    Solution solution;
    if (this.SolutionContainer.ResolveSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(ent.Owner), ent.Comp.BloodSolutionName, ref ent.Comp.BloodSolution, out solution))
      this.TryModifyBloodLevel(ent.AsNullable(), solution.AvailableVolume);
    if (!this.SolutionContainer.ResolveSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(ent.Owner), ent.Comp.ChemicalSolutionName, ref ent.Comp.ChemicalSolution))
      return;
    this.SolutionContainer.RemoveAllSolution(ent.Comp.ChemicalSolution.Value);
  }

  public float GetBloodLevelPercentage(Entity<BloodstreamComponent?> ent)
  {
    Solution solution;
    return !this.Resolve<BloodstreamComponent>(Entity<BloodstreamComponent>.op_Implicit(ent), ref ent.Comp, true) || !this.SolutionContainer.ResolveSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(ent.Owner), ent.Comp.BloodSolutionName, ref ent.Comp.BloodSolution, out solution) ? 0.0f : solution.FillFraction;
  }

  public void SetBloodLossThreshold(Entity<BloodstreamComponent?> ent, float threshold)
  {
    if (!this.Resolve<BloodstreamComponent>(Entity<BloodstreamComponent>.op_Implicit(ent), ref ent.Comp, true))
      return;
    ent.Comp.BloodlossThreshold = threshold;
    this.DirtyField<BloodstreamComponent>(Entity<BloodstreamComponent>.op_Implicit(ent), ent.Comp, "BloodlossThreshold", (MetaDataComponent) null);
  }

  public bool TryAddToChemicals(Entity<BloodstreamComponent?> ent, Solution solution)
  {
    return this.Resolve<BloodstreamComponent>(Entity<BloodstreamComponent>.op_Implicit(ent), ref ent.Comp, false) && this.SolutionContainer.ResolveSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(ent.Owner), ent.Comp.ChemicalSolutionName, ref ent.Comp.ChemicalSolution) && this.SolutionContainer.TryAddSolution(ent.Comp.ChemicalSolution.Value, solution);
  }

  public bool FlushChemicals(
    Entity<BloodstreamComponent?> ent,
    ProtoId<ReagentPrototype>? excludedReagentID,
    FixedPoint2 quantity)
  {
    Solution solution;
    if (!this.Resolve<BloodstreamComponent>(Entity<BloodstreamComponent>.op_Implicit(ent), ref ent.Comp, false) || !this.SolutionContainer.ResolveSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(ent.Owner), ent.Comp.ChemicalSolutionName, ref ent.Comp.ChemicalSolution, out solution))
      return false;
    for (int index = solution.Contents.Count - 1; index >= 0; --index)
    {
      (ReagentId reagentId, FixedPoint2 _) = solution.Contents[index];
      ProtoId<ReagentPrototype>? nullable1 = ProtoId<ReagentPrototype>.op_Implicit(reagentId.Prototype);
      ProtoId<ReagentPrototype>? nullable2 = excludedReagentID;
      if ((nullable1.HasValue == nullable2.HasValue ? (nullable1.HasValue ? (ProtoId<ReagentPrototype>.op_Inequality(nullable1.GetValueOrDefault(), nullable2.GetValueOrDefault()) ? 1 : 0) : 0) : 1) != 0)
        this.SolutionContainer.RemoveReagent(ent.Comp.ChemicalSolution.Value, reagentId, quantity);
    }
    return true;
  }

  public bool TryModifyBloodLevel(Entity<BloodstreamComponent?> ent, FixedPoint2 amount)
  {
    if (!this.Resolve<BloodstreamComponent>(Entity<BloodstreamComponent>.op_Implicit(ent), ref ent.Comp, false) || !this.SolutionContainer.ResolveSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(ent.Owner), ent.Comp.BloodSolutionName, ref ent.Comp.BloodSolution))
      return false;
    if (amount >= 0)
      return this.SolutionContainer.TryAddReagent(ent.Comp.BloodSolution.Value, ProtoId<ReagentPrototype>.op_Implicit(ent.Comp.BloodReagent), amount, data: this.GetEntityBloodData(Entity<BloodstreamComponent>.op_Implicit(ent)));
    Solution otherSolution1 = this.SolutionContainer.SplitSolution(ent.Comp.BloodSolution.Value, -amount);
    Solution solution;
    if (!this.SolutionContainer.ResolveSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(ent.Owner), ent.Comp.BloodTemporarySolutionName, ref ent.Comp.TemporarySolution, out solution))
      return true;
    solution.AddSolution(otherSolution1, this._prototypeManager);
    if (solution.Volume > ent.Comp.BleedPuddleThreshold)
    {
      if (ent.Comp.SpillChemicals && this.SolutionContainer.ResolveSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(ent.Owner), ent.Comp.ChemicalSolutionName, ref ent.Comp.ChemicalSolution))
      {
        Solution otherSolution2 = this.SolutionContainer.SplitSolution(ent.Comp.ChemicalSolution.Value, solution.Volume / 10f);
        solution.AddSolution(otherSolution2, this._prototypeManager);
      }
      this._puddle.TrySpillAt(ent.Owner, solution, out EntityUid _, false);
      solution.RemoveAllSolution();
    }
    this.SolutionContainer.UpdateChemicals(ent.Comp.TemporarySolution.Value);
    return true;
  }

  public bool TryModifyBleedAmount(Entity<BloodstreamComponent?> ent, float amount)
  {
    if (!this.Resolve<BloodstreamComponent>(Entity<BloodstreamComponent>.op_Implicit(ent), ref ent.Comp, false))
      return false;
    ent.Comp.BleedAmount += amount;
    ent.Comp.BleedAmount = Math.Clamp(ent.Comp.BleedAmount, 0.0f, ent.Comp.MaxBleedAmount);
    this.DirtyField<BloodstreamComponent>(Entity<BloodstreamComponent>.op_Implicit(ent), ent.Comp, "BleedAmount", (MetaDataComponent) null);
    if ((double) ent.Comp.BleedAmount == 0.0)
    {
      this._alertsSystem.ClearAlert(Entity<BloodstreamComponent>.op_Implicit(ent), ent.Comp.BleedingAlert);
    }
    else
    {
      short num = (short) Math.Clamp(Math.Round((double) ent.Comp.BleedAmount, MidpointRounding.ToZero), 0.0, 10.0);
      this._alertsSystem.ShowAlert(Entity<BloodstreamComponent>.op_Implicit(ent), ent.Comp.BleedingAlert, new short?(num));
    }
    return true;
  }

  public void SpillAllSolutions(Entity<BloodstreamComponent?> ent)
  {
    if (!this.Resolve<BloodstreamComponent>(Entity<BloodstreamComponent>.op_Implicit(ent), ref ent.Comp, true))
      return;
    Solution solution1 = new Solution();
    Solution solution2;
    if (this.SolutionContainer.ResolveSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(ent.Owner), ent.Comp.BloodSolutionName, ref ent.Comp.BloodSolution, out solution2))
    {
      solution1.MaxVolume += solution2.MaxVolume;
      solution1.AddSolution(solution2, this._prototypeManager);
      this.SolutionContainer.RemoveAllSolution(ent.Comp.BloodSolution.Value);
    }
    Solution solution3;
    if (this.SolutionContainer.ResolveSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(ent.Owner), ent.Comp.ChemicalSolutionName, ref ent.Comp.ChemicalSolution, out solution3))
    {
      solution1.MaxVolume += solution3.MaxVolume;
      solution1.AddSolution(solution3, this._prototypeManager);
      this.SolutionContainer.RemoveAllSolution(ent.Comp.ChemicalSolution.Value);
    }
    Solution solution4;
    if (this.SolutionContainer.ResolveSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(ent.Owner), ent.Comp.BloodTemporarySolutionName, ref ent.Comp.TemporarySolution, out solution4))
    {
      solution1.MaxVolume += solution4.MaxVolume;
      solution1.AddSolution(solution4, this._prototypeManager);
      this.SolutionContainer.RemoveAllSolution(ent.Comp.TemporarySolution.Value);
    }
    this._puddle.TrySpillAt(Entity<BloodstreamComponent>.op_Implicit(ent), solution1, out EntityUid _);
  }

  public void ChangeBloodReagent(
    Entity<BloodstreamComponent?> ent,
    ProtoId<ReagentPrototype> reagent)
  {
    if (!this.Resolve<BloodstreamComponent>(Entity<BloodstreamComponent>.op_Implicit(ent), ref ent.Comp, false) || ProtoId<ReagentPrototype>.op_Equality(reagent, ent.Comp.BloodReagent))
      return;
    Solution solution;
    if (!this.SolutionContainer.ResolveSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(ent.Owner), ent.Comp.BloodSolutionName, ref ent.Comp.BloodSolution, out solution))
    {
      ent.Comp.BloodReagent = reagent;
    }
    else
    {
      FixedPoint2 quantity = solution.RemoveReagent(ProtoId<ReagentPrototype>.op_Implicit(ent.Comp.BloodReagent), solution.Volume, ignoreReagentData: true);
      ent.Comp.BloodReagent = reagent;
      this.DirtyField<BloodstreamComponent>(Entity<BloodstreamComponent>.op_Implicit(ent), ent.Comp, "BloodReagent", (MetaDataComponent) null);
      if (!(quantity > 0))
        return;
      this.SolutionContainer.TryAddReagent(ent.Comp.BloodSolution.Value, ProtoId<ReagentPrototype>.op_Implicit(ent.Comp.BloodReagent), quantity, data: this.GetEntityBloodData(Entity<BloodstreamComponent>.op_Implicit(ent)));
    }
  }

  public List<ReagentData> GetEntityBloodData(EntityUid uid)
  {
    DnaComponent dnaComponent;
    return new List<ReagentData>()
    {
      (ReagentData) new DnaData()
      {
        DNA = (!this.TryComp<DnaComponent>(uid, ref dnaComponent) || dnaComponent.DNA == null ? this.Loc.GetString("forensics-dna-unknown") : dnaComponent.DNA)
      }
    };
  }
}
