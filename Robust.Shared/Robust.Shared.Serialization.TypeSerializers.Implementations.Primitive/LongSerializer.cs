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
public sealed class LongSerializer : ITypeSerializer<long, ValueDataNode>, ITypeReader<long, ValueDataNode>, ITypeValidator<long, ValueDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<long, ValueDataNode>, ITypeWriter<long>, BaseSerializerInterfaces.ITypeInterface<long>
{
	public ValidationNode Validate(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, ISerializationContext? context = null)
	{
		if (!Parse.TryInt64(node.Value.AsSpan(), out var _))
		{
			return new ErrorNode(node, "Failed parsing long value: " + node.Value);
		}
		return new ValidatedValueNode(node);
	}

	public long Read(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null, ISerializationManager.InstantiationDelegate<long>? instanceProvider = null)
	{
		return Parse.Int64(node.Value.AsSpan());
	}

	public DataNode Write(ISerializationManager serializationManager, long value, IDependencyCollection dependencies, bool alwaysWrite = false, ISerializationContext? context = null)
	{
		return new ValueDataNode(value.ToString(CultureInfo.InvariantCulture));
	}

	public long Copy(ISerializationManager serializationManager, long source, long target, bool skipHook, ISerializationContext? context = null)
	{
		return source;
	}
}
