namespace Robust.Shared.GameObjects;

public enum EntityLifeStage : byte
{
	PreInit,
	Initializing,
	Initialized,
	MapInitialized,
	Terminating,
	Deleted
}
