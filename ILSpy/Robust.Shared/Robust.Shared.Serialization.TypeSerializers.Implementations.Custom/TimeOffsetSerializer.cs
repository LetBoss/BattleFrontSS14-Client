using System;
using System.Globalization;
using Robust.Shared.EntitySerialization;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;
using Robust.Shared.Timing;

namespace Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

public sealed class TimeOffsetSerializer : ITypeSerializer<TimeSpan, ValueDataNode>, ITypeReader<TimeSpan, ValueDataNode>, ITypeValidator<TimeSpan, ValueDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<TimeSpan, ValueDataNode>, ITypeWriter<TimeSpan>, BaseSerializerInterfaces.ITypeInterface<TimeSpan>
{
	public TimeSpan Read(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null, ISerializationManager.InstantiationDelegate<TimeSpan>? instanceProvider = null)
	{
		if (context != null && context.WritingReadingPrototypes)
		{
			return TimeSpan.Zero;
		}
		if (!(context is EntityDeserializer { CurrentReadingEntity: { PostInit: not false } } entityDeserializer))
		{
			return TimeSpan.Zero;
		}
		IGameTiming timing = entityDeserializer.Timing;
		TimeSpan timeSpan = TimeSpan.FromSeconds(double.Parse(node.Value, CultureInfo.InvariantCulture));
		if (timeSpan > TimeSpan.MaxValue - timing.CurTime)
		{
			return TimeSpan.MaxValue;
		}
		return timeSpan + timing.CurTime;
	}

	public ValidationNode Validate(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, ISerializationContext? context = null)
	{
		if (!double.TryParse(node.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var _))
		{
			return new ErrorNode(node, "Failed parsing TimeSpan");
		}
		return new ValidatedValueNode(node);
	}

	public DataNode Write(ISerializationManager serializationManager, TimeSpan value, IDependencyCollection dependencies, bool alwaysWrite = false, ISerializationContext? context = null)
	{
		if (!(context is EntitySerializer { WritingReadingPrototypes: false } entitySerializer) || !entitySerializer.EntMan.TryGetComponent((EntityUid?)entitySerializer.CurrentEntity, out MetaDataComponent component) || (int)component.EntityLifeStage < 3)
		{
			return new ValueDataNode("0");
		}
		if (component.PauseTime.HasValue)
		{
			value -= component.PauseTime.Value;
		}
		else
		{
			value -= entitySerializer.Timing.CurTime;
		}
		return new ValueDataNode(value.TotalSeconds.ToString(CultureInfo.InvariantCulture));
	}
}
