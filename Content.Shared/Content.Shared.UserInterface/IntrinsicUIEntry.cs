using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.UserInterface;

[DataDefinition]
public sealed class IntrinsicUIEntry : ISerializationGenerated<IntrinsicUIEntry>, ISerializationGenerated
{
	[DataField("toggleAction", false, 1, true, false, null)]
	public EntProtoId? ToggleAction;

	[DataField("toggleActionEntity", false, 1, false, false, null)]
	public EntityUid? ToggleActionEntity = default(EntityUid);

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref IntrinsicUIEntry target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		if (!serialization.TryCustomCopy<IntrinsicUIEntry>(this, ref target, hookCtx, false, context))
		{
			EntProtoId? ToggleActionTemp = null;
			if (!serialization.TryCustomCopy<EntProtoId?>(ToggleAction, ref ToggleActionTemp, hookCtx, false, context))
			{
				ToggleActionTemp = serialization.CreateCopy<EntProtoId?>(ToggleAction, hookCtx, context, false);
			}
			target.ToggleAction = ToggleActionTemp;
			EntityUid? ToggleActionEntityTemp = null;
			if (!serialization.TryCustomCopy<EntityUid?>(ToggleActionEntity, ref ToggleActionEntityTemp, hookCtx, false, context))
			{
				ToggleActionEntityTemp = serialization.CreateCopy<EntityUid?>(ToggleActionEntity, hookCtx, context, false);
			}
			target.ToggleActionEntity = ToggleActionEntityTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref IntrinsicUIEntry target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		IntrinsicUIEntry cast = (IntrinsicUIEntry)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public IntrinsicUIEntry Instantiate()
	{
		return new IntrinsicUIEntry();
	}
}
