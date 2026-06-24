using System;
using Robust.Shared.IoC;
using Robust.Shared.Reflection;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;

namespace Robust.Shared.Serialization.TypeSerializers.Implementations.Generic;

[TypeSerializer]
public sealed class ObjectSerializer : ITypeSerializer<object, ValueDataNode>, ITypeReader<object, ValueDataNode>, ITypeValidator<object, ValueDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<object, ValueDataNode>, ITypeWriter<object>, BaseSerializerInterfaces.ITypeInterface<object>, ITypeCopier<object>
{
	public ValidationNode Validate(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, ISerializationContext? context = null)
	{
		IReflectionManager reflectionManager = dependencies.Resolve<IReflectionManager>();
		if (node.Tag != null)
		{
			string tag = node.Tag;
			string text = tag.Substring(6, tag.Length - 6);
			if (!reflectionManager.TryLooseGetType(text, out Type type))
			{
				return new ErrorNode(node, "Unable to find type for " + text);
			}
			return serializationManager.ValidateNode(type, node, context);
		}
		return new ErrorNode(node, $"Unable to find type for {node}");
	}

	public object Read(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null, ISerializationManager.InstantiationDelegate<object>? instanceProvider = null)
	{
		IReflectionManager reflectionManager = dependencies.Resolve<IReflectionManager>();
		object obj = ((instanceProvider != null) ? instanceProvider() : new object());
		if (node.Tag != null)
		{
			string tag = node.Tag;
			string text = tag.Substring(6, tag.Length - 6);
			if (!reflectionManager.TryLooseGetType(text, out Type type))
			{
				throw new NullReferenceException("Found null type for " + text);
			}
			obj = serializationManager.Read(type, node, hookCtx, context);
			if (obj == null)
			{
				throw new NullReferenceException($"Found null data for {node}, expected {type}");
			}
		}
		return obj;
	}

	public DataNode Write(ISerializationManager serializationManager, object value, IDependencyCollection dependencies, bool alwaysWrite = false, ISerializationContext? context = null)
	{
		return serializationManager.WriteValue(value.GetType(), value) ?? throw new NullReferenceException($"Attempted to write node with type {value.GetType()}, node returned null");
	}

	public void CopyTo(ISerializationManager serializationManager, object source, ref object target, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		target = source;
	}
}
