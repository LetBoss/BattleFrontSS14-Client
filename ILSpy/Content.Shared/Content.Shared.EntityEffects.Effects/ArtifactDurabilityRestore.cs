using System;
using Content.Shared.Xenoarchaeology.Artifact;
using Content.Shared.Xenoarchaeology.Artifact.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.EntityEffects.Effects;

public sealed class ArtifactDurabilityRestore : EntityEffect, ISerializationGenerated<ArtifactDurabilityRestore>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public int RestoredDurability = 1;

	public override void Effect(EntityEffectBaseArgs args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		IEntityManager entityManager = args.EntityManager;
		SharedXenoArtifactSystem xenoArtifactSys = entityManager.System<SharedXenoArtifactSystem>();
		XenoArtifactComponent xenoArtifact = default(XenoArtifactComponent);
		if (!entityManager.TryGetComponent<XenoArtifactComponent>(args.TargetEntity, ref xenoArtifact))
		{
			return;
		}
		foreach (Entity<XenoArtifactNodeComponent> activeNode in xenoArtifactSys.GetActiveNodes(Entity<XenoArtifactComponent>.op_Implicit((args.TargetEntity, xenoArtifact))))
		{
			xenoArtifactSys.AdjustNodeDurability(Entity<XenoArtifactNodeComponent>.op_Implicit(activeNode.Owner), RestoredDurability);
		}
	}

	protected override string ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
	{
		return Loc.GetString("reagent-effect-guidebook-artifact-durability-restore", new(string, object)[1] { ("restored", RestoredDurability) });
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ArtifactDurabilityRestore target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EntityEffect definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ArtifactDurabilityRestore)definitionCast;
		if (!serialization.TryCustomCopy<ArtifactDurabilityRestore>(this, ref target, hookCtx, false, context))
		{
			int RestoredDurabilityTemp = 0;
			if (!serialization.TryCustomCopy<int>(RestoredDurability, ref RestoredDurabilityTemp, hookCtx, false, context))
			{
				RestoredDurabilityTemp = RestoredDurability;
			}
			target.RestoredDurability = RestoredDurabilityTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ArtifactDurabilityRestore target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EntityEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ArtifactDurabilityRestore cast = (ArtifactDurabilityRestore)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ArtifactDurabilityRestore cast = (ArtifactDurabilityRestore)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ArtifactDurabilityRestore Instantiate()
	{
		return new ArtifactDurabilityRestore();
	}
}
