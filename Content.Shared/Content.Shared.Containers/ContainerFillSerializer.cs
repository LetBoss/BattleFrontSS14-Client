using System.Collections.Generic;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Sequence;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;

namespace Content.Shared.Containers;

public sealed class ContainerFillSerializer : ITypeValidator<Dictionary<string, List<string>>, MappingDataNode>, ITypeNodeInterface<Dictionary<string, List<string>>, MappingDataNode>
{
	private static PrototypeIdListSerializer<EntityPrototype> ListSerializer => new PrototypeIdListSerializer<EntityPrototype>();

	public ValidationNode Validate(ISerializationManager serializationManager, MappingDataNode node, IDependencyCollection dependencies, ISerializationContext? context = null)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Expected O, but got Unknown
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Expected O, but got Unknown
		Dictionary<ValidationNode, ValidationNode> mapping = new Dictionary<ValidationNode, ValidationNode>();
		foreach (KeyValuePair<string, DataNode> child in node.Children)
		{
			child.Deconstruct(out var key, out var value);
			string key2 = key;
			DataNode val = value;
			SequenceDataNode seq = (SequenceDataNode)(object)((val is SequenceDataNode) ? val : null);
			ValidationNode listVal = (ValidationNode)((seq != null) ? ((object)ListSerializer.Validate(serializationManager, seq, dependencies, context)) : ((object)new ErrorNode(val, "ContainerFillComponent prototypes must be a sequence/list", true)));
			mapping.Add((ValidationNode)new ValidatedValueNode((DataNode)(object)node.GetKeyNode(key2)), listVal);
		}
		return (ValidationNode)new ValidatedMappingNode(mapping);
	}
}
