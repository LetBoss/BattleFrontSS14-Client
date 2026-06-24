using System;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;

namespace Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

public sealed class ConstantSerializer<TTag> : ITypeSerializer<int, ValueDataNode>, ITypeReader<int, ValueDataNode>, ITypeValidator<int, ValueDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<int, ValueDataNode>, ITypeWriter<int>, BaseSerializerInterfaces.ITypeInterface<int>
{
	public ValidationNode Validate(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, ISerializationContext? context = null)
	{
		if (!Enum.TryParse(serializationManager.GetConstantTypeFromTag(typeof(TTag)), node.Value, out object _))
		{
			return new ErrorNode(node, "Failed parsing constant.", alwaysRelevant: false);
		}
		return new ValidatedValueNode(node);
	}

	public int Read(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null, ISerializationManager.InstantiationDelegate<int>? instanceProvider = null)
	{
		return (int)Enum.Parse(serializationManager.GetConstantTypeFromTag(typeof(TTag)), node.Value);
	}

	public DataNode Write(ISerializationManager serializationManager, int value, IDependencyCollection dependencies, bool alwaysWrite = false, ISerializationContext? context = null)
	{
		Type constantTypeFromTag = serializationManager.GetConstantTypeFromTag(typeof(TTag));
		return new ValueDataNode(Enum.GetName(constantTypeFromTag, value) ?? throw new InvalidOperationException($"No constant corresponding to value {value} in {constantTypeFromTag}."));
	}
}
