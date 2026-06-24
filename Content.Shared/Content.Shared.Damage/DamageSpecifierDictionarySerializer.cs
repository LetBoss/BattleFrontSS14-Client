using System.Collections.Generic;
using Content.Shared.Damage.Prototypes;
using Content.Shared.FixedPoint;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;

namespace Content.Shared.Damage;

public sealed class DamageSpecifierDictionarySerializer : ITypeReader<Dictionary<string, FixedPoint2>, MappingDataNode>, ITypeValidator<Dictionary<string, FixedPoint2>, MappingDataNode>, ITypeNodeInterface<Dictionary<string, FixedPoint2>, MappingDataNode>
{
	private ITypeValidator<Dictionary<string, FixedPoint2>, MappingDataNode> _damageTypeSerializer = (ITypeValidator<Dictionary<string, FixedPoint2>, MappingDataNode>)(object)new PrototypeIdDictionarySerializer<FixedPoint2, DamageTypePrototype>();

	private ITypeValidator<Dictionary<string, FixedPoint2>, MappingDataNode> _damageGroupSerializer = (ITypeValidator<Dictionary<string, FixedPoint2>, MappingDataNode>)(object)new PrototypeIdDictionarySerializer<FixedPoint2, DamageGroupPrototype>();

	public ValidationNode Validate(ISerializationManager serializationManager, MappingDataNode node, IDependencyCollection dependencies, ISerializationContext? context = null)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Expected O, but got Unknown
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Expected O, but got Unknown
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Expected O, but got Unknown
		Dictionary<ValidationNode, ValidationNode> vals = new Dictionary<ValidationNode, ValidationNode>();
		MappingDataNode typesNode = default(MappingDataNode);
		if (node.TryGet<MappingDataNode>("types", ref typesNode))
		{
			vals.Add((ValidationNode)new ValidatedValueNode((DataNode)new ValueDataNode("types")), _damageTypeSerializer.Validate(serializationManager, typesNode, dependencies, context));
		}
		MappingDataNode groupsNode = default(MappingDataNode);
		if (node.TryGet<MappingDataNode>("groups", ref groupsNode))
		{
			vals.Add((ValidationNode)new ValidatedValueNode((DataNode)new ValueDataNode("groups")), _damageGroupSerializer.Validate(serializationManager, groupsNode, dependencies, context));
		}
		return (ValidationNode)new ValidatedMappingNode(vals);
	}

	public Dictionary<string, FixedPoint2> Read(ISerializationManager serializationManager, MappingDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null, InstantiationDelegate<Dictionary<string, FixedPoint2>>? instanceProvider = null)
	{
		Dictionary<string, FixedPoint2> dict = ((instanceProvider != null) ? instanceProvider.Invoke() : new Dictionary<string, FixedPoint2>());
		MappingDataNode typesNode = default(MappingDataNode);
		if (node.TryGet<MappingDataNode>("types", ref typesNode))
		{
			serializationManager.Read<Dictionary<string, FixedPoint2>>((DataNode)(object)typesNode, (ISerializationContext)null, false, (InstantiationDelegate<Dictionary<string, FixedPoint2>>)(() => dict), true);
		}
		MappingDataNode groupsNode = default(MappingDataNode);
		if (!node.TryGet<MappingDataNode>("groups", ref groupsNode))
		{
			return dict;
		}
		IPrototypeManager prototypeManager = dependencies.Resolve<IPrototypeManager>();
		DamageGroupPrototype group = default(DamageGroupPrototype);
		foreach (KeyValuePair<string, FixedPoint2> entry in serializationManager.Read<Dictionary<string, FixedPoint2>>((DataNode)(object)groupsNode, (ISerializationContext)null, false, (InstantiationDelegate<Dictionary<string, FixedPoint2>>)null, true))
		{
			if (!prototypeManager.TryIndex<DamageGroupPrototype>(entry.Key, ref group))
			{
				dependencies.Resolve<ILogManager>().RootSawmill.Error("Unknown damage group given to DamageSpecifier: " + entry.Key);
				continue;
			}
			int remainingTypes = group.DamageTypes.Count;
			FixedPoint2 remainingDamage = entry.Value;
			foreach (string damageType in group.DamageTypes)
			{
				FixedPoint2 damage = remainingDamage / FixedPoint2.New(remainingTypes);
				if (!dict.TryAdd(damageType, damage))
				{
					dict[damageType] += damage;
				}
				remainingDamage -= damage;
				remainingTypes--;
			}
		}
		return dict;
	}
}
