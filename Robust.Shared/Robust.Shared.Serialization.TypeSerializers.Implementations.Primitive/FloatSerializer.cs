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
public sealed class FloatSerializer : ITypeSerializer<float, ValueDataNode>, ITypeReader<float, ValueDataNode>, ITypeValidator<float, ValueDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<float, ValueDataNode>, ITypeWriter<float>, BaseSerializerInterfaces.ITypeInterface<float>
{
	public ValidationNode Validate(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, ISerializationContext? context = null)
	{
		if (!float.TryParse(node.Value, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var _))
		{
			return new ErrorNode(node, "Failed parsing float value: " + node.Value);
		}
		return new ValidatedValueNode(node);
	}

	public float Read(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null, ISerializationManager.InstantiationDelegate<float>? instanceProvider = null)
	{
		return float.Parse(node.Value, CultureInfo.InvariantCulture);
	}

	public DataNode Write(ISerializationManager serializationManager, float value, IDependencyCollection dependencies, bool alwaysWrite = false, ISerializationContext? context = null)
	{
		return new ValueDataNode(value.ToString(CultureInfo.InvariantCulture));
	}
}
