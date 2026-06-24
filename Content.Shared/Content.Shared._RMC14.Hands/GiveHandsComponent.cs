using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._RMC14.Hands;

[RegisterComponent]
[NetworkedComponent]
public sealed class GiveHandsComponent : Component, ISerializationGenerated<GiveHandsComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public List<GivenHand> Hands = new List<GivenHand>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref GiveHandsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (GiveHandsComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<GiveHandsComponent>(this, ref target, hookCtx, false, context))
		{
			List<GivenHand> HandsTemp = null;
			if (Hands == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<GivenHand>>(Hands, ref HandsTemp, hookCtx, true, context))
			{
				HandsTemp = serialization.CreateCopy<List<GivenHand>>(Hands, hookCtx, context, false);
			}
			target.Hands = HandsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref GiveHandsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GiveHandsComponent cast = (GiveHandsComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GiveHandsComponent cast = (GiveHandsComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GiveHandsComponent def = (GiveHandsComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override GiveHandsComponent Instantiate()
	{
		return new GiveHandsComponent();
	}
}
