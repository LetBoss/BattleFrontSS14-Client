// Decompiled with JetBrains decompiler
// Type: Content.Shared.Roles.SpeciesRequirement
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Humanoid.Prototypes;
using Content.Shared.Preferences;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
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
using System.Text;

#nullable enable
namespace Content.Shared.Roles;

[NetSerializable]
[Serializable]
public sealed class SpeciesRequirement : 
  JobRequirement,
  ISerializationGenerated<SpeciesRequirement>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public HashSet<ProtoId<SpeciesPrototype>> Species = new HashSet<ProtoId<SpeciesPrototype>>();

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
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.Append("[color=yellow]");
    foreach (ProtoId<SpeciesPrototype> specy in this.Species)
      stringBuilder.Append(Loc.GetString(protoManager.Index<SpeciesPrototype>(specy).Name) + " ");
    stringBuilder.Append("[/color]");
    if (!this.Inverted)
    {
      reason = FormattedMessage.FromMarkupPermissive($"{Loc.GetString("role-timer-whitelisted-species")}\n{stringBuilder}");
      if (!this.Species.Contains(profile.Species))
        return false;
    }
    else
    {
      reason = FormattedMessage.FromMarkupPermissive($"{Loc.GetString("role-timer-blacklisted-species")}\n{stringBuilder}");
      if (this.Species.Contains(profile.Species))
        return false;
    }
    return true;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SpeciesRequirement target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    JobRequirement target1 = (JobRequirement) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (SpeciesRequirement) target1;
    if (serialization.TryCustomCopy<SpeciesRequirement>(this, ref target, hookCtx, false, context))
      return;
    HashSet<ProtoId<SpeciesPrototype>> target2 = (HashSet<ProtoId<SpeciesPrototype>>) null;
    if (this.Species == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<ProtoId<SpeciesPrototype>>>(this.Species, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<HashSet<ProtoId<SpeciesPrototype>>>(this.Species, hookCtx, context);
    target.Species = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SpeciesRequirement target,
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
    SpeciesRequirement target1 = (SpeciesRequirement) target;
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
    SpeciesRequirement target1 = (SpeciesRequirement) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual SpeciesRequirement JobRequirement.Instantiate() => new SpeciesRequirement();
}
