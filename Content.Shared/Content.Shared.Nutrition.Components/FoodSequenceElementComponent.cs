using System;
using System.Collections.Generic;
using Content.Shared.Nutrition.EntitySystems;
using Content.Shared.Nutrition.Prototypes;
using Content.Shared.Tag;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Nutrition.Components;

[RegisterComponent]
[Access(new Type[] { typeof(SharedFoodSequenceSystem) })]
public sealed class FoodSequenceElementComponent : Component, ISerializationGenerated<FoodSequenceElementComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public Dictionary<ProtoId<TagPrototype>, ProtoId<FoodSequenceElementPrototype>> Entries = new Dictionary<ProtoId<TagPrototype>, ProtoId<FoodSequenceElementPrototype>>();

	[DataField(null, false, 1, false, false, null)]
	public string Solution = "food";

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref FoodSequenceElementComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (FoodSequenceElementComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<FoodSequenceElementComponent>(this, ref target, hookCtx, false, context))
		{
			Dictionary<ProtoId<TagPrototype>, ProtoId<FoodSequenceElementPrototype>> EntriesTemp = null;
			if (Entries == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<ProtoId<TagPrototype>, ProtoId<FoodSequenceElementPrototype>>>(Entries, ref EntriesTemp, hookCtx, true, context))
			{
				EntriesTemp = serialization.CreateCopy<Dictionary<ProtoId<TagPrototype>, ProtoId<FoodSequenceElementPrototype>>>(Entries, hookCtx, context, false);
			}
			target.Entries = EntriesTemp;
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
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref FoodSequenceElementComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FoodSequenceElementComponent cast = (FoodSequenceElementComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FoodSequenceElementComponent cast = (FoodSequenceElementComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FoodSequenceElementComponent def = (FoodSequenceElementComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override FoodSequenceElementComponent Instantiate()
	{
		return new FoodSequenceElementComponent();
	}
}
