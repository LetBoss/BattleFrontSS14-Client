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
public sealed class AngleSerializer : ITypeSerializer<Angle, ValueDataNode>, ITypeReader<Angle, ValueDataNode>, ITypeValidator<Angle, ValueDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<Angle, ValueDataNode>, ITypeWriter<Angle>, BaseSerializerInterfaces.ITypeInterface<Angle>, ITypeCopyCreator<Angle>
{
	public Angle Read(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null, ISerializationManager.InstantiationDelegate<Angle>? instanceProvider = null)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		string value = node.Value;
		if (!value.EndsWith("rad"))
		{
			return Angle.FromDegrees(double.Parse(value, CultureInfo.InvariantCulture));
		}
		return new Angle(double.Parse(value.Substring(0, value.Length - 3), CultureInfo.InvariantCulture));
	}

	public ValidationNode Validate(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, ISerializationContext? context = null)
	{
		string value = node.Value;
		if (!double.TryParse(value.EndsWith("rad") ? value.Substring(0, value.Length - 3) : value, CultureInfo.InvariantCulture, out var _))
		{
			return new ErrorNode(node, "Failed parsing angle.");
		}
		return new ValidatedValueNode(node);
	}

	public DataNode Write(ISerializationManager serializationManager, Angle value, IDependencyCollection dependencies, bool alwaysWrite = false, ISerializationContext? context = null)
	{
		return new ValueDataNode(value.Theta.ToString(CultureInfo.InvariantCulture) + " rad");
	}

	public Angle CreateCopy(ISerializationManager serializationManager, Angle source, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return new Angle(Angle.op_Implicit(source));
	}
}
