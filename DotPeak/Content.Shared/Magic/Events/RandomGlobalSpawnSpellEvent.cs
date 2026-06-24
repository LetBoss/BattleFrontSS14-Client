// Decompiled with JetBrains decompiler
// Type: Content.Shared.Magic.Events.RandomGlobalSpawnSpellEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Content.Shared.Storage;
using Robust.Shared.Audio;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Magic.Events;

public sealed class RandomGlobalSpawnSpellEvent : 
  InstantActionEvent,
  ISerializationGenerated<RandomGlobalSpawnSpellEvent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public List<EntitySpawnEntry> Spawns = new List<EntitySpawnEntry>();
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier Sound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Magic/staff_animation.ogg");
  [DataField(null, false, 1, false, false, null)]
  public bool MakeSurvivorAntagonist;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RandomGlobalSpawnSpellEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    InstantActionEvent target1 = (InstantActionEvent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RandomGlobalSpawnSpellEvent) target1;
    if (serialization.TryCustomCopy<RandomGlobalSpawnSpellEvent>(this, ref target, hookCtx, false, context))
      return;
    List<EntitySpawnEntry> target2 = (List<EntitySpawnEntry>) null;
    if (this.Spawns == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<EntitySpawnEntry>>(this.Spawns, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<List<EntitySpawnEntry>>(this.Spawns, hookCtx, context);
    target.Spawns = target2;
    SoundSpecifier target3 = (SoundSpecifier) null;
    if (this.Sound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sound, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<SoundSpecifier>(this.Sound, hookCtx, context);
    target.Sound = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.MakeSurvivorAntagonist, ref target4, hookCtx, false, context))
      target4 = this.MakeSurvivorAntagonist;
    target.MakeSurvivorAntagonist = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RandomGlobalSpawnSpellEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref InstantActionEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    RandomGlobalSpawnSpellEvent target1 = (RandomGlobalSpawnSpellEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (InstantActionEvent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    RandomGlobalSpawnSpellEvent target1 = (RandomGlobalSpawnSpellEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual RandomGlobalSpawnSpellEvent InstantActionEvent.Instantiate()
  {
    return new RandomGlobalSpawnSpellEvent();
  }
}
