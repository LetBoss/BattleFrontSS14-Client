using System;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Content.Shared.Objectives;

[Serializable]
[NetSerializable]
public record struct ObjectiveInfo(string Title, string Description, SpriteSpecifier Icon, float Progress);
