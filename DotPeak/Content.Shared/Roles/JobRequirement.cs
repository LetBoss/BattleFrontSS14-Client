// Decompiled with JetBrains decompiler
// Type: Content.Shared.Roles.JobRequirement
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Preferences;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Content.Shared.Roles;

[ImplicitDataDefinitionForInheritors]
[NetSerializable]
[Serializable]
public abstract class JobRequirement : 
  ISerializationGenerated<JobRequirement>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public bool Inverted;

  public abstract bool Check(
    IEntityManager entManager,
    IPrototypeManager protoManager,
    HumanoidCharacterProfile? profile,
    IReadOnlyDictionary<string, TimeSpan> playTimes,
    [NotNullWhen(false)] out FormattedMessage? reason);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref JobRequirement target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<JobRequirement>(this, ref target, hookCtx, false, context))
      return;
    bool target1 = false;
    if (!serialization.TryCustomCopy<bool>(this.Inverted, ref target1, hookCtx, false, context))
      target1 = this.Inverted;
    target.Inverted = target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref JobRequirement target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    JobRequirement target1 = (JobRequirement) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public virtual JobRequirement Instantiate() => throw new NotImplementedException();
}
