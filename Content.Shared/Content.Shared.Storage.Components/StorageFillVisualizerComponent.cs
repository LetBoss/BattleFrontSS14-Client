using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Storage.Components;

[RegisterComponent]
public sealed class StorageFillVisualizerComponent : Component, ISerializationGenerated<StorageFillVisualizerComponent>, ISerializationGenerated
{
	[DataField("maxFillLevels", false, 1, true, false, null)]
	public int MaxFillLevels;

	[DataField("fillBaseName", false, 1, true, false, null)]
	public string FillBaseName;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref StorageFillVisualizerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (StorageFillVisualizerComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<StorageFillVisualizerComponent>(this, ref target, hookCtx, false, context))
		{
			int MaxFillLevelsTemp = 0;
			if (!serialization.TryCustomCopy<int>(MaxFillLevels, ref MaxFillLevelsTemp, hookCtx, false, context))
			{
				MaxFillLevelsTemp = MaxFillLevels;
			}
			target.MaxFillLevels = MaxFillLevelsTemp;
			string FillBaseNameTemp = null;
			if (FillBaseName == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(FillBaseName, ref FillBaseNameTemp, hookCtx, false, context))
			{
				FillBaseNameTemp = FillBaseName;
			}
			target.FillBaseName = FillBaseNameTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref StorageFillVisualizerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StorageFillVisualizerComponent cast = (StorageFillVisualizerComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StorageFillVisualizerComponent cast = (StorageFillVisualizerComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StorageFillVisualizerComponent def = (StorageFillVisualizerComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override StorageFillVisualizerComponent Instantiate()
	{
		return new StorageFillVisualizerComponent();
	}
}
