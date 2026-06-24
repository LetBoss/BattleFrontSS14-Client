using System;
using System.Collections.Generic;
using Robust.Shared.IoC;
using Robust.Shared.Reflection;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;

namespace Robust.Shared.Serialization.TypeSerializers.Implementations;

[TypeSerializer]
public sealed class TypeSerializer : ITypeSerializer<Type, ValueDataNode>, ITypeReader<Type, ValueDataNode>, ITypeValidator<Type, ValueDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<Type, ValueDataNode>, ITypeWriter<Type>, BaseSerializerInterfaces.ITypeInterface<Type>, ITypeCopyCreator<Type>
{
	private static readonly Dictionary<string, Type> Shortcuts = new Dictionary<string, Type> { 
	{
		"bool",
		typeof(bool)
	} };

	public ValidationNode Validate(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, ISerializationContext? context = null)
	{
		if (Shortcuts.ContainsKey(node.Value))
		{
			return new ValidatedValueNode(node);
		}
		if (!(dependencies.Resolve<IReflectionManager>().GetType(node.Value) == null))
		{
			return new ValidatedValueNode(node);
		}
		return new ErrorNode(node, "Type '" + node.Value + "' not found.");
	}

	public Type Read(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null, ISerializationManager.InstantiationDelegate<Type>? instanceProvider = null)
	{
		if (Shortcuts.TryGetValue(node.Value, out Type value))
		{
			return value;
		}
		Type type = dependencies.Resolve<IReflectionManager>().GetType(node.Value);
		if (!(type == null))
		{
			return type;
		}
		throw new InvalidMappingException("Type '" + node.Value + "' not found.");
	}

	public DataNode Write(ISerializationManager serializationManager, Type value, IDependencyCollection dependencies, bool alwaysWrite = false, ISerializationContext? context = null)
	{
		return new ValueDataNode(value.FullName ?? value.Name);
	}

	public Type CreateCopy(ISerializationManager serializationManager, Type source, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		return source;
	}
}
