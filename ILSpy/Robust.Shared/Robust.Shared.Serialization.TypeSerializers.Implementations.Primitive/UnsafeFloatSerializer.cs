using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;

namespace Robust.Shared.Serialization.TypeSerializers.Implementations.Primitive;

[TypeSerializer]
internal sealed class UnsafeFloatSerializer : ITypeSerializer<UnsafeFloat, ValueDataNode>, ITypeReader<UnsafeFloat, ValueDataNode>, ITypeValidator<UnsafeFloat, ValueDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<UnsafeFloat, ValueDataNode>, ITypeWriter<UnsafeFloat>, BaseSerializerInterfaces.ITypeInterface<UnsafeFloat>, ITypeCopyCreator<UnsafeFloat>, ITypeSerializer<UnsafeDouble, ValueDataNode>, ITypeReader<UnsafeDouble, ValueDataNode>, ITypeValidator<UnsafeDouble, ValueDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<UnsafeDouble, ValueDataNode>, ITypeWriter<UnsafeDouble>, BaseSerializerInterfaces.ITypeInterface<UnsafeDouble>, ITypeCopyCreator<UnsafeDouble>
{
	ValidationNode ITypeValidator<UnsafeFloat, ValueDataNode>.Validate(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, ISerializationContext? context)
	{
		return serializationManager.ValidateNode<float>(node, context);
	}

	public UnsafeFloat Read(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null, ISerializationManager.InstantiationDelegate<UnsafeFloat>? instanceProvider = null)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		return UnsafeFloat.op_Implicit(serializationManager.Read<float>(node, hookCtx, context));
	}

	public DataNode Write(ISerializationManager serializationManager, UnsafeFloat value, IDependencyCollection dependencies, bool alwaysWrite = false, ISerializationContext? context = null)
	{
		return serializationManager.WriteValue(((UnsafeFloat)(ref value)).Value, alwaysWrite, context);
	}

	ValidationNode ITypeValidator<UnsafeDouble, ValueDataNode>.Validate(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, ISerializationContext? context)
	{
		return serializationManager.ValidateNode<double>(node, context);
	}

	public UnsafeDouble Read(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null, ISerializationManager.InstantiationDelegate<UnsafeDouble>? instanceProvider = null)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		return UnsafeDouble.op_Implicit(serializationManager.Read<double>(node, hookCtx, context));
	}

	public DataNode Write(ISerializationManager serializationManager, UnsafeDouble value, IDependencyCollection dependencies, bool alwaysWrite = false, ISerializationContext? context = null)
	{
		return serializationManager.WriteValue(((UnsafeDouble)(ref value)).Value, alwaysWrite, context);
	}

	public UnsafeFloat CreateCopy(ISerializationManager serializationManager, UnsafeFloat source, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return source;
	}

	public UnsafeDouble CreateCopy(ISerializationManager serializationManager, UnsafeDouble source, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return source;
	}
}
