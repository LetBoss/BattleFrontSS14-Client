// Decompiled with JetBrains decompiler
// Type: Content.Shared.Animals.UdderSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.DoAfter;
using Content.Shared.FixedPoint;
using Content.Shared.IdentityManagement;
using Content.Shared.Mobs.Systems;
using Content.Shared.Nutrition.Components;
using Content.Shared.Nutrition.EntitySystems;
using Content.Shared.Popups;
using Content.Shared.Udder;
using Content.Shared.Verbs;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared.Animals;

public sealed class UdderSystem : EntitySystem
{
  [Dependency]
  private HungerSystem _hunger;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private MobStateSystem _mobState;
  [Dependency]
  private SharedPopupSystem _popupSystem;
  [Dependency]
  private SharedDoAfterSystem _doAfterSystem;
  [Dependency]
  private SharedSolutionContainerSystem _solutionContainerSystem;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<UdderComponent, MapInitEvent>(new ComponentEventHandler<UdderComponent, MapInitEvent>((object) this, __methodptr(OnMapInit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<UdderComponent, GetVerbsEvent<AlternativeVerb>>(new EntityEventRefHandler<UdderComponent, GetVerbsEvent<AlternativeVerb>>((object) this, __methodptr(AddMilkVerb)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<UdderComponent, MilkingDoAfterEvent>(new EntityEventRefHandler<UdderComponent, MilkingDoAfterEvent>((object) this, __methodptr(OnDoAfter)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<UdderComponent, EntRemovedFromContainerMessage>(new EntityEventRefHandler<UdderComponent, EntRemovedFromContainerMessage>((object) this, __methodptr(OnEntRemoved)), (Type[]) null, (Type[]) null);
  }

  private void OnMapInit(EntityUid uid, UdderComponent component, MapInitEvent args)
  {
    component.NextGrowth = this._timing.CurTime + component.GrowthDelay;
  }

  private void OnEntRemoved(Entity<UdderComponent> entity, ref EntRemovedFromContainerMessage args)
  {
    if (!entity.Comp.Solution.HasValue || EntityUid.op_Inequality(((ContainerModifiedMessage) args).Entity, entity.Comp.Solution.Value.Owner))
      return;
    entity.Comp.Solution = new Entity<SolutionComponent>?();
  }

  public virtual void Update(float frameTime)
  {
    base.Update(frameTime);
    EntityQueryEnumerator<UdderComponent> entityQueryEnumerator = this.EntityQueryEnumerator<UdderComponent>();
    EntityUid entityUid;
    UdderComponent udderComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref udderComponent))
    {
      if (!(this._timing.CurTime < udderComponent.NextGrowth))
      {
        udderComponent.NextGrowth += udderComponent.GrowthDelay;
        Solution solution;
        if (!this._mobState.IsDead(entityUid) && this._solutionContainerSystem.ResolveSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(entityUid), udderComponent.SolutionName, ref udderComponent.Solution, out solution) && !(solution.AvailableVolume == 0))
        {
          HungerComponent component;
          if (this.TryComp<HungerComponent>(entityUid, ref component))
          {
            if (this._hunger.GetHungerThreshold(component) >= HungerThreshold.Okay)
              this._hunger.ModifyHunger(entityUid, -udderComponent.HungerUsage, component);
            else
              continue;
          }
          this._solutionContainerSystem.TryAddReagent(udderComponent.Solution.Value, ProtoId<ReagentPrototype>.op_Implicit(udderComponent.ReagentId), udderComponent.QuantityPerUpdate, out FixedPoint2 _);
        }
      }
    }
  }

  private void AttemptMilk(Entity<UdderComponent?> udder, EntityUid userUid, EntityUid containerUid)
  {
    if (!this.Resolve<UdderComponent>(Entity<UdderComponent>.op_Implicit(udder), ref udder.Comp, true))
      return;
    this._doAfterSystem.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, userUid, 5f, (DoAfterEvent) new MilkingDoAfterEvent(), new EntityUid?(Entity<UdderComponent>.op_Implicit(udder)), new EntityUid?(Entity<UdderComponent>.op_Implicit(udder)), new EntityUid?(containerUid))
    {
      BreakOnMove = true,
      BreakOnDamage = true,
      MovementThreshold = 1f
    });
  }

  private void OnDoAfter(Entity<UdderComponent> entity, ref MilkingDoAfterEvent args)
  {
    Solution solution1;
    Entity<SolutionComponent>? soln;
    Solution solution2;
    if (args.Cancelled || args.Handled || !args.Args.Used.HasValue || !this._solutionContainerSystem.ResolveSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(entity.Owner), entity.Comp.SolutionName, ref entity.Comp.Solution, out solution1) || !this._solutionContainerSystem.TryGetRefillableSolution(Entity<RefillableSolutionComponent, SolutionContainerManagerComponent>.op_Implicit(args.Args.Used.Value), out soln, out solution2))
      return;
    args.Handled = true;
    FixedPoint2 quantity = solution1.Volume;
    if (quantity == 0)
    {
      this._popupSystem.PopupClient(this.Loc.GetString("udder-system-dry"), entity.Owner, new EntityUid?(args.Args.User));
    }
    else
    {
      if (quantity > solution2.AvailableVolume)
        quantity = solution2.AvailableVolume;
      Solution toAdd = this._solutionContainerSystem.SplitSolution(entity.Comp.Solution.Value, quantity);
      this._solutionContainerSystem.TryAddSolution(soln.Value, toAdd);
      this._popupSystem.PopupClient(this.Loc.GetString("udder-system-success", ("amount", (object) quantity), ("target", (object) Identity.Entity(args.Args.Used.Value, (IEntityManager) this.EntityManager))), entity.Owner, new EntityUid?(args.Args.User), PopupType.Medium);
    }
  }

  private void AddMilkVerb(Entity<UdderComponent> entity, ref GetVerbsEvent<AlternativeVerb> args)
  {
    if (!args.Using.HasValue || !args.CanInteract || !this.HasComp<RefillableSolutionComponent>(args.Using.Value))
      return;
    EntityUid uid = entity.Owner;
    EntityUid user = args.User;
    EntityUid used = args.Using.Value;
    AlternativeVerb alternativeVerb1 = new AlternativeVerb();
    alternativeVerb1.Act = (Action) (() => this.AttemptMilk(Entity<UdderComponent>.op_Implicit(uid), user, used));
    alternativeVerb1.Text = this.Loc.GetString("udder-system-verb-milk");
    alternativeVerb1.Priority = 2;
    AlternativeVerb alternativeVerb2 = alternativeVerb1;
    args.Verbs.Add(alternativeVerb2);
  }
}
