// Decompiled with JetBrains decompiler
// Type: Content.Shared.Preferences.Loadouts.Effects.GroupLoadoutEffect
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.IoC;
using Robust.Shared.Player;
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
namespace Content.Shared.Preferences.Loadouts.Effects;

public sealed class GroupLoadoutEffect : 
  LoadoutEffect,
  ISerializationGenerated<GroupLoadoutEffect>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public ProtoId<LoadoutEffectGroupPrototype> Proto;

  public override bool Validate(
    HumanoidCharacterProfile profile,
    RoleLoadout loadout,
    ICommonSession? session,
    IDependencyCollection collection,
    [NotNullWhen(false)] out FormattedMessage? reason)
  {
    LoadoutEffectGroupPrototype effectGroupPrototype = collection.Resolve<IPrototypeManager>().Index<LoadoutEffectGroupPrototype>(this.Proto);
    List<string> values = new List<string>();
    foreach (LoadoutEffect effect in effectGroupPrototype.Effects)
    {
      if (!effect.Validate(profile, loadout, session, collection, out reason))
        values.Add(reason.ToMarkup());
    }
    reason = values.Count == 0 ? (FormattedMessage) null : FormattedMessage.FromMarkupOrThrow(string.Join<string>('\n', (IEnumerable<string>) values));
    return reason == null;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref GroupLoadoutEffect target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    LoadoutEffect target1 = (LoadoutEffect) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (GroupLoadoutEffect) target1;
    if (serialization.TryCustomCopy<GroupLoadoutEffect>(this, ref target, hookCtx, false, context))
      return;
    ProtoId<LoadoutEffectGroupPrototype> target2 = new ProtoId<LoadoutEffectGroupPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<LoadoutEffectGroupPrototype>>(this.Proto, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<ProtoId<LoadoutEffectGroupPrototype>>(this.Proto, hookCtx, context);
    target.Proto = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref GroupLoadoutEffect target,
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
    GroupLoadoutEffect target1 = (GroupLoadoutEffect) target;
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
    GroupLoadoutEffect target1 = (GroupLoadoutEffect) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual GroupLoadoutEffect LoadoutEffect.Instantiate() => new GroupLoadoutEffect();
}
