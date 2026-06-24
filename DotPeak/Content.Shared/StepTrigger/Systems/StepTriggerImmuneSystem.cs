// Decompiled with JetBrains decompiler
// Type: Content.Shared.StepTrigger.Systems.StepTriggerImmuneSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.StepTrigger;
using Content.Shared.Examine;
using Content.Shared.Inventory;
using Content.Shared.StepTrigger.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared.StepTrigger.Systems;

public sealed class StepTriggerImmuneSystem : EntitySystem
{
  [Dependency]
  private InventorySystem _inventory;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<PreventableStepTriggerComponent, StepTriggerAttemptEvent>(new EntityEventRefHandler<PreventableStepTriggerComponent, StepTriggerAttemptEvent>(this.OnStepTriggerClothingAttempt));
    this.SubscribeLocalEvent<PreventableStepTriggerComponent, ExaminedEvent>(new ComponentEventHandler<PreventableStepTriggerComponent, ExaminedEvent>(this.OnExamined));
  }

  private void OnStepTriggerClothingAttempt(
    Entity<PreventableStepTriggerComponent> ent,
    ref StepTriggerAttemptEvent args)
  {
    if (this.HasComp<ProtectedFromStepTriggersComponent>(args.Tripper) || this._inventory.TryGetInventoryEntity<ProtectedFromStepTriggersComponent>((Entity<InventoryComponent>) args.Tripper, out Entity<ProtectedFromStepTriggersComponent> _))
      args.Cancelled = true;
    if (!this.HasComp<ImmuneToClothingRequiredStepTriggerComponent>(args.Tripper))
      return;
    args.Cancelled = true;
  }

  private void OnExamined(
    EntityUid uid,
    PreventableStepTriggerComponent component,
    ExaminedEvent args)
  {
    args.PushMarkup(this.Loc.GetString("clothing-required-step-trigger-examine"));
  }
}
