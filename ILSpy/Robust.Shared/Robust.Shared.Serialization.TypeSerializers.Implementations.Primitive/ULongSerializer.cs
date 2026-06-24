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
public sealed class ULongSerializer : ITypeSerializer<ulong, ValueDataNode>, ITypeReader<ulong, ValueDataNode>, ITypeValidator<ulong, ValueDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<ulong, ValueDataNode>, ITypeWriter<ulong>, BaseSerializerInterfaces.ITypeInterface<ulong>
{
	public ValidationNode Validate(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, ISerializationContext? context = null)
	{
		if (!Parse.TryUInt64(node.Value.AsSpan(), out var _))
		{
			return new ErrorNode(node, "Failed parsing unsigned long value: " + node.Value);
		}
		return new ValidatedValueNode(node);
	}

	public ulong Read(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null, ISerializationManager.InstantiationDelegate<ulong>? instanceProvider = null)
	{
		return Parse.UInt64(node.ToString().AsSpan());
	}

	public DataNode Write(ISerializationManager serializationManager, ulong value, IDependencyCollection dependencies, bool alwaysWrite = false, ISerializationContext? context = null)
	{
		return new ValueDataNode(value.ToString(CultureInfo.InvariantCulture));
	}
}
