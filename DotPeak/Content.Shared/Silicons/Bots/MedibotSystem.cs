// Decompiled with JetBrains decompiler
// Type: Content.Shared.Silicons.Bots.MedibotSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Damage;
using Content.Shared.DoAfter;
using Content.Shared.Emag.Components;
using Content.Shared.Emag.Systems;
using Content.Shared.FixedPoint;
using Content.Shared.Interaction;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.NPC.Components;
using Content.Shared.Popups;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Content.Shared.Silicons.Bots;

public sealed class MedibotSystem : EntitySystem
{
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private EmagSystem _emag;
  [Dependency]
  private SharedInteractionSystem _interaction;
  [Dependency]
  private SharedSolutionContainerSystem _solutionContainer;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedDoAfterSystem _doAfter;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<EmaggableMedibotComponent, GotEmaggedEvent>(new ComponentEventRefHandler<EmaggableMedibotComponent, GotEmaggedEvent>(this.OnEmagged));
    this.SubscribeLocalEvent<MedibotComponent, UserActivateInWorldEvent>(new EntityEventRefHandler<MedibotComponent, UserActivateInWorldEvent>(this.OnInteract));
    this.SubscribeLocalEvent<MedibotComponent, MedibotInjectDoAfterEvent>(new ComponentEventRefHandler<MedibotComponent, MedibotInjectDoAfterEvent>(this.OnInject));
  }

  private void OnEmagged(EntityUid uid, EmaggableMedibotComponent comp, ref GotEmaggedEvent args)
  {
    MedibotComponent comp1;
    if (!this._emag.CompareFlag(args.Type, EmagType.Interaction) || this._emag.CheckFlag(uid, EmagType.Interaction) || !this.TryComp<MedibotComponent>(uid, out comp1))
      return;
    foreach ((MobState key, MedibotTreatment medibotTreatment) in comp.Replacements)
      comp1.Treatments[key] = medibotTreatment;
    args.Handled = true;
  }

  private void OnInteract(Entity<MedibotComponent> medibot, ref UserActivateInWorldEvent args)
  {
    if (!this.CheckInjectable(medibot, args.Target, true))
      return;
    this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, args.User, 2f, (DoAfterEvent) new MedibotInjectDoAfterEvent(), new EntityUid?(args.User), new EntityUid?(args.Target))
    {
      BlockDuplicate = true,
      BreakOnMove = true
    });
  }

  private void OnInject(EntityUid uid, MedibotComponent comp, ref MedibotInjectDoAfterEvent args)
  {
    if (args.Cancelled)
      return;
    EntityUid? target = args.Target;
    if (!target.HasValue)
      return;
    EntityUid valueOrDefault = target.GetValueOrDefault();
    this.TryInject((Entity<MedibotComponent>) uid, valueOrDefault);
  }

  public bool TryGetTreatment(
    MedibotComponent comp,
    MobState state,
    [NotNullWhen(true)] out MedibotTreatment? treatment)
  {
    return comp.Treatments.TryGetValue(state, out treatment);
  }

  public bool CheckInjectable(Entity<MedibotComponent?> medibot, EntityUid target, bool manual = false)
  {
    if (!this.Resolve<MedibotComponent>((EntityUid) medibot, ref medibot.Comp, false))
      return false;
    if (this.HasComp<NPCRecentlyInjectedComponent>(target))
    {
      this._popup.PopupClient(this.Loc.GetString("medibot-recently-injected"), (EntityUid) medibot, new EntityUid?((EntityUid) medibot));
      return false;
    }
    MobStateComponent comp1;
    DamageableComponent comp2;
    if (!this.TryComp<MobStateComponent>(target, out comp1) || !this.TryComp<DamageableComponent>(target, out comp2) || !this._solutionContainer.TryGetInjectableSolution((Entity<InjectableSolutionComponent, SolutionContainerManagerComponent>) target, out Entity<SolutionComponent>? _, out Solution _))
      return false;
    if (comp1.CurrentState != MobState.Alive && comp1.CurrentState != MobState.Critical)
    {
      this._popup.PopupClient(this.Loc.GetString("medibot-target-dead"), (EntityUid) medibot, new EntityUid?((EntityUid) medibot));
      return false;
    }
    FixedPoint2 totalDamage = comp2.TotalDamage;
    if (totalDamage == 0 && !this.HasComp<EmaggedComponent>((EntityUid) medibot))
    {
      this._popup.PopupClient(this.Loc.GetString("medibot-target-healthy"), (EntityUid) medibot, new EntityUid?((EntityUid) medibot));
      return false;
    }
    MedibotTreatment treatment;
    return this.TryGetTreatment(medibot.Comp, comp1.CurrentState, out treatment) && (treatment.IsValid(totalDamage) || manual);
  }

  public bool TryInject(Entity<MedibotComponent?> medibot, EntityUid target)
  {
    MobStateComponent comp;
    MedibotTreatment treatment;
    Entity<SolutionComponent>? soln;
    if (!this.Resolve<MedibotComponent>((EntityUid) medibot, ref medibot.Comp, false) || !this._interaction.InRangeUnobstructed((Entity<TransformComponent>) medibot.Owner, (Entity<TransformComponent>) target) || !this.TryComp<MobStateComponent>(target, out comp) || !this.TryGetTreatment(medibot.Comp, comp.CurrentState, out treatment) || !this._solutionContainer.TryGetInjectableSolution((Entity<InjectableSolutionComponent, SolutionContainerManagerComponent>) target, out soln, out Solution _))
      return false;
    this.EnsureComp<NPCRecentlyInjectedComponent>(target);
    this._solutionContainer.TryAddReagent(soln.Value, (string) treatment.Reagent, treatment.Quantity, out FixedPoint2 _);
    this._popup.PopupEntity(this.Loc.GetString("hypospray-component-feel-prick-message"), target, target);
    this._popup.PopupClient(this.Loc.GetString("medibot-target-injected"), (EntityUid) medibot, new EntityUid?((EntityUid) medibot));
    this._audio.PlayPredicted(medibot.Comp.InjectSound, (EntityUid) medibot, new EntityUid?((EntityUid) medibot));
    return true;
  }
}
