namespace Robust.Shared.GameObjects;

public static class BoundUserInterfaceRegisterExt
{
	public delegate void BuiEventSubscriber<TComp>(Subscriber<TComp> subscriber) where TComp : IComponent;

	public sealed class Subscriber<TComp> where TComp : IComponent
	{
		private readonly EntitySystem.Subscriptions _subs;

		private readonly object _uiKey;

		internal Subscriber(EntitySystem.Subscriptions subs, object uiKey)
		{
			_subs = subs;
			_uiKey = uiKey;
		}

		public void Event<TEvent>(ComponentEventHandler<TComp, TEvent> handler) where TEvent : BaseBoundUserInterfaceEvent
		{
			_subs.SubscribeLocalEvent(delegate(EntityUid uid, TComp component, TEvent args)
			{
				if (_uiKey.Equals(args.UiKey))
				{
					handler(uid, component, args);
				}
			});
		}

		public void Event<TEvent>(EntityEventRefHandler<TComp, TEvent> handler) where TEvent : BaseBoundUserInterfaceEvent
		{
			_subs.SubscribeLocalEvent(delegate(Entity<TComp> ent, ref TEvent args)
			{
				if (_uiKey.Equals(args.UiKey))
				{
					handler(ent, ref args);
				}
			});
		}
	}

	public static void BuiEvents<TComp>(this EntitySystem.Subscriptions subs, object uiKey, BuiEventSubscriber<TComp> subscriber) where TComp : IComponent
	{
		subscriber(new Subscriber<TComp>(subs, uiKey));
	}
}
