using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Cases;

[Serializable]
[NetSerializable]
public sealed class CaseOpenErrorMessage : EntityEventArgs
{
	public string CaseId { get; }

	public string ErrorCode { get; }

	public CaseOpenErrorMessage(string caseId, string errorCode)
	{
		CaseId = caseId;
		ErrorCode = errorCode;
	}
}
