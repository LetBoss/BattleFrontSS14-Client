using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Robust.Shared.GameObjects;

public abstract class SharedAppearanceSystem : EntitySystem
{
	[Dependency]
	private readonly IGameTiming _timing;

	public override void Initialize()
	{
		base.Initialize();
		SubscribeLocalEvent<AppearanceComponent, ComponentGetState>(OnAppearanceGetState);
	}

	protected abstract void OnAppearanceGetState(EntityUid uid, AppearanceComponent component, ref ComponentGetState args);

	public virtual void QueueUpdate(EntityUid uid, AppearanceComponent component)
	{
	}

	private bool CheckIfApplyingState(AppearanceComponent component)
	{
		if (_timing.ApplyingState)
		{
			return component.NetSyncEnabled;
		}
		return false;
	}

	public void SetData(EntityUid uid, Enum key, object value, AppearanceComponent? component = null)
	{
		if (Resolve(uid, ref component, logMissing: false) && !CheckIfApplyingState(component) && (!component.AppearanceData.TryGetValue(key, out object value2) || !value2.Equals(value)))
		{
			component.AppearanceData[key] = value;
			Dirty(uid, component);
			QueueUpdate(uid, component);
		}
	}

	public void RemoveData(EntityUid uid, Enum key, AppearanceComponent? component = null)
	{
		if (Resolve(uid, ref component, logMissing: false) && !CheckIfApplyingState(component))
		{
			component.AppearanceData.Remove(key);
			Dirty(uid, component);
			QueueUpdate(uid, component);
		}
	}

	public bool TryGetData<T>(EntityUid uid, Enum key, [NotNullWhen(true)] out T value, AppearanceComponent? component = null)
	{
		if (Resolve(uid, ref component) && component.AppearanceData.TryGetValue(key, out object value2) && value2 is T)
		{
			value = (T)value2;
			return true;
		}
		value = default(T);
		return false;
	}

	public bool TryGetData(EntityUid uid, Enum key, [NotNullWhen(true)] out object? value, AppearanceComponent? component = null)
	{
		if (!Resolve(uid, ref component))
		{
			value = null;
			return false;
		}
		return component.AppearanceData.TryGetValue(key, out value);
	}

	public void CopyData(Entity<AppearanceComponent?> src, Entity<AppearanceComponent?> dest)
	{
		if (!Resolve(src, ref src.Comp, logMissing: false))
		{
			return;
		}
		ref AppearanceComponent comp = ref dest.Comp;
		if (comp == null)
		{
			comp = EnsureComp<AppearanceComponent>(dest);
		}
		dest.Comp.AppearanceData.Clear();
		foreach (var (key, value) in src.Comp.AppearanceData)
		{
			dest.Comp.AppearanceData[key] = value;
		}
		Dirty(dest, dest.Comp);
		QueueUpdate(dest, dest.Comp);
	}

	public void AppendData(Entity<AppearanceComponent?> src, Entity<AppearanceComponent?> dest)
	{
		if (Resolve(src, ref src.Comp, logMissing: false))
		{
			AppendData(src.Comp, dest);
		}
	}

	public void AppendData(AppearanceComponent srcComp, Entity<AppearanceComponent?> dest)
	{
		ref AppearanceComponent comp = ref dest.Comp;
		if (comp == null)
		{
			comp = EnsureComp<AppearanceComponent>(dest);
		}
		foreach (var (key, value) in srcComp.AppearanceData)
		{
			dest.Comp.AppearanceData[key] = value;
		}
		Dirty(dest, dest.Comp);
		QueueUpdate(dest, dest.Comp);
	}
}
