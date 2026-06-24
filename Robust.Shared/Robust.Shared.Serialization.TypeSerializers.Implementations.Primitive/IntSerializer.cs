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
public sealed class IntSerializer : ITypeSerializer<int, ValueDataNode>, ITypeReader<int, ValueDataNode>, ITypeValidator<int, ValueDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<int, ValueDataNode>, ITypeWriter<int>, BaseSerializerInterfaces.ITypeInterface<int>
{
	public ValidationNode Validate(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, ISerializationContext? context = null)
	{
		if (!Parse.TryInt32(node.Value.AsSpan(), out var _))
		{
			return new ErrorNode(node, "Failed parsing int value: " + node.Value);
		}
		return new ValidatedValueNode(node);
	}

	public int Read(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null, ISerializationManager.InstantiationDelegate<int>? instanceProvider = null)
	{
		return Parse.Int32(node.Value.AsSpan());
	}

	public DataNode Write(ISerializationManager serializationManager, int value, IDependencyCollection dependencies, bool alwaysWrite = false, ISerializationContext? context = null)
	{
		return new ValueDataNode(value.ToString(CultureInfo.InvariantCulture));
	}
}
