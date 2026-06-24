using System;
using Content.Shared.Actions;
using Content.Shared.Chat.Prototypes;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Magic.Events;

public sealed class VoidApplauseSpellEvent : EntityTargetActionEvent, ISerializationGenerated<VoidApplauseSpellEvent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public ProtoId<EmotePrototype> Emote = ProtoId<EmotePrototype>.op_Implicit("ClapSingle");

	[DataField(null, false, 1, false, false, null)]
	public EntProtoId Effect = EntProtoId.op_Implicit("EffectVoidBlink");

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref VoidApplauseSpellEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		EntityTargetActionEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (VoidApplauseSpellEvent)definitionCast;
		if (!serialization.TryCustomCopy<VoidApplauseSpellEvent>(this, ref target, hookCtx, false, context))
		{
			ProtoId<EmotePrototype> EmoteTemp = default(ProtoId<EmotePrototype>);
			if (!serialization.TryCustomCopy<ProtoId<EmotePrototype>>(Emote, ref EmoteTemp, hookCtx, false, context))
			{
				EmoteTemp = serialization.CreateCopy<ProtoId<EmotePrototype>>(Emote, hookCtx, context, false);
			}
			target.Emote = EmoteTemp;
			EntProtoId EffectTemp = default(EntProtoId);
			if (!serialization.TryCustomCopy<EntProtoId>(Effect, ref EffectTemp, hookCtx, false, context))
			{
				EffectTemp = serialization.CreateCopy<EntProtoId>(Effect, hookCtx, context, false);
			}
			target.Effect = EffectTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref VoidApplauseSpellEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EntityTargetActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		VoidApplauseSpellEvent cast = (VoidApplauseSpellEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		VoidApplauseSpellEvent cast = (VoidApplauseSpellEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override VoidApplauseSpellEvent Instantiate()
	{
		return new VoidApplauseSpellEvent();
	}
}
