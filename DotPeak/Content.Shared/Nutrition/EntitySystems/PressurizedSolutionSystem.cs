// Decompiled with JetBrains decompiler
// Type: Content.Shared.Nutrition.EntitySystems.PressurizedSolutionSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Chemistry.Reagent;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Fluids;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.IdentityManagement;
using Content.Shared.Nutrition.Components;
using Content.Shared.Popups;
using Content.Shared.Throwing;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared.Nutrition.EntitySystems;

public sealed class PressurizedSolutionSystem : EntitySystem
{
  [Dependency]
  private SharedSolutionContainerSystem _solutionContainer;
  [Dependency]
  private OpenableSystem _openable;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedPuddleSystem _puddle;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private IRobustRandom _random;
  [Dependency]
  private IPrototypeManager _prototypeManager;
  [Dependency]
  private RMCReagentSystem _reagents;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<PressurizedSolutionComponent, MapInitEvent>(new EntityEventRefHandler<PressurizedSolutionComponent, MapInitEvent>(this.OnMapInit));
    this.SubscribeLocalEvent<PressurizedSolutionComponent, ShakeEvent>(new EntityEventRefHandler<PressurizedSolutionComponent, ShakeEvent>(this.OnShake));
    this.SubscribeLocalEvent<PressurizedSolutionComponent, OpenableOpenedEvent>(new EntityEventRefHandler<PressurizedSolutionComponent, OpenableOpenedEvent>(this.OnOpened));
    this.SubscribeLocalEvent<PressurizedSolutionComponent, LandEvent>(new EntityEventRefHandler<PressurizedSolutionComponent, LandEvent>(this.OnLand));
    this.SubscribeLocalEvent<PressurizedSolutionComponent, SolutionContainerChangedEvent>(new EntityEventRefHandler<PressurizedSolutionComponent, SolutionContainerChangedEvent>(this.OnSolutionUpdate));
  }

  private bool SprayCheck(Entity<PressurizedSolutionComponent> entity, float chanceMod = 0.0f)
  {
    return this.Fizziness((Entity<PressurizedSolutionComponent>) ((EntityUid) entity, entity.Comp)) + (double) chanceMod > (double) entity.Comp.SprayFizzinessThresholdRoll;
  }

  private float SolutionFizzability(Entity<PressurizedSolutionComponent> entity)
  {
    Solution solution;
    if (!this._solutionContainer.TryGetSolution((Entity<SolutionContainerManagerComponent>) entity.Owner, entity.Comp.Solution, out Entity<SolutionComponent>? _, out solution) || solution.Volume <= 0)
      return 0.0f;
    float num1 = 0.0f;
    foreach (ReagentQuantity content in solution.Contents)
    {
      Content.Shared._RMC14.Chemistry.Reagent.Reagent reagent;
      if (this._reagents.TryIndex((ProtoId<ReagentPrototype>) content.Reagent.Prototype, out reagent) && reagent != null)
      {
        float num2 = (float) (content.Quantity / solution.Volume);
        num1 += reagent.Fizziness * num2;
      }
    }
    return num1;
  }

  private void AddFizziness(Entity<PressurizedSolutionComponent> entity, float amount)
  {
    float num = this.SolutionFizzability(entity);
    if ((double) num <= 0.0)
      return;
    AttemptAddFizzinessEvent args = new AttemptAddFizzinessEvent(entity, amount);
    this.RaiseLocalEvent<AttemptAddFizzinessEvent>((EntityUid) entity, ref args);
    if (args.Cancelled)
      return;
    amount *= num;
    TimeSpan timeSpan1 = (double) amount * entity.Comp.FizzinessMaxDuration;
    TimeSpan timeSpan2 = (entity.Comp.FizzySettleTime > this._timing.CurTime ? entity.Comp.FizzySettleTime : this._timing.CurTime) + timeSpan1;
    TimeSpan timeSpan3 = this._timing.CurTime + entity.Comp.FizzinessMaxDuration;
    if (timeSpan2 > timeSpan3)
      timeSpan2 = timeSpan3;
    entity.Comp.FizzySettleTime = timeSpan2;
    this.RollSprayThreshold(entity);
  }

  private void SprayOrAddFizziness(
    Entity<PressurizedSolutionComponent> entity,
    float chanceMod = 0.0f,
    float fizzinessToAdd = 0.0f,
    EntityUid? user = null)
  {
    if (this.SprayCheck(entity, chanceMod))
      this.TrySpray((Entity<PressurizedSolutionComponent>) ((EntityUid) entity, entity.Comp), user);
    else
      this.AddFizziness(entity, fizzinessToAdd);
  }

  private void RollSprayThreshold(Entity<PressurizedSolutionComponent> entity)
  {
    if (!this._net.IsServer)
      return;
    entity.Comp.SprayFizzinessThresholdRoll = this._random.NextFloat();
    this.Dirty((EntityUid) entity, (IComponent) entity.Comp);
  }

  public bool CanSpray(Entity<PressurizedSolutionComponent?> entity)
  {
    return this.Resolve<PressurizedSolutionComponent>((EntityUid) entity, ref entity.Comp, false) && (double) this.SolutionFizzability((Entity<PressurizedSolutionComponent>) ((EntityUid) entity, entity.Comp)) > 0.0;
  }

  public bool TrySpray(Entity<PressurizedSolutionComponent?> entity, EntityUid? target = null)
  {
    Entity<SolutionComponent>? entity1;
    Solution solution1;
    if (!this.Resolve<PressurizedSolutionComponent>((EntityUid) entity, ref entity.Comp) || !this.CanSpray(entity) || !this._solutionContainer.TryGetSolution((Entity<SolutionContainerManagerComponent>) entity.Owner, entity.Comp.Solution, out entity1, out solution1))
      return false;
    this._openable.SetOpen((EntityUid) entity);
    Solution solution2 = this._solutionContainer.SplitSolution(entity1.Value, solution1.Volume);
    TransformComponent comp;
    if (this.TryComp((EntityUid) entity, out comp))
      this._puddle.TrySplashSpillAt((EntityUid) entity, comp.Coordinates, solution2, out EntityUid _, false);
    EntityUid entityUid1 = Identity.Entity((EntityUid) entity, (IEntityManager) this.EntityManager);
    if (target.HasValue)
    {
      EntityUid entityUid2 = Identity.Entity(target.Value, (IEntityManager) this.EntityManager);
      this._popup.PopupPredicted(this.Loc.GetString((string) entity.Comp.SprayHolderMessageSelf, ("victim", (object) entityUid2), ("drink", (object) entityUid1)), this.Loc.GetString((string) entity.Comp.SprayHolderMessageOthers, ("victim", (object) entityUid2), ("drink", (object) entityUid1)), target.Value, new EntityUid?(target.Value));
    }
    else if (this._timing.IsFirstTimePredicted)
      this._popup.PopupEntity(this.Loc.GetString((string) entity.Comp.SprayGroundMessage, ("drink", (object) entityUid1)), (EntityUid) entity);
    this._audio.PlayPredicted(entity.Comp.SpraySound, (EntityUid) entity, target);
    this.TryClearFizziness(entity);
    return true;
  }

  public double Fizziness(Entity<PressurizedSolutionComponent?> entity)
  {
    return !this.Resolve<PressurizedSolutionComponent>((EntityUid) entity, ref entity.Comp, false) || entity.Comp.FizzySettleTime <= this._timing.CurTime ? 0.0 : (double) Easings.InOutCubic((float) Math.Min((entity.Comp.FizzySettleTime - this._timing.CurTime) / entity.Comp.FizzinessMaxDuration, 1.0));
  }

  public void TryClearFizziness(Entity<PressurizedSolutionComponent?> entity)
  {
    if (!this.Resolve<PressurizedSolutionComponent>((EntityUid) entity, ref entity.Comp))
      return;
    entity.Comp.FizzySettleTime = TimeSpan.Zero;
    this.RollSprayThreshold((Entity<PressurizedSolutionComponent>) ((EntityUid) entity, entity.Comp));
  }

  private void OnMapInit(Entity<PressurizedSolutionComponent> entity, ref MapInitEvent args)
  {
    this.RollSprayThreshold(entity);
  }

  private void OnOpened(Entity<PressurizedSolutionComponent> entity, ref OpenableOpenedEvent args)
  {
    EntityUid? nullable;
    int num;
    if (args.User.HasValue)
    {
      SharedHandsSystem hands = this._hands;
      nullable = args.User;
      Entity<HandsComponent> ent = (Entity<HandsComponent>) nullable.Value;
      EntityUid? entity1 = new EntityUid?((EntityUid) entity);
      string str;
      ref string local = ref str;
      num = hands.IsHolding(ent, entity1, out local) ? 1 : 0;
    }
    else
      num = 0;
    bool flag = num != 0;
    Entity<PressurizedSolutionComponent> entity2 = entity;
    double chanceModOnOpened = (double) entity.Comp.SprayChanceModOnOpened;
    EntityUid? user;
    if (!flag)
    {
      nullable = new EntityUid?();
      user = nullable;
    }
    else
      user = args.User;
    this.SprayOrAddFizziness(entity2, (float) chanceModOnOpened, -1f, user);
  }

  private void OnShake(Entity<PressurizedSolutionComponent> entity, ref ShakeEvent args)
  {
    this.SprayOrAddFizziness(entity, entity.Comp.SprayChanceModOnShake, entity.Comp.FizzinessAddedOnShake, args.Shaker);
  }

  private void OnLand(Entity<PressurizedSolutionComponent> entity, ref LandEvent args)
  {
    this.SprayOrAddFizziness(entity, entity.Comp.SprayChanceModOnLand, entity.Comp.FizzinessAddedOnLand);
  }

  private void OnSolutionUpdate(
    Entity<PressurizedSolutionComponent> entity,
    ref SolutionContainerChangedEvent args)
  {
    if (args.SolutionId != entity.Comp.Solution || (double) this.SolutionFizzability(entity) > 0.0)
      return;
    this.TryClearFizziness((Entity<PressurizedSolutionComponent>) ((EntityUid) entity, entity.Comp));
  }
}
