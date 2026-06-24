namespace Robust.Shared.Configuration;

public delegate void CVarChanged<in T>(T newValue, in CVarChangeInfo info);
