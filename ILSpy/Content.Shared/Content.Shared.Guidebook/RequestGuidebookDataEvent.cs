using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Guidebook;

[Serializable]
[NetSerializable]
public sealed class RequestGuidebookDataEvent : EntityEventArgs
{
}
