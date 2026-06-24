using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.UserInterface;

[RegisterComponent]
[NetworkedComponent]
public sealed class IntrinsicUIComponent : Component, ISerializationGenerated<IntrinsicUIComponent>, ISerializationGenerated
{
	[DataField("uis", false, 1, true, false, null)]
	public Dictionary<Enum, IntrinsicUIEntry> UIs = new Dictionary<Enum, IntrinsicUIEntry>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref IntrinsicUIComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (IntrinsicUIComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<IntrinsicUIComponent>(this, ref target, hookCtx, false, context))
		{
			Dictionary<Enum, IntrinsicUIEntry> UIsTemp = null;
			if (UIs == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<Enum, IntrinsicUIEntry>>(UIs, ref UIsTemp, hookCtx, true, context))
			{
				UIsTemp = serialization.CreateCopy<Dictionary<Enum, IntrinsicUIEntry>>(UIs, hookCtx, context, false);
			}
			target.UIs = UIsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref IntrinsicUIComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		IntrinsicUIComponent cast = (IntrinsicUIComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		IntrinsicUIComponent cast = (IntrinsicUIComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		IntrinsicUIComponent def = (IntrinsicUIComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override IntrinsicUIComponent Instantiate()
	{
		return new IntrinsicUIComponent();
	}
}
