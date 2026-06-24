// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.BoundUserInterfaceRegisterExt
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

#nullable enable
namespace Robust.Shared.GameObjects;

public static class BoundUserInterfaceRegisterExt
{
  public static void BuiEvents<TComp>(
    this EntitySystem.Subscriptions subs,
    object uiKey,
    BoundUserInterfaceRegisterExt.BuiEventSubscriber<TComp> subscriber)
    where TComp : IComponent
  {
    subscriber(new BoundUserInterfaceRegisterExt.Subscriber<TComp>(subs, uiKey));
  }

  public delegate void BuiEventSubscriber<TComp>(
    BoundUserInterfaceRegisterExt.Subscriber<TComp> subscriber)
    where TComp : IComponent;

  public sealed class Subscriber<TComp> where TComp : IComponent
  {
    private readonly EntitySystem.Subscriptions _subs;
    private readonly object _uiKey;

    internal Subscriber(EntitySystem.Subscriptions subs, object uiKey)
    {
      this._subs = subs;
      this._uiKey = uiKey;
    }

    public void Event<TEvent>(ComponentEventHandler<TComp, TEvent> handler) where TEvent : BaseBoundUserInterfaceEvent
    {
      this._subs.SubscribeLocalEvent<TComp, TEvent>((ComponentEventHandler<TComp, TEvent>) ((uid, component, args) =>
      {
        if (!this._uiKey.Equals((object) args.UiKey))
          return;
        handler(uid, component, args);
      }));
    }

    public void Event<TEvent>(EntityEventRefHandler<TComp, TEvent> handler) where TEvent : BaseBoundUserInterfaceEvent
    {
      this._subs.SubscribeLocalEvent<TComp, TEvent>((EntityEventRefHandler<TComp, TEvent>) ((Entity<TComp> ent, ref TEvent args) =>
      {
        if (!this._uiKey.Equals((object) args.UiKey))
          return;
        handler(ent, ref args);
      }));
    }
  }
}
