// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Medical.Defibrillator.RMCDefibrillatorSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Body;
using Content.Shared._RMC14.Chemistry.Effects;
using Content.Shared._RMC14.Chemistry.Reagent;
using Content.Shared._RMC14.Damage;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.EntityEffects;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Medical;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

#nullable enable
namespace Content.Shared._RMC14.Medical.Defibrillator;

public sealed class RMCDefibrillatorSystem : EntitySystem
{
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private DamageableSystem _damageable;
  [Dependency]
  private SharedRMCBloodstreamSystem _rmcBloodstream;
  [Dependency]
  private SharedRMCDamageableSystem _rmcDamageable;
  [Dependency]
  private RMCReagentSystem _rmcReagent;
  [Dependency]
  private SharedSolutionContainerSystem _solutionContainer;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<DefibrillatorComponent, RMCDefibrillatorDamageModifyEvent>(new EntityEventRefHandler<DefibrillatorComponent, RMCDefibrillatorDamageModifyEvent>(this.OnDefibrillatorDamageModify));
    this.SubscribeLocalEvent<RMCDefibrillatorAudioComponent, EntityTerminatingEvent>(new EntityEventRefHandler<RMCDefibrillatorAudioComponent, EntityTerminatingEvent>(this.OnDefibrillatorAudioTerminating));
    this.SubscribeLocalEvent<RMCDefibrillatorBlockedComponent, ExaminedEvent>(new EntityEventRefHandler<RMCDefibrillatorBlockedComponent, ExaminedEvent>(this.OnNoDefibExamine));
  }

  private void OnDefibrillatorDamageModify(
    Entity<DefibrillatorComponent> ent,
    ref RMCDefibrillatorDamageModifyEvent args)
  {
    if (ent.Comp.RMCZapDamage != null)
    {
      foreach ((ProtoId<DamageGroupPrototype> protoId, int num) in ent.Comp.RMCZapDamage)
        args.Heal = this._rmcDamageable.DistributeDamageCached((Entity<DamageableComponent>) args.Target, protoId, (FixedPoint2) num, args.Heal);
    }
    Entity<SolutionComponent> solutionEnt;
    if (!this._rmcBloodstream.TryGetChemicalSolution(args.Target, out solutionEnt, out Solution _))
      return;
    (Content.Shared._RMC14.Chemistry.Reagent.Reagent, FixedPoint2, Electrogenetic)? nullable = new (Content.Shared._RMC14.Chemistry.Reagent.Reagent, FixedPoint2, Electrogenetic)?();
    foreach (ReagentQuantity content in solutionEnt.Comp.Solution.Contents)
    {
      Content.Shared._RMC14.Chemistry.Reagent.Reagent reagent;
      ReagentEffectsEntry reagentEffectsEntry;
      if (this._rmcReagent.TryIndex((ProtoId<ReagentPrototype>) content.Reagent.Prototype, out reagent) && reagent.Metabolisms != null && reagent.Metabolisms.TryGetValue(ent.Comp.MetabolismId, out reagentEffectsEntry))
      {
        foreach (EntityEffect effect in reagentEffectsEntry.Effects)
        {
          if (effect is Electrogenetic electrogenetic && (!nullable.HasValue || electrogenetic.HealAmount > nullable.Value.Item2))
            nullable = new (Content.Shared._RMC14.Chemistry.Reagent.Reagent, FixedPoint2, Electrogenetic)?((reagent, electrogenetic.HealAmount, electrogenetic));
        }
      }
    }
    if (!nullable.HasValue)
      return;
    args.Heal += nullable.Value.Item3.CalculateHeal(this._damageable, args.Target, (IEntityManager) this.EntityManager);
    this._solutionContainer.RemoveReagent(solutionEnt, nullable.Value.Item1.ID, (FixedPoint2) 1);
  }

  private void OnNoDefibExamine(
    Entity<RMCDefibrillatorBlockedComponent> ent,
    ref ExaminedEvent args)
  {
    if (!ent.Comp.ShowOnExamine)
      return;
    args.PushMarkup(this.Loc.GetString((string) ent.Comp.Examine, ("victim", (object) ent)));
  }

  private void OnDefibrillatorAudioTerminating(
    Entity<RMCDefibrillatorAudioComponent> ent,
    ref EntityTerminatingEvent args)
  {
    DefibrillatorComponent comp;
    if (!this.TryComp<DefibrillatorComponent>(ent.Comp.Defibrillator, out comp))
      return;
    comp.ChargeSoundEntity = new EntityUid?();
  }

  public void StopChargingAudio(Entity<DefibrillatorComponent> defib)
  {
    this._audio.Stop(defib.Comp.ChargeSoundEntity);
    this.QueueDel(defib.Comp.ChargeSoundEntity);
    defib.Comp.ChargeSoundEntity = new EntityUid?();
  }
}
