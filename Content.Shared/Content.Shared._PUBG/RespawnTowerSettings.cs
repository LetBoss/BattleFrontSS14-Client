using System;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._PUBG;

[DataDefinition]
public sealed class RespawnTowerSettings : ISerializationGenerated<RespawnTowerSettings>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public int Count;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RespawnTowerSettings target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		if (!serialization.TryCustomCopy<RespawnTowerSettings>(this, ref target, hookCtx, false, context))
		{
			int CountTemp = 0;
			if (!serialization.TryCustomCopy<int>(Count, ref CountTemp, hookCtx, false, context))
			{
				CountTemp = Count;
			}
			target.Count = CountTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RespawnTowerSettings target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RespawnTowerSettings cast = (RespawnTowerSettings)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public RespawnTowerSettings Instantiate()
	{
		return new RespawnTowerSettings();
	}
}
