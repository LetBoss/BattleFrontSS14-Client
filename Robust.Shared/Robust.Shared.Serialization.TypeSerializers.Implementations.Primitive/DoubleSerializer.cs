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
public sealed class DoubleSerializer : ITypeSerializer<double, ValueDataNode>, ITypeReader<double, ValueDataNode>, ITypeValidator<double, ValueDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<double, ValueDataNode>, ITypeWriter<double>, BaseSerializerInterfaces.ITypeInterface<double>
{
	public ValidationNode Validate(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, ISerializationContext? context = null)
	{
		if (!Parse.TryDouble(node.Value.AsSpan(), NumberStyles.Any, out var _))
		{
			return new ErrorNode(node, "Failed parsing double value: " + node.Value);
		}
		return new ValidatedValueNode(node);
	}

	public double Read(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null, ISerializationManager.InstantiationDelegate<double>? instanceProvider = null)
	{
		return Parse.Double(node.Value.AsSpan());
	}

	public DataNode Write(ISerializationManager serializationManager, double value, IDependencyCollection dependencies, bool alwaysWrite = false, ISerializationContext? context = null)
	{
		return new ValueDataNode(value.ToString(CultureInfo.InvariantCulture));
	}
}
