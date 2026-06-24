using System;
using System.Globalization;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;

namespace Robust.Shared.Prototypes;

internal sealed class YamlValidationContext : ISerializationContext, ITypeSerializer<EntityUid, ValueDataNode>, ITypeReader<EntityUid, ValueDataNode>, ITypeValidator<EntityUid, ValueDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<EntityUid, ValueDataNode>, ITypeWriter<EntityUid>, BaseSerializerInterfaces.ITypeInterface<EntityUid>, ITypeSerializer<NetEntity, ValueDataNode>, ITypeReader<NetEntity, ValueDataNode>, ITypeValidator<NetEntity, ValueDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<NetEntity, ValueDataNode>, ITypeWriter<NetEntity>, BaseSerializerInterfaces.ITypeInterface<NetEntity>, ITypeSerializer<MapId, ValueDataNode>, ITypeReader<MapId, ValueDataNode>, ITypeValidator<MapId, ValueDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<MapId, ValueDataNode>, ITypeWriter<MapId>, BaseSerializerInterfaces.ITypeInterface<MapId>
{
	public SerializationManager.SerializerProvider SerializerProvider { get; } = new SerializationManager.SerializerProvider();

	public bool WritingReadingPrototypes => true;

	public YamlValidationContext()
	{
		SerializerProvider.RegisterSerializer(this);
	}

	ValidationNode ITypeValidator<EntityUid, ValueDataNode>.Validate(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, ISerializationContext? context)
	{
		if (node.Value == "invalid")
		{
			return new ValidatedValueNode(node);
		}
		return new ErrorNode(node, "Prototypes should not contain EntityUids");
	}

	public DataNode Write(ISerializationManager serializationManager, EntityUid value, IDependencyCollection dependencies, bool alwaysWrite = false, ISerializationContext? context = null)
	{
		if (!value.Valid)
		{
			return new ValueDataNode("invalid");
		}
		return new ValueDataNode(value.Id.ToString(CultureInfo.InvariantCulture));
	}

	EntityUid ITypeReader<EntityUid, ValueDataNode>.Read(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context, ISerializationManager.InstantiationDelegate<EntityUid>? _)
	{
		if (node.Value == "invalid")
		{
			return EntityUid.Invalid;
		}
		return EntityUid.Parse(node.Value.AsSpan());
	}

	public ValidationNode Validate(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, ISerializationContext? context = null)
	{
		if (node.Value == "invalid")
		{
			return new ValidatedValueNode(node);
		}
		return new ErrorNode(node, "Prototypes should not contain NetEntities");
	}

	public NetEntity Read(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null, ISerializationManager.InstantiationDelegate<NetEntity>? instanceProvider = null)
	{
		if (node.Value == "invalid")
		{
			return NetEntity.Invalid;
		}
		return NetEntity.Parse(node.Value.AsSpan());
	}

	public DataNode Write(ISerializationManager serializationManager, NetEntity value, IDependencyCollection dependencies, bool alwaysWrite = false, ISerializationContext? context = null)
	{
		if (!value.Valid)
		{
			return new ValueDataNode("invalid");
		}
		return new ValueDataNode(value.Id.ToString(CultureInfo.InvariantCulture));
	}

	ValidationNode ITypeValidator<MapId, ValueDataNode>.Validate(ISerializationManager seri, ValueDataNode node, IDependencyCollection deps, ISerializationContext? context)
	{
		if (node.Value == "invalid")
		{
			return new ValidatedValueNode(node);
		}
		return new ErrorNode(node, "Prototypes should not contain map ids");
	}

	MapId ITypeReader<MapId, ValueDataNode>.Read(ISerializationManager seri, ValueDataNode node, IDependencyCollection deps, SerializationHookContext hookCtx, ISerializationContext? ctx, ISerializationManager.InstantiationDelegate<MapId>? instanceProvider)
	{
		if (!(node.Value == "invalid"))
		{
			return new MapId(int.Parse(node.Value));
		}
		return MapId.Nullspace;
	}

	DataNode ITypeWriter<MapId>.Write(ISerializationManager seri, MapId value, IDependencyCollection deps, bool alwaysWrite, ISerializationContext? ctx)
	{
		if (value == MapId.Nullspace)
		{
			return new ValueDataNode("invalid");
		}
		return new ValueDataNode(value.Value.ToString(CultureInfo.InvariantCulture));
	}
}
