using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Construction.NodeEntities;

[DataDefinition]
public sealed class StaticNodeEntity : IGraphNodeEntity, ISerializationGenerated<StaticNodeEntity>, ISerializationGenerated
{
	[DataField("id", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string? Id { get; private set; }

	public StaticNodeEntity()
	{
	}

	public StaticNodeEntity(string id)
	{
		Id = id;
	}

	public string? GetId(EntityUid? uid, EntityUid? userUid, GraphNodeEntityArgs args)
	{
		return Id;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref StaticNodeEntity target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		if (!serialization.TryCustomCopy<StaticNodeEntity>(this, ref target, hookCtx, false, context))
		{
			string IdTemp = null;
			if (!serialization.TryCustomCopy<string>(Id, ref IdTemp, hookCtx, false, context))
			{
				IdTemp = Id;
			}
			target.Id = IdTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref StaticNodeEntity target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StaticNodeEntity cast = (StaticNodeEntity)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public StaticNodeEntity Instantiate()
	{
		return new StaticNodeEntity();
	}
}
