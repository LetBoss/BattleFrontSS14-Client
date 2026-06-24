using System;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.CartridgeLoader.Cartridges;

[Serializable]
[NetSerializable]
[DataRecord]
public sealed class NanoTaskItem
{
	public static int MaximumStringLength = 30;

	public readonly string Description;

	public readonly string TaskIsFor;

	public readonly bool IsTaskDone;

	public readonly NanoTaskPriority Priority;

	public NanoTaskItem(string description, string taskIsFor, bool isTaskDone, NanoTaskPriority priority)
	{
		Description = description;
		TaskIsFor = taskIsFor;
		IsTaskDone = isTaskDone;
		Priority = priority;
	}

	public bool Validate()
	{
		if (Description.Length <= MaximumStringLength)
		{
			return TaskIsFor.Length <= MaximumStringLength;
		}
		return false;
	}
}
