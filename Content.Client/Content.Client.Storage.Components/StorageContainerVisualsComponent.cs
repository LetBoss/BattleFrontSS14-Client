using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Client.Storage.Components;

[RegisterComponent]
public sealed class StorageContainerVisualsComponent : Component, ISerializationGenerated<StorageContainerVisualsComponent>, ISerializationGenerated
{
	[DataField("maxFillLevels", false, 1, false, false, null)]
	public int MaxFillLevels;

	[DataField("fillBaseName", false, 1, false, false, null)]
	public string? FillBaseName;

	[DataField("layer", false, 1, false, false, null)]
	public StorageContainerVisualLayers FillLayer;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref StorageContainerVisualsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component val = (Component)(object)target;
		((Component)this).InternalCopy(ref val, serialization, hookCtx, context);
		target = (StorageContainerVisualsComponent)(object)val;
		if (!serialization.TryCustomCopy<StorageContainerVisualsComponent>(this, ref target, hookCtx, false, context))
		{
			int maxFillLevels = 0;
			if (!serialization.TryCustomCopy<int>(MaxFillLevels, ref maxFillLevels, hookCtx, false, context))
			{
				maxFillLevels = MaxFillLevels;
			}
			target.MaxFillLevels = maxFillLevels;
			string fillBaseName = null;
			if (!serialization.TryCustomCopy<string>(FillBaseName, ref fillBaseName, hookCtx, false, context))
			{
				fillBaseName = FillBaseName;
			}
			target.FillBaseName = fillBaseName;
			StorageContainerVisualLayers fillLayer = StorageContainerVisualLayers.Fill;
			if (!serialization.TryCustomCopy<StorageContainerVisualLayers>(FillLayer, ref fillLayer, hookCtx, false, context))
			{
				fillLayer = FillLayer;
			}
			target.FillLayer = fillLayer;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref StorageContainerVisualsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StorageContainerVisualsComponent target2 = (StorageContainerVisualsComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (Component)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StorageContainerVisualsComponent target2 = (StorageContainerVisualsComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StorageContainerVisualsComponent target2 = (StorageContainerVisualsComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (IComponent)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override StorageContainerVisualsComponent Instantiate()
	{
		return new StorageContainerVisualsComponent();
	}
}
