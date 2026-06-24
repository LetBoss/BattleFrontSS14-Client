// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityEffects.Effects.ArtifactUnlock
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Popups;
using Content.Shared.Xenoarchaeology.Artifact;
using Content.Shared.Xenoarchaeology.Artifact.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.EntityEffects.Effects;

public sealed class ArtifactUnlock : 
  EntityEffect,
  ISerializationGenerated<ArtifactUnlock>,
  ISerializationGenerated
{
  public override void Effect(EntityEffectBaseArgs args)
  {
    IEntityManager entityManager = args.EntityManager;
    SharedXenoArtifactSystem xenoArtifactSystem = entityManager.System<SharedXenoArtifactSystem>();
    SharedPopupSystem sharedPopupSystem = entityManager.System<SharedPopupSystem>();
    XenoArtifactComponent component1;
    if (!entityManager.TryGetComponent<XenoArtifactComponent>(args.TargetEntity, out component1))
      return;
    XenoArtifactUnlockingComponent component2;
    if (!entityManager.TryGetComponent<XenoArtifactUnlockingComponent>(args.TargetEntity, out component2))
    {
      xenoArtifactSystem.TriggerXenoArtifact((Entity<XenoArtifactComponent>) (args.TargetEntity, component1), new Entity<XenoArtifactNodeComponent>?(), true);
      component2 = entityManager.EnsureComponent<XenoArtifactUnlockingComponent>(args.TargetEntity);
    }
    else if (!component2.ArtifexiumApplied)
      sharedPopupSystem.PopupEntity(Loc.GetString("artifact-activation-artifexium"), args.TargetEntity, PopupType.Medium);
    if (component2.ArtifexiumApplied)
      return;
    xenoArtifactSystem.SetArtifexiumApplied((Entity<XenoArtifactUnlockingComponent>) (args.TargetEntity, component2), true);
  }

  protected override string ReagentEffectGuidebookText(
    IPrototypeManager prototype,
    IEntitySystemManager entSys)
  {
    return Loc.GetString("reagent-effect-guidebook-artifact-unlock", ("chance", (object) this.Probability));
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ArtifactUnlock target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EntityEffect target1 = (EntityEffect) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ArtifactUnlock) target1;
    serialization.TryCustomCopy<ArtifactUnlock>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ArtifactUnlock target,
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
    ArtifactUnlock target1 = (ArtifactUnlock) target;
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
    ArtifactUnlock target1 = (ArtifactUnlock) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual ArtifactUnlock EntityEffect.Instantiate() => new ArtifactUnlock();
}
