using Content.Shared.Construction.NodeEntities;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;

namespace Content.Shared.Construction.Serialization;

public sealed class GraphNodeEntitySerializer : ITypeSerializer<IGraphNodeEntity, ValueDataNode>, ITypeReader<IGraphNodeEntity, ValueDataNode>, ITypeValidator<IGraphNodeEntity, ValueDataNode>, ITypeNodeInterface<IGraphNodeEntity, ValueDataNode>, ITypeWriter<IGraphNodeEntity>, ITypeInterface<IGraphNodeEntity>, ITypeSerializer<IGraphNodeEntity, MappingDataNode>, ITypeReader<IGraphNodeEntity, MappingDataNode>, ITypeValidator<IGraphNodeEntity, MappingDataNode>, ITypeNodeInterface<IGraphNodeEntity, MappingDataNode>
{
	public ValidationNode Validate(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, ISerializationContext? context = null)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected O, but got Unknown
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected O, but got Unknown
		string id = node.Value;
		if (dependencies.Resolve<IPrototypeManager>().HasIndex<EntityPrototype>(id))
		{
			return (ValidationNode)new ValidatedValueNode((DataNode)(object)node);
		}
		return (ValidationNode)new ErrorNode((DataNode)(object)node, "Entity Prototype " + id + " was not found!", true);
	}

	public IGraphNodeEntity Read(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null, InstantiationDelegate<IGraphNodeEntity>? instanceProvider = null)
	{
		return new StaticNodeEntity(node.Value);
	}

	public ValidationNode Validate(ISerializationManager serializationManager, MappingDataNode node, IDependencyCollection dependencies, ISerializationContext? context = null)
	{
		return serializationManager.ValidateNode<IGraphNodeEntity>((DataNode)(object)node, context);
	}

	public IGraphNodeEntity Read(ISerializationManager serializationManager, MappingDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null, InstantiationDelegate<IGraphNodeEntity>? instanceProvider = null)
	{
		return serializationManager.Read<IGraphNodeEntity>((DataNode)(object)node, hookCtx, context, instanceProvider, false);
	}

	public DataNode Write(ISerializationManager serializationManager, IGraphNodeEntity value, IDependencyCollection dependencies, bool alwaysWrite = false, ISerializationContext? context = null)
	{
		return serializationManager.WriteValue<IGraphNodeEntity>(value, alwaysWrite, context, false);
	}
}
