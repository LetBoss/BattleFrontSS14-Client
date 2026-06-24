// Decompiled with JetBrains decompiler
// Type: Content.Client.DamageState.DamageStateVisualsComponent
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Mobs;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Client.DamageState;

[RegisterComponent]
public sealed class DamageStateVisualsComponent : 
  Component,
  ISerializationGenerated<DamageStateVisualsComponent>,
  ISerializationGenerated
{
  public int? OriginalDrawDepth;
  [DataField("states", false, 1, false, false, null)]
  public Dictionary<MobState, Dictionary<DamageStateVisualLayers, string>> States = new Dictionary<MobState, Dictionary<DamageStateVisualLayers, string>>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DamageStateVisualsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (DamageStateVisualsComponent) component;
    if (serialization.TryCustomCopy<DamageStateVisualsComponent>(this, ref target, hookCtx, false, context))
      return;
    Dictionary<MobState, Dictionary<DamageStateVisualLayers, string>> dictionary = (Dictionary<MobState, Dictionary<DamageStateVisualLayers, string>>) null;
    if (this.States == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<MobState, Dictionary<DamageStateVisualLayers, string>>>(this.States, ref dictionary, hookCtx, true, context))
      dictionary = serialization.CreateCopy<Dictionary<MobState, Dictionary<DamageStateVisualLayers, string>>>(this.States, hookCtx, context, false);
    target.States = dictionary;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DamageStateVisualsComponent target,
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
    DamageStateVisualsComponent target1 = (DamageStateVisualsComponent) target;
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
    DamageStateVisualsComponent target1 = (DamageStateVisualsComponent) target;
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
    DamageStateVisualsComponent target1 = (DamageStateVisualsComponent) target;
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
  virtual DamageStateVisualsComponent Component.Instantiate() => new DamageStateVisualsComponent();
}
