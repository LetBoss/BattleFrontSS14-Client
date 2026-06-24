using System;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Hands.Components;

[Serializable]
[DataDefinition]
[NetSerializable]
public record struct Hand : ISerializationGenerated<Hand>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public HandLocation Location;

	public Hand()
	{
		Location = HandLocation.Right;
	}

	public Hand(HandLocation location)
	{
		Location = HandLocation.Right;
		Location = location;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref Hand target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		if (!serialization.TryCustomCopy<Hand>(this, ref target, hookCtx, false, context))
		{
			HandLocation LocationTemp = HandLocation.Left;
			if (!serialization.TryCustomCopy<HandLocation>(Location, ref LocationTemp, hookCtx, false, context))
			{
				LocationTemp = Location;
			}
			target = target with
			{
				Location = LocationTemp
			};
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref Hand target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Hand cast = (Hand)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public Hand Instantiate()
	{
		return new Hand();
	}
}
