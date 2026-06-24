using System;
using Content.Shared.DoAfter;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Intel;

[Serializable]
[NetSerializable]
public sealed class IntelSubmitDoAfterEvent : SimpleDoAfterEvent, ISerializationGenerated<IntelSubmitDoAfterEvent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public NetEntity Intel;

	[DataField(null, false, 1, false, false, null)]
	public int Amount;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref IntelSubmitDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		SimpleDoAfterEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (IntelSubmitDoAfterEvent)definitionCast;
		if (!serialization.TryCustomCopy<IntelSubmitDoAfterEvent>(this, ref target, hookCtx, false, context))
		{
			NetEntity IntelTemp = default(NetEntity);
			if (!serialization.TryCustomCopy<NetEntity>(Intel, ref IntelTemp, hookCtx, false, context))
			{
				IntelTemp = serialization.CreateCopy<NetEntity>(Intel, hookCtx, context, false);
			}
			target.Intel = IntelTemp;
			int AmountTemp = 0;
			if (!serialization.TryCustomCopy<int>(Amount, ref AmountTemp, hookCtx, false, context))
			{
				AmountTemp = Amount;
			}
			target.Amount = AmountTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref IntelSubmitDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref SimpleDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		IntelSubmitDoAfterEvent cast = (IntelSubmitDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		IntelSubmitDoAfterEvent cast = (IntelSubmitDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override IntelSubmitDoAfterEvent Instantiate()
	{
		return new IntelSubmitDoAfterEvent();
	}
}
