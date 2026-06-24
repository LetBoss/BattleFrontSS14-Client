using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Supply;

[Serializable]
[NetSerializable]
public sealed class CivSupplyRefillRequestEvent : EntityEventArgs
{
}
