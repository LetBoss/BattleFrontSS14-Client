using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Sequence;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;

namespace Robust.Shared.Serialization.TypeSerializers.Implementations;

[TypeSerializer]
public sealed class EntProtoIdSerializer : ITypeSerializer<EntProtoId, ValueDataNode>, ITypeReader<EntProtoId, ValueDataNode>, ITypeValidator<EntProtoId, ValueDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<EntProtoId, ValueDataNode>, ITypeWriter<EntProtoId>, BaseSerializerInterfaces.ITypeInterface<EntProtoId>, ITypeCopyCreator<EntProtoId>
{
	public ValidationNode Validate(ISerializationManager serialization, ValueDataNode node, IDependencyCollection dependencies, ISerializationContext? context = null)
	{
		IPrototypeManager prototypeManager = dependencies.Resolve<IPrototypeManager>();
		if (prototypeManager.TryGetKindFrom<EntityPrototype>(out string _) && prototypeManager.HasMapping<EntityPrototype>(node.Value))
		{
			return new ValidatedValueNode(node);
		}
		return new ErrorNode(node, "No EntityPrototype found with id " + node.Value);
	}

	public EntProtoId Read(ISerializationManager serialization, ValueDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null, ISerializationManager.InstantiationDelegate<EntProtoId>? instanceProvider = null)
	{
		return new EntProtoId(node.Value);
	}

	public DataNode Write(ISerializationManager serialization, EntProtoId value, IDependencyCollection dependencies, bool alwaysWrite = false, ISerializationContext? context = null)
	{
		return new ValueDataNode(value.Id);
	}

	public EntProtoId CreateCopy(ISerializationManager serializationManager, EntProtoId source, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		return source;
	}
}
[TypeSerializer]
public sealed class EntProtoIdSerializer<T> : ITypeSerializer<EntProtoId<T>, ValueDataNode>, ITypeReader<EntProtoId<T>, ValueDataNode>, ITypeValidator<EntProtoId<T>, ValueDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<EntProtoId<T>, ValueDataNode>, ITypeWriter<EntProtoId<T>>, BaseSerializerInterfaces.ITypeInterface<EntProtoId<T>>, ITypeCopyCreator<EntProtoId<T>> where T : IComponent, new()
{
	public ValidationNode Validate(ISerializationManager serialization, ValueDataNode node, IDependencyCollection dependencies, ISerializationContext? context = null)
	{
		IPrototypeManager prototypeManager = dependencies.Resolve<IPrototypeManager>();
		if (!prototypeManager.TryGetKindFrom<EntityPrototype>(out string _) || !prototypeManager.TryGetMapping(typeof(EntityPrototype), node.Value, out MappingDataNode mappings))
		{
			return new ErrorNode(node, $"No {"EntityPrototype"} found with id {node.Value} that has a {typeof(T).Name}");
		}
		if (!mappings.TryGet("components", out SequenceDataNode node2))
		{
			return new ErrorNode(node, $"{"EntityPrototype"} {node.Value} doesn't have a {typeof(T).Name}.");
		}
		ComponentRegistration registration = dependencies.Resolve<IComponentFactory>().GetRegistration<T>();
		foreach (DataNode item in node2)
		{
			if (item is MappingDataNode mappingDataNode && mappingDataNode.TryGet("type", out ValueDataNode node3) && node3.Value == registration.Name)
			{
				return new ValidatedValueNode(node);
			}
		}
		return new ErrorNode(node, $"{"EntityPrototype"} {node.Value} doesn't have a {typeof(T).Name}.");
	}

	public EntProtoId<T> Read(ISerializationManager serialization, ValueDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null, ISerializationManager.InstantiationDelegate<EntProtoId<T>>? instanceProvider = null)
	{
		return new EntProtoId<T>(node.Value);
	}

	public DataNode Write(ISerializationManager serialization, EntProtoId<T> value, IDependencyCollection dependencies, bool alwaysWrite = false, ISerializationContext? context = null)
	{
		return new ValueDataNode(value.Id);
	}

	public EntProtoId<T> CreateCopy(ISerializationManager serializationManager, EntProtoId<T> source, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		return source;
	}
}
