// Decompiled with JetBrains decompiler
// Type: Content.Client.Options.OptionsVisualizerComponent
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Client.Options;

[RegisterComponent]
public sealed class OptionsVisualizerComponent : 
  Component,
  ISerializationGenerated<OptionsVisualizerComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public Dictionary<string, OptionsVisualizerComponent.LayerDatum[]> Visuals;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref OptionsVisualizerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (OptionsVisualizerComponent) component;
    if (serialization.TryCustomCopy<OptionsVisualizerComponent>(this, ref target, hookCtx, false, context))
      return;
    Dictionary<string, OptionsVisualizerComponent.LayerDatum[]> dictionary = (Dictionary<string, OptionsVisualizerComponent.LayerDatum[]>) null;
    if (this.Visuals == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<string, OptionsVisualizerComponent.LayerDatum[]>>(this.Visuals, ref dictionary, hookCtx, true, context))
      dictionary = serialization.CreateCopy<Dictionary<string, OptionsVisualizerComponent.LayerDatum[]>>(this.Visuals, hookCtx, context, false);
    target.Visuals = dictionary;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref OptionsVisualizerComponent target,
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
    OptionsVisualizerComponent target1 = (OptionsVisualizerComponent) target;
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
    OptionsVisualizerComponent target1 = (OptionsVisualizerComponent) target;
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
    OptionsVisualizerComponent target1 = (OptionsVisualizerComponent) target;
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
  virtual OptionsVisualizerComponent Component.Instantiate() => new OptionsVisualizerComponent();

  [DataDefinition]
  public sealed class LayerDatum : 
    ISerializationGenerated<OptionsVisualizerComponent.LayerDatum>,
    ISerializationGenerated
  {
    [DataField(null, false, 1, false, false, null)]
    public OptionVisualizerOptions Options { get; set; }

    [DataField(null, false, 1, false, false, null)]
    public PrototypeLayerData Data { get; set; }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void InternalCopy(
      ref OptionsVisualizerComponent.LayerDatum target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      if (serialization.TryCustomCopy<OptionsVisualizerComponent.LayerDatum>(this, ref target, hookCtx, false, context))
        return;
      OptionVisualizerOptions visualizerOptions = OptionVisualizerOptions.Default;
      if (!serialization.TryCustomCopy<OptionVisualizerOptions>(this.Options, ref visualizerOptions, hookCtx, false, context))
        visualizerOptions = this.Options;
      target.Options = visualizerOptions;
      PrototypeLayerData prototypeLayerData = (PrototypeLayerData) null;
      if (this.Data == null)
        throw new NullNotAllowedException();
      if (!serialization.TryCustomCopy<PrototypeLayerData>(this.Data, ref prototypeLayerData, hookCtx, false, context))
      {
        if (this.Data == null)
          prototypeLayerData = (PrototypeLayerData) null;
        else
          serialization.CopyTo<PrototypeLayerData>(this.Data, ref prototypeLayerData, hookCtx, context, true);
      }
      target.Data = prototypeLayerData;
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void Copy(
      ref OptionsVisualizerComponent.LayerDatum target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      this.InternalCopy(ref target, serialization, hookCtx, context);
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void Copy(
      ref object target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      OptionsVisualizerComponent.LayerDatum target1 = (OptionsVisualizerComponent.LayerDatum) target;
      this.Copy(ref target1, serialization, hookCtx, context);
      target = (object) target1;
    }

    [Obsolete("Use ISerializationManager.CreateCopy instead")]
    public OptionsVisualizerComponent.LayerDatum Instantiate()
    {
      return new OptionsVisualizerComponent.LayerDatum();
    }
  }
}
