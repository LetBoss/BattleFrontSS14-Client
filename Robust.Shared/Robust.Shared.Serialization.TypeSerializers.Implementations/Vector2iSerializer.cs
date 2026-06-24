using System.Globalization;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;
using Robust.Shared.Utility;

namespace Robust.Shared.Serialization.TypeSerializers.Implementations;

[TypeSerializer]
public sealed class Vector2iSerializer : ITypeSerializer<Vector2i, ValueDataNode>, ITypeReader<Vector2i, ValueDataNode>, ITypeValidator<Vector2i, ValueDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<Vector2i, ValueDataNode>, ITypeWriter<Vector2i>, BaseSerializerInterfaces.ITypeInterface<Vector2i>, ITypeCopyCreator<Vector2i>
{
	public Vector2i Read(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null, ISerializationManager.InstantiationDelegate<Vector2i>? instanceProvider = null)
	{
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		if (!VectorSerializerUtility.TryParseArgs(node.Value, 2, out string[] args))
		{
			throw new InvalidMappingException($"Could not parse {"Vector2i"}: '{node.Value}'");
		}
		int num = int.Parse(args[0], CultureInfo.InvariantCulture);
		int num2 = int.Parse(args[1], CultureInfo.InvariantCulture);
		return new Vector2i(num, num2);
	}

	public ValidationNode Validate(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, ISerializationContext? context = null)
	{
		if (!VectorSerializerUtility.TryParseArgs(node.Value, 2, out string[] args))
		{
			return new ErrorNode(node, "Failed parsing values for Vector2i.");
		}
		if (!int.TryParse(args[0], NumberStyles.Any, CultureInfo.InvariantCulture, out var result) || !int.TryParse(args[1], NumberStyles.Any, CultureInfo.InvariantCulture, out result))
		{
			return new ErrorNode(node, "Failed parsing values for Vector2i.");
		}
		return new ValidatedValueNode(node);
	}

	public DataNode Write(ISerializationManager serializationManager, Vector2i value, IDependencyCollection dependencies, bool alwaysWrite = false, ISerializationContext? context = null)
	{
		return new ValueDataNode(value.X.ToString(CultureInfo.InvariantCulture) + "," + value.Y.ToString(CultureInfo.InvariantCulture));
	}

	public Vector2i CreateCopy(ISerializationManager serializationManager, Vector2i source, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2i(source.X, source.Y);
	}
}
