using System;
using Content.Shared.Popups;
using Content.Shared.Xenoarchaeology.Artifact;
using Content.Shared.Xenoarchaeology.Artifact.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared.EntityEffects.Effects;

public sealed class ArtifactUnlock : EntityEffect, ISerializationGenerated<ArtifactUnlock>, ISerializationGenerated
{
	public override void Effect(EntityEffectBaseArgs args)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		IEntityManager entMan = args.EntityManager;
		SharedXenoArtifactSystem xenoArtifactSys = entMan.System<SharedXenoArtifactSystem>();
		SharedPopupSystem popupSys = entMan.System<SharedPopupSystem>();
		XenoArtifactComponent xenoArtifact = default(XenoArtifactComponent);
		if (entMan.TryGetComponent<XenoArtifactComponent>(args.TargetEntity, ref xenoArtifact))
		{
			XenoArtifactUnlockingComponent unlocking = default(XenoArtifactUnlockingComponent);
			if (!entMan.TryGetComponent<XenoArtifactUnlockingComponent>(args.TargetEntity, ref unlocking))
			{
				xenoArtifactSys.TriggerXenoArtifact(Entity<XenoArtifactComponent>.op_Implicit((args.TargetEntity, xenoArtifact)), null, force: true);
				unlocking = entMan.EnsureComponent<XenoArtifactUnlockingComponent>(args.TargetEntity);
			}
			else if (!unlocking.ArtifexiumApplied)
			{
				popupSys.PopupEntity(Loc.GetString("artifact-activation-artifexium"), args.TargetEntity, PopupType.Medium);
			}
			if (!unlocking.ArtifexiumApplied)
			{
				xenoArtifactSys.SetArtifexiumApplied(Entity<XenoArtifactUnlockingComponent>.op_Implicit((args.TargetEntity, unlocking)), val: true);
			}
		}
	}

	protected override string ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
	{
		return Loc.GetString("reagent-effect-guidebook-artifact-unlock", new(string, object)[1] { ("chance", Probability) });
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ArtifactUnlock target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EntityEffect definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ArtifactUnlock)definitionCast;
		serialization.TryCustomCopy<ArtifactUnlock>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ArtifactUnlock target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EntityEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ArtifactUnlock cast = (ArtifactUnlock)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ArtifactUnlock cast = (ArtifactUnlock)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ArtifactUnlock Instantiate()
	{
		return new ArtifactUnlock();
	}
}
