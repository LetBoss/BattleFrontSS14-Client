using Robust.Shared.Input.Binding;

namespace Robust.Shared.GameObjects;

public abstract class SharedInputSystem : EntitySystem
{
	private CommandBindRegistry _bindRegistry;

	public ICommandBindRegistry BindRegistry => _bindRegistry;

	protected override void PostInject()
	{
		base.PostInject();
		_bindRegistry = new CommandBindRegistry(base.Log);
	}
}
