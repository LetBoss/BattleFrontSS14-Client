using System;
using System.Globalization;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;
using Robust.Shared.Utility;

namespace Content.Shared.EntityTable.ValueSelector;

[TypeSerializer]
public sealed class NumberSelectorTypeSerializer : ITypeReader<NumberSelector, ValueDataNode>, ITypeValidator<NumberSelector, ValueDataNode>, ITypeNodeInterface<NumberSelector, ValueDataNode>
{
	public ValidationNode Validate(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, ISerializationContext? context = null)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Expected O, but got Unknown
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected O, but got Unknown
		if (!int.TryParse(node.Value, out var _))
		{
			string[] array = default(string[]);
			if (!VectorSerializerUtility.TryParseArgs(node.Value, 2, ref array))
			{
				return (ValidationNode)new ErrorNode((DataNode)(object)node, "Custom validation not supported! Please specify the type manually!", true);
			}
			return (ValidationNode)new ValidatedValueNode((DataNode)(object)node);
		}
		return (ValidationNode)new ValidatedValueNode((DataNode)(object)node);
	}

	public NumberSelector Read(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null, InstantiationDelegate<NumberSelector>? instanceProvider = null)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		Type type = typeof(NumberSelector);
		if (int.TryParse(node.Value, out var result))
		{
			return new ConstantNumberSelector(result);
		}
		string[] args = default(string[]);
		if (VectorSerializerUtility.TryParseArgs(node.Value, 2, ref args))
		{
			int num = int.Parse(args[0], CultureInfo.InvariantCulture);
			int y = int.Parse(args[1], CultureInfo.InvariantCulture);
			return new RangeNumberSelector(new Vector2i(num, y));
		}
		return (NumberSelector)serializationManager.Read(type, (DataNode)(object)node, context, false, false);
	}
}
