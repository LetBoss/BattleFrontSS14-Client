namespace Robust.Shared.Physics.Systems;

internal enum ContactStatus : byte
{
	NoContact,
	StartTouching,
	Touching,
	EndTouching
}
