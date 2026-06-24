using System;
using Robust.Shared.Analyzers;
using Robust.Shared.Serialization;

namespace Robust.Shared.GameObjects;

[Serializable]
[RequiresSerializable]
[NetSerializable]
[Virtual]
public abstract class ComponentState : IComponentState
{
}
