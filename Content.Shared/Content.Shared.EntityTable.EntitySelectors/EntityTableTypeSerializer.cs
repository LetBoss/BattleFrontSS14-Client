using System;
using Robust.Shared.IoC;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;

namespace Content.Shared.EntityTable.EntitySelectors;

[TypeSerializer]
public sealed class EntityTableTypeSerializer : ITypeReader<EntityTableSelector, MappingDataNode>, ITypeValidator<EntityTableSelector, MappingDataNode>, ITypeNodeInterface<EntityTableSelector, MappingDataNode>
{
	public ValidationNode Validate(ISerializationManager serializationManager, MappingDataNode node, IDependencyCollection dependencies, ISerializationContext? context = null)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected O, but got Unknown
		if (node.Has("id"))
		{
			return serializationManager.ValidateNode<EntSelector>((DataNode)(object)node, context);
		}
		return (ValidationNode)new ErrorNode((DataNode)(object)node, "Custom validation not supported! Please specify the type manually!", true);
	}

	public EntityTableSelector Read(ISerializationManager serializationManager, MappingDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null, InstantiationDelegate<EntityTableSelector>? instanceProvider = null)
	{
		Type type = typeof(EntityTableSelector);
		if (node.Has("id"))
		{
			type = typeof(EntSelector);
		}
		return (EntityTableSelector)serializationManager.Read(type, (DataNode)(object)node, context, false, false);
	}
}
