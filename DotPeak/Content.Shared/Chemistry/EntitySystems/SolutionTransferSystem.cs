// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chemistry.EntitySystems.SolutionTransferSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Administration.Logs;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Database;
using Content.Shared.FixedPoint;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Chemistry.EntitySystems;

public sealed class SolutionTransferSystem : EntitySystem
{
  [Dependency]
  private ISharedAdminLogManager _adminLogger;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedSolutionContainerSystem _solution;
  [Dependency]
  private SharedUserInterfaceSystem _ui;
  public static readonly FixedPoint2[] DefaultTransferAmounts = new FixedPoint2[9]
  {
    (FixedPoint2) 1,
    (FixedPoint2) 5,
    (FixedPoint2) 10,
    (FixedPoint2) 25,
    (FixedPoint2) 50,
    (FixedPoint2) 100,
    (FixedPoint2) 250,
    (FixedPoint2) 500,
    (FixedPoint2) 1000
  };

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<SolutionTransferComponent, GetVerbsEvent<AlternativeVerb>>(new EntityEventRefHandler<SolutionTransferComponent, GetVerbsEvent<AlternativeVerb>>((object) this, __methodptr(AddSetTransferVerbs)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<SolutionTransferComponent, AfterInteractEvent>(new EntityEventRefHandler<SolutionTransferComponent, AfterInteractEvent>((object) this, __methodptr(OnAfterInteract)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<SolutionTransferComponent, TransferAmountSetValueMessage>(new EntityEventRefHandler<SolutionTransferComponent, TransferAmountSetValueMessage>((object) this, __methodptr(OnTransferAmountSetValueMessage)), (Type[]) null, (Type[]) null);
  }

  private void OnTransferAmountSetValueMessage(
    Entity<SolutionTransferComponent> ent,
    ref TransferAmountSetValueMessage message)
  {
    EntityUid entityUid;
    SolutionTransferComponent transferComponent1;
    ent.Deconstruct(ref entityUid, ref transferComponent1);
    EntityUid uid = entityUid;
    SolutionTransferComponent transferComponent2 = transferComponent1;
    FixedPoint2 fixedPoint2 = FixedPoint2.Clamp(message.Value, transferComponent2.MinimumTransferAmount, transferComponent2.MaximumTransferAmount);
    transferComponent2.TransferAmount = fixedPoint2;
    EntityUid actor = ((BaseBoundUserInterfaceEvent) message).Actor;
    if (((EntityUid) ref actor).Valid)
      this._popup.PopupEntity(this.Loc.GetString("comp-solution-transfer-set-amount", ("amount", (object) fixedPoint2)), uid, actor);
    this.Dirty(uid, (IComponent) transferComponent2, (MetaDataComponent) null);
  }

  private void AddSetTransferVerbs(
    Entity<SolutionTransferComponent> ent,
    ref GetVerbsEvent<AlternativeVerb> args)
  {
    EntityUid entityUid;
    SolutionTransferComponent transferComponent;
    ent.Deconstruct(ref entityUid, ref transferComponent);
    EntityUid uid = entityUid;
    SolutionTransferComponent comp = transferComponent;
    if (!args.CanAccess || !args.CanInteract || !comp.CanChangeTransferAmount || args.Hands == null)
      return;
    GetVerbsEvent<AlternativeVerb> @event = args;
    SortedSet<AlternativeVerb> verbs = args.Verbs;
    AlternativeVerb alternativeVerb1 = new AlternativeVerb();
    alternativeVerb1.Text = this.Loc.GetString("comp-solution-transfer-verb-custom-amount");
    alternativeVerb1.Category = VerbCategory.SetTransferAmount;
    alternativeVerb1.Act = (Action) (() => this._ui.OpenUi(Entity<UserInterfaceComponent>.op_Implicit(uid), (Enum) TransferAmountUiKey.Key, new EntityUid?(@event.User), false));
    alternativeVerb1.Priority = 1;
    verbs.Add(alternativeVerb1);
    int num = 0;
    EntityUid user = args.User;
    foreach (FixedPoint2 fixedPoint2 in ent.Comp.TransferAmounts ?? SolutionTransferSystem.DefaultTransferAmounts)
    {
      FixedPoint2 amount = fixedPoint2;
      if (!(amount < comp.MinimumTransferAmount) && !(amount > comp.MaximumTransferAmount))
      {
        AlternativeVerb alternativeVerb2 = new AlternativeVerb();
        alternativeVerb2.Text = this.Loc.GetString("comp-solution-transfer-verb-amount", ("amount", (object) amount));
        alternativeVerb2.Category = VerbCategory.SetTransferAmount;
        alternativeVerb2.Act = (Action) (() =>
        {
          comp.TransferAmount = amount;
          this._popup.PopupClient(this.Loc.GetString("comp-solution-transfer-set-amount", ("amount", (object) amount)), uid, new EntityUid?(user));
          this.Dirty(uid, (IComponent) comp, (MetaDataComponent) null);
        });
        alternativeVerb2.Priority = num;
        --num;
        args.Verbs.Add(alternativeVerb2);
      }
    }
  }

  private void OnAfterInteract(Entity<SolutionTransferComponent> ent, ref AfterInteractEvent args)
  {
    if (!args.CanReach)
      return;
    EntityUid? target = args.Target;
    if (!target.HasValue)
      return;
    EntityUid valueOrDefault1 = target.GetValueOrDefault();
    EntityUid entityUid1;
    SolutionTransferComponent transferComponent1;
    ent.Deconstruct(ref entityUid1, ref transferComponent1);
    EntityUid entityUid2 = entityUid1;
    SolutionTransferComponent transferComponent2 = transferComponent1;
    Entity<SolutionComponent>? soln1;
    Solution solution1;
    RefillableSolutionComponent solutionComponent1;
    Entity<SolutionComponent>? soln2;
    Solution solution2;
    if (transferComponent2.CanReceive && !this.HasComp<RefillableSolutionComponent>(valueOrDefault1) && this._solution.TryGetDrainableSolution(Entity<DrainableSolutionComponent, SolutionContainerManagerComponent>.op_Implicit(valueOrDefault1), out soln1, out solution1) && this.TryComp<RefillableSolutionComponent>(entityUid2, ref solutionComponent1) && this._solution.TryGetRefillableSolution(Entity<RefillableSolutionComponent, SolutionContainerManagerComponent>.op_Implicit((entityUid2, solutionComponent1, (SolutionContainerManagerComponent) null)), out soln2, out solution2))
    {
      FixedPoint2 fixedPoint2_1 = transferComponent2.TransferAmount;
      FixedPoint2? maxRefill = (FixedPoint2?) solutionComponent1?.MaxRefill;
      if (maxRefill.HasValue)
      {
        FixedPoint2 valueOrDefault2 = maxRefill.GetValueOrDefault();
        fixedPoint2_1 = FixedPoint2.Min(fixedPoint2_1, valueOrDefault2);
      }
      FixedPoint2 fixedPoint2_2 = this.Transfer(new EntityUid?(args.User), valueOrDefault1, soln1.Value, entityUid2, soln2.Value, fixedPoint2_1);
      args.Handled = true;
      if (fixedPoint2_2 > 0)
      {
        this._popup.PopupClient(this.Loc.GetString(solution2.AvailableVolume == 0 ? "comp-solution-transfer-fill-fully" : "comp-solution-transfer-fill-normal", new (string, object)[3]
        {
          ("owner", (object) args.Target),
          ("amount", (object) fixedPoint2_2),
          ("target", (object) entityUid2)
        }), entityUid2, new EntityUid?(args.User));
        return;
      }
    }
    RefillableSolutionComponent solutionComponent2;
    if (!transferComponent2.CanSend || !this.TryComp<RefillableSolutionComponent>(valueOrDefault1, ref solutionComponent2) || !this._solution.TryGetRefillableSolution(Entity<RefillableSolutionComponent, SolutionContainerManagerComponent>.op_Implicit((valueOrDefault1, solutionComponent2, (SolutionContainerManagerComponent) null)), out soln1, out solution1) || !this._solution.TryGetDrainableSolution(Entity<DrainableSolutionComponent, SolutionContainerManagerComponent>.op_Implicit(entityUid2), out soln2, out solution1))
      return;
    FixedPoint2 fixedPoint2_3 = transferComponent2.TransferAmount;
    FixedPoint2? maxRefill1 = (FixedPoint2?) solutionComponent2?.MaxRefill;
    if (maxRefill1.HasValue)
    {
      FixedPoint2 valueOrDefault3 = maxRefill1.GetValueOrDefault();
      fixedPoint2_3 = FixedPoint2.Min(fixedPoint2_3, valueOrDefault3);
    }
    FixedPoint2 fixedPoint2_4 = this.Transfer(new EntityUid?(args.User), entityUid2, soln2.Value, valueOrDefault1, soln1.Value, fixedPoint2_3);
    args.Handled = true;
    if (!(fixedPoint2_4 > 0))
      return;
    this._popup.PopupClient(this.Loc.GetString("comp-solution-transfer-transfer-solution", ("amount", (object) fixedPoint2_4), ("target", (object) valueOrDefault1)), entityUid2, new EntityUid?(args.User));
  }

  public FixedPoint2 Transfer(
    EntityUid? user,
    EntityUid sourceEntity,
    Entity<SolutionComponent> source,
    EntityUid targetEntity,
    Entity<SolutionComponent> target,
    FixedPoint2 amount)
  {
    SolutionTransferAttemptEvent transferAttemptEvent = new SolutionTransferAttemptEvent(sourceEntity, source, targetEntity, target);
    this.RaiseLocalEvent<SolutionTransferAttemptEvent>(sourceEntity, ref transferAttemptEvent, false);
    string cancelReason1 = transferAttemptEvent.CancelReason;
    if (cancelReason1 != null)
    {
      this._popup.PopupClient(cancelReason1, sourceEntity, user);
      return FixedPoint2.Zero;
    }
    Solution solution1 = source.Comp.Solution;
    if (solution1.Volume == 0)
    {
      this._popup.PopupClient(this.Loc.GetString("comp-solution-transfer-is-empty", (nameof (target), (object) sourceEntity)), sourceEntity, user);
      return FixedPoint2.Zero;
    }
    this.RaiseLocalEvent<SolutionTransferAttemptEvent>(targetEntity, ref transferAttemptEvent, false);
    string cancelReason2 = transferAttemptEvent.CancelReason;
    if (cancelReason2 != null)
    {
      this._popup.PopupClient(cancelReason2, targetEntity, user);
      return FixedPoint2.Zero;
    }
    Solution solution2 = target.Comp.Solution;
    if (solution2.AvailableVolume == 0)
    {
      this._popup.PopupClient(this.Loc.GetString("comp-solution-transfer-is-full", (nameof (target), (object) targetEntity)), targetEntity, user);
      return FixedPoint2.Zero;
    }
    FixedPoint2 fixedPoint2 = FixedPoint2.Min(amount, FixedPoint2.Min(solution1.Volume, solution2.AvailableVolume));
    Solution solution3 = this._solution.SplitSolution(source, fixedPoint2);
    this._solution.AddSolution(target, solution3);
    SolutionTransferredEvent transferredEvent = new SolutionTransferredEvent(sourceEntity, targetEntity, user, fixedPoint2);
    this.RaiseLocalEvent<SolutionTransferredEvent>(targetEntity, ref transferredEvent, false);
    ISharedAdminLogManager adminLogger = this._adminLogger;
    LogStringHandler logStringHandler = new LogStringHandler(38, 4);
    logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(user, (MetaDataComponent) null), "player", "ToPrettyString(user)");
    logStringHandler.AppendLiteral(" transferred ");
    logStringHandler.AppendFormatted(SharedSolutionContainerSystem.ToPrettyString(solution3));
    logStringHandler.AppendLiteral(" to ");
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(targetEntity)), nameof (target), "ToPrettyString(targetEntity)");
    logStringHandler.AppendLiteral(", which now contains ");
    logStringHandler.AppendFormatted(SharedSolutionContainerSystem.ToPrettyString(solution2));
    ref LogStringHandler local = ref logStringHandler;
    adminLogger.Add(LogType.Action, LogImpact.Medium, ref local);
    return fixedPoint2;
  }
}
