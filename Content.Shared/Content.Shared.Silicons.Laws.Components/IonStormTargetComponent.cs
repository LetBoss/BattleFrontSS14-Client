using System;
using Content.Shared.Random;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Silicons.Laws.Components;

[RegisterComponent]
public sealed class IonStormTargetComponent : Component, ISerializationGenerated<IonStormTargetComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public ProtoId<WeightedRandomPrototype> RandomLawsets = ProtoId<WeightedRandomPrototype>.op_Implicit("IonStormLawsets");

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float Chance = 0.8f;

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float RandomLawsetChance = 0.25f;

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float RemoveChance = 0.2f;

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float ReplaceChance = 0.2f;

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float ShuffleChance = 0.2f;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref IonStormTargetComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (IonStormTargetComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<IonStormTargetComponent>(this, ref target, hookCtx, false, context))
		{
			ProtoId<WeightedRandomPrototype> RandomLawsetsTemp = default(ProtoId<WeightedRandomPrototype>);
			if (!serialization.TryCustomCopy<ProtoId<WeightedRandomPrototype>>(RandomLawsets, ref RandomLawsetsTemp, hookCtx, false, context))
			{
				RandomLawsetsTemp = serialization.CreateCopy<ProtoId<WeightedRandomPrototype>>(RandomLawsets, hookCtx, context, false);
			}
			target.RandomLawsets = RandomLawsetsTemp;
			float ChanceTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Chance, ref ChanceTemp, hookCtx, false, context))
			{
				ChanceTemp = Chance;
			}
			target.Chance = ChanceTemp;
			float RandomLawsetChanceTemp = 0f;
			if (!serialization.TryCustomCopy<float>(RandomLawsetChance, ref RandomLawsetChanceTemp, hookCtx, false, context))
			{
				RandomLawsetChanceTemp = RandomLawsetChance;
			}
			target.RandomLawsetChance = RandomLawsetChanceTemp;
			float RemoveChanceTemp = 0f;
			if (!serialization.TryCustomCopy<float>(RemoveChance, ref RemoveChanceTemp, hookCtx, false, context))
			{
				RemoveChanceTemp = RemoveChance;
			}
			target.RemoveChance = RemoveChanceTemp;
			float ReplaceChanceTemp = 0f;
			if (!serialization.TryCustomCopy<float>(ReplaceChance, ref ReplaceChanceTemp, hookCtx, false, context))
			{
				ReplaceChanceTemp = ReplaceChance;
			}
			target.ReplaceChance = ReplaceChanceTemp;
			float ShuffleChanceTemp = 0f;
			if (!serialization.TryCustomCopy<float>(ShuffleChance, ref ShuffleChanceTemp, hookCtx, false, context))
			{
				ShuffleChanceTemp = ShuffleChance;
			}
			target.ShuffleChance = ShuffleChanceTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref IonStormTargetComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		IonStormTargetComponent cast = (IonStormTargetComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		IonStormTargetComponent cast = (IonStormTargetComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		IonStormTargetComponent def = (IonStormTargetComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override IonStormTargetComponent Instantiate()
	{
		return new IonStormTargetComponent();
	}
}
