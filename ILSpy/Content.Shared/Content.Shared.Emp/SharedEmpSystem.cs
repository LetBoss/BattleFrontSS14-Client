using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Shared.Emp;

public abstract class SharedEmpSystem : EntitySystem
{
	[Dependency]
	protected IGameTiming Timing;

	protected const string EmpDisabledEffectPrototype = "EffectEmpDisabled";
}
