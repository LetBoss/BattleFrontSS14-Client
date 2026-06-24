using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Teleportation.Components;

[Serializable]
[NetSerializable]
[DataDefinition]
public record struct TeleportPoint : ISerializationGenerated<TeleportPoint>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public string Location;

	[DataField(null, false, 1, false, false, null)]
	public NetEntity TelePoint;

	public TeleportPoint(string Location, NetEntity TelePoint)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		this.Location = Location;
		this.TelePoint = TelePoint;
	}

	public TeleportPoint()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		Location = null;
		TelePoint = default(NetEntity);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref TeleportPoint target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<TeleportPoint>(this, ref target, hookCtx, false, context))
		{
			string LocationTemp = null;
			if (Location == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Location, ref LocationTemp, hookCtx, false, context))
			{
				LocationTemp = Location;
			}
			NetEntity TelePointTemp = default(NetEntity);
			if (!serialization.TryCustomCopy<NetEntity>(TelePoint, ref TelePointTemp, hookCtx, false, context))
			{
				TelePointTemp = serialization.CreateCopy<NetEntity>(TelePoint, hookCtx, context, false);
			}
			target = target with
			{
				Location = LocationTemp,
				TelePoint = TelePointTemp
			};
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref TeleportPoint target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		TeleportPoint cast = (TeleportPoint)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public TeleportPoint Instantiate()
	{
		return new TeleportPoint();
	}
}
