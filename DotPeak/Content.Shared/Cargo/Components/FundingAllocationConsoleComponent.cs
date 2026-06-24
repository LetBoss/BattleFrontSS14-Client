// Decompiled with JetBrains decompiler
// Type: Content.Shared.Cargo.Components.FundingAllocationConsoleComponent
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
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Cargo.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SharedCargoSystem)})]
public sealed class FundingAllocationConsoleComponent : 
  Component,
  ISerializationGenerated<FundingAllocationConsoleComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier SetDistributionSound = (SoundSpecifier) new SoundCollectionSpecifier("CargoPing", new AudioParams?());

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref FundingAllocationConsoleComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (FundingAllocationConsoleComponent) component;
    if (serialization.TryCustomCopy<FundingAllocationConsoleComponent>(this, ref target, hookCtx, false, context))
      return;
    SoundSpecifier soundSpecifier = (SoundSpecifier) null;
    if (this.SetDistributionSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.SetDistributionSound, ref soundSpecifier, hookCtx, true, context))
      soundSpecifier = serialization.CreateCopy<SoundSpecifier>(this.SetDistributionSound, hookCtx, context, false);
    target.SetDistributionSound = soundSpecifier;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref FundingAllocationConsoleComponent target,
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
    FundingAllocationConsoleComponent target1 = (FundingAllocationConsoleComponent) target;
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
    FundingAllocationConsoleComponent target1 = (FundingAllocationConsoleComponent) target;
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
    FundingAllocationConsoleComponent target1 = (FundingAllocationConsoleComponent) target;
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
  virtual FundingAllocationConsoleComponent Component.Instantiate()
  {
    return new FundingAllocationConsoleComponent();
  }
}
