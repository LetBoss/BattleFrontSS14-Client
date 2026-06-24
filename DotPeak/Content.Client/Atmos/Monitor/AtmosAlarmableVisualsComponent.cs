// Decompiled with JetBrains decompiler
// Type: Content.Client.Atmos.Monitor.AtmosAlarmableVisualsComponent
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Atmos.Monitor;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Client.Atmos.Monitor;

[RegisterComponent]
public sealed class AtmosAlarmableVisualsComponent : 
  Component,
  ISerializationGenerated<AtmosAlarmableVisualsComponent>,
  ISerializationGenerated
{
  [DataField("alarmStates", false, 1, false, false, null)]
  public Dictionary<AtmosAlarmType, string> AlarmStates = new Dictionary<AtmosAlarmType, string>();
  [DataField("hideOnDepowered", false, 1, false, false, null)]
  public List<string>? HideOnDepowered;
  [DataField("setOnDepowered", false, 1, false, false, null)]
  public Dictionary<string, string>? SetOnDepowered;

  [DataField("layerMap", false, 1, false, false, null)]
  public string LayerMap { get; private set; } = string.Empty;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref AtmosAlarmableVisualsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (AtmosAlarmableVisualsComponent) component;
    if (serialization.TryCustomCopy<AtmosAlarmableVisualsComponent>(this, ref target, hookCtx, false, context))
      return;
    string str = (string) null;
    if (this.LayerMap == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.LayerMap, ref str, hookCtx, false, context))
      str = this.LayerMap;
    target.LayerMap = str;
    Dictionary<AtmosAlarmType, string> dictionary1 = (Dictionary<AtmosAlarmType, string>) null;
    if (this.AlarmStates == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<AtmosAlarmType, string>>(this.AlarmStates, ref dictionary1, hookCtx, true, context))
      dictionary1 = serialization.CreateCopy<Dictionary<AtmosAlarmType, string>>(this.AlarmStates, hookCtx, context, false);
    target.AlarmStates = dictionary1;
    List<string> stringList = (List<string>) null;
    if (!serialization.TryCustomCopy<List<string>>(this.HideOnDepowered, ref stringList, hookCtx, true, context))
      stringList = serialization.CreateCopy<List<string>>(this.HideOnDepowered, hookCtx, context, false);
    target.HideOnDepowered = stringList;
    Dictionary<string, string> dictionary2 = (Dictionary<string, string>) null;
    if (!serialization.TryCustomCopy<Dictionary<string, string>>(this.SetOnDepowered, ref dictionary2, hookCtx, true, context))
      dictionary2 = serialization.CreateCopy<Dictionary<string, string>>(this.SetOnDepowered, hookCtx, context, false);
    target.SetOnDepowered = dictionary2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref AtmosAlarmableVisualsComponent target,
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
    AtmosAlarmableVisualsComponent target1 = (AtmosAlarmableVisualsComponent) target;
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
    AtmosAlarmableVisualsComponent target1 = (AtmosAlarmableVisualsComponent) target;
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
    AtmosAlarmableVisualsComponent target1 = (AtmosAlarmableVisualsComponent) target;
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
  virtual AtmosAlarmableVisualsComponent Component.Instantiate()
  {
    return new AtmosAlarmableVisualsComponent();
  }
}
