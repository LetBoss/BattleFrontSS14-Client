// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chemistry.EntitySystems.SharedInjectorSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Administration.Logs;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.CombatMode;
using Content.Shared.DoAfter;
using Content.Shared.FixedPoint;
using Content.Shared.Interaction.Events;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Shared.Chemistry.EntitySystems;

public abstract class SharedInjectorSystem : EntitySystem
{
  public static readonly FixedPoint2[] TransferAmounts = new FixedPoint2[4]
  {
    (FixedPoint2) 1,
    (FixedPoint2) 5,
    (FixedPoint2) 10,
    (FixedPoint2) 15
  };
  [Dependency]
  protected SharedPopupSystem Popup;
  [Dependency]
  protected SharedSolutionContainerSystem SolutionContainers;
  [Dependency]
  protected MobStateSystem MobState;
  [Dependency]
  protected SharedCombatModeSystem Combat;
  [Dependency]
  protected SharedDoAfterSystem DoAfter;
  [Dependency]
  protected ISharedAdminLogManager AdminLogger;

  public virtual void Initialize()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<InjectorComponent, GetVerbsEvent<AlternativeVerb>>(new EntityEventRefHandler<InjectorComponent, GetVerbsEvent<AlternativeVerb>>((object) this, __methodptr(AddSetTransferVerbs)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<InjectorComponent, ComponentStartup>(new EntityEventRefHandler<InjectorComponent, ComponentStartup>((object) this, __methodptr(OnInjectorStartup)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<InjectorComponent, UseInHandEvent>(new EntityEventRefHandler<InjectorComponent, UseInHandEvent>((object) this, __methodptr(OnInjectorUse)), (Type[]) null, (Type[]) null);
  }

  private void AddSetTransferVerbs(
    Entity<InjectorComponent> entity,
    ref GetVerbsEvent<AlternativeVerb> args)
  {
    if (!args.CanAccess || !args.CanInteract || args.Hands == null)
      return;
    EntityUid user = args.User;
    EntityUid entityUid;
    InjectorComponent injectorComponent;
    entity.Deconstruct(ref entityUid, ref injectorComponent);
    InjectorComponent component = injectorComponent;
    FixedPoint2 minimumTransferAmount = component.MinimumTransferAmount;
    FixedPoint2 maximumTransferAmount = component.MaximumTransferAmount;
    FixedPoint2 toggleAmount = component.TransferAmount == maximumTransferAmount ? minimumTransferAmount : maximumTransferAmount;
    int num1 = 0;
    AlternativeVerb alternativeVerb1 = new AlternativeVerb();
    alternativeVerb1.Text = this.Loc.GetString("comp-solution-transfer-verb-toggle", ("amount", (object) toggleAmount));
    alternativeVerb1.Category = VerbCategory.SetTransferAmount;
    alternativeVerb1.Act = (Action) (() =>
    {
      component.TransferAmount = toggleAmount;
      this.Popup.PopupClient(this.Loc.GetString("comp-solution-transfer-set-amount", ("amount", (object) toggleAmount)), user, new EntityUid?(user));
      this.Dirty<InjectorComponent>(entity, (MetaDataComponent) null);
    });
    alternativeVerb1.Priority = num1;
    AlternativeVerb alternativeVerb2 = alternativeVerb1;
    args.Verbs.Add(alternativeVerb2);
    int num2 = num1 - 1;
    foreach (FixedPoint2 transferAmount in SharedInjectorSystem.TransferAmounts)
    {
      FixedPoint2 amount = transferAmount;
      if (!(amount < component.MinimumTransferAmount) && !(amount > component.MaximumTransferAmount))
      {
        AlternativeVerb alternativeVerb3 = new AlternativeVerb();
        alternativeVerb3.Text = this.Loc.GetString("comp-solution-transfer-verb-amount", ("amount", (object) amount));
        alternativeVerb3.Category = VerbCategory.SetTransferAmount;
        alternativeVerb3.Act = (Action) (() =>
        {
          component.TransferAmount = amount;
          this.Popup.PopupClient(this.Loc.GetString("comp-solution-transfer-set-amount", ("amount", (object) amount)), user, new EntityUid?(user));
          this.Dirty<InjectorComponent>(entity, (MetaDataComponent) null);
        });
        alternativeVerb3.Priority = num2;
        AlternativeVerb alternativeVerb4 = alternativeVerb3;
        --num2;
        args.Verbs.Add(alternativeVerb4);
      }
    }
  }

  private void OnInjectorStartup(Entity<InjectorComponent> entity, ref ComponentStartup args)
  {
    this.Dirty<InjectorComponent>(entity, (MetaDataComponent) null);
  }

  private void OnInjectorUse(Entity<InjectorComponent> entity, ref UseInHandEvent args)
  {
    if (args.Handled)
      return;
    this.Toggle(entity, args.User);
    args.Handled = true;
  }

  private void Toggle(Entity<InjectorComponent> injector, EntityUid user)
  {
    Solution solution;
    if (injector.Comp.InjectOnly || !this.SolutionContainers.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(injector.Owner), injector.Comp.SolutionName, out Entity<SolutionComponent>? _, out solution))
      return;
    string str;
    switch (injector.Comp.ToggleState)
    {
      case InjectorToggleMode.Inject:
        if (solution.AvailableVolume > 0)
        {
          this.SetMode(injector, InjectorToggleMode.Draw);
          str = "injector-component-drawing-text";
          break;
        }
        str = "injector-component-cannot-toggle-draw-message";
        break;
      case InjectorToggleMode.Draw:
        if (solution.Volume > 0)
        {
          this.SetMode(injector, InjectorToggleMode.Inject);
          str = "injector-component-injecting-text";
          break;
        }
        str = "injector-component-cannot-toggle-inject-message";
        break;
      default:
        throw new ArgumentOutOfRangeException();
    }
    this.Popup.PopupClient(this.Loc.GetString(str), Entity<InjectorComponent>.op_Implicit(injector), new EntityUid?(user));
  }

  public void SetMode(Entity<InjectorComponent> injector, InjectorToggleMode mode)
  {
    injector.Comp.ToggleState = mode;
    this.Dirty<InjectorComponent>(injector, (MetaDataComponent) null);
  }
}
