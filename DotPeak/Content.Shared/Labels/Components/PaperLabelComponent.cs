// Decompiled with JetBrains decompiler
// Type: Content.Shared.Labels.Components.PaperLabelComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Containers.ItemSlots;
using Content.Shared.Labels.EntitySystems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Labels.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (LabelSystem)})]
public sealed class PaperLabelComponent : 
  Component,
  ISerializationGenerated<PaperLabelComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public ItemSlot LabelSlot = new ItemSlot();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PaperLabelComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (PaperLabelComponent) target1;
    if (serialization.TryCustomCopy<PaperLabelComponent>(this, ref target, hookCtx, false, context))
      return;
    ItemSlot target2 = (ItemSlot) null;
    if (this.LabelSlot == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<ItemSlot>(this.LabelSlot, ref target2, hookCtx, false, context))
    {
      if (this.LabelSlot == null)
        target2 = (ItemSlot) null;
      else
        serialization.CopyTo<ItemSlot>(this.LabelSlot, ref target2, hookCtx, context, true);
    }
    target.LabelSlot = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PaperLabelComponent target,
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
    PaperLabelComponent target1 = (PaperLabelComponent) target;
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
    PaperLabelComponent target1 = (PaperLabelComponent) target;
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
    PaperLabelComponent target1 = (PaperLabelComponent) target;
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
  virtual PaperLabelComponent Component.Instantiate() => new PaperLabelComponent();
}
