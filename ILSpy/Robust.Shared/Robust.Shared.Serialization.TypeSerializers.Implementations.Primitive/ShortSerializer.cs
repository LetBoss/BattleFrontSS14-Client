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
public sealed class ShortSerializer : ITypeSerializer<short, ValueDataNode>, ITypeReader<short, ValueDataNode>, ITypeValidator<short, ValueDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<short, ValueDataNode>, ITypeWriter<short>, BaseSerializerInterfaces.ITypeInterface<short>
{
	public ValidationNode Validate(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, ISerializationContext? context = null)
	{
		if (!Parse.TryInt16(node.Value.AsSpan(), out var _))
		{
			return new ErrorNode(node, "Failed parsing short value: " + node.Value);
		}
		return new ValidatedValueNode(node);
	}

	public short Read(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null, ISerializationManager.InstantiationDelegate<short>? instanceProvider = null)
	{
		return Parse.Int16(node.Value.AsSpan());
	}

	public DataNode Write(ISerializationManager serializationManager, short value, IDependencyCollection dependencies, bool alwaysWrite = false, ISerializationContext? context = null)
	{
		return new ValueDataNode(value.ToString(CultureInfo.InvariantCulture));
	}
}
