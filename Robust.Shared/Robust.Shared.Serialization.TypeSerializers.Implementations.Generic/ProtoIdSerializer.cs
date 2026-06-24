using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;

namespace Robust.Shared.Serialization.TypeSerializers.Implementations.Generic;

[TypeSerializer]
public sealed class ProtoIdSerializer<T> : ITypeSerializer<ProtoId<T>, ValueDataNode>, ITypeReader<ProtoId<T>, ValueDataNode>, ITypeValidator<ProtoId<T>, ValueDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<ProtoId<T>, ValueDataNode>, ITypeWriter<ProtoId<T>>, BaseSerializerInterfaces.ITypeInterface<ProtoId<T>>, ITypeCopyCreator<ProtoId<T>> where T : class, IPrototype
{
	public ValidationNode Validate(ISerializationManager serialization, ValueDataNode node, IDependencyCollection dependencies, ISerializationContext? context = null)
	{
		return Validate(dependencies, node);
	}

	public static ValidationNode Validate(IDependencyCollection deps, ValueDataNode node)
	{
		IPrototypeManager prototypeManager = deps.Resolve<IPrototypeManager>();
		if (!prototypeManager.TryGetKindFrom<T>(out string kind))
		{
			return new ErrorNode(node, $"Unknown prototype kind: {typeof(T)}");
		}
		if (prototypeManager.IsIgnored(kind))
		{
			return new ErrorNode(node, $"Attempting to validate an ignored prototype: {typeof(T)}.\nDid you forget to remove the IPrototypeManager.RegisterIgnore(\"{kind}\") call when moving a prototype to Shared?");
		}
		if (prototypeManager.HasMapping<T>(node.Value))
		{
			return new ValidatedValueNode(node);
		}
		return new ErrorNode(node, $"No {typeof(T)} found with id {node.Value}");
	}

	public ProtoId<T> Read(ISerializationManager serialization, ValueDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null, ISerializationManager.InstantiationDelegate<ProtoId<T>>? instanceProvider = null)
	{
		return new ProtoId<T>(node.Value);
	}

	public DataNode Write(ISerializationManager serialization, ProtoId<T> value, IDependencyCollection dependencies, bool alwaysWrite = false, ISerializationContext? context = null)
	{
		return new ValueDataNode(value.Id);
	}

	public ProtoId<T> CreateCopy(ISerializationManager serializationManager, ProtoId<T> source, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		return source;
	}
}
