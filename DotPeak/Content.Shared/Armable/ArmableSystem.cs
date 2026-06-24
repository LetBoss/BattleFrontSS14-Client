// Decompiled with JetBrains decompiler
// Type: Content.Shared.Armable.ArmableSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Examine;
using Content.Shared.Item.ItemToggle;
using Content.Shared.Item.ItemToggle.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using System;

#nullable enable
namespace Content.Shared.Armable;

public sealed class ArmableSystem : EntitySystem
{
  [Dependency]
  private ItemToggleSystem _itemToggle;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ArmableComponent, ExaminedEvent>(new ComponentEventHandler<ArmableComponent, ExaminedEvent>((object) this, __methodptr(OnExamine)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ArmableComponent, ItemToggledEvent>(new EntityEventRefHandler<ArmableComponent, ItemToggledEvent>((object) this, __methodptr(ArmingDone)), (Type[]) null, (Type[]) null);
  }

  private void OnExamine(EntityUid uid, ArmableComponent comp, ExaminedEvent args)
  {
    ItemToggleComponent itemToggleComponent;
    if (!args.IsInDetailsRange || !comp.ShowStatusOnExamination || !this.TryComp<ItemToggleComponent>(uid, ref itemToggleComponent))
      return;
    if (itemToggleComponent.Activated)
    {
      LocId? examineTextArmed1 = comp.ExamineTextArmed;
      if (string.IsNullOrEmpty(examineTextArmed1.HasValue ? LocId.op_Implicit(examineTextArmed1.GetValueOrDefault()) : (string) null))
        return;
      ExaminedEvent examinedEvent = args;
      ILocalizationManager loc = this.Loc;
      LocId? examineTextArmed2 = comp.ExamineTextArmed;
      string str = examineTextArmed2.HasValue ? LocId.op_Implicit(examineTextArmed2.GetValueOrDefault()) : (string) null;
      (string, object) valueTuple = ("name", (object) uid);
      string markup = loc.GetString(str, valueTuple);
      examinedEvent.PushMarkup(markup);
    }
    else
    {
      LocId? examineTextNotArmed1 = comp.ExamineTextNotArmed;
      if (string.IsNullOrEmpty(examineTextNotArmed1.HasValue ? LocId.op_Implicit(examineTextNotArmed1.GetValueOrDefault()) : (string) null))
        return;
      ExaminedEvent examinedEvent = args;
      ILocalizationManager loc = this.Loc;
      LocId? examineTextNotArmed2 = comp.ExamineTextNotArmed;
      string str = examineTextNotArmed2.HasValue ? LocId.op_Implicit(examineTextNotArmed2.GetValueOrDefault()) : (string) null;
      (string, object) valueTuple = ("name", (object) uid);
      string markup = loc.GetString(str, valueTuple);
      examinedEvent.PushMarkup(markup);
    }
  }

  private void ArmingDone(Entity<ArmableComponent> entity, ref ItemToggledEvent args)
  {
    if (!args.Activated)
      return;
    this._itemToggle.SetOnActivate(Entity<ItemToggleComponent>.op_Implicit(entity.Owner), false);
  }
}
