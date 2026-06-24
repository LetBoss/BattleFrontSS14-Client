// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Respawn.XenoRespawnComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Respawn;

[RegisterComponent]
[NetworkedComponent]
public sealed class XenoRespawnComponent : 
  Component,
  ISerializationGenerated<XenoRespawnComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public EntityUid? Hive;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan RespawnAt;
  [DataField(null, false, 1, false, false, null)]
  public bool RespawnAtCorpse;
  [DataField(null, false, 1, false, false, null)]
  public EntityCoordinates? CorpseLocation;
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId Larva = (EntProtoId) "CMXenoLarva";
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier CorpseSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Xeno/xeno_newlarva.ogg");

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoRespawnComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoRespawnComponent) target1;
    if (serialization.TryCustomCopy<XenoRespawnComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityUid? target2 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Hive, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntityUid?>(this.Hive, hookCtx, context);
    target.Hive = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.RespawnAt, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.RespawnAt, hookCtx, context);
    target.RespawnAt = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.RespawnAtCorpse, ref target4, hookCtx, false, context))
      target4 = this.RespawnAtCorpse;
    target.RespawnAtCorpse = target4;
    EntityCoordinates? target5 = new EntityCoordinates?();
    if (!serialization.TryCustomCopy<EntityCoordinates?>(this.CorpseLocation, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<EntityCoordinates?>(this.CorpseLocation, hookCtx, context);
    target.CorpseLocation = target5;
    EntProtoId target6 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Larva, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<EntProtoId>(this.Larva, hookCtx, context);
    target.Larva = target6;
    SoundSpecifier target7 = (SoundSpecifier) null;
    if (this.CorpseSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.CorpseSound, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<SoundSpecifier>(this.CorpseSound, hookCtx, context);
    target.CorpseSound = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoRespawnComponent target,
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
    XenoRespawnComponent target1 = (XenoRespawnComponent) target;
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
    XenoRespawnComponent target1 = (XenoRespawnComponent) target;
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
    XenoRespawnComponent target1 = (XenoRespawnComponent) target;
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
  virtual XenoRespawnComponent Component.Instantiate() => new XenoRespawnComponent();
}
