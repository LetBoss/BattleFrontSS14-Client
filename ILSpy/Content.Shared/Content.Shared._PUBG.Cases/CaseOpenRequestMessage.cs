using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Cases;

[Serializable]
[NetSerializable]
public sealed class CaseOpenRequestMessage : EntityEventArgs
{
	public string CaseId { get; }

	public CaseOpenRequestMessage(string caseId)
	{
		CaseId = caseId;
	}
}
