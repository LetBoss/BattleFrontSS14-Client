// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityEffects.Effects.ArtifactDurabilityRestore
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Xenoarchaeology.Artifact;
using Content.Shared.Xenoarchaeology.Artifact.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.EntityEffects.Effects;

public sealed class ArtifactDurabilityRestore : 
  EntityEffect,
  ISerializationGenerated<ArtifactDurabilityRestore>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public int RestoredDurability = 1;

  public override void Effect(EntityEffectBaseArgs args)
  {
    IEntityManager entityManager = args.EntityManager;
    SharedXenoArtifactSystem xenoArtifactSystem = entityManager.System<SharedXenoArtifactSystem>();
    XenoArtifactComponent component;
    if (!entityManager.TryGetComponent<XenoArtifactComponent>(args.TargetEntity, out component))
      return;
    foreach (Entity<XenoArtifactNodeComponent> activeNode in xenoArtifactSystem.GetActiveNodes((Entity<XenoArtifactComponent>) (args.TargetEntity, component)))
      xenoArtifactSystem.AdjustNodeDurability((Entity<XenoArtifactNodeComponent>) activeNode.Owner, this.RestoredDurability);
  }

  protected override string ReagentEffectGuidebookText(
    IPrototypeManager prototype,
    IEntitySystemManager entSys)
  {
    return Loc.GetString("reagent-effect-guidebook-artifact-durability-restore", ("restored", (object) this.RestoredDurability));
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ArtifactDurabilityRestore target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EntityEffect target1 = (EntityEffect) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ArtifactDurabilityRestore) target1;
    if (serialization.TryCustomCopy<ArtifactDurabilityRestore>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.RestoredDurability, ref target2, hookCtx, false, context))
      target2 = this.RestoredDurability;
    target.RestoredDurability = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ArtifactDurabilityRestore target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref EntityEffect target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ArtifactDurabilityRestore target1 = (ArtifactDurabilityRestore) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (EntityEffect) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ArtifactDurabilityRestore target1 = (ArtifactDurabilityRestore) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual ArtifactDurabilityRestore EntityEffect.Instantiate() => new ArtifactDurabilityRestore();
}
