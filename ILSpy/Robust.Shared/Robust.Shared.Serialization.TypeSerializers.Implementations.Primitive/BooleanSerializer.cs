using System.Globalization;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;

namespace Robust.Shared.Serialization.TypeSerializers.Implementations.Primitive;

[TypeSerializer]
public sealed class BooleanSerializer : ITypeSerializer<bool, ValueDataNode>, ITypeReader<bool, ValueDataNode>, ITypeValidator<bool, ValueDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<bool, ValueDataNode>, ITypeWriter<bool>, BaseSerializerInterfaces.ITypeInterface<bool>
{
	public ValidationNode Validate(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, ISerializationContext? context = null)
	{
		if (!bool.TryParse(node.Value, out var _))
		{
			return new ErrorNode(node, "Failed parsing boolean value: " + node.Value);
		}
		return new ValidatedValueNode(node);
	}

	public bool Read(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null, ISerializationManager.InstantiationDelegate<bool>? instanceProvider = null)
	{
		return bool.Parse(node.Value);
	}

	public DataNode Write(ISerializationManager serializationManager, bool value, IDependencyCollection dependencies, bool alwaysWrite = false, ISerializationContext? context = null)
	{
		return new ValueDataNode(value.ToString(CultureInfo.InvariantCulture));
	}
}
