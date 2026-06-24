using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Instruments.UI;

[Serializable]
[NetSerializable]
public sealed class InstrumentBandRequestBuiMessage : BoundUserInterfaceMessage
{
}
