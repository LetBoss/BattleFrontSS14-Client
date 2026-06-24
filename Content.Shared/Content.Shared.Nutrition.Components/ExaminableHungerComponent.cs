using System;
using System.Collections.Generic;
using Content.Shared.Nutrition.EntitySystems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Nutrition.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(ExaminableHungerSystem) })]
public sealed class ExaminableHungerComponent : Component, ISerializationGenerated<ExaminableHungerComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public Dictionary<HungerThreshold, LocId> Descriptions = new Dictionary<HungerThreshold, LocId>
	{
		{
			HungerThreshold.Overfed,
			LocId.op_Implicit("examinable-hunger-component-examine-overfed")
		},
		{
			HungerThreshold.Okay,
			LocId.op_Implicit("examinable-hunger-component-examine-okay")
		},
		{
			HungerThreshold.Peckish,
			LocId.op_Implicit("examinable-hunger-component-examine-peckish")
		},
		{
			HungerThreshold.Starving,
			LocId.op_Implicit("examinable-hunger-component-examine-starving")
		},
		{
			HungerThreshold.Dead,
			LocId.op_Implicit("examinable-hunger-component-examine-starving")
		}
	};

	public LocId NoHungerDescription = LocId.op_Implicit("examinable-hunger-component-examine-none");

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ExaminableHungerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ExaminableHungerComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<ExaminableHungerComponent>(this, ref target, hookCtx, false, context))
		{
			Dictionary<HungerThreshold, LocId> DescriptionsTemp = null;
			if (Descriptions == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<HungerThreshold, LocId>>(Descriptions, ref DescriptionsTemp, hookCtx, true, context))
			{
				DescriptionsTemp = serialization.CreateCopy<Dictionary<HungerThreshold, LocId>>(Descriptions, hookCtx, context, false);
			}
			target.Descriptions = DescriptionsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ExaminableHungerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ExaminableHungerComponent cast = (ExaminableHungerComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ExaminableHungerComponent cast = (ExaminableHungerComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ExaminableHungerComponent def = (ExaminableHungerComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ExaminableHungerComponent Instantiate()
	{
		return new ExaminableHungerComponent();
	}
}
