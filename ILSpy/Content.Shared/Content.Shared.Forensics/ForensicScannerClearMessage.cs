using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Forensics;

[Serializable]
[NetSerializable]
public sealed class ForensicScannerClearMessage : BoundUserInterfaceMessage
{
}
