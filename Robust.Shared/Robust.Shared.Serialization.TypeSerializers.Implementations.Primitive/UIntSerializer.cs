using System;
using System.Globalization;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;
using Robust.Shared.Utility;

namespace Robust.Shared.Serialization.TypeSerializers.Implementations.Primitive;

[TypeSerializer]
public sealed class UIntSerializer : ITypeSerializer<uint, ValueDataNode>, ITypeReader<uint, ValueDataNode>, ITypeValidator<uint, ValueDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<uint, ValueDataNode>, ITypeWriter<uint>, BaseSerializerInterfaces.ITypeInterface<uint>
{
	public ValidationNode Validate(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, ISerializationContext? context = null)
	{
		if (!Parse.TryUInt32(node.Value.AsSpan(), out var _))
		{
			return new ErrorNode(node, "Failed parsing unsigned int value: " + node.Value);
		}
		return new ValidatedValueNode(node);
	}

	public uint Read(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null, ISerializationManager.InstantiationDelegate<uint>? instanceProvider = null)
	{
		return Parse.UInt32(node.Value.AsSpan());
	}

	public DataNode Write(ISerializationManager serializationManager, uint value, IDependencyCollection dependencies, bool alwaysWrite = false, ISerializationContext? context = null)
	{
		return new ValueDataNode(value.ToString(CultureInfo.InvariantCulture));
	}
}
