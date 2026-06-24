// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Gulag.Components.GulagArenaComponent
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
public sealed class GulagArenaComponent : 
  Component,
  ISerializationGenerated<GulagArenaComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public GulagArenaState State;
  [DataField(null, false, 1, false, false, null)]
  public EntityUid? Fighter1;
  [DataField(null, false, 1, false, false, null)]
  public EntityUid? Fighter2;
  [DataField(null, false, 1, false, false, null)]
  public EntityUid? NPCInterference;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan? FightStartTime;
  [DataField(null, false, 1, false, false, null)]
  public float FightDuration = 20f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref GulagArenaComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (GulagArenaComponent) target1;
    if (serialization.TryCustomCopy<GulagArenaComponent>(this, ref target, hookCtx, false, context))
      return;
    GulagArenaState target2 = GulagArenaState.Waiting;
    if (!serialization.TryCustomCopy<GulagArenaState>(this.State, ref target2, hookCtx, false, context))
      target2 = this.State;
    target.State = target2;
    EntityUid? target3 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Fighter1, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntityUid?>(this.Fighter1, hookCtx, context);
    target.Fighter1 = target3;
    EntityUid? target4 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Fighter2, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntityUid?>(this.Fighter2, hookCtx, context);
    target.Fighter2 = target4;
    EntityUid? target5 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.NPCInterference, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<EntityUid?>(this.NPCInterference, hookCtx, context);
    target.NPCInterference = target5;
    TimeSpan? target6 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.FightStartTime, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan?>(this.FightStartTime, hookCtx, context);
    target.FightStartTime = target6;
    float target7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.FightDuration, ref target7, hookCtx, false, context))
      target7 = this.FightDuration;
    target.FightDuration = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref GulagArenaComponent target,
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
    GulagArenaComponent target1 = (GulagArenaComponent) target;
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
    GulagArenaComponent target1 = (GulagArenaComponent) target;
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
    GulagArenaComponent target1 = (GulagArenaComponent) target;
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
  virtual GulagArenaComponent Component.Instantiate() => new GulagArenaComponent();
}
