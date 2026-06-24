using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Cases;

[Serializable]
[NetSerializable]
public sealed class CaseOpenResultEvent : EntityEventArgs
{
	public int WinningIndex;
}
