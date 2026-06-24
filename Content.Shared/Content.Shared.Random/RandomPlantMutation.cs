using System;
using Content.Shared.EntityEffects;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Random;

[Serializable]
[NetSerializable]
[DataDefinition]
public sealed class RandomPlantMutation : ISerializationGenerated<RandomPlantMutation>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public float BaseOdds;

	[DataField(null, false, 1, false, false, null)]
	public string Name = "";

	[DataField(null, false, 1, false, false, null)]
	public LocId? Description;

	[DataField(null, false, 1, false, false, null)]
	public EntityEffect Effect;

	[DataField(null, false, 1, false, false, null)]
	public bool AppliesToProduce = true;

	[DataField(null, false, 1, false, false, null)]
	public bool AppliesToPlant = true;

	[DataField(null, false, 1, false, false, null)]
	public bool Persists = true;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RandomPlantMutation target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<RandomPlantMutation>(this, ref target, hookCtx, false, context))
		{
			float BaseOddsTemp = 0f;
			if (!serialization.TryCustomCopy<float>(BaseOdds, ref BaseOddsTemp, hookCtx, false, context))
			{
				BaseOddsTemp = BaseOdds;
			}
			target.BaseOdds = BaseOddsTemp;
			string NameTemp = null;
			if (Name == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Name, ref NameTemp, hookCtx, false, context))
			{
				NameTemp = Name;
			}
			target.Name = NameTemp;
			LocId? DescriptionTemp = null;
			if (!serialization.TryCustomCopy<LocId?>(Description, ref DescriptionTemp, hookCtx, false, context))
			{
				DescriptionTemp = serialization.CreateCopy<LocId?>(Description, hookCtx, context, false);
			}
			target.Description = DescriptionTemp;
			EntityEffect EffectTemp = null;
			if (Effect == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<EntityEffect>(Effect, ref EffectTemp, hookCtx, true, context))
			{
				EffectTemp = serialization.CreateCopy<EntityEffect>(Effect, hookCtx, context, false);
			}
			target.Effect = EffectTemp;
			bool AppliesToProduceTemp = false;
			if (!serialization.TryCustomCopy<bool>(AppliesToProduce, ref AppliesToProduceTemp, hookCtx, false, context))
			{
				AppliesToProduceTemp = AppliesToProduce;
			}
			target.AppliesToProduce = AppliesToProduceTemp;
			bool AppliesToPlantTemp = false;
			if (!serialization.TryCustomCopy<bool>(AppliesToPlant, ref AppliesToPlantTemp, hookCtx, false, context))
			{
				AppliesToPlantTemp = AppliesToPlant;
			}
			target.AppliesToPlant = AppliesToPlantTemp;
			bool PersistsTemp = false;
			if (!serialization.TryCustomCopy<bool>(Persists, ref PersistsTemp, hookCtx, false, context))
			{
				PersistsTemp = Persists;
			}
			target.Persists = PersistsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RandomPlantMutation target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RandomPlantMutation cast = (RandomPlantMutation)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public RandomPlantMutation Instantiate()
	{
		return new RandomPlantMutation();
	}
}
