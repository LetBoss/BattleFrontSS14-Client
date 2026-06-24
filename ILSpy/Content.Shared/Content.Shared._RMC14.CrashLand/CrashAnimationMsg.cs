using System;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.CrashLand;

[Serializable]
[NetSerializable]
public abstract class CrashAnimationMsg : FallAnimationEventArgs
{
}
