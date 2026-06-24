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
public sealed class ByteSerializer : ITypeSerializer<byte, ValueDataNode>, ITypeReader<byte, ValueDataNode>, ITypeValidator<byte, ValueDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<byte, ValueDataNode>, ITypeWriter<byte>, BaseSerializerInterfaces.ITypeInterface<byte>
{
	public ValidationNode Validate(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, ISerializationContext? context = null)
	{
		if (!byte.TryParse(node.Value, out var _))
		{
			return new ErrorNode(node, "Failed parsing byte value: " + node.Value);
		}
		return new ValidatedValueNode(node);
	}

	public byte Read(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null, ISerializationManager.InstantiationDelegate<byte>? instanceProvider = null)
	{
		return byte.Parse(node.Value, CultureInfo.InvariantCulture);
	}

	public DataNode Write(ISerializationManager serializationManager, byte value, IDependencyCollection dependencies, bool alwaysWrite = false, ISerializationContext? context = null)
	{
		return new ValueDataNode(value.ToString(CultureInfo.InvariantCulture));
	}
}
