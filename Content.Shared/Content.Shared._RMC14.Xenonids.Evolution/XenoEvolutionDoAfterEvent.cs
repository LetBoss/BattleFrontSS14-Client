using System;
using Content.Shared.DoAfter;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Xenonids.Evolution;

[Serializable]
[NetSerializable]
public sealed class XenoEvolutionDoAfterEvent : DoAfterEvent, ISerializationGenerated<XenoEvolutionDoAfterEvent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public EntProtoId Choice = EntProtoId.op_Implicit("CMXenoDrone");

	public XenoEvolutionDoAfterEvent(EntProtoId choice)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		Choice = choice;
	}

	public override DoAfterEvent Clone()
	{
		return this;
	}

	public XenoEvolutionDoAfterEvent()
	{
	}//IL_0006: Unknown result type (might be due to invalid IL or missing references)
	//IL_000b: Unknown result type (might be due to invalid IL or missing references)


	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref XenoEvolutionDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		DoAfterEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (XenoEvolutionDoAfterEvent)definitionCast;
		if (!serialization.TryCustomCopy<XenoEvolutionDoAfterEvent>(this, ref target, hookCtx, false, context))
		{
			EntProtoId ChoiceTemp = default(EntProtoId);
			if (!serialization.TryCustomCopy<EntProtoId>(Choice, ref ChoiceTemp, hookCtx, false, context))
			{
				ChoiceTemp = serialization.CreateCopy<EntProtoId>(Choice, hookCtx, context, false);
			}
			target.Choice = ChoiceTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref XenoEvolutionDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref DoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoEvolutionDoAfterEvent cast = (XenoEvolutionDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoEvolutionDoAfterEvent cast = (XenoEvolutionDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override XenoEvolutionDoAfterEvent Instantiate()
	{
		return new XenoEvolutionDoAfterEvent();
	}
}
