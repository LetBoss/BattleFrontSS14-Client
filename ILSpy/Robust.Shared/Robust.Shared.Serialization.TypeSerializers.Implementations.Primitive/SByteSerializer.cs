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
public sealed class SByteSerializer : ITypeSerializer<sbyte, ValueDataNode>, ITypeReader<sbyte, ValueDataNode>, ITypeValidator<sbyte, ValueDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<sbyte, ValueDataNode>, ITypeWriter<sbyte>, BaseSerializerInterfaces.ITypeInterface<sbyte>
{
	public ValidationNode Validate(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, ISerializationContext? context = null)
	{
		if (!sbyte.TryParse(node.Value, out var _))
		{
			return new ErrorNode(node, "Failed parsing signed byte value: " + node.Value);
		}
		return new ValidatedValueNode(node);
	}

	public sbyte Read(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null, ISerializationManager.InstantiationDelegate<sbyte>? instanceProvider = null)
	{
		return sbyte.Parse(node.Value, CultureInfo.InvariantCulture);
	}

	public DataNode Write(ISerializationManager serializationManager, sbyte value, IDependencyCollection dependencies, bool alwaysWrite = false, ISerializationContext? context = null)
	{
		return new ValueDataNode(value.ToString(CultureInfo.InvariantCulture));
	}
}
