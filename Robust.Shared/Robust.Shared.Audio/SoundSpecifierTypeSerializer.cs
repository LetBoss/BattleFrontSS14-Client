using System;
using Robust.Shared.IoC;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;
using Robust.Shared.Utility;

namespace Robust.Shared.Audio;

[TypeSerializer]
public sealed class SoundSpecifierTypeSerializer : ITypeReader<SoundSpecifier, MappingDataNode>, ITypeValidator<SoundSpecifier, MappingDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<SoundSpecifier, MappingDataNode>, ITypeReader<SoundSpecifier, ValueDataNode>, ITypeValidator<SoundSpecifier, ValueDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<SoundSpecifier, ValueDataNode>
{
	private Type GetType(MappingDataNode node)
	{
		bool flag = node.Has("path");
		bool flag2 = node.Has("collection");
		if (flag || !(flag ^ flag2))
		{
			return typeof(SoundPathSpecifier);
		}
		if (flag2)
		{
			return typeof(SoundCollectionSpecifier);
		}
		return typeof(SoundPathSpecifier);
	}

	public SoundSpecifier Read(ISerializationManager serializationManager, MappingDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null, ISerializationManager.InstantiationDelegate<SoundSpecifier>? instanceProvider = null)
	{
		Type type = GetType(node);
		return (SoundSpecifier)serializationManager.Read(type, node, hookCtx, context);
	}

	public SoundSpecifier Read(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null, ISerializationManager.InstantiationDelegate<SoundSpecifier>? instanceProvider = null)
	{
		return new SoundPathSpecifier(node.Value);
	}

	public ValidationNode Validate(ISerializationManager serializationManager, MappingDataNode node, IDependencyCollection dependencies, ISerializationContext? context = null)
	{
		if (node.Has("path") && node.Has("collection"))
		{
			return new ErrorNode(node, "You can only specify either a sound path or a sound collection!");
		}
		if (!node.Has("path") && !node.Has("collection"))
		{
			return new ErrorNode(node, "You need to specify either a sound path or a sound collection!");
		}
		return serializationManager.ValidateNode(GetType(node), node, context);
	}

	public ValidationNode Validate(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, ISerializationContext? context = null)
	{
		if (!(serializationManager.ValidateNode<ResPath>(node, context) is ErrorNode))
		{
			return new ValidatedValueNode(node);
		}
		return new ErrorNode(node, "SoundSpecifier value is not a valid resource path!");
	}
}
