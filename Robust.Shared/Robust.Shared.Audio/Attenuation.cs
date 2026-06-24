namespace Robust.Shared.Audio;

public enum Attenuation
{
	Invalid = 0,
	NoAttenuation = 1,
	InverseDistance = 2,
	InverseDistanceClamped = 4,
	LinearDistance = 8,
	LinearDistanceClamped = 0x10,
	ExponentDistance = 0x20,
	ExponentDistanceClamped = 0x40
}
