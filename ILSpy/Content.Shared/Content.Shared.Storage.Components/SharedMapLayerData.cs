using System;
using Content.Shared.Whitelist;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Storage.Components;

[Serializable]
[DataDefinition]
public sealed class SharedMapLayerData : ISerializationGenerated<SharedMapLayerData>, ISerializationGenerated
{
	public string Layer = string.Empty;

	[DataField(null, false, 1, false, false, null)]
	public int MinCount = 1;

	[DataField(null, false, 1, false, false, null)]
	public int MaxCount = int.MaxValue;

	[DataField(null, false, 1, true, false, null)]
	public EntityWhitelist? Whitelist { get; set; }

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref SharedMapLayerData target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		if (serialization.TryCustomCopy<SharedMapLayerData>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		EntityWhitelist WhitelistTemp = null;
		if (!serialization.TryCustomCopy<EntityWhitelist>(Whitelist, ref WhitelistTemp, hookCtx, false, context))
		{
			if (Whitelist == null)
			{
				WhitelistTemp = null;
			}
			else
			{
				serialization.CopyTo<EntityWhitelist>(Whitelist, ref WhitelistTemp, hookCtx, context, false);
			}
		}
		target.Whitelist = WhitelistTemp;
		int MinCountTemp = 0;
		if (!serialization.TryCustomCopy<int>(MinCount, ref MinCountTemp, hookCtx, false, context))
		{
			MinCountTemp = MinCount;
		}
		target.MinCount = MinCountTemp;
		int MaxCountTemp = 0;
		if (!serialization.TryCustomCopy<int>(MaxCount, ref MaxCountTemp, hookCtx, false, context))
		{
			MaxCountTemp = MaxCount;
		}
		target.MaxCount = MaxCountTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref SharedMapLayerData target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SharedMapLayerData cast = (SharedMapLayerData)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public SharedMapLayerData Instantiate()
	{
		return new SharedMapLayerData();
	}
}
