using System;
using System.Globalization;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;
using Robust.Shared.Utility;

namespace Robust.Shared.Serialization.TypeSerializers.Implementations;

[TypeSerializer]
public sealed class Box2Serializer : ITypeSerializer<Box2, ValueDataNode>, ITypeReader<Box2, ValueDataNode>, ITypeValidator<Box2, ValueDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<Box2, ValueDataNode>, ITypeWriter<Box2>, BaseSerializerInterfaces.ITypeInterface<Box2>, ITypeCopyCreator<Box2>, ITypeSerializer<Box2i, ValueDataNode>, ITypeReader<Box2i, ValueDataNode>, ITypeValidator<Box2i, ValueDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<Box2i, ValueDataNode>, ITypeWriter<Box2i>, BaseSerializerInterfaces.ITypeInterface<Box2i>, ITypeCopyCreator<Box2i>
{
	public Box2 Read(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null, ISerializationManager.InstantiationDelegate<Box2>? instanceProvider = null)
	{
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		ReadOnlySpan<char> source = node.Value.AsSpan();
		NextOrThrow(ref source, out var splitValue, node.Value);
		float num = Parse.Float(splitValue);
		NextOrThrow(ref source, out splitValue, node.Value);
		float num2 = Parse.Float(splitValue);
		NextOrThrow(ref source, out splitValue, node.Value);
		float num3 = Parse.Float(splitValue);
		NextOrThrow(ref source, out splitValue, node.Value);
		float num4 = Parse.Float(splitValue);
		return new Box2(num, num2, num3, num4);
	}

	public DataNode Write(ISerializationManager serializationManager, Box2 value, IDependencyCollection dependencies, bool alwaysWrite = false, ISerializationContext? context = null)
	{
		return new ValueDataNode($"{value.Left.ToString(CultureInfo.InvariantCulture)},{value.Bottom.ToString(CultureInfo.InvariantCulture)},{value.Right.ToString(CultureInfo.InvariantCulture)},{value.Top.ToString(CultureInfo.InvariantCulture)}");
	}

	ValidationNode ITypeValidator<Box2, ValueDataNode>.Validate(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, ISerializationContext? context)
	{
		string[] array = node.Value.Split(',');
		if (array.Length != 4)
		{
			return new ErrorNode(node, "Invalid amount of args for Box2.");
		}
		if (!Parse.TryFloat(array[0].AsSpan(), out var result) || !Parse.TryFloat(array[1].AsSpan(), out result) || !Parse.TryFloat(array[2].AsSpan(), out result) || !Parse.TryFloat(array[3].AsSpan(), out result))
		{
			return new ErrorNode(node, "Failed parsing values of Box2.");
		}
		return new ValidatedValueNode(node);
	}

	public Box2 CreateCopy(ISerializationManager serializationManager, Box2 source, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		return new Box2(source.Left, source.Bottom, source.Right, source.Top);
	}

	public Box2i Read(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null, ISerializationManager.InstantiationDelegate<Box2i>? instanceProvider = null)
	{
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		ReadOnlySpan<char> source = node.Value.AsSpan();
		NextOrThrow(ref source, out var splitValue, node.Value);
		int num = Parse.Int32(splitValue);
		NextOrThrow(ref source, out splitValue, node.Value);
		int num2 = Parse.Int32(splitValue);
		NextOrThrow(ref source, out splitValue, node.Value);
		int num3 = Parse.Int32(splitValue);
		NextOrThrow(ref source, out splitValue, node.Value);
		int num4 = Parse.Int32(splitValue);
		return new Box2i(num, num2, num3, num4);
	}

	public DataNode Write(ISerializationManager serializationManager, Box2i value, IDependencyCollection dependencies, bool alwaysWrite = false, ISerializationContext? context = null)
	{
		return new ValueDataNode($"{value.Left.ToString(CultureInfo.InvariantCulture)},{value.Bottom.ToString(CultureInfo.InvariantCulture)},{value.Right.ToString(CultureInfo.InvariantCulture)},{value.Top.ToString(CultureInfo.InvariantCulture)}");
	}

	ValidationNode ITypeValidator<Box2i, ValueDataNode>.Validate(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, ISerializationContext? context)
	{
		string[] array = node.Value.Split(',');
		if (array.Length != 4)
		{
			return new ErrorNode(node, "Invalid amount of args for Box2i.");
		}
		if (!Parse.TryInt32(array[0].AsSpan(), out var result) || !Parse.TryInt32(array[1].AsSpan(), out result) || !Parse.TryInt32(array[2].AsSpan(), out result) || !Parse.TryInt32(array[3].AsSpan(), out result))
		{
			return new ErrorNode(node, "Failed parsing values of Box2i.");
		}
		return new ValidatedValueNode(node);
	}

	public Box2i CreateCopy(ISerializationManager serializationManager, Box2i source, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		return new Box2i(source.Left, source.Bottom, source.Right, source.Top);
	}

	private static void NextOrThrow(ref ReadOnlySpan<char> source, out ReadOnlySpan<char> splitValue, string errValue)
	{
		if (!SpanSplitExtensions.SplitFindNext(ref source, ',', out splitValue))
		{
			throw new InvalidMappingException($"Could not parse {"Box2"}: '{errValue}'");
		}
	}
}
