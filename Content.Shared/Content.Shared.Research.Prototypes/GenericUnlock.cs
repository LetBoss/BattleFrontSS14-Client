using System;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Research.Prototypes;

[DataDefinition]
public record struct GenericUnlock() : ISerializationGenerated<GenericUnlock>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public object? PurchaseEvent = null;

	[DataField(null, false, 1, false, false, null)]
	public string UnlockDescription = string.Empty;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref GenericUnlock target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<GenericUnlock>(this, ref target, hookCtx, false, context))
		{
			object PurchaseEventTemp = null;
			if (!serialization.TryCustomCopy<object>(PurchaseEvent, ref PurchaseEventTemp, hookCtx, true, context))
			{
				PurchaseEventTemp = serialization.CreateCopy(PurchaseEvent, hookCtx, context, false);
			}
			string UnlockDescriptionTemp = null;
			if (UnlockDescription == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(UnlockDescription, ref UnlockDescriptionTemp, hookCtx, false, context))
			{
				UnlockDescriptionTemp = UnlockDescription;
			}
			target = target with
			{
				PurchaseEvent = PurchaseEventTemp,
				UnlockDescription = UnlockDescriptionTemp
			};
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref GenericUnlock target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GenericUnlock cast = (GenericUnlock)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public GenericUnlock Instantiate()
	{
		return new GenericUnlock();
	}
}
