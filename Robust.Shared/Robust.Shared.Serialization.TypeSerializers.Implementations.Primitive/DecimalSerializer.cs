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
public sealed class DecimalSerializer : ITypeSerializer<decimal, ValueDataNode>, ITypeReader<decimal, ValueDataNode>, ITypeValidator<decimal, ValueDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<decimal, ValueDataNode>, ITypeWriter<decimal>, BaseSerializerInterfaces.ITypeInterface<decimal>
{
	public ValidationNode Validate(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, ISerializationContext? context = null)
	{
		if (!Parse.TryDecimal(node.Value.AsSpan(), out var _))
		{
			return new ErrorNode(node, "Failed parsing decimal value: " + node.Value);
		}
		return new ValidatedValueNode(node);
	}

	public decimal Read(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null, ISerializationManager.InstantiationDelegate<decimal>? instanceProvider = null)
	{
		return Parse.Decimal(node.Value.AsSpan());
	}

	public DataNode Write(ISerializationManager serializationManager, decimal value, IDependencyCollection dependencies, bool alwaysWrite = false, ISerializationContext? context = null)
	{
		return new ValueDataNode(value.ToString(CultureInfo.InvariantCulture));
	}
}
