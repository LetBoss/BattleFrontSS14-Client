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
public sealed class Vector4Serializer : ITypeSerializer<Vector4, ValueDataNode>, ITypeReader<Vector4, ValueDataNode>, ITypeValidator<Vector4, ValueDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<Vector4, ValueDataNode>, ITypeWriter<Vector4>, BaseSerializerInterfaces.ITypeInterface<Vector4>, ITypeCopyCreator<Vector4>
{
	public Vector4 Read(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null, ISerializationManager.InstantiationDelegate<Vector4>? instanceProvider = null)
	{
		if (!VectorSerializerUtility.TryParseArgs(node.Value, 4, out string[] args))
		{
			throw new InvalidMappingException($"Could not parse {"Vector4"}: '{node.Value}'");
		}
		float x = float.Parse(args[0], CultureInfo.InvariantCulture);
		float y = float.Parse(args[1], CultureInfo.InvariantCulture);
		float z = float.Parse(args[2], CultureInfo.InvariantCulture);
		float w = float.Parse(args[3], CultureInfo.InvariantCulture);
		return new Vector4(x, y, z, w);
	}

	public ValidationNode Validate(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, ISerializationContext? context = null)
	{
		if (!VectorSerializerUtility.TryParseArgs(node.Value, 4, out string[] args))
		{
			return new ErrorNode(node, "Failed parsing values for Vector4.");
		}
		if (!float.TryParse(args[0], NumberStyles.Any, CultureInfo.InvariantCulture, out var result) || !float.TryParse(args[1], NumberStyles.Any, CultureInfo.InvariantCulture, out result) || !float.TryParse(args[2], NumberStyles.Any, CultureInfo.InvariantCulture, out result) || !float.TryParse(args[3], NumberStyles.Any, CultureInfo.InvariantCulture, out result))
		{
			return new ErrorNode(node, "Failed parsing values for Vector4.");
		}
		return new ValidatedValueNode(node);
	}

	public DataNode Write(ISerializationManager serializationManager, Vector4 value, IDependencyCollection dependencies, bool alwaysWrite = false, ISerializationContext? context = null)
	{
		return new ValueDataNode($"{value.X.ToString(CultureInfo.InvariantCulture)},{value.Y.ToString(CultureInfo.InvariantCulture)},{value.Z.ToString(CultureInfo.InvariantCulture)},{value.W.ToString(CultureInfo.InvariantCulture)}");
	}

	public Vector4 CreateCopy(ISerializationManager serializationManager, Vector4 source, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		return source;
	}
}
