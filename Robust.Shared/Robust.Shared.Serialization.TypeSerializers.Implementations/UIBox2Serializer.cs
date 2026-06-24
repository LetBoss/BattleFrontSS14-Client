using System.Globalization;
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
public sealed class UIBox2Serializer : ITypeSerializer<UIBox2, ValueDataNode>, ITypeReader<UIBox2, ValueDataNode>, ITypeValidator<UIBox2, ValueDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<UIBox2, ValueDataNode>, ITypeWriter<UIBox2>, BaseSerializerInterfaces.ITypeInterface<UIBox2>, ITypeCopyCreator<UIBox2>
{
	public UIBox2 Read(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null, ISerializationManager.InstantiationDelegate<UIBox2>? instanceProvider = null)
	{
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		string[] array = node.Value.Split(',');
		if (array.Length != 4)
		{
			throw new InvalidMappingException($"Could not parse {"UIBox2"}: '{node.Value}'");
		}
		float num = float.Parse(array[0], CultureInfo.InvariantCulture);
		float num2 = float.Parse(array[1], CultureInfo.InvariantCulture);
		float num3 = float.Parse(array[2], CultureInfo.InvariantCulture);
		float num4 = float.Parse(array[3], CultureInfo.InvariantCulture);
		return new UIBox2(num2, num, num4, num3);
	}

	public ValidationNode Validate(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, ISerializationContext? context = null)
	{
		string[] array = node.Value.Split(',');
		if (array.Length != 4)
		{
			return new ErrorNode(node, "Invalid amount of arguments for UIBox2.");
		}
		if (!float.TryParse(array[0], NumberStyles.Any, CultureInfo.InvariantCulture, out var result) || !float.TryParse(array[1], NumberStyles.Any, CultureInfo.InvariantCulture, out result) || !float.TryParse(array[2], NumberStyles.Any, CultureInfo.InvariantCulture, out result) || !float.TryParse(array[3], NumberStyles.Any, CultureInfo.InvariantCulture, out result))
		{
			return new ErrorNode(node, "Failed parsing values for UIBox2.");
		}
		return new ValidatedValueNode(node);
	}

	public DataNode Write(ISerializationManager serializationManager, UIBox2 value, IDependencyCollection dependencies, bool alwaysWrite = false, ISerializationContext? context = null)
	{
		return new ValueDataNode($"{value.Top.ToString(CultureInfo.InvariantCulture)},{value.Left.ToString(CultureInfo.InvariantCulture)},{value.Bottom.ToString(CultureInfo.InvariantCulture)},{value.Right.ToString(CultureInfo.InvariantCulture)}");
	}

	public UIBox2 CreateCopy(ISerializationManager serializationManager, UIBox2 source, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		return new UIBox2(source.Left, source.Top, source.Right, source.Bottom);
	}
}
