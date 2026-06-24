using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Wires;

[Serializable]
[NetSerializable]
public sealed class ClientWire
{
	public int Id;

	public bool IsCut;

	public WireColor Color;

	public WireLetter Letter;

	public ClientWire(int id, bool isCut, WireColor color, WireLetter letter)
	{
		Id = id;
		IsCut = isCut;
		Letter = letter;
		Color = color;
	}
}
