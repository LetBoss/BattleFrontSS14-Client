using System;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;

namespace Robust.Shared.Serialization.TypeSerializers.Implementations;

[TypeSerializer]
public sealed class EnumSerializer : ITypeSerializer<Enum, ValueDataNode>, ITypeReader<Enum, ValueDataNode>, ITypeValidator<Enum, ValueDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<Enum, ValueDataNode>, ITypeWriter<Enum>, BaseSerializerInterfaces.ITypeInterface<Enum>
{
	public ValidationNode Validate(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, ISerializationContext? context = null)
	{
		if (serializationManager.ReflectionManager.TryParseEnumReference(node.Value, out Enum _, shouldThrow: false))
		{
			return new ValidatedValueNode(node);
		}
		return new ErrorNode(node, "Failed to parse enum " + node.Value);
	}

	public Enum Read(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null, ISerializationManager.InstantiationDelegate<Enum>? instanceProvider = null)
	{
		if (serializationManager.ReflectionManager.TryParseEnumReference(node.Value, out Enum @enum))
		{
			return @enum;
		}
		throw new ArgumentException("Failed to parse enum " + node.Value);
	}

	public DataNode Write(ISerializationManager serializationManager, Enum value, IDependencyCollection dependencies, bool alwaysWrite = false, ISerializationContext? context = null)
	{
		return new ValueDataNode(serializationManager.ReflectionManager.GetEnumReference(value));
	}
}
