namespace Robust.Shared.GameObjects;

public delegate void ComponentEventHandler<in TComp, in TEvent>(EntityUid uid, TComp component, TEvent args) where TComp : IComponent where TEvent : notnull;
