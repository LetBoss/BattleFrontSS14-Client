// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Gulag.Components.GulagFighterComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._PUBG.Gulag.Components;

[RegisterComponent]
public sealed class GulagFighterComponent : 
  Component,
  ISerializationGenerated<GulagFighterComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public EntityUid ArenaEntity;
  [DataField(null, false, 1, false, false, null)]
  public bool IsAlive = true;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref GulagFighterComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (GulagFighterComponent) target1;
    if (serialization.TryCustomCopy<GulagFighterComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityUid target2 = new EntityUid();
    if (!serialization.TryCustomCopy<EntityUid>(this.ArenaEntity, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntityUid>(this.ArenaEntity, hookCtx, context);
    target.ArenaEntity = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.IsAlive, ref target3, hookCtx, false, context))
      target3 = this.IsAlive;
    target.IsAlive = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref GulagFighterComponent target,
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
    GulagFighterComponent target1 = (GulagFighterComponent) target;
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
    GulagFighterComponent target1 = (GulagFighterComponent) target;
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
    GulagFighterComponent target1 = (GulagFighterComponent) target;
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
  virtual GulagFighterComponent Component.Instantiate() => new GulagFighterComponent();
}
