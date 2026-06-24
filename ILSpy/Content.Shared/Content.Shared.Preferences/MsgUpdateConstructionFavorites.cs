using System.Collections.Generic;
using Content.Shared.Construction.Prototypes;
using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.Preferences;

public sealed class MsgUpdateConstructionFavorites : NetMessage
{
	public List<ProtoId<ConstructionPrototype>> Favorites = new List<ProtoId<ConstructionPrototype>>();

	public override MsgGroups MsgGroup => (MsgGroups)4;

	public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		int length = ((NetBuffer)buffer).ReadVariableInt32();
		Favorites.Clear();
		for (int i = 0; i < length; i++)
		{
			Favorites.Add(new ProtoId<ConstructionPrototype>(((NetBuffer)buffer).ReadString()));
		}
	}

	public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		((NetBuffer)buffer).WriteVariableInt32(Favorites.Count);
		foreach (ProtoId<ConstructionPrototype> favorite in Favorites)
		{
			((NetBuffer)buffer).Write(ProtoId<ConstructionPrototype>.op_Implicit(favorite));
		}
	}
}
