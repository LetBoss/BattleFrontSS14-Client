using System.Numerics;
using Robust.Shared.Physics.Dynamics;

namespace Robust.Shared.Physics.Systems;

public delegate float CastResult(FixtureProxy proxy, Vector2 point, Vector2 normal, float fraction, ref RayResult result);
