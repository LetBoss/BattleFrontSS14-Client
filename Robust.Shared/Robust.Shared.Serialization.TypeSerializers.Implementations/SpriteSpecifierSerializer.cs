using System;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;
using Robust.Shared.Utility;

namespace Robust.Shared.Serialization.TypeSerializers.Implementations;

public abstract class SpriteSpecifierSerializer : ITypeSerializer<SpriteSpecifier.Texture, ValueDataNode>, ITypeReader<SpriteSpecifier.Texture, ValueDataNode>, ITypeValidator<SpriteSpecifier.Texture, ValueDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<SpriteSpecifier.Texture, ValueDataNode>, ITypeWriter<SpriteSpecifier.Texture>, BaseSerializerInterfaces.ITypeInterface<SpriteSpecifier.Texture>, ITypeSerializer<SpriteSpecifier.EntityPrototype, ValueDataNode>, ITypeReader<SpriteSpecifier.EntityPrototype, ValueDataNode>, ITypeValidator<SpriteSpecifier.EntityPrototype, ValueDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<SpriteSpecifier.EntityPrototype, ValueDataNode>, ITypeWriter<SpriteSpecifier.EntityPrototype>, BaseSerializerInterfaces.ITypeInterface<SpriteSpecifier.EntityPrototype>, ITypeSerializer<SpriteSpecifier.Rsi, MappingDataNode>, ITypeReader<SpriteSpecifier.Rsi, MappingDataNode>, ITypeValidator<SpriteSpecifier.Rsi, MappingDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<SpriteSpecifier.Rsi, MappingDataNode>, ITypeWriter<SpriteSpecifier.Rsi>, BaseSerializerInterfaces.ITypeInterface<SpriteSpecifier.Rsi>, ITypeSerializer<SpriteSpecifier, MappingDataNode>, ITypeReader<SpriteSpecifier, MappingDataNode>, ITypeValidator<SpriteSpecifier, MappingDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<SpriteSpecifier, MappingDataNode>, ITypeWriter<SpriteSpecifier>, BaseSerializerInterfaces.ITypeInterface<SpriteSpecifier>, ITypeSerializer<SpriteSpecifier, ValueDataNode>, ITypeReader<SpriteSpecifier, ValueDataNode>, ITypeValidator<SpriteSpecifier, ValueDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<SpriteSpecifier, ValueDataNode>, ITypeCopyCreator<SpriteSpecifier>, ITypeCopyCreator<SpriteSpecifier.Rsi>, ITypeCopyCreator<SpriteSpecifier.Texture>, ITypeCopyCreator<SpriteSpecifier.EntityPrototype>, ITypeCopier<SpriteSpecifier.Rsi>, ITypeCopier<SpriteSpecifier.Texture>
{
	public static readonly ResPath TextureRoot = new ResPath("/Textures");

	SpriteSpecifier.Texture ITypeReader<SpriteSpecifier.Texture, ValueDataNode>.Read(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context, ISerializationManager.InstantiationDelegate<SpriteSpecifier.Texture>? instanceProvider)
	{
		return new SpriteSpecifier.Texture(serializationManager.Read<ResPath>(node, hookCtx, context));
	}

	SpriteSpecifier ITypeReader<SpriteSpecifier, ValueDataNode>.Read(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context, ISerializationManager.InstantiationDelegate<SpriteSpecifier>? instanceProvider)
	{
		return ((ITypeReader<SpriteSpecifier.Texture, ValueDataNode>)this).Read(serializationManager, node, dependencies, hookCtx, context, (ISerializationManager.InstantiationDelegate<SpriteSpecifier.Texture>)instanceProvider);
	}

	SpriteSpecifier.EntityPrototype ITypeReader<SpriteSpecifier.EntityPrototype, ValueDataNode>.Read(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context, ISerializationManager.InstantiationDelegate<SpriteSpecifier.EntityPrototype>? instanceProvider)
	{
		return new SpriteSpecifier.EntityPrototype(node.Value);
	}

	SpriteSpecifier.Rsi ITypeReader<SpriteSpecifier.Rsi, MappingDataNode>.Read(ISerializationManager serializationManager, MappingDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context, ISerializationManager.InstantiationDelegate<SpriteSpecifier.Rsi>? instanceProvider)
	{
		if (!node.TryGet("sprite", out DataNode node2))
		{
			throw new InvalidMappingException("Expected sprite-node");
		}
		if (!node.TryGet("state", out DataNode node3) || !(node3 is ValueDataNode valueDataNode))
		{
			throw new InvalidMappingException("Expected state-node as a valuenode");
		}
		return new SpriteSpecifier.Rsi(serializationManager.Read<ResPath>(node2, hookCtx, context), valueDataNode.Value);
	}

	SpriteSpecifier ITypeReader<SpriteSpecifier, MappingDataNode>.Read(ISerializationManager serializationManager, MappingDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context, ISerializationManager.InstantiationDelegate<SpriteSpecifier>? instanceProvider)
	{
		if (node.TryGet("entity", out DataNode node2) && node2 is ValueDataNode node3)
		{
			return ((ITypeReader<SpriteSpecifier.EntityPrototype, ValueDataNode>)this).Read(serializationManager, node3, dependencies, hookCtx, context, (ISerializationManager.InstantiationDelegate<SpriteSpecifier.EntityPrototype>)instanceProvider);
		}
		return ((ITypeReader<SpriteSpecifier.Rsi, MappingDataNode>)this).Read(serializationManager, node, dependencies, hookCtx, context, (ISerializationManager.InstantiationDelegate<SpriteSpecifier.Rsi>)instanceProvider);
	}

	ValidationNode ITypeValidator<SpriteSpecifier, ValueDataNode>.Validate(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, ISerializationContext? context)
	{
		return ((ITypeValidator<SpriteSpecifier.Texture, ValueDataNode>)this).Validate(serializationManager, node, dependencies, context);
	}

	ValidationNode ITypeValidator<SpriteSpecifier.EntityPrototype, ValueDataNode>.Validate(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, ISerializationContext? context)
	{
		if (dependencies.Resolve<IPrototypeManager>().HasIndex<EntityPrototype>(node.Value))
		{
			return new ValidatedValueNode(node);
		}
		return new ErrorNode(node, "Invalid EntityPrototype id");
	}

	ValidationNode ITypeValidator<SpriteSpecifier.Texture, ValueDataNode>.Validate(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, ISerializationContext? context)
	{
		ResPath resPath = TextureRoot / node.Value;
		if (resPath.ToString().Contains(".rsi/"))
		{
			return new ErrorNode(node, "Texture paths may not be inside RSI files.");
		}
		return serializationManager.ValidateNode<ResPath>(new ValueDataNode(resPath.ToString()), context);
	}

	ValidationNode ITypeValidator<SpriteSpecifier, MappingDataNode>.Validate(ISerializationManager serializationManager, MappingDataNode node, IDependencyCollection dependencies, ISerializationContext? context)
	{
		if (node.TryGet("entity", out DataNode node2))
		{
			if (node2 is ValueDataNode node3)
			{
				return ((ITypeValidator<SpriteSpecifier.EntityPrototype, ValueDataNode>)this).Validate(serializationManager, node3, dependencies, context);
			}
			return new ErrorNode(node, "Sprite specifier entity node must be a ValueDataNode");
		}
		return ((ITypeValidator<SpriteSpecifier.Rsi, MappingDataNode>)this).Validate(serializationManager, node, dependencies, context);
	}

	ValidationNode ITypeValidator<SpriteSpecifier.Rsi, MappingDataNode>.Validate(ISerializationManager serializationManager, MappingDataNode node, IDependencyCollection dependencies, ISerializationContext? context)
	{
		return ValidateRsi(serializationManager, node, dependencies, context);
	}

	public abstract ValidationNode ValidateRsi(ISerializationManager serializationManager, MappingDataNode node, IDependencyCollection dependencies, ISerializationContext? context);

	public DataNode Write(ISerializationManager serializationManager, SpriteSpecifier.Texture value, IDependencyCollection dependencies, bool alwaysWrite = false, ISerializationContext? context = null)
	{
		return serializationManager.WriteValue(value.TexturePath, alwaysWrite, context);
	}

	public DataNode Write(ISerializationManager serializationManager, SpriteSpecifier.EntityPrototype value, IDependencyCollection dependencies, bool alwaysWrite = false, ISerializationContext? context = null)
	{
		return new MappingDataNode { 
		{
			"entity",
			new ValueDataNode(value.EntityPrototypeId)
		} };
	}

	public DataNode Write(ISerializationManager serializationManager, SpriteSpecifier.Rsi value, IDependencyCollection dependencies, bool alwaysWrite = false, ISerializationContext? context = null)
	{
		return new MappingDataNode
		{
			{
				"sprite",
				serializationManager.WriteValue(value.RsiPath)
			},
			{
				"state",
				new ValueDataNode(value.RsiState)
			}
		};
	}

	public SpriteSpecifier.Texture CreateCopy(ISerializationManager serializationManager, SpriteSpecifier.Texture source, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		return new SpriteSpecifier.Texture(source.TexturePath);
	}

	public SpriteSpecifier.EntityPrototype CreateCopy(ISerializationManager serializationManager, SpriteSpecifier.EntityPrototype source, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		return new SpriteSpecifier.EntityPrototype(source.EntityPrototypeId);
	}

	public SpriteSpecifier.Rsi CreateCopy(ISerializationManager serializationManager, SpriteSpecifier.Rsi source, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		return new SpriteSpecifier.Rsi(source.RsiPath, source.RsiState);
	}

	public DataNode Write(ISerializationManager serializationManager, SpriteSpecifier value, IDependencyCollection dependencies, bool alwaysWrite = false, ISerializationContext? context = null)
	{
		if (!(value is SpriteSpecifier.Rsi value2))
		{
			if (!(value is SpriteSpecifier.Texture value3))
			{
				if (value is SpriteSpecifier.EntityPrototype value4)
				{
					return Write(serializationManager, value4, dependencies, alwaysWrite, context);
				}
				throw new InvalidOperationException("Invalid SpriteSpecifier specified!");
			}
			return Write(serializationManager, value3, dependencies, alwaysWrite, context);
		}
		return Write(serializationManager, value2, dependencies, alwaysWrite, context);
	}

	public SpriteSpecifier CreateCopy(ISerializationManager serializationManager, SpriteSpecifier source, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		if (!(source is SpriteSpecifier.Rsi source2))
		{
			if (!(source is SpriteSpecifier.Texture source3))
			{
				if (source is SpriteSpecifier.EntityPrototype source4)
				{
					return CreateCopy(serializationManager, source4, dependencies, hookCtx, context);
				}
				throw new InvalidOperationException("Invalid SpriteSpecifier specified!");
			}
			return CreateCopy(serializationManager, source3, dependencies, hookCtx, context);
		}
		return CreateCopy(serializationManager, source2, dependencies, hookCtx, context);
	}

	public void CopyTo(ISerializationManager serializationManager, SpriteSpecifier.Rsi source, ref SpriteSpecifier.Rsi target, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		target.RsiPath = source.RsiPath;
		target.RsiState = source.RsiState;
	}

	public void CopyTo(ISerializationManager serializationManager, SpriteSpecifier.Texture source, ref SpriteSpecifier.Texture target, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		target.TexturePath = source.TexturePath;
	}
}
