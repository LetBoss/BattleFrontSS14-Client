using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Procedural.Components;

[RegisterComponent]
public sealed class EntityRemapComponent : Component, ISerializationGenerated<EntityRemapComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public Dictionary<EntProtoId, EntProtoId> Mask = new Dictionary<EntProtoId, EntProtoId>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref EntityRemapComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (EntityRemapComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<EntityRemapComponent>(this, ref target, hookCtx, false, context))
		{
			Dictionary<EntProtoId, EntProtoId> MaskTemp = null;
			if (Mask == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<EntProtoId, EntProtoId>>(Mask, ref MaskTemp, hookCtx, true, context))
			{
				MaskTemp = serialization.CreateCopy<Dictionary<EntProtoId, EntProtoId>>(Mask, hookCtx, context, false);
			}
			target.Mask = MaskTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref EntityRemapComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EntityRemapComponent cast = (EntityRemapComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EntityRemapComponent cast = (EntityRemapComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EntityRemapComponent def = (EntityRemapComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override EntityRemapComponent Instantiate()
	{
		return new EntityRemapComponent();
	}
}
