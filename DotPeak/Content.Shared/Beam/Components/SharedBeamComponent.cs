// Decompiled with JetBrains decompiler
// Type: Content.Shared.Beam.Components.SharedBeamComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Beam.Components;

public abstract class SharedBeamComponent : 
  Component,
  ISerializationGenerated<SharedBeamComponent>,
  ISerializationGenerated
{
  [DataField("hitTargets", false, 1, false, false, null)]
  public HashSet<EntityUid> HitTargets = new HashSet<EntityUid>();
  [DataField("virtualBeamController", false, 1, false, false, null)]
  public EntityUid? VirtualBeamController;
  [DataField("originBeam", false, 1, false, false, null)]
  public EntityUid OriginBeam;
  [DataField("beamShooter", false, 1, false, false, null)]
  public EntityUid BeamShooter;
  [DataField("createdBeams", false, 1, false, false, null)]
  public HashSet<EntityUid> CreatedBeams = new HashSet<EntityUid>();
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField("sound", false, 1, false, false, null)]
  public SoundSpecifier? Sound;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref SharedBeamComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (SharedBeamComponent) component;
    if (serialization.TryCustomCopy<SharedBeamComponent>(this, ref target, hookCtx, false, context))
      return;
    HashSet<EntityUid> entityUidSet1 = (HashSet<EntityUid>) null;
    if (this.HitTargets == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<EntityUid>>(this.HitTargets, ref entityUidSet1, hookCtx, true, context))
      entityUidSet1 = serialization.CreateCopy<HashSet<EntityUid>>(this.HitTargets, hookCtx, context, false);
    target.HitTargets = entityUidSet1;
    EntityUid? nullable = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.VirtualBeamController, ref nullable, hookCtx, false, context))
      nullable = serialization.CreateCopy<EntityUid?>(this.VirtualBeamController, hookCtx, context, false);
    target.VirtualBeamController = nullable;
    EntityUid entityUid1 = new EntityUid();
    if (!serialization.TryCustomCopy<EntityUid>(this.OriginBeam, ref entityUid1, hookCtx, false, context))
      entityUid1 = serialization.CreateCopy<EntityUid>(this.OriginBeam, hookCtx, context, false);
    target.OriginBeam = entityUid1;
    EntityUid entityUid2 = new EntityUid();
    if (!serialization.TryCustomCopy<EntityUid>(this.BeamShooter, ref entityUid2, hookCtx, false, context))
      entityUid2 = serialization.CreateCopy<EntityUid>(this.BeamShooter, hookCtx, context, false);
    target.BeamShooter = entityUid2;
    HashSet<EntityUid> entityUidSet2 = (HashSet<EntityUid>) null;
    if (this.CreatedBeams == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<EntityUid>>(this.CreatedBeams, ref entityUidSet2, hookCtx, true, context))
      entityUidSet2 = serialization.CreateCopy<HashSet<EntityUid>>(this.CreatedBeams, hookCtx, context, false);
    target.CreatedBeams = entityUidSet2;
    SoundSpecifier soundSpecifier = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sound, ref soundSpecifier, hookCtx, true, context))
      soundSpecifier = serialization.CreateCopy<SoundSpecifier>(this.Sound, hookCtx, context, false);
    target.Sound = soundSpecifier;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref SharedBeamComponent target,
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
    SharedBeamComponent target1 = (SharedBeamComponent) target;
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
    SharedBeamComponent target1 = (SharedBeamComponent) target;
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
    SharedBeamComponent target1 = (SharedBeamComponent) target;
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
  virtual SharedBeamComponent Component.Instantiate() => throw new NotImplementedException();
}
