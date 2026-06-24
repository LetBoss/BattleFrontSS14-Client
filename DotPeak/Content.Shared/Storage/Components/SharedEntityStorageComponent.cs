// Decompiled with JetBrains decompiler
// Type: Content.Shared.Storage.Components.SharedEntityStorageComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Physics;
using Content.Shared.Whitelist;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Storage.Components;

[NetworkedComponent]
public abstract class SharedEntityStorageComponent : 
  Component,
  ISerializationGenerated<SharedEntityStorageComponent>,
  ISerializationGenerated
{
  public readonly float MaxSize = 1f;
  public static readonly TimeSpan InternalOpenAttemptDelay = TimeSpan.FromSeconds(0.5);
  public TimeSpan NextInternalOpenAttempt;
  public readonly int MasksToRemove = 28;
  [DataField(null, false, 1, false, false, null)]
  public int RemovedMasks;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public int Capacity = 30;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public bool IsCollidableWhenOpen;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public bool OpenOnMove = true;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public Vector2 EnteringOffset = new Vector2(0.0f, 0.0f);
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public CollisionGroup EnteringOffsetCollisionFlags = CollisionGroup.TableMask;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public float EnteringRange = 0.18f;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public bool ShowContents;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public bool OccludesLight = true;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public bool DeleteContentsOnDestruction;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public bool Airtight = true;
  [DataField(null, false, 1, false, false, null)]
  public bool Open;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier CloseSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Effects/closetclose.ogg");
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier OpenSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Effects/closetopen.ogg");
  [DataField(null, false, 1, false, false, null)]
  public EntityWhitelist? Whitelist;
  [Robust.Shared.ViewVariables.ViewVariables]
  public Container Contents;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref SharedEntityStorageComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (SharedEntityStorageComponent) target1;
    if (serialization.TryCustomCopy<SharedEntityStorageComponent>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.RemovedMasks, ref target2, hookCtx, false, context))
      target2 = this.RemovedMasks;
    target.RemovedMasks = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.Capacity, ref target3, hookCtx, false, context))
      target3 = this.Capacity;
    target.Capacity = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.IsCollidableWhenOpen, ref target4, hookCtx, false, context))
      target4 = this.IsCollidableWhenOpen;
    target.IsCollidableWhenOpen = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.OpenOnMove, ref target5, hookCtx, false, context))
      target5 = this.OpenOnMove;
    target.OpenOnMove = target5;
    Vector2 target6 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.EnteringOffset, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<Vector2>(this.EnteringOffset, hookCtx, context);
    target.EnteringOffset = target6;
    CollisionGroup target7 = CollisionGroup.None;
    if (!serialization.TryCustomCopy<CollisionGroup>(this.EnteringOffsetCollisionFlags, ref target7, hookCtx, false, context))
      target7 = this.EnteringOffsetCollisionFlags;
    target.EnteringOffsetCollisionFlags = target7;
    float target8 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.EnteringRange, ref target8, hookCtx, false, context))
      target8 = this.EnteringRange;
    target.EnteringRange = target8;
    bool target9 = false;
    if (!serialization.TryCustomCopy<bool>(this.ShowContents, ref target9, hookCtx, false, context))
      target9 = this.ShowContents;
    target.ShowContents = target9;
    bool target10 = false;
    if (!serialization.TryCustomCopy<bool>(this.OccludesLight, ref target10, hookCtx, false, context))
      target10 = this.OccludesLight;
    target.OccludesLight = target10;
    bool target11 = false;
    if (!serialization.TryCustomCopy<bool>(this.DeleteContentsOnDestruction, ref target11, hookCtx, false, context))
      target11 = this.DeleteContentsOnDestruction;
    target.DeleteContentsOnDestruction = target11;
    bool target12 = false;
    if (!serialization.TryCustomCopy<bool>(this.Airtight, ref target12, hookCtx, false, context))
      target12 = this.Airtight;
    target.Airtight = target12;
    bool target13 = false;
    if (!serialization.TryCustomCopy<bool>(this.Open, ref target13, hookCtx, false, context))
      target13 = this.Open;
    target.Open = target13;
    SoundSpecifier target14 = (SoundSpecifier) null;
    if (this.CloseSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.CloseSound, ref target14, hookCtx, true, context))
      target14 = serialization.CreateCopy<SoundSpecifier>(this.CloseSound, hookCtx, context);
    target.CloseSound = target14;
    SoundSpecifier target15 = (SoundSpecifier) null;
    if (this.OpenSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.OpenSound, ref target15, hookCtx, true, context))
      target15 = serialization.CreateCopy<SoundSpecifier>(this.OpenSound, hookCtx, context);
    target.OpenSound = target15;
    EntityWhitelist target16 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Whitelist, ref target16, hookCtx, false, context))
    {
      if (this.Whitelist == null)
        target16 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Whitelist, ref target16, hookCtx, context);
    }
    target.Whitelist = target16;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref SharedEntityStorageComponent target,
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
    SharedEntityStorageComponent target1 = (SharedEntityStorageComponent) target;
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
    SharedEntityStorageComponent target1 = (SharedEntityStorageComponent) target;
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
    SharedEntityStorageComponent target1 = (SharedEntityStorageComponent) target;
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
  virtual SharedEntityStorageComponent Component.Instantiate()
  {
    throw new NotImplementedException();
  }
}
