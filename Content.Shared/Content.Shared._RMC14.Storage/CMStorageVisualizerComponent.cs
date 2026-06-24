using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Storage;

[RegisterComponent]
public sealed class CMStorageVisualizerComponent : Component, ISerializationGenerated<CMStorageVisualizerComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public string? StorageClosed;

	[DataField(null, false, 1, false, false, null)]
	public string? StorageOpen;

	[DataField(null, false, 1, false, false, null)]
	public string? StorageEmpty;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref CMStorageVisualizerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (CMStorageVisualizerComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<CMStorageVisualizerComponent>(this, ref target, hookCtx, false, context))
		{
			string StorageClosedTemp = null;
			if (!serialization.TryCustomCopy<string>(StorageClosed, ref StorageClosedTemp, hookCtx, false, context))
			{
				StorageClosedTemp = StorageClosed;
			}
			target.StorageClosed = StorageClosedTemp;
			string StorageOpenTemp = null;
			if (!serialization.TryCustomCopy<string>(StorageOpen, ref StorageOpenTemp, hookCtx, false, context))
			{
				StorageOpenTemp = StorageOpen;
			}
			target.StorageOpen = StorageOpenTemp;
			string StorageEmptyTemp = null;
			if (!serialization.TryCustomCopy<string>(StorageEmpty, ref StorageEmptyTemp, hookCtx, false, context))
			{
				StorageEmptyTemp = StorageEmpty;
			}
			target.StorageEmpty = StorageEmptyTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref CMStorageVisualizerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CMStorageVisualizerComponent cast = (CMStorageVisualizerComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CMStorageVisualizerComponent cast = (CMStorageVisualizerComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CMStorageVisualizerComponent def = (CMStorageVisualizerComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override CMStorageVisualizerComponent Instantiate()
	{
		return new CMStorageVisualizerComponent();
	}
}
