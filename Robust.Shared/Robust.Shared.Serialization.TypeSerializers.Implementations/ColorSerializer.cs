using System;
using System.Runtime.CompilerServices;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;

namespace Robust.Shared.Serialization.TypeSerializers.Implementations;

[TypeSerializer]
public sealed class ColorSerializer : ITypeSerializer<Color, ValueDataNode>, ITypeReader<Color, ValueDataNode>, ITypeValidator<Color, ValueDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<Color, ValueDataNode>, ITypeWriter<Color>, BaseSerializerInterfaces.ITypeInterface<Color>, ITypeCopyCreator<Color>
{
	public Color Read(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null, ISerializationManager.InstantiationDelegate<Color>? instanceProvider = null)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		Unsafe.SkipInit(out Color result);
		if (!Color.TryFromName(node.Value, ref result))
		{
			return Color.FromHex(node.Value.AsSpan(), (Color?)null);
		}
		return result;
	}

	public ValidationNode Validate(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, ISerializationContext? context = null)
	{
		Unsafe.SkipInit(out Color val);
		if (!Color.TryFromName(node.Value, ref val) && !Color.TryFromHex(node.Value.AsSpan()).HasValue)
		{
			return new ErrorNode(node, "Failed parsing Color.");
		}
		return new ValidatedValueNode(node);
	}

	public DataNode Write(ISerializationManager serializationManager, Color value, IDependencyCollection dependencies, bool alwaysWrite = false, ISerializationContext? context = null)
	{
		return new ValueDataNode(((Color)(ref value)).ToHex());
	}

	public Color CreateCopy(ISerializationManager serializationManager, Color source, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		return new Color(source.R, source.G, source.B, source.A);
	}
}
