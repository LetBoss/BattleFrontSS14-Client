namespace Robust.Shared.Network;

internal interface IHWId
{
	byte[] GetLegacy();

	byte[]? GetModern();
}
