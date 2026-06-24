using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.HeliSupply;

[Serializable]
[NetSerializable]
public sealed class CivHeliCancelMessage : EntityEventArgs
{
}
