using System;
using System.Linq;
using Robust.Shared.ContentPack;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;
using Robust.Shared.Utility;

namespace Robust.Shared.Serialization.TypeSerializers.Implementations;

[TypeSerializer]
public sealed class ResPathSerializer : ITypeSerializer<ResPath, ValueDataNode>, ITypeReader<ResPath, ValueDataNode>, ITypeValidator<ResPath, ValueDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<ResPath, ValueDataNode>, ITypeWriter<ResPath>, BaseSerializerInterfaces.ITypeInterface<ResPath>, ITypeCopyCreator<ResPath>
{
	public ValidationNode Validate(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, ISerializationContext? context = null)
	{
		ResPath resPath = ResPath.FromRelativeSystemPath(node.Value);
		if (resPath.Extension.Equals("rsi"))
		{
			resPath /= "meta.json";
		}
		if (!resPath.CanonPath.Split('/').First().Equals("Textures", StringComparison.InvariantCultureIgnoreCase))
		{
			resPath = SpriteSpecifierSerializer.TextureRoot / resPath;
		}
		resPath = resPath.ToRootedPath();
		try
		{
			IResourceManager resourceManager = dependencies.Resolve<IResourceManager>();
			if (node.Value.EndsWith('/'))
			{
				if (resourceManager.ContentGetDirectoryEntries(resPath).Any())
				{
					return new ValidatedValueNode(node);
				}
				return new ErrorNode(node, $"Folder not found. ({resPath})");
			}
			if (resourceManager.ContentFileExists(resPath))
			{
				return new ValidatedValueNode(node);
			}
			return new ErrorNode(node, $"File not found. ({resPath})");
		}
		catch (Exception ex)
		{
			return new ErrorNode(node, $"Failed parsing filepath. ({resPath}) ({ex.Message})");
		}
	}

	public ResPath Read(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null, ISerializationManager.InstantiationDelegate<ResPath>? instanceProvider = null)
	{
		return new ResPath(node.Value);
	}

	public DataNode Write(ISerializationManager serializationManager, ResPath value, IDependencyCollection dependencies, bool alwaysWrite = false, ISerializationContext? context = null)
	{
		return new ValueDataNode(value.ToString());
	}

	public ResPath CreateCopy(ISerializationManager serializationManager, ResPath source, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		return new ResPath(source.ToString());
	}
}
