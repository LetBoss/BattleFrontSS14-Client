namespace Robust.Shared.GameObjects;

public enum ComponentLifeStage : byte
{
	PreAdd,
	Adding,
	Added,
	Initializing,
	Initialized,
	Starting,
	Running,
	Stopping,
	Stopped,
	Removing,
	Deleted
}
