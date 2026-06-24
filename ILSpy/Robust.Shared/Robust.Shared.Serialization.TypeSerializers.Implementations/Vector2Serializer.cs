using System.Globalization;
using System.Numerics;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;
using Robust.Shared.Utility;

namespace Robust.Shared.Serialization.TypeSerializers.Implementations;

[TypeSerializer]
public sealed class Vector2Serializer : ITypeSerializer<Vector2, ValueDataNode>, ITypeReader<Vector2, ValueDataNode>, ITypeValidator<Vector2, ValueDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<Vector2, ValueDataNode>, ITypeWriter<Vector2>, BaseSerializerInterfaces.ITypeInterface<Vector2>, ITypeCopyCreator<Vector2>
{
	public Vector2 Read(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null, ISerializationManager.InstantiationDelegate<Vector2>? instanceProvider = null)
	{
		if (!VectorSerializerUtility.TryParseArgs(node.Value, 2, out string[] args))
		{
			throw new InvalidMappingException($"Could not parse {"Vector2"}: '{node.Value}'");
		}
		float x = float.Parse(args[0], CultureInfo.InvariantCulture);
		float y = float.Parse(args[1], CultureInfo.InvariantCulture);
		return new Vector2(x, y);
	}

	public ValidationNode Validate(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, ISerializationContext? context = null)
	{
		if (!VectorSerializerUtility.TryParseArgs(node.Value, 2, out string[] args))
		{
			return new ErrorNode(node, "Failed parsing values for Vector2.");
		}
		if (!float.TryParse(args[0], NumberStyles.Any, CultureInfo.InvariantCulture, out var result) || !float.TryParse(args[1], NumberStyles.Any, CultureInfo.InvariantCulture, out result))
		{
			return new ErrorNode(node, "Failed parsing values for Vector2.");
		}
		return new ValidatedValueNode(node);
	}

	public DataNode Write(ISerializationManager serializationManager, Vector2 value, IDependencyCollection dependencies, bool alwaysWrite = false, ISerializationContext? context = null)
	{
		return new ValueDataNode(value.X.ToString(CultureInfo.InvariantCulture) + "," + value.Y.ToString(CultureInfo.InvariantCulture));
	}

	public Vector2 CreateCopy(ISerializationManager serializationManager, Vector2 source, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		return new Vector2(source.X, source.Y);
	}
}
