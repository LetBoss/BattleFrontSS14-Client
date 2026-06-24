// Decompiled with JetBrains decompiler
// Type: Content.Shared.Ghost.Roles.Raffles.GhostRoleRaffleSettings
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;
using System;

#nullable enable
namespace Content.Shared.Ghost.Roles.Raffles;

[DataDefinition]
public sealed class GhostRoleRaffleSettings : 
  ISerializationGenerated<GhostRoleRaffleSettings>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public float RoundTimeRequirement;

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField(null, false, 1, true, false, null)]
  public uint InitialDuration { get; set; }

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField(null, false, 1, true, false, null)]
  public uint JoinExtendsDurationBy { get; set; }

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField(null, false, 1, true, false, null)]
  public uint MaxDuration { get; set; }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref GhostRoleRaffleSettings target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<GhostRoleRaffleSettings>(this, ref target, hookCtx, false, context))
      return;
    uint target1 = 0;
    if (!serialization.TryCustomCopy<uint>(this.InitialDuration, ref target1, hookCtx, false, context))
      target1 = this.InitialDuration;
    target.InitialDuration = target1;
    uint target2 = 0;
    if (!serialization.TryCustomCopy<uint>(this.JoinExtendsDurationBy, ref target2, hookCtx, false, context))
      target2 = this.JoinExtendsDurationBy;
    target.JoinExtendsDurationBy = target2;
    uint target3 = 0;
    if (!serialization.TryCustomCopy<uint>(this.MaxDuration, ref target3, hookCtx, false, context))
      target3 = this.MaxDuration;
    target.MaxDuration = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RoundTimeRequirement, ref target4, hookCtx, false, context))
      target4 = this.RoundTimeRequirement;
    target.RoundTimeRequirement = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref GhostRoleRaffleSettings target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    GhostRoleRaffleSettings target1 = (GhostRoleRaffleSettings) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public GhostRoleRaffleSettings Instantiate() => new GhostRoleRaffleSettings();
}
