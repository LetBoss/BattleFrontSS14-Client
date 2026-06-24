// Decompiled with JetBrains decompiler
// Type: Content.Shared.PDA.SharedPdaSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Access.Components;
using Content.Shared.Containers.ItemSlots;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Shared.PDA;

public abstract class SharedPdaSystem : EntitySystem
{
  [Dependency]
  protected ItemSlotsSystem ItemSlotsSystem;
  [Dependency]
  protected SharedAppearanceSystem Appearance;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<PdaComponent, ComponentInit>(new ComponentEventHandler<PdaComponent, ComponentInit>(this.OnComponentInit));
    this.SubscribeLocalEvent<PdaComponent, ComponentRemove>(new ComponentEventHandler<PdaComponent, ComponentRemove>(this.OnComponentRemove));
    this.SubscribeLocalEvent<PdaComponent, EntInsertedIntoContainerMessage>(new ComponentEventHandler<PdaComponent, EntInsertedIntoContainerMessage>(this.OnItemInserted));
    this.SubscribeLocalEvent<PdaComponent, EntRemovedFromContainerMessage>(new ComponentEventHandler<PdaComponent, EntRemovedFromContainerMessage>(this.OnItemRemoved));
    this.SubscribeLocalEvent<PdaComponent, GetAdditionalAccessEvent>(new ComponentEventRefHandler<PdaComponent, GetAdditionalAccessEvent>(this.OnGetAdditionalAccess));
  }

  protected virtual void OnComponentInit(EntityUid uid, PdaComponent pda, ComponentInit args)
  {
    if (pda.IdCard != null)
      pda.IdSlot.StartingItem = pda.IdCard;
    this.ItemSlotsSystem.AddItemSlot(uid, "PDA-id", pda.IdSlot);
    this.ItemSlotsSystem.AddItemSlot(uid, "PDA-pen", pda.PenSlot);
    this.ItemSlotsSystem.AddItemSlot(uid, "PDA-pai", pda.PaiSlot);
    this.UpdatePdaAppearance(uid, pda);
  }

  private void OnComponentRemove(EntityUid uid, PdaComponent pda, ComponentRemove args)
  {
    this.ItemSlotsSystem.RemoveItemSlot(uid, pda.IdSlot);
    this.ItemSlotsSystem.RemoveItemSlot(uid, pda.PenSlot);
    this.ItemSlotsSystem.RemoveItemSlot(uid, pda.PaiSlot);
  }

  protected virtual void OnItemInserted(
    EntityUid uid,
    PdaComponent pda,
    EntInsertedIntoContainerMessage args)
  {
    if (args.Container.ID == "PDA-id")
      pda.ContainedId = new EntityUid?(args.Entity);
    this.UpdatePdaAppearance(uid, pda);
  }

  protected virtual void OnItemRemoved(
    EntityUid uid,
    PdaComponent pda,
    EntRemovedFromContainerMessage args)
  {
    if (args.Container.ID == pda.IdSlot.ID)
      pda.ContainedId = new EntityUid?();
    this.UpdatePdaAppearance(uid, pda);
  }

  private void OnGetAdditionalAccess(
    EntityUid uid,
    PdaComponent component,
    ref GetAdditionalAccessEvent args)
  {
    EntityUid? containedId = component.ContainedId;
    if (!containedId.HasValue)
      return;
    EntityUid valueOrDefault = containedId.GetValueOrDefault();
    args.Entities.Add(valueOrDefault);
  }

  private void UpdatePdaAppearance(EntityUid uid, PdaComponent pda)
  {
    this.Appearance.SetData(uid, (Enum) PdaVisuals.IdCardInserted, (object) pda.ContainedId.HasValue);
  }

  public virtual void UpdatePdaUi(EntityUid uid, PdaComponent? pda = null)
  {
  }
}
