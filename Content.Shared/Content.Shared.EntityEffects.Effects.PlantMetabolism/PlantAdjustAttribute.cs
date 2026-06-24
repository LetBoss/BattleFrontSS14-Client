using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.EntityEffects.Effects.PlantMetabolism;

[ImplicitDataDefinitionForInheritors]
public abstract class PlantAdjustAttribute<T> : EventEntityEffect<T>, ISerializationGenerated<PlantAdjustAttribute<T>>, ISerializationGenerated where T : PlantAdjustAttribute<T>
{
	[DataField(null, false, 1, false, false, null)]
	public float Amount { get; protected set; } = 1f;

	[DataField(null, false, 1, false, false, null)]
	public abstract string GuidebookAttributeName { get; set; }

	[DataField(null, false, 1, false, false, null)]
	public virtual bool GuidebookIsAttributePositive { get; protected set; } = true;

	protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
	{
		string color = ((!(GuidebookIsAttributePositive ^ ((double)Amount < 0.0))) ? "red" : "green");
		return Loc.GetString("reagent-effect-guidebook-plant-attribute", new(string, object)[4]
		{
			("attribute", Loc.GetString(GuidebookAttributeName)),
			("amount", Amount.ToString("0.00")),
			("colorName", color),
			("chance", Probability)
		});
	}

	public PlantAdjustAttribute()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void InternalCopy(ref PlantAdjustAttribute<T> target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		EventEntityEffect<T> definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (PlantAdjustAttribute<T>)definitionCast;
		if (!serialization.TryCustomCopy<PlantAdjustAttribute<T>>(this, ref target, hookCtx, false, context))
		{
			float AmountTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Amount, ref AmountTemp, hookCtx, false, context))
			{
				AmountTemp = Amount;
			}
			target.Amount = AmountTemp;
			string GuidebookAttributeNameTemp = null;
			if (GuidebookAttributeName == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(GuidebookAttributeName, ref GuidebookAttributeNameTemp, hookCtx, false, context))
			{
				GuidebookAttributeNameTemp = GuidebookAttributeName;
			}
			target.GuidebookAttributeName = GuidebookAttributeNameTemp;
			bool GuidebookIsAttributePositiveTemp = false;
			if (!serialization.TryCustomCopy<bool>(GuidebookIsAttributePositive, ref GuidebookIsAttributePositiveTemp, hookCtx, false, context))
			{
				GuidebookIsAttributePositiveTemp = GuidebookIsAttributePositive;
			}
			target.GuidebookIsAttributePositive = GuidebookIsAttributePositiveTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref PlantAdjustAttribute<T> target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EventEntityEffect<T> target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PlantAdjustAttribute<T> cast = (PlantAdjustAttribute<T>)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PlantAdjustAttribute<T> cast = (PlantAdjustAttribute<T>)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override PlantAdjustAttribute<T> Instantiate()
	{
		throw new NotImplementedException();
	}
}
