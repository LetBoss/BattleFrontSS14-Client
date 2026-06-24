using System;
using Content.Shared.Tag;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared.Mind.Components;

[RegisterComponent]
public sealed class TransferMindOnGibComponent : Component, ISerializationGenerated<TransferMindOnGibComponent>, ISerializationGenerated
{
	[DataField("targetTag", false, 1, false, false, typeof(PrototypeIdSerializer<TagPrototype>))]
	public string TargetTag = "MindTransferTarget";

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref TransferMindOnGibComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (TransferMindOnGibComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<TransferMindOnGibComponent>(this, ref target, hookCtx, false, context))
		{
			string TargetTagTemp = null;
			if (TargetTag == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(TargetTag, ref TargetTagTemp, hookCtx, false, context))
			{
				TargetTagTemp = TargetTag;
			}
			target.TargetTag = TargetTagTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref TransferMindOnGibComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		TransferMindOnGibComponent cast = (TransferMindOnGibComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		TransferMindOnGibComponent cast = (TransferMindOnGibComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		TransferMindOnGibComponent def = (TransferMindOnGibComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override TransferMindOnGibComponent Instantiate()
	{
		return new TransferMindOnGibComponent();
	}
}
