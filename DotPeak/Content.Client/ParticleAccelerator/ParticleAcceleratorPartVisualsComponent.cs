// Decompiled with JetBrains decompiler
// Type: Content.Client.ParticleAccelerator.ParticleAcceleratorPartVisualsComponent
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Singularity.Components;
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
namespace Content.Client.ParticleAccelerator;

[RegisterComponent]
[Access(new Type[] {typeof (ParticleAcceleratorPartVisualizerSystem)})]
public sealed class ParticleAcceleratorPartVisualsComponent : 
  Component,
  ISerializationGenerated<ParticleAcceleratorPartVisualsComponent>,
  ISerializationGenerated
{
  [DataField("stateBase", false, 1, true, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public string StateBase;
  [DataField("stateSuffixes", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public Dictionary<ParticleAcceleratorVisualState, string> StatesSuffixes = new Dictionary<ParticleAcceleratorVisualState, string>()
  {
    {
      ParticleAcceleratorVisualState.Powered,
      "p"
    },
    {
      ParticleAcceleratorVisualState.Level0,
      "p0"
    },
    {
      ParticleAcceleratorVisualState.Level1,
      "p1"
    },
    {
      ParticleAcceleratorVisualState.Level2,
      "p2"
    },
    {
      ParticleAcceleratorVisualState.Level3,
      "p3"
    }
  };

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ParticleAcceleratorPartVisualsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (ParticleAcceleratorPartVisualsComponent) component;
    if (serialization.TryCustomCopy<ParticleAcceleratorPartVisualsComponent>(this, ref target, hookCtx, false, context))
      return;
    string str = (string) null;
    if (this.StateBase == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.StateBase, ref str, hookCtx, false, context))
      str = this.StateBase;
    target.StateBase = str;
    Dictionary<ParticleAcceleratorVisualState, string> dictionary = (Dictionary<ParticleAcceleratorVisualState, string>) null;
    if (this.StatesSuffixes == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<ParticleAcceleratorVisualState, string>>(this.StatesSuffixes, ref dictionary, hookCtx, true, context))
      dictionary = serialization.CreateCopy<Dictionary<ParticleAcceleratorVisualState, string>>(this.StatesSuffixes, hookCtx, context, false);
    target.StatesSuffixes = dictionary;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ParticleAcceleratorPartVisualsComponent target,
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
    ParticleAcceleratorPartVisualsComponent target1 = (ParticleAcceleratorPartVisualsComponent) target;
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
    ParticleAcceleratorPartVisualsComponent target1 = (ParticleAcceleratorPartVisualsComponent) target;
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
    ParticleAcceleratorPartVisualsComponent target1 = (ParticleAcceleratorPartVisualsComponent) target;
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
  virtual ParticleAcceleratorPartVisualsComponent Component.Instantiate()
  {
    return new ParticleAcceleratorPartVisualsComponent();
  }
}
