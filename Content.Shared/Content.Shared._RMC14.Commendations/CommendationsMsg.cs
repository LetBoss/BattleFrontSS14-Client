using System.Collections.Generic;
using Content.Shared.Database;
using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Commendations;

public sealed class CommendationsMsg : NetMessage
{
	public List<Commendation> CommendationsReceived = new List<Commendation>();

	public List<Commendation> CommendationsGiven = new List<Commendation>();

	public override MsgGroups MsgGroup => (MsgGroups)1;

	public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
	{
		CommendationsReceived.Clear();
		CommendationsGiven.Clear();
		ReadCommendations(buffer, CommendationsReceived);
		ReadCommendations(buffer, CommendationsGiven);
	}

	public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
	{
		WriteCommendations(buffer, CommendationsReceived);
		WriteCommendations(buffer, CommendationsGiven);
	}

	private void ReadCommendations(NetIncomingMessage buffer, List<Commendation> commendations)
	{
		int length = ((NetBuffer)buffer).ReadInt32();
		for (int i = 0; i < length; i++)
		{
			string giver = ((NetBuffer)buffer).ReadString();
			string receiver = ((NetBuffer)buffer).ReadString();
			string name = ((NetBuffer)buffer).ReadString();
			string text = ((NetBuffer)buffer).ReadString();
			CommendationType type = (CommendationType)((NetBuffer)buffer).ReadInt32();
			int round = ((NetBuffer)buffer).ReadInt32();
			commendations.Add(new Commendation(giver, receiver, name, text, type, round));
		}
	}

	private void WriteCommendations(NetOutgoingMessage buffer, List<Commendation> commendations)
	{
		((NetBuffer)buffer).Write(commendations.Count);
		foreach (Commendation commendation in commendations)
		{
			((NetBuffer)buffer).Write(commendation.Giver);
			((NetBuffer)buffer).Write(commendation.Receiver);
			((NetBuffer)buffer).Write(commendation.Name);
			((NetBuffer)buffer).Write(commendation.Text);
			((NetBuffer)buffer).Write((int)commendation.Type);
			((NetBuffer)buffer).Write(commendation.Round);
		}
	}
}
