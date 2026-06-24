// Decompiled with JetBrains decompiler
// Type: Content.Shared.Preferences.Loadouts.Effects.JobRequirementLoadoutEffect
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Players.PlayTimeTracking;
using Content.Shared.Roles;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Preferences.Loadouts.Effects;

public sealed class JobRequirementLoadoutEffect : 
  LoadoutEffect,
  ISerializationGenerated<JobRequirementLoadoutEffect>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public JobRequirement Requirement;

  public override bool Validate(
    HumanoidCharacterProfile profile,
    RoleLoadout loadout,
    ICommonSession? session,
    IDependencyCollection collection,
    [NotNullWhen(false)] out FormattedMessage? reason)
  {
    if (session == null)
    {
      reason = FormattedMessage.Empty;
      return true;
    }
    IReadOnlyDictionary<string, TimeSpan> playTimes = collection.Resolve<ISharedPlaytimeManager>().GetPlayTimes(session);
    return this.Requirement.Check(collection.Resolve<IEntityManager>(), collection.Resolve<IPrototypeManager>(), profile, playTimes, out reason);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref JobRequirementLoadoutEffect target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    LoadoutEffect target1 = (LoadoutEffect) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (JobRequirementLoadoutEffect) target1;
    if (serialization.TryCustomCopy<JobRequirementLoadoutEffect>(this, ref target, hookCtx, false, context))
      return;
    JobRequirement target2 = (JobRequirement) null;
    if (this.Requirement == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<JobRequirement>(this.Requirement, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<JobRequirement>(this.Requirement, hookCtx, context);
    target.Requirement = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref JobRequirementLoadoutEffect target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref LoadoutEffect target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    JobRequirementLoadoutEffect target1 = (JobRequirementLoadoutEffect) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (LoadoutEffect) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    JobRequirementLoadoutEffect target1 = (JobRequirementLoadoutEffect) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual JobRequirementLoadoutEffect LoadoutEffect.Instantiate()
  {
    return new JobRequirementLoadoutEffect();
  }
}
