// Decompiled with JetBrains decompiler
// Type: Content.Shared.Preferences.Loadouts.Effects.PointsCostLoadoutEffect
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
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Preferences.Loadouts.Effects;

public sealed class PointsCostLoadoutEffect : 
  LoadoutEffect,
  ISerializationGenerated<PointsCostLoadoutEffect>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public int Cost = 1;

  public override bool Validate(
    HumanoidCharacterProfile profile,
    RoleLoadout loadout,
    ICommonSession? session,
    IDependencyCollection collection,
    [NotNullWhen(false)] out FormattedMessage? reason)
  {
    reason = (FormattedMessage) null;
    RoleLoadoutPrototype prototype;
    if (!collection.Resolve<IPrototypeManager>().TryIndex<RoleLoadoutPrototype>(loadout.Role, out prototype) || !prototype.Points.HasValue)
      return true;
    int? points = loadout.Points;
    int cost = this.Cost;
    if (!(points.GetValueOrDefault() <= cost & points.HasValue))
      return true;
    reason = FormattedMessage.FromUnformatted("loadout-group-points-insufficient");
    return false;
  }

  public override void Apply(RoleLoadout loadout)
  {
    RoleLoadout roleLoadout = loadout;
    int? points = roleLoadout.Points;
    int cost = this.Cost;
    roleLoadout.Points = points.HasValue ? new int?(points.GetValueOrDefault() - cost) : new int?();
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PointsCostLoadoutEffect target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    LoadoutEffect target1 = (LoadoutEffect) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (PointsCostLoadoutEffect) target1;
    if (serialization.TryCustomCopy<PointsCostLoadoutEffect>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.Cost, ref target2, hookCtx, false, context))
      target2 = this.Cost;
    target.Cost = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PointsCostLoadoutEffect target,
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
    PointsCostLoadoutEffect target1 = (PointsCostLoadoutEffect) target;
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
    PointsCostLoadoutEffect target1 = (PointsCostLoadoutEffect) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual PointsCostLoadoutEffect LoadoutEffect.Instantiate() => new PointsCostLoadoutEffect();
}
