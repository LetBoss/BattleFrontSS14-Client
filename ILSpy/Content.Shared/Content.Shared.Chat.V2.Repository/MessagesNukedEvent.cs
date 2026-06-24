using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Chat.V2.Repository;

[Serializable]
[NetSerializable]
public sealed class MessagesNukedEvent(List<uint> set) : EntityEventArgs
{
	public uint[] MessageIds = CollectionsMarshal.AsSpan(set).ToArray();
}
