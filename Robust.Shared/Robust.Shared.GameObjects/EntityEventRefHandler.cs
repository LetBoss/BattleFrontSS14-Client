namespace Robust.Shared.GameObjects;

public delegate void EntityEventRefHandler<TComp, TEvent>(Entity<TComp> ent, ref TEvent args) where TComp : IComponent where TEvent : notnull;
public delegate void EntityEventRefHandler<T>(ref T ev);
