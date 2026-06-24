using System;
using System.Collections.Generic;
using Content.Shared.FixedPoint;
using Content.Shared.Nutrition.EntitySystems;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Nutrition.Components;

[RegisterComponent]
[Access(new Type[]
{
	typeof(FoodSystem),
	typeof(FoodSequenceSystem)
})]
public sealed class FoodComponent : Component, ISerializationGenerated<FoodComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public string Solution = "food";

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier UseSound = (SoundSpecifier)new SoundCollectionSpecifier("eating", (AudioParams?)null);

	[DataField(null, false, 1, false, false, null)]
	public List<EntProtoId> Trash = new List<EntProtoId>();

	[DataField(null, false, 1, false, false, null)]
	public FixedPoint2? TransferAmount = FixedPoint2.New(5);

	[DataField(null, false, 1, false, false, null)]
	public UtensilType Utensil = UtensilType.Fork;

	[DataField(null, false, 1, false, false, null)]
	public bool UtensilRequired;

	[DataField(null, false, 1, false, false, null)]
	public bool RequiresSpecialDigestion;

	[DataField(null, false, 1, false, false, null)]
	public int RequiredStomachs = 1;

	[DataField(null, false, 1, false, false, null)]
	public LocId EatMessage = LocId.op_Implicit("food-nom");

	[DataField(null, false, 1, false, false, null)]
	public float Delay = 0.5f;

	[DataField(null, false, 1, false, false, null)]
	public float ForceFeedDelay = 3f;

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public bool RequireDead = true;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref FoodComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (FoodComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<FoodComponent>(this, ref target, hookCtx, false, context))
		{
			string SolutionTemp = null;
			if (Solution == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Solution, ref SolutionTemp, hookCtx, false, context))
			{
				SolutionTemp = Solution;
			}
			target.Solution = SolutionTemp;
			SoundSpecifier UseSoundTemp = null;
			if (UseSound == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<SoundSpecifier>(UseSound, ref UseSoundTemp, hookCtx, true, context))
			{
				UseSoundTemp = serialization.CreateCopy<SoundSpecifier>(UseSound, hookCtx, context, false);
			}
			target.UseSound = UseSoundTemp;
			List<EntProtoId> TrashTemp = null;
			if (Trash == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<EntProtoId>>(Trash, ref TrashTemp, hookCtx, true, context))
			{
				TrashTemp = serialization.CreateCopy<List<EntProtoId>>(Trash, hookCtx, context, false);
			}
			target.Trash = TrashTemp;
			FixedPoint2? TransferAmountTemp = null;
			if (!serialization.TryCustomCopy<FixedPoint2?>(TransferAmount, ref TransferAmountTemp, hookCtx, false, context))
			{
				TransferAmountTemp = serialization.CreateCopy<FixedPoint2?>(TransferAmount, hookCtx, context, false);
			}
			target.TransferAmount = TransferAmountTemp;
			UtensilType UtensilTemp = UtensilType.None;
			if (!serialization.TryCustomCopy<UtensilType>(Utensil, ref UtensilTemp, hookCtx, false, context))
			{
				UtensilTemp = Utensil;
			}
			target.Utensil = UtensilTemp;
			bool UtensilRequiredTemp = false;
			if (!serialization.TryCustomCopy<bool>(UtensilRequired, ref UtensilRequiredTemp, hookCtx, false, context))
			{
				UtensilRequiredTemp = UtensilRequired;
			}
			target.UtensilRequired = UtensilRequiredTemp;
			bool RequiresSpecialDigestionTemp = false;
			if (!serialization.TryCustomCopy<bool>(RequiresSpecialDigestion, ref RequiresSpecialDigestionTemp, hookCtx, false, context))
			{
				RequiresSpecialDigestionTemp = RequiresSpecialDigestion;
			}
			target.RequiresSpecialDigestion = RequiresSpecialDigestionTemp;
			int RequiredStomachsTemp = 0;
			if (!serialization.TryCustomCopy<int>(RequiredStomachs, ref RequiredStomachsTemp, hookCtx, false, context))
			{
				RequiredStomachsTemp = RequiredStomachs;
			}
			target.RequiredStomachs = RequiredStomachsTemp;
			LocId EatMessageTemp = default(LocId);
			if (!serialization.TryCustomCopy<LocId>(EatMessage, ref EatMessageTemp, hookCtx, false, context))
			{
				EatMessageTemp = serialization.CreateCopy<LocId>(EatMessage, hookCtx, context, false);
			}
			target.EatMessage = EatMessageTemp;
			float DelayTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Delay, ref DelayTemp, hookCtx, false, context))
			{
				DelayTemp = Delay;
			}
			target.Delay = DelayTemp;
			float ForceFeedDelayTemp = 0f;
			if (!serialization.TryCustomCopy<float>(ForceFeedDelay, ref ForceFeedDelayTemp, hookCtx, false, context))
			{
				ForceFeedDelayTemp = ForceFeedDelay;
			}
			target.ForceFeedDelay = ForceFeedDelayTemp;
			bool RequireDeadTemp = false;
			if (!serialization.TryCustomCopy<bool>(RequireDead, ref RequireDeadTemp, hookCtx, false, context))
			{
				RequireDeadTemp = RequireDead;
			}
			target.RequireDead = RequireDeadTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref FoodComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FoodComponent cast = (FoodComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FoodComponent cast = (FoodComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FoodComponent def = (FoodComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override FoodComponent Instantiate()
	{
		return new FoodComponent();
	}
}
