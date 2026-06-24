using System.Collections.Generic;
using Robust.Shared.Prototypes;

namespace Content.Shared.Roles;

public interface IEquipmentLoadout
{
	Dictionary<string, EntProtoId> Equipment { get; set; }

	List<EntProtoId> Inhand { get; set; }

	Dictionary<string, List<EntProtoId>> Storage { get; set; }

	string GetGear(string slot)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (!Equipment.TryGetValue(slot, out var equipment))
		{
			return string.Empty;
		}
		return EntProtoId.op_Implicit(equipment);
	}
}
