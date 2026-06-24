using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.WorldEdit;

[Serializable]
[NetSerializable]
public sealed class WorldEditCancelPreviewEvent : EntityEventArgs
{
}
