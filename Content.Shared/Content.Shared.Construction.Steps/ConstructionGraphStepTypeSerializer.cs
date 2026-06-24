using System;
using Robust.Shared.IoC;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;

namespace Content.Shared.Construction.Steps;

[TypeSerializer]
public sealed class ConstructionGraphStepTypeSerializer : ITypeReader<ConstructionGraphStep, MappingDataNode>, ITypeValidator<ConstructionGraphStep, MappingDataNode>, ITypeNodeInterface<ConstructionGraphStep, MappingDataNode>
{
	private Type? GetType(MappingDataNode node)
	{
		if (node.Has("material"))
		{
			return typeof(MaterialConstructionGraphStep);
		}
		if (node.Has("tool"))
		{
			return typeof(ToolConstructionGraphStep);
		}
		if (node.Has("component"))
		{
			return typeof(ComponentConstructionGraphStep);
		}
		if (node.Has("tag"))
		{
			return typeof(TagConstructionGraphStep);
		}
		if (node.Has("allTags") || node.Has("anyTags"))
		{
			return typeof(MultipleTagsConstructionGraphStep);
		}
		if (node.Has("minTemperature") || node.Has("maxTemperature"))
		{
			return typeof(TemperatureConstructionGraphStep);
		}
		if (node.Has("assemblyId") || node.Has("guideString"))
		{
			return typeof(PartAssemblyConstructionGraphStep);
		}
		return null;
	}

	public ConstructionGraphStep Read(ISerializationManager serializationManager, MappingDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null, InstantiationDelegate<ConstructionGraphStep>? instanceProvider = null)
	{
		Type type = GetType(node) ?? throw new ArgumentException("Tried to convert invalid YAML node mapping to ConstructionGraphStep!");
		return (ConstructionGraphStep)serializationManager.Read(type, (DataNode)(object)node, hookCtx, context, false);
	}

	public ValidationNode Validate(ISerializationManager serializationManager, MappingDataNode node, IDependencyCollection dependencies, ISerializationContext? context = null)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected O, but got Unknown
		Type type = GetType(node);
		if (type == null)
		{
			return (ValidationNode)new ErrorNode((DataNode)(object)node, "No construction graph step type found.", true);
		}
		return serializationManager.ValidateNode(type, (DataNode)(object)node, context);
	}
}
