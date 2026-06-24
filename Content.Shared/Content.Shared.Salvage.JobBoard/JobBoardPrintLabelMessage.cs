using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Salvage.JobBoard;

[Serializable]
[NetSerializable]
public sealed class JobBoardPrintLabelMessage : BoundUserInterfaceMessage
{
	public string JobId;

	public JobBoardPrintLabelMessage(string jobId)
	{
		JobId = jobId;
	}
}
