using System.Collections.Generic;
using Robust.Shared.Physics.Dynamics;

namespace Robust.Shared.GameObjects;

public sealed class GridFixtureChangeEvent : EntityEventArgs
{
	public Dictionary<string, Fixture> NewFixtures { get; init; }
}
