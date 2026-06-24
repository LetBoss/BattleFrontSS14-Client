using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Fax;

[Serializable]
[NetSerializable]
public sealed class FaxCopyMessage : BoundUserInterfaceMessage
{
}
