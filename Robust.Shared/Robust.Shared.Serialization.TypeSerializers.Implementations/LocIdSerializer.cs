using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;

namespace Robust.Shared.Serialization.TypeSerializers.Implementations;

[TypeSerializer]
public sealed class LocIdSerializer : ITypeSerializer<LocId, ValueDataNode>, ITypeReader<LocId, ValueDataNode>, ITypeValidator<LocId, ValueDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<LocId, ValueDataNode>, ITypeWriter<LocId>, BaseSerializerInterfaces.ITypeInterface<LocId>, ITypeCopyCreator<LocId>
{
	public ValidationNode Validate(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, ISerializationContext? context = null)
	{
		if (dependencies.Resolve<ILocalizationManager>().HasString(node.Value))
		{
			return new ValidatedValueNode(node);
		}
		return new ErrorNode(node, "No localization message found with id " + node.Value);
	}

	public LocId Read(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null, ISerializationManager.InstantiationDelegate<LocId>? instanceProvider = null)
	{
		return new LocId(node.Value);
	}

	public DataNode Write(ISerializationManager serializationManager, LocId value, IDependencyCollection dependencies, bool alwaysWrite = false, ISerializationContext? context = null)
	{
		return new ValueDataNode((string)value);
	}

	public LocId CreateCopy(ISerializationManager serializationManager, LocId source, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		return source;
	}
}
