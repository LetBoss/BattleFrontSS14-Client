// Decompiled with JetBrains decompiler
// Type: Content.Shared.Preferences.Loadouts.Effects.SpeciesLoadoutEffect
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Humanoid.Prototypes;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
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

public sealed class SpeciesLoadoutEffect : 
  LoadoutEffect,
  ISerializationGenerated<SpeciesLoadoutEffect>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public List<ProtoId<SpeciesPrototype>> Species = new List<ProtoId<SpeciesPrototype>>();

  public override bool Validate(
    HumanoidCharacterProfile profile,
    RoleLoadout loadout,
    ICommonSession? session,
    IDependencyCollection collection,
    [NotNullWhen(false)] out FormattedMessage? reason)
  {
    if (this.Species.Contains(profile.Species))
    {
      reason = (FormattedMessage) null;
      return true;
    }
    reason = FormattedMessage.FromUnformatted(Loc.GetString("loadout-group-species-restriction"));
    return false;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SpeciesLoadoutEffect target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    LoadoutEffect target1 = (LoadoutEffect) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (SpeciesLoadoutEffect) target1;
    if (serialization.TryCustomCopy<SpeciesLoadoutEffect>(this, ref target, hookCtx, false, context))
      return;
    List<ProtoId<SpeciesPrototype>> target2 = (List<ProtoId<SpeciesPrototype>>) null;
    if (this.Species == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<ProtoId<SpeciesPrototype>>>(this.Species, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<List<ProtoId<SpeciesPrototype>>>(this.Species, hookCtx, context);
    target.Species = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SpeciesLoadoutEffect target,
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
    SpeciesLoadoutEffect target1 = (SpeciesLoadoutEffect) target;
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
    SpeciesLoadoutEffect target1 = (SpeciesLoadoutEffect) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual SpeciesLoadoutEffect LoadoutEffect.Instantiate() => new SpeciesLoadoutEffect();
}
