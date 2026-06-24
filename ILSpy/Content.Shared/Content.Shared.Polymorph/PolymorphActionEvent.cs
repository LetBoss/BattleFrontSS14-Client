using System;
using Content.Shared.Actions;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Polymorph;

public sealed class PolymorphActionEvent : InstantActionEvent, ISerializationGenerated<PolymorphActionEvent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public ProtoId<PolymorphPrototype>? ProtoId;

	public PolymorphActionEvent(ProtoId<PolymorphPrototype> protoId)
		: this()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		ProtoId = protoId;
	}

	public PolymorphActionEvent()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PolymorphActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InstantActionEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (PolymorphActionEvent)definitionCast;
		if (!serialization.TryCustomCopy<PolymorphActionEvent>(this, ref target, hookCtx, false, context))
		{
			ProtoId<PolymorphPrototype>? ProtoIdTemp = null;
			if (!serialization.TryCustomCopy<ProtoId<PolymorphPrototype>?>(ProtoId, ref ProtoIdTemp, hookCtx, false, context))
			{
				ProtoIdTemp = serialization.CreateCopy<ProtoId<PolymorphPrototype>?>(ProtoId, hookCtx, context, false);
			}
			target.ProtoId = ProtoIdTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PolymorphActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref InstantActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PolymorphActionEvent cast = (PolymorphActionEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PolymorphActionEvent cast = (PolymorphActionEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override PolymorphActionEvent Instantiate()
	{
		return new PolymorphActionEvent();
	}
}
