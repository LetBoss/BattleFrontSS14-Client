namespace Content.Shared._RMC14.Attachable.Events;

public enum AttachableAlteredType : byte
{
	Attached = 1,
	Detached = 2,
	Wielded = 4,
	Unwielded = 8,
	Activated = 16,
	Deactivated = 32,
	Interrupted = 64,
	AppearanceChanged = 128,
	DetachedDeactivated = 34
}
