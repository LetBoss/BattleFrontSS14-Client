namespace Content.Shared.Nuke;

public enum NukeStatus : byte
{
	AWAIT_DISK,
	AWAIT_CODE,
	AWAIT_ARM,
	ARMED,
	COOLDOWN
}
