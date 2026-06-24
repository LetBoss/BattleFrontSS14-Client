// Decompiled with JetBrains decompiler
// Type: Content.Shared.Kitchen.Components.KitchenSpikeComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Kitchen.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SharedKitchenSpikeSystem)})]
public sealed class KitchenSpikeComponent : 
  Component,
  ISerializationGenerated<KitchenSpikeComponent>,
  ISerializationGenerated
{
  [DataField("delay", false, 1, false, false, null)]
  public float SpikeDelay = 7f;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("sound", false, 1, false, false, null)]
  public SoundSpecifier SpikeSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Effects/Fluids/splat.ogg");
  public List<string>? PrototypesToSpawn;
  public string MeatSource1p = "?";
  public string MeatSource0 = "?";
  public string Victim = "?";
  public bool InUse;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref KitchenSpikeComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (KitchenSpikeComponent) target1;
    if (serialization.TryCustomCopy<KitchenSpikeComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SpikeDelay, ref target2, hookCtx, false, context))
      target2 = this.SpikeDelay;
    target.SpikeDelay = target2;
    SoundSpecifier target3 = (SoundSpecifier) null;
    if (this.SpikeSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.SpikeSound, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<SoundSpecifier>(this.SpikeSound, hookCtx, context);
    target.SpikeSound = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref KitchenSpikeComponent target,
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
    KitchenSpikeComponent target1 = (KitchenSpikeComponent) target;
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
    KitchenSpikeComponent target1 = (KitchenSpikeComponent) target;
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
    KitchenSpikeComponent target1 = (KitchenSpikeComponent) target;
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
  virtual KitchenSpikeComponent Component.Instantiate() => new KitchenSpikeComponent();

  [NetSerializable]
  [Serializable]
  public enum KitchenSpikeVisuals : byte
  {
    Status,
  }

  [NetSerializable]
  [Serializable]
  public enum KitchenSpikeStatus : byte
  {
    Empty,
    Bloody,
  }
}
