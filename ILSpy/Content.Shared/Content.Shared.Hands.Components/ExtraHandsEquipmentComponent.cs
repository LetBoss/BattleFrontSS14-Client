using System;
using System.Collections.Generic;
using Content.Shared.Hands.EntitySystems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Hands.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(ExtraHandsEquipmentSystem) })]
public sealed class ExtraHandsEquipmentComponent : Component, ISerializationGenerated<ExtraHandsEquipmentComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public Dictionary<string, Hand> Hands = new Dictionary<string, Hand>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ExtraHandsEquipmentComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ExtraHandsEquipmentComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<ExtraHandsEquipmentComponent>(this, ref target, hookCtx, false, context))
		{
			Dictionary<string, Hand> HandsTemp = null;
			if (Hands == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<string, Hand>>(Hands, ref HandsTemp, hookCtx, true, context))
			{
				HandsTemp = serialization.CreateCopy<Dictionary<string, Hand>>(Hands, hookCtx, context, false);
			}
			target.Hands = HandsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ExtraHandsEquipmentComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ExtraHandsEquipmentComponent cast = (ExtraHandsEquipmentComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ExtraHandsEquipmentComponent cast = (ExtraHandsEquipmentComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ExtraHandsEquipmentComponent def = (ExtraHandsEquipmentComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ExtraHandsEquipmentComponent Instantiate()
	{
		return new ExtraHandsEquipmentComponent();
	}
}
