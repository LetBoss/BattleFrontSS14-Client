using Content.Shared.Stacks;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;

namespace Content.Shared._RMC14.Stack;

public abstract class SharedRMCStackSystem : EntitySystem
{
	[Dependency]
	private SharedStackSystem _stack;

	public virtual EntityUid? Split(Entity<StackComponent?> stack, int amount, EntityCoordinates spawnPosition)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_stack.Use(Entity<StackComponent>.op_Implicit(stack), amount);
		return null;
	}
}
