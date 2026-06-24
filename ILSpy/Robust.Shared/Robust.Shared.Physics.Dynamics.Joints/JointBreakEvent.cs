using Robust.Shared.GameObjects;

namespace Robust.Shared.Physics.Dynamics.Joints;

[ByRefEvent]
public readonly record struct JointBreakEvent(Joint Joint, float JointError);
