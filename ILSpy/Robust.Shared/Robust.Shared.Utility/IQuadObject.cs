using Robust.Shared.Analyzers;
using Robust.Shared.Maths;

namespace Robust.Shared.Utility;

[NotContentImplementable]
public interface IQuadObject
{
	Box2 Bounds { get; }
}
