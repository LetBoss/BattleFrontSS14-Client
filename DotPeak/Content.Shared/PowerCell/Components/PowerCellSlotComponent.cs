// Decompiled with JetBrains decompiler
// Type: Content.Shared.PowerCell.Components.PowerCellSlotComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.PowerCell.Components;

[RegisterComponent]
public sealed class PowerCellSlotComponent : 
  Component,
  ISerializationGenerated<PowerCellSlotComponent>,
  ISerializationGenerated
{
  [DataField("cellSlotId", false, 1, true, false, null)]
  public string CellSlotId = string.Empty;
  [DataField("fitsInCharger", false, 1, false, false, null)]
  public bool FitsInCharger = true;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PowerCellSlotComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (PowerCellSlotComponent) target1;
    if (serialization.TryCustomCopy<PowerCellSlotComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.CellSlotId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.CellSlotId, ref target2, hookCtx, false, context))
      target2 = this.CellSlotId;
    target.CellSlotId = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.FitsInCharger, ref target3, hookCtx, false, context))
      target3 = this.FitsInCharger;
    target.FitsInCharger = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PowerCellSlotComponent target,
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
    PowerCellSlotComponent target1 = (PowerCellSlotComponent) target;
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
    PowerCellSlotComponent target1 = (PowerCellSlotComponent) target;
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
    PowerCellSlotComponent target1 = (PowerCellSlotComponent) target;
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
  virtual PowerCellSlotComponent Component.Instantiate() => new PowerCellSlotComponent();
}
