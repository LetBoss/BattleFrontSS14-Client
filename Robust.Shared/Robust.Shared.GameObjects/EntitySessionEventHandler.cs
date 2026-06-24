namespace Robust.Shared.GameObjects;

public delegate void EntitySessionEventHandler<in T>(T msg, EntitySessionEventArgs args);
