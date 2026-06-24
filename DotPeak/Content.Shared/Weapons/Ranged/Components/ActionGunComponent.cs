// Decompiled with JetBrains decompiler
// Type: Content.Shared.Weapons.Ranged.Components.ActionGunComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Weapons.Ranged.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (ActionGunSystem)})]
public sealed class ActionGunComponent : 
  Component,
  ISerializationGenerated<ActionGunComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public EntProtoId Action = (EntProtoId) string.Empty;
  [DataField(null, false, 1, false, false, null)]
  public EntityUid? ActionEntity;
  [DataField(null, false, 1, true, false, null)]
  public EntProtoId GunProto = (EntProtoId) string.Empty;
  [DataField(null, false, 1, false, false, null)]
  public EntityUid? Gun;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ActionGunComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ActionGunComponent) target1;
    if (serialization.TryCustomCopy<ActionGunComponent>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId target2 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Action, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntProtoId>(this.Action, hookCtx, context);
    target.Action = target2;
    EntityUid? target3 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.ActionEntity, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntityUid?>(this.ActionEntity, hookCtx, context);
    target.ActionEntity = target3;
    EntProtoId target4 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.GunProto, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntProtoId>(this.GunProto, hookCtx, context);
    target.GunProto = target4;
    EntityUid? target5 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Gun, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<EntityUid?>(this.Gun, hookCtx, context);
    target.Gun = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ActionGunComponent target,
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
    ActionGunComponent target1 = (ActionGunComponent) target;
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
    ActionGunComponent target1 = (ActionGunComponent) target;
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
    ActionGunComponent target1 = (ActionGunComponent) target;
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
  virtual ActionGunComponent Component.Instantiate() => new ActionGunComponent();
}
