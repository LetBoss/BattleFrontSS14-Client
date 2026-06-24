using System;
using Robust.Shared.Audio;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Intel.Tech;

[Serializable]
[DataRecord]
[NetSerializable]
public sealed record TechAnnounceEvent(string Author, string Message, SoundSpecifier? Sound);
