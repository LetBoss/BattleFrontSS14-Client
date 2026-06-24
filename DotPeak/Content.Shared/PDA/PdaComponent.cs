// Decompiled with JetBrains decompiler
// Type: Content.Shared.PDA.PdaComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Containers.ItemSlots;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.PDA;

[RegisterComponent]
[NetworkedComponent]
public sealed class PdaComponent : 
  Component,
  ISerializationGenerated<PdaComponent>,
  ISerializationGenerated
{
  public const string PdaIdSlotId = "PDA-id";
  public const string PdaPenSlotId = "PDA-pen";
  public const string PdaPaiSlotId = "PDA-pai";
  [DataField("idSlot", false, 1, false, false, null)]
  public ItemSlot IdSlot = new ItemSlot();
  [DataField("penSlot", false, 1, false, false, null)]
  public ItemSlot PenSlot = new ItemSlot();
  [DataField("paiSlot", false, 1, false, false, null)]
  public ItemSlot PaiSlot = new ItemSlot();
  [DataField("id", false, 1, false, false, typeof (PrototypeIdSerializer<EntityPrototype>))]
  public string? IdCard;
  [Robust.Shared.ViewVariables.ViewVariables]
  public EntityUid? ContainedId;
  [Robust.Shared.ViewVariables.ViewVariables]
  public bool FlashlightOn;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public string? OwnerName;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public EntityUid? PdaOwner;
  [Robust.Shared.ViewVariables.ViewVariables]
  public string? StationName;
  [Robust.Shared.ViewVariables.ViewVariables]
  public string? StationAlertLevel;
  [Robust.Shared.ViewVariables.ViewVariables]
  public Color StationAlertColor = Color.White;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PdaComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (PdaComponent) target1;
    if (serialization.TryCustomCopy<PdaComponent>(this, ref target, hookCtx, false, context))
      return;
    ItemSlot target2 = (ItemSlot) null;
    if (this.IdSlot == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<ItemSlot>(this.IdSlot, ref target2, hookCtx, false, context))
    {
      if (this.IdSlot == null)
        target2 = (ItemSlot) null;
      else
        serialization.CopyTo<ItemSlot>(this.IdSlot, ref target2, hookCtx, context, true);
    }
    target.IdSlot = target2;
    ItemSlot target3 = (ItemSlot) null;
    if (this.PenSlot == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<ItemSlot>(this.PenSlot, ref target3, hookCtx, false, context))
    {
      if (this.PenSlot == null)
        target3 = (ItemSlot) null;
      else
        serialization.CopyTo<ItemSlot>(this.PenSlot, ref target3, hookCtx, context, true);
    }
    target.PenSlot = target3;
    ItemSlot target4 = (ItemSlot) null;
    if (this.PaiSlot == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<ItemSlot>(this.PaiSlot, ref target4, hookCtx, false, context))
    {
      if (this.PaiSlot == null)
        target4 = (ItemSlot) null;
      else
        serialization.CopyTo<ItemSlot>(this.PaiSlot, ref target4, hookCtx, context, true);
    }
    target.PaiSlot = target4;
    string target5 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.IdCard, ref target5, hookCtx, false, context))
      target5 = this.IdCard;
    target.IdCard = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PdaComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    PdaComponent target1 = (PdaComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    PdaComponent target1 = (PdaComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    PdaComponent target1 = (PdaComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual PdaComponent Component.Instantiate() => new PdaComponent();
}
