// Decompiled with JetBrains decompiler
// Type: Content.Shared.ParcelWrap.Components.WrappedParcelComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.ParcelWrap.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.ParcelWrap.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (ParcelWrappingSystem)})]
public sealed class WrappedParcelComponent : 
  Component,
  ISerializationGenerated<WrappedParcelComponent>,
  ISerializationGenerated
{
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadOnly)]
  public ContainerSlot Contents;
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId? UnwrapTrash;
  [DataField(null, false, 1, true, false, null)]
  public TimeSpan UnwrapDelay = TimeSpan.FromSeconds(1L);
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? UnwrapSound;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadOnly)]
  public string ContainerId = "contents";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref WrappedParcelComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (WrappedParcelComponent) target1;
    if (serialization.TryCustomCopy<WrappedParcelComponent>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId? target2 = new EntProtoId?();
    if (!serialization.TryCustomCopy<EntProtoId?>(this.UnwrapTrash, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntProtoId?>(this.UnwrapTrash, hookCtx, context);
    target.UnwrapTrash = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.UnwrapDelay, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.UnwrapDelay, hookCtx, context);
    target.UnwrapDelay = target3;
    SoundSpecifier target4 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.UnwrapSound, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<SoundSpecifier>(this.UnwrapSound, hookCtx, context);
    target.UnwrapSound = target4;
    string target5 = (string) null;
    if (this.ContainerId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ContainerId, ref target5, hookCtx, false, context))
      target5 = this.ContainerId;
    target.ContainerId = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref WrappedParcelComponent target,
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
    WrappedParcelComponent target1 = (WrappedParcelComponent) target;
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
    WrappedParcelComponent target1 = (WrappedParcelComponent) target;
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
    WrappedParcelComponent target1 = (WrappedParcelComponent) target;
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
  virtual WrappedParcelComponent Component.Instantiate() => new WrappedParcelComponent();
}
