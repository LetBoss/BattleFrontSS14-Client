using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Content.Shared.Body.Organ;
using Content.Shared.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Sequence;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;

namespace Content.Shared.Body.Prototypes;

[TypeSerializer]
public sealed class BodyPrototypeSerializer : ITypeReader<BodyPrototype, MappingDataNode>, ITypeValidator<BodyPrototype, MappingDataNode>, ITypeNodeInterface<BodyPrototype, MappingDataNode>
{
	private (ValidationNode Node, List<string> Connections) ValidateSlot(MappingDataNode slot, IDependencyCollection dependencies)
	{
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Expected O, but got Unknown
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Expected O, but got Unknown
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Expected O, but got Unknown
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Expected O, but got Unknown
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Expected O, but got Unknown
		List<ValidationNode> nodes = new List<ValidationNode>();
		IPrototypeManager prototypes = dependencies.Resolve<IPrototypeManager>();
		IComponentFactory factory = dependencies.Resolve<IComponentFactory>();
		List<string> connections = new List<string>();
		SequenceDataNode connectionsNode = default(SequenceDataNode);
		if (slot.TryGet<SequenceDataNode>("connections", ref connectionsNode))
		{
			foreach (DataNode node in connectionsNode)
			{
				ValueDataNode connection = (ValueDataNode)(object)((node is ValueDataNode) ? node : null);
				if (connection == null)
				{
					nodes.Add((ValidationNode)new ErrorNode(node, "Connection is not a value data node", true));
				}
				else
				{
					connections.Add(connection.Value);
				}
			}
		}
		MappingDataNode organsNode = default(MappingDataNode);
		if (slot.TryGet<MappingDataNode>("organs", ref organsNode))
		{
			EntityPrototype organPrototype = default(EntityPrototype);
			foreach (KeyValuePair<string, DataNode> item in organsNode)
			{
				item.Deconstruct(out var _, out var value);
				DataNode value2 = value;
				ValueDataNode organ = (ValueDataNode)(object)((value2 is ValueDataNode) ? value2 : null);
				if (organ == null)
				{
					nodes.Add((ValidationNode)new ErrorNode(value2, "Value is not a value data node", true));
				}
				else if (!prototypes.TryIndex<EntityPrototype>(organ.Value, ref organPrototype))
				{
					nodes.Add((ValidationNode)new ErrorNode(value2, "No organ entity prototype found with id " + organ.Value, true));
				}
				else if (!organPrototype.HasComponent<OrganComponent>(factory))
				{
					nodes.Add((ValidationNode)new ErrorNode(value2, "Organ " + organ.Value + " does not have a body component", true));
				}
			}
		}
		return (Node: (ValidationNode)new ValidatedSequenceNode(nodes), Connections: connections);
	}

	public ValidationNode Validate(ISerializationManager serializationManager, MappingDataNode node, IDependencyCollection dependencies, ISerializationContext? context = null)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Expected O, but got Unknown
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Expected O, but got Unknown
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Expected O, but got Unknown
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Expected O, but got Unknown
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Expected O, but got Unknown
		List<ValidationNode> nodes = new List<ValidationNode>();
		ValueDataNode root = default(ValueDataNode);
		if (!node.TryGet<ValueDataNode>("root", ref root))
		{
			nodes.Add((ValidationNode)new ErrorNode((DataNode)(object)node, "No root value data node found", true));
		}
		MappingDataNode slots = default(MappingDataNode);
		if (!node.TryGet<MappingDataNode>("slots", ref slots))
		{
			nodes.Add((ValidationNode)new ErrorNode((DataNode)(object)node, "No slots mapping data node found", true));
		}
		else if (root != null)
		{
			MappingDataNode val = default(MappingDataNode);
			if (!slots.TryGet<MappingDataNode>(root.Value, ref val))
			{
				nodes.Add((ValidationNode)new ErrorNode((DataNode)(object)slots, "No slot found with id " + root.Value, true));
				return (ValidationNode)new ValidatedSequenceNode(nodes);
			}
			foreach (KeyValuePair<string, DataNode> item in slots)
			{
				item.Deconstruct(out var _, out var value);
				DataNode value2 = value;
				MappingDataNode slot = (MappingDataNode)(object)((value2 is MappingDataNode) ? value2 : null);
				if (slot == null)
				{
					nodes.Add((ValidationNode)new ErrorNode(value2, "Slot is not a mapping data node", true));
					continue;
				}
				(ValidationNode, List<string>) result = ValidateSlot(slot, dependencies);
				nodes.Add(result.Item1);
				foreach (string connection in result.Item2)
				{
					if (!slots.TryGet<MappingDataNode>(connection, ref val))
					{
						nodes.Add((ValidationNode)new ErrorNode((DataNode)(object)slots, "No slot found with id " + connection, true));
					}
				}
			}
		}
		return (ValidationNode)new ValidatedSequenceNode(nodes);
	}

	public BodyPrototype Read(ISerializationManager serializationManager, MappingDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null, InstantiationDelegate<BodyPrototype>? instanceProvider = null)
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Expected O, but got Unknown
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		string id = node.Get<ValueDataNode>("id").Value;
		string name = node.Get<ValueDataNode>("name").Value;
		string root = node.Get<ValueDataNode>("root").Value;
		MappingDataNode obj = node.Get<MappingDataNode>("slots");
		Dictionary<string, (string, HashSet<string>, Dictionary<string, string>)> allConnections = new Dictionary<string, (string, HashSet<string>, Dictionary<string, string>)>();
		string key;
		ValueDataNode value2 = default(ValueDataNode);
		SequenceDataNode slotConnectionsNode = default(SequenceDataNode);
		MappingDataNode slotOrgansNode = default(MappingDataNode);
		foreach (KeyValuePair<string, DataNode> item2 in obj)
		{
			item2.Deconstruct(out key, out var value);
			string slotId = key;
			MappingDataNode slot = (MappingDataNode)value;
			string part = null;
			if (slot.TryGet<ValueDataNode>("part", ref value2))
			{
				part = value2.Value;
			}
			HashSet<string> connections = null;
			if (slot.TryGet<SequenceDataNode>("connections", ref slotConnectionsNode))
			{
				connections = new HashSet<string>();
				foreach (ValueDataNode connection in ((IEnumerable)slotConnectionsNode).Cast<ValueDataNode>())
				{
					connections.Add(connection.Value);
				}
			}
			Dictionary<string, string> organs = null;
			if (slot.TryGet<MappingDataNode>("organs", ref slotOrgansNode))
			{
				organs = new Dictionary<string, string>();
				foreach (KeyValuePair<string, DataNode> item3 in slotOrgansNode)
				{
					item3.Deconstruct(out key, out value);
					string organKey = key;
					DataNode organValueNode = value;
					organs.Add(organKey, ((ValueDataNode)organValueNode).Value);
				}
			}
			allConnections.Add(slotId, (part, connections, organs));
		}
		(string, HashSet<string>, Dictionary<string, string>) value3;
		foreach (KeyValuePair<string, (string, HashSet<string>, Dictionary<string, string>)> item4 in allConnections)
		{
			item4.Deconstruct(out key, out value3);
			(string, HashSet<string>, Dictionary<string, string>) tuple = value3;
			string slotId2 = key;
			HashSet<string> connections2 = tuple.Item2;
			if (connections2 == null)
			{
				continue;
			}
			foreach (string connection2 in connections2)
			{
				(string, HashSet<string>, Dictionary<string, string>) other = allConnections[connection2];
				ref HashSet<string> item = ref other.Item2;
				if (item == null)
				{
					item = new HashSet<string>();
				}
				other.Item2.Add(slotId2);
				allConnections[connection2] = other;
			}
		}
		Dictionary<string, BodyPrototypeSlot> slots = new Dictionary<string, BodyPrototypeSlot>();
		foreach (KeyValuePair<string, (string, HashSet<string>, Dictionary<string, string>)> item5 in allConnections)
		{
			item5.Deconstruct(out key, out value3);
			(string, HashSet<string>, Dictionary<string, string>) tuple2 = value3;
			string slotId3 = key;
			string part2 = tuple2.Item1;
			HashSet<string> connections3 = tuple2.Item2;
			Dictionary<string, string> organs2 = tuple2.Item3;
			BodyPrototypeSlot slot2 = new BodyPrototypeSlot(EntProtoId.op_Implicit(part2), connections3 ?? new HashSet<string>(), organs2 ?? new Dictionary<string, string>());
			slots.Add(slotId3, slot2);
		}
		return new BodyPrototype(id, name, root, slots);
	}
}
