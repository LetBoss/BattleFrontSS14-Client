using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Respawn;

[RegisterComponent]
[NetworkedComponent]
public sealed class SpecialRespawnComponent : Component, ISerializationGenerated<SpecialRespawnComponent>, ISerializationGenerated
{
	[ViewVariables]
	[DataField("stationMap", false, 1, false, false, null)]
	public (EntityUid?, EntityUid?) StationMap;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("respawn", false, 1, false, false, null)]
	public bool Respawn = true;

	[ViewVariables]
	[DataField("prototype", false, 1, true, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
	public string Prototype = "";

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref SpecialRespawnComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (SpecialRespawnComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<SpecialRespawnComponent>(this, ref target, hookCtx, false, context))
		{
			(EntityUid?, EntityUid?) StationMapTemp = default((EntityUid?, EntityUid?));
			if (!serialization.TryCustomCopy<(EntityUid?, EntityUid?)>(StationMap, ref StationMapTemp, hookCtx, false, context))
			{
				StationMapTemp = serialization.CreateCopy<(EntityUid?, EntityUid?)>(StationMap, hookCtx, context, false);
			}
			target.StationMap = StationMapTemp;
			bool RespawnTemp = false;
			if (!serialization.TryCustomCopy<bool>(Respawn, ref RespawnTemp, hookCtx, false, context))
			{
				RespawnTemp = Respawn;
			}
			target.Respawn = RespawnTemp;
			string PrototypeTemp = null;
			if (Prototype == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Prototype, ref PrototypeTemp, hookCtx, false, context))
			{
				PrototypeTemp = Prototype;
			}
			target.Prototype = PrototypeTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref SpecialRespawnComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SpecialRespawnComponent cast = (SpecialRespawnComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SpecialRespawnComponent cast = (SpecialRespawnComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SpecialRespawnComponent def = (SpecialRespawnComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override SpecialRespawnComponent Instantiate()
	{
		return new SpecialRespawnComponent();
	}
}
