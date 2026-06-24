using System;
using Content.Shared.Nutrition.Components;
using Content.Shared.Nutrition.EntitySystems;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared.EntityEffects.Effects;

public sealed class WashCreamPieReaction : EntityEffect, ISerializationGenerated<WashCreamPieReaction>, ISerializationGenerated
{
	protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
	{
		return Loc.GetString("reagent-effect-guidebook-wash-cream-pie-reaction", new(string, object)[1] { ("chance", Probability) });
	}

	public override void Effect(EntityEffectBaseArgs args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		CreamPiedComponent creamPied = default(CreamPiedComponent);
		if (args.EntityManager.TryGetComponent<CreamPiedComponent>(args.TargetEntity, ref creamPied))
		{
			args.EntityManager.System<SharedCreamPieSystem>().SetCreamPied(args.TargetEntity, creamPied, value: false);
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref WashCreamPieReaction target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EntityEffect definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (WashCreamPieReaction)definitionCast;
		serialization.TryCustomCopy<WashCreamPieReaction>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref WashCreamPieReaction target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EntityEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		WashCreamPieReaction cast = (WashCreamPieReaction)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		WashCreamPieReaction cast = (WashCreamPieReaction)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override WashCreamPieReaction Instantiate()
	{
		return new WashCreamPieReaction();
	}
}
