using System;
using System.Globalization;
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
public sealed class TimespanSerializer : ITypeSerializer<TimeSpan, ValueDataNode>, ITypeReader<TimeSpan, ValueDataNode>, ITypeValidator<TimeSpan, ValueDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<TimeSpan, ValueDataNode>, ITypeWriter<TimeSpan>, BaseSerializerInterfaces.ITypeInterface<TimeSpan>, ITypeCopyCreator<TimeSpan>
{
	public TimeSpan Read(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null, ISerializationManager.InstantiationDelegate<TimeSpan>? instanceProvider = null)
	{
		if (TimeSpanExt.TryTimeSpan(node, out var timeSpan))
		{
			return timeSpan;
		}
		throw new FormatException("The input string '" + node.Value + "' can't be converted to TimeSpan");
	}

	public ValidationNode Validate(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, ISerializationContext? context = null)
	{
		if (!TimeSpanExt.TryTimeSpan(node, out var _) && !double.TryParse(node.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var _))
		{
			return new ErrorNode(node, "Failed parsing TimeSpan");
		}
		return new ValidatedValueNode(node);
	}

	public DataNode Write(ISerializationManager serializationManager, TimeSpan value, IDependencyCollection dependencies, bool alwaysWrite = false, ISerializationContext? context = null)
	{
		return new ValueDataNode(value.TotalSeconds.ToString(CultureInfo.InvariantCulture));
	}

	public TimeSpan CreateCopy(ISerializationManager serializationManager, TimeSpan source, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		return source;
	}
}
