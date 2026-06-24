using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Pinpointer;

[RegisterComponent]
public sealed class StationMapComponent : Component, ISerializationGenerated<StationMapComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public bool ShowLocation = true;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref StationMapComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (StationMapComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<StationMapComponent>(this, ref target, hookCtx, false, context))
		{
			bool ShowLocationTemp = false;
			if (!serialization.TryCustomCopy<bool>(ShowLocation, ref ShowLocationTemp, hookCtx, false, context))
			{
				ShowLocationTemp = ShowLocation;
			}
			target.ShowLocation = ShowLocationTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref StationMapComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StationMapComponent cast = (StationMapComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StationMapComponent cast = (StationMapComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StationMapComponent def = (StationMapComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override StationMapComponent Instantiate()
	{
		return new StationMapComponent();
	}
}
