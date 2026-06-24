// Decompiled with JetBrains decompiler
// Type: Content.Client.PowerCell.PowerChargerVisualsComponent
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Power;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Client.PowerCell;

[RegisterComponent]
[Access(new Type[] {typeof (PowerChargerVisualizerSystem)})]
public sealed class PowerChargerVisualsComponent : 
  Component,
  ISerializationGenerated<PowerChargerVisualsComponent>,
  ISerializationGenerated
{
  [DataField("emptyState", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public string EmptyState = "empty";
  [DataField("occupiedState", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public string OccupiedState = "full";
  [DataField("lightStates", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public Dictionary<CellChargerStatus, string> LightStates = new Dictionary<CellChargerStatus, string>()
  {
    [CellChargerStatus.Off] = "light-off",
    [CellChargerStatus.Empty] = "light-empty",
    [CellChargerStatus.Charging] = "light-charging",
    [CellChargerStatus.Charged] = "light-charged"
  };

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PowerChargerVisualsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (PowerChargerVisualsComponent) component;
    if (serialization.TryCustomCopy<PowerChargerVisualsComponent>(this, ref target, hookCtx, false, context))
      return;
    string str1 = (string) null;
    if (this.EmptyState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.EmptyState, ref str1, hookCtx, false, context))
      str1 = this.EmptyState;
    target.EmptyState = str1;
    string str2 = (string) null;
    if (this.OccupiedState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.OccupiedState, ref str2, hookCtx, false, context))
      str2 = this.OccupiedState;
    target.OccupiedState = str2;
    Dictionary<CellChargerStatus, string> dictionary = (Dictionary<CellChargerStatus, string>) null;
    if (this.LightStates == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<CellChargerStatus, string>>(this.LightStates, ref dictionary, hookCtx, true, context))
      dictionary = serialization.CreateCopy<Dictionary<CellChargerStatus, string>>(this.LightStates, hookCtx, context, false);
    target.LightStates = dictionary;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PowerChargerVisualsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    PowerChargerVisualsComponent target1 = (PowerChargerVisualsComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    PowerChargerVisualsComponent target1 = (PowerChargerVisualsComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    PowerChargerVisualsComponent target1 = (PowerChargerVisualsComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    base.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual PowerChargerVisualsComponent Component.Instantiate()
  {
    return new PowerChargerVisualsComponent();
  }
}
