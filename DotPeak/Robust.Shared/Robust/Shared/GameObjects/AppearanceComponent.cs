// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.AppearanceComponent
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Robust.Shared.GameObjects;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SharedAppearanceSystem)})]
public sealed class AppearanceComponent : 
  Component,
  ISerializationGenerated<AppearanceComponent>,
  ISerializationGenerated
{
  [Robust.Shared.ViewVariables.ViewVariables]
  internal bool AppearanceDirty;
  [Robust.Shared.ViewVariables.ViewVariables]
  internal bool UpdateQueued;
  [Robust.Shared.ViewVariables.ViewVariables]
  internal Dictionary<Enum, object> AppearanceData = new Dictionary<Enum, object>();
  private Dictionary<Enum, object>? _appearanceDataInit;

  [DataField(null, true, 1, false, false, null)]
  public Dictionary<Enum, object>? AppearanceDataInit
  {
    get => this._appearanceDataInit;
    set
    {
      this.AppearanceData = value ?? this.AppearanceData;
      this._appearanceDataInit = value;
    }
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref AppearanceComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (AppearanceComponent) target1;
    if (serialization.TryCustomCopy<AppearanceComponent>(this, ref target, hookCtx, false, context))
      return;
    Dictionary<Enum, object> target2 = (Dictionary<Enum, object>) null;
    if (!serialization.TryCustomCopy<Dictionary<Enum, object>>(this.AppearanceDataInit, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<Dictionary<Enum, object>>(this.AppearanceDataInit, hookCtx, context);
    target.AppearanceDataInit = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref AppearanceComponent target,
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
    AppearanceComponent target1 = (AppearanceComponent) target;
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
    AppearanceComponent target1 = (AppearanceComponent) target;
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
    AppearanceComponent target1 = (AppearanceComponent) target;
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
  virtual AppearanceComponent Component.Instantiate() => new AppearanceComponent();
}
