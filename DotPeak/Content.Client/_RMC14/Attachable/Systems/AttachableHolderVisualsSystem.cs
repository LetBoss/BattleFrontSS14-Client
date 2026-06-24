// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Attachable.Systems.AttachableHolderVisualsSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._RMC14.Attachable.Components;
using Content.Shared._RMC14.Attachable.Components;
using Content.Shared._RMC14.Attachable.Events;
using Content.Shared._RMC14.Attachable.Systems;
using Robust.Client.GameObjects;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Utility;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client._RMC14.Attachable.Systems;

public sealed class AttachableHolderVisualsSystem : EntitySystem
{
  [Dependency]
  private AttachableHolderSystem _attachableHolderSystem;
  [Dependency]
  private SpriteSystem _sprite;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AttachableHolderVisualsComponent, EntRemovedFromContainerMessage>(new EntityEventRefHandler<AttachableHolderVisualsComponent, EntRemovedFromContainerMessage>((object) this, __methodptr(OnDetached)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AttachableHolderVisualsComponent, AttachableHolderAttachablesAlteredEvent>(new EntityEventRefHandler<AttachableHolderVisualsComponent, AttachableHolderAttachablesAlteredEvent>((object) this, __methodptr(OnAttachablesAltered)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AttachableVisualsComponent, AppearanceChangeEvent>(new EntityEventRefHandler<AttachableVisualsComponent, AppearanceChangeEvent>((object) this, __methodptr(OnAttachableAppearanceChange)), (Type[]) null, (Type[]) null);
  }

  private void OnDetached(
    Entity<AttachableHolderVisualsComponent> holder,
    ref EntRemovedFromContainerMessage args)
  {
    if (!this.HasComp<AttachableVisualsComponent>(((ContainerModifiedMessage) args).Entity) || !this._attachableHolderSystem.HasSlot(Entity<AttachableHolderComponent>.op_Implicit(holder.Owner), ((ContainerModifiedMessage) args).Container.ID))
      return;
    AttachableHolderAttachablesAlteredEvent attachablesAlteredEvent = new AttachableHolderAttachablesAlteredEvent(((ContainerModifiedMessage) args).Entity, ((ContainerModifiedMessage) args).Container.ID, AttachableAlteredType.Detached);
    this.RaiseLocalEvent<AttachableHolderAttachablesAlteredEvent>(Entity<AttachableHolderVisualsComponent>.op_Implicit(holder), ref attachablesAlteredEvent, false);
  }

  private void OnAttachablesAltered(
    Entity<AttachableHolderVisualsComponent> holder,
    ref AttachableHolderAttachablesAlteredEvent args)
  {
    AttachableVisualsComponent visualsComponent;
    if (!this.TryComp<AttachableVisualsComponent>(args.Attachable, ref visualsComponent))
      return;
    string suffix = "";
    AttachableToggleableComponent toggleableComponent;
    if (visualsComponent.ShowActive && this.TryComp<AttachableToggleableComponent>(args.Attachable, ref toggleableComponent) && toggleableComponent.Active)
      suffix = "-on";
    Entity<AttachableVisualsComponent> attachable = new Entity<AttachableVisualsComponent>(args.Attachable, visualsComponent);
    AttachableAlteredType alteration = args.Alteration;
    if ((uint) alteration <= 16U /*0x10*/)
    {
      switch (alteration)
      {
        case AttachableAlteredType.Attached:
          this.SetAttachableOverlay(holder, attachable, args.SlotId, suffix);
          break;
        case AttachableAlteredType.Detached:
          this.RemoveAttachableOverlay(holder, args.SlotId);
          break;
        case AttachableAlteredType.Activated:
          if (!visualsComponent.ShowActive)
            break;
          this.SetAttachableOverlay(holder, attachable, args.SlotId, suffix);
          break;
      }
    }
    else
    {
      switch (alteration)
      {
        case AttachableAlteredType.Deactivated:
          if (!visualsComponent.ShowActive)
            break;
          this.SetAttachableOverlay(holder, attachable, args.SlotId, suffix);
          break;
        case AttachableAlteredType.Interrupted:
          if (!visualsComponent.ShowActive)
            break;
          this.SetAttachableOverlay(holder, attachable, args.SlotId);
          break;
        case AttachableAlteredType.AppearanceChanged:
          this.SetAttachableOverlay(holder, attachable, args.SlotId, suffix);
          break;
      }
    }
  }

  private void RemoveAttachableOverlay(
    Entity<AttachableHolderVisualsComponent> holder,
    string slotId)
  {
    SpriteComponent spriteComponent;
    int num;
    if (!holder.Comp.Offsets.ContainsKey(slotId) || !this.TryComp<SpriteComponent>(Entity<AttachableHolderVisualsComponent>.op_Implicit(holder), ref spriteComponent) || !this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((holder.Owner, spriteComponent)), slotId, ref num, false))
      return;
    this._sprite.LayerMapRemove(Entity<SpriteComponent>.op_Implicit((holder.Owner, spriteComponent)), slotId);
    this._sprite.RemoveLayer(Entity<SpriteComponent>.op_Implicit((holder.Owner, spriteComponent)), num, true);
  }

  private void SetAttachableOverlay(
    Entity<AttachableHolderVisualsComponent> holder,
    Entity<AttachableVisualsComponent> attachable,
    string slotId,
    string suffix = "")
  {
    this.RefreshVisuals(holder, attachable, slotId, suffix);
  }

  private void OnAttachableAppearanceChange(
    Entity<AttachableVisualsComponent> attachable,
    ref AppearanceChangeEvent args)
  {
    EntityUid? holderUid;
    string slotId;
    if (!attachable.Comp.RedrawOnAppearanceChange || !this._attachableHolderSystem.TryGetHolder(attachable.Owner, out holderUid) || !this._attachableHolderSystem.TryGetSlotId(holderUid.Value, attachable.Owner, out slotId))
      return;
    AttachableHolderAttachablesAlteredEvent attachablesAlteredEvent = new AttachableHolderAttachablesAlteredEvent(attachable.Owner, slotId, AttachableAlteredType.AppearanceChanged);
    this.RaiseLocalEvent<AttachableHolderAttachablesAlteredEvent>(holderUid.Value, ref attachablesAlteredEvent, false);
  }

  public void RefreshVisuals(
    Entity<AttachableHolderVisualsComponent> holder,
    Entity<AttachableVisualsComponent> attachable,
    string slotId,
    string suffix)
  {
    SpriteComponent spriteComponent1;
    SpriteComponent spriteComponent2;
    if (!holder.Comp.Offsets.ContainsKey(slotId) || !this.TryComp<SpriteComponent>(Entity<AttachableHolderVisualsComponent>.op_Implicit(holder), ref spriteComponent1) || !this.TryComp<SpriteComponent>(Entity<AttachableVisualsComponent>.op_Implicit(attachable), ref spriteComponent2))
      return;
    attachable.Comp.LastSlotId = slotId;
    attachable.Comp.LastSuffix = suffix;
    ResPath? nullable = this._sprite.LayerGetEffectiveRsi(Entity<SpriteComponent>.op_Implicit((attachable.Owner, spriteComponent2)), attachable.Comp.Layer)?.Path;
    string prefix = this._sprite.LayerGetRsiState(Entity<SpriteComponent>.op_Implicit((attachable.Owner, spriteComponent2)), attachable.Comp.Layer).ToString();
    ResPath? rsi = attachable.Comp.Rsi;
    if (rsi.HasValue)
      nullable = new ResPath?(rsi.GetValueOrDefault());
    if (!string.IsNullOrWhiteSpace(attachable.Comp.Prefix))
      prefix = attachable.Comp.Prefix;
    if (attachable.Comp.IncludeSlotName)
      prefix += slotId;
    if (!string.IsNullOrWhiteSpace(attachable.Comp.Suffix))
      prefix += attachable.Comp.Suffix;
    string str = prefix + suffix;
    PrototypeLayerData prototypeLayerData = new PrototypeLayerData()
    {
      RsiPath = nullable.ToString(),
      State = str,
      Offset = new Vector2?(holder.Comp.Offsets[slotId] + attachable.Comp.Offset),
      Visible = new bool?(true)
    };
    int num1;
    if (this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((holder.Owner, spriteComponent1)), slotId, ref num1, false))
    {
      this._sprite.LayerSetData(Entity<SpriteComponent>.op_Implicit((holder.Owner, spriteComponent1)), num1, prototypeLayerData);
    }
    else
    {
      int num2 = this._sprite.LayerMapReserve(Entity<SpriteComponent>.op_Implicit((holder.Owner, spriteComponent1)), slotId);
      this._sprite.LayerSetData(Entity<SpriteComponent>.op_Implicit((holder.Owner, spriteComponent1)), num2, prototypeLayerData);
    }
  }
}
