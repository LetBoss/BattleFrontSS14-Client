namespace Robust.Shared.GameObjects;

public delegate void ComponentQueryCallback<T>(EntityUid uid, T component) where T : IComponent;
