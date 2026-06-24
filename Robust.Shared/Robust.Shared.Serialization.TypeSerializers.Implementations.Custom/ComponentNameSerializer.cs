using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;

namespace Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

public sealed class ComponentNameSerializer : ITypeSerializer<string, ValueDataNode>, ITypeReader<string, ValueDataNode>, ITypeValidator<string, ValueDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<string, ValueDataNode>, ITypeWriter<string>, BaseSerializerInterfaces.ITypeInterface<string>
{
	public ValidationNode Validate(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, ISerializationContext? context = null)
	{
		IComponentFactory componentFactory = dependencies.Resolve<IComponentFactory>();
		if (!componentFactory.TryGetRegistration(node.Value, out ComponentRegistration _) && componentFactory.GetComponentAvailability(node.Value) != ComponentAvailability.Ignore)
		{
			return new ErrorNode(node, "Unknown component kind: " + node.Value);
		}
		return new ValidatedValueNode(node);
	}

	public string Read(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null, ISerializationManager.InstantiationDelegate<string>? instanceProvider = null)
	{
		return node.Value;
	}

	public DataNode Write(ISerializationManager serializationManager, string value, IDependencyCollection dependencies, bool alwaysWrite = false, ISerializationContext? context = null)
	{
		return new ValueDataNode(value);
	}
}
