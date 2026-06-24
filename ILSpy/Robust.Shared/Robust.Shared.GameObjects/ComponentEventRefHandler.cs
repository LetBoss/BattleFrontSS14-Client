namespace Robust.Shared.GameObjects;

public delegate void ComponentEventRefHandler<in TComp, TEvent>(EntityUid uid, TComp component, ref TEvent args) where TComp : IComponent where TEvent : notnull;
