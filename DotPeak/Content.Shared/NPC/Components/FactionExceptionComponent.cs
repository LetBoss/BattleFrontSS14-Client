// Decompiled with JetBrains decompiler
// Type: Content.Shared.NPC.Components.FactionExceptionComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.NPC.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.NPC.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (NpcFactionSystem), typeof (SharedNPCImprintingOnSpawnBehaviourSystem)})]
public sealed class FactionExceptionComponent : 
  Component,
  ISerializationGenerated<FactionExceptionComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public HashSet<EntityUid> Ignored = new HashSet<EntityUid>();
  [DataField(null, false, 1, false, false, null)]
  public HashSet<EntityUid> Hostiles = new HashSet<EntityUid>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref FactionExceptionComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (FactionExceptionComponent) target1;
    if (serialization.TryCustomCopy<FactionExceptionComponent>(this, ref target, hookCtx, false, context))
      return;
    HashSet<EntityUid> target2 = (HashSet<EntityUid>) null;
    if (this.Ignored == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<EntityUid>>(this.Ignored, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<HashSet<EntityUid>>(this.Ignored, hookCtx, context);
    target.Ignored = target2;
    HashSet<EntityUid> target3 = (HashSet<EntityUid>) null;
    if (this.Hostiles == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<EntityUid>>(this.Hostiles, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<HashSet<EntityUid>>(this.Hostiles, hookCtx, context);
    target.Hostiles = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref FactionExceptionComponent target,
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
    FactionExceptionComponent target1 = (FactionExceptionComponent) target;
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
    FactionExceptionComponent target1 = (FactionExceptionComponent) target;
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
    FactionExceptionComponent target1 = (FactionExceptionComponent) target;
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
  virtual FactionExceptionComponent Component.Instantiate() => new FactionExceptionComponent();
}
