// Decompiled with JetBrains decompiler
// Type: Content.Shared.Roles.AgeRequirement
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Preferences;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Roles;

[NetSerializable]
[Serializable]
public sealed class AgeRequirement : 
  JobRequirement,
  ISerializationGenerated<AgeRequirement>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public int RequiredAge;

  public override bool Check(
    IEntityManager entManager,
    IPrototypeManager protoManager,
    HumanoidCharacterProfile? profile,
    IReadOnlyDictionary<string, TimeSpan> playTimes,
    [NotNullWhen(false)] out FormattedMessage? reason)
  {
    reason = new FormattedMessage();
    if (profile == null)
      return true;
    if (!this.Inverted)
    {
      reason = FormattedMessage.FromMarkupPermissive(Loc.GetString("role-timer-age-too-young", ("age", (object) this.RequiredAge)));
      if (profile.Age < this.RequiredAge)
        return false;
    }
    else
    {
      reason = FormattedMessage.FromMarkupPermissive(Loc.GetString("role-timer-age-too-old", ("age", (object) this.RequiredAge)));
      if (profile.Age > this.RequiredAge)
        return false;
    }
    return true;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref AgeRequirement target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    JobRequirement target1 = (JobRequirement) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (AgeRequirement) target1;
    if (serialization.TryCustomCopy<AgeRequirement>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.RequiredAge, ref target2, hookCtx, false, context))
      target2 = this.RequiredAge;
    target.RequiredAge = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref AgeRequirement target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref JobRequirement target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    AgeRequirement target1 = (AgeRequirement) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (JobRequirement) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    AgeRequirement target1 = (AgeRequirement) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual AgeRequirement JobRequirement.Instantiate() => new AgeRequirement();
}
