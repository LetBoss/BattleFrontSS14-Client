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
public sealed class CharSerializer : ITypeSerializer<char, ValueDataNode>, ITypeReader<char, ValueDataNode>, ITypeValidator<char, ValueDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<char, ValueDataNode>, ITypeWriter<char>, BaseSerializerInterfaces.ITypeInterface<char>
{
	public ValidationNode Validate(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, ISerializationContext? context = null)
	{
		if (!char.TryParse(node.Value, out var _))
		{
			return new ErrorNode(node, "Failed parsing char value: " + node.Value);
		}
		return new ValidatedValueNode(node);
	}

	public char Read(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null, ISerializationManager.InstantiationDelegate<char>? instanceProvider = null)
	{
		return char.Parse(node.Value);
	}

	public DataNode Write(ISerializationManager serializationManager, char value, IDependencyCollection dependencies, bool alwaysWrite = false, ISerializationContext? context = null)
	{
		return new ValueDataNode(value.ToString(CultureInfo.InvariantCulture));
	}

	public char Copy(ISerializationManager serializationManager, char source, char target, bool skipHook, ISerializationContext? context = null)
	{
		return source;
	}
}
