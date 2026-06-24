// Decompiled with JetBrains decompiler
// Type: Content.Shared.NukeOps.NukeOperativeComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.StatusIcon;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.NukeOps;

[RegisterComponent]
[NetworkedComponent]
public sealed class NukeOperativeComponent : 
  Component,
  ISerializationGenerated<NukeOperativeComponent>,
  ISerializationGenerated
{
  [DataField("syndStatusIcon", false, 1, false, false, typeof (PrototypeIdSerializer<FactionIconPrototype>))]
  public string SyndStatusIcon = "SyndicateFaction";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref NukeOperativeComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (NukeOperativeComponent) target1;
    if (serialization.TryCustomCopy<NukeOperativeComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.SyndStatusIcon == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.SyndStatusIcon, ref target2, hookCtx, false, context))
      target2 = this.SyndStatusIcon;
    target.SyndStatusIcon = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref NukeOperativeComponent target,
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
    NukeOperativeComponent target1 = (NukeOperativeComponent) target;
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
    NukeOperativeComponent target1 = (NukeOperativeComponent) target;
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
    NukeOperativeComponent target1 = (NukeOperativeComponent) target;
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
  virtual NukeOperativeComponent Component.Instantiate() => new NukeOperativeComponent();
}
