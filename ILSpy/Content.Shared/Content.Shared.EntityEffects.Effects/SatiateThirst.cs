using System;
using Content.Shared.Nutrition.Components;
using Content.Shared.Nutrition.EntitySystems;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.EntityEffects.Effects;

public sealed class SatiateThirst : EntityEffect, ISerializationGenerated<SatiateThirst>, ISerializationGenerated
{
	private const float DefaultHydrationFactor = 3f;

	[DataField("factor", false, 1, false, false, null)]
	public float HydrationFactor { get; set; } = 3f;

	public override void Effect(EntityEffectBaseArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		EntityUid uid = args.TargetEntity;
		ThirstComponent thirst = default(ThirstComponent);
		if (args.EntityManager.TryGetComponent<ThirstComponent>(uid, ref thirst))
		{
			args.EntityManager.System<ThirstSystem>().ModifyThirst(uid, thirst, HydrationFactor);
		}
	}

	protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
	{
		return Loc.GetString("reagent-effect-guidebook-satiate-thirst", new(string, object)[2]
		{
			("chance", Probability),
			("relative", HydrationFactor / 3f)
		});
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref SatiateThirst target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EntityEffect definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (SatiateThirst)definitionCast;
		if (!serialization.TryCustomCopy<SatiateThirst>(this, ref target, hookCtx, false, context))
		{
			float HydrationFactorTemp = 0f;
			if (!serialization.TryCustomCopy<float>(HydrationFactor, ref HydrationFactorTemp, hookCtx, false, context))
			{
				HydrationFactorTemp = HydrationFactor;
			}
			target.HydrationFactor = HydrationFactorTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref SatiateThirst target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EntityEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SatiateThirst cast = (SatiateThirst)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SatiateThirst cast = (SatiateThirst)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override SatiateThirst Instantiate()
	{
		return new SatiateThirst();
	}
}
