namespace Content.Shared.Pinpointer;

public enum NavMapChunkType : byte
{
	Invalid = byte.MaxValue,
	Floor = 0,
	Wall = 4,
	Airlock = 8
}
