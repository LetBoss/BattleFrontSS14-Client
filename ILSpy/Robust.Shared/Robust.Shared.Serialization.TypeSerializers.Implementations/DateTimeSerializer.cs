using System;
using System.Globalization;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;

namespace Robust.Shared.Serialization.TypeSerializers.Implementations;

[TypeSerializer]
public sealed class DateTimeSerializer : ITypeSerializer<DateTime, ValueDataNode>, ITypeReader<DateTime, ValueDataNode>, ITypeValidator<DateTime, ValueDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<DateTime, ValueDataNode>, ITypeWriter<DateTime>, BaseSerializerInterfaces.ITypeInterface<DateTime>, ITypeCopyCreator<DateTime>
{
	public ValidationNode Validate(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, ISerializationContext? context = null)
	{
		if (!DateTime.TryParse(node.Value, null, DateTimeStyles.RoundtripKind, out var _))
		{
			return new ErrorNode(node, "Failed parsing DateTime");
		}
		return new ValidatedValueNode(node);
	}

	public DateTime Read(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null, ISerializationManager.InstantiationDelegate<DateTime>? instanceProvider = null)
	{
		return DateTime.Parse(node.Value, null, DateTimeStyles.RoundtripKind);
	}

	public DataNode Write(ISerializationManager serializationManager, DateTime value, IDependencyCollection dependencies, bool alwaysWrite = false, ISerializationContext? context = null)
	{
		return new ValueDataNode(value.ToString("o"));
	}

	public DateTime CreateCopy(ISerializationManager serializationManager, DateTime source, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		return source;
	}
}
