using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Robust.Shared.GameObjects;

public readonly struct EntityQuery<TComp1> where TComp1 : IComponent
{
	private readonly EntityManager _entMan;

	private readonly Dictionary<EntityUid, IComponent> _traitDict;

	internal EntityQuery(EntityManager entMan, Dictionary<EntityUid, IComponent> traitDict)
	{
		_entMan = entMan;
		_traitDict = traitDict;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public TComp1 GetComponent(EntityUid uid)
	{
		if (_traitDict.TryGetValue(uid, out IComponent value) && !value.Deleted)
		{
			return (TComp1)value;
		}
		throw new KeyNotFoundException($"Entity {uid} does not have a component of type {typeof(TComp1)}");
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Entity<TComp1> Get(EntityUid uid)
	{
		if (_traitDict.TryGetValue(uid, out IComponent value) && !value.Deleted)
		{
			return new Entity<TComp1>(uid, (TComp1)value);
		}
		throw new KeyNotFoundException($"Entity {uid} does not have a component of type {typeof(TComp1)}");
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool TryGetComponent([NotNullWhen(true)] EntityUid? uid, [NotNullWhen(true)] out TComp1? component)
	{
		if (!uid.HasValue)
		{
			component = default(TComp1);
			return false;
		}
		return TryGetComponent(uid.Value, out component);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool TryGetComponent(EntityUid uid, [NotNullWhen(true)] out TComp1? component)
	{
		if (_traitDict.TryGetValue(uid, out IComponent value) && !value.Deleted)
		{
			component = (TComp1)value;
			return true;
		}
		component = default(TComp1);
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool TryComp(EntityUid uid, [NotNullWhen(true)] out TComp1? component)
	{
		return TryGetComponent(uid, out component);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool TryComp([NotNullWhen(true)] EntityUid? uid, [NotNullWhen(true)] out TComp1? component)
	{
		return TryGetComponent(uid, out component);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool HasComp(EntityUid uid)
	{
		return HasComponent(uid);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool HasComp([NotNullWhen(true)] EntityUid? uid)
	{
		return HasComponent(uid);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool HasComponent(EntityUid uid)
	{
		if (_traitDict.TryGetValue(uid, out IComponent value))
		{
			return !value.Deleted;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool HasComponent([NotNullWhen(true)] EntityUid? uid)
	{
		if (uid.HasValue)
		{
			return HasComponent(uid.Value);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Resolve(EntityUid uid, [NotNullWhen(true)] ref TComp1? component, bool logMissing = true)
	{
		if (component != null)
		{
			return true;
		}
		if (_traitDict.TryGetValue(uid, out IComponent value) && !value.Deleted)
		{
			component = (TComp1)value;
			return true;
		}
		if (logMissing)
		{
			_entMan.ResolveSawmill.Error($"Can't resolve \"{typeof(TComp1)}\" on entity {_entMan.ToPrettyString(uid)}!\n{Environment.StackTrace}");
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Resolve(ref Entity<TComp1?> entity, bool logMissing = true)
	{
		return Resolve(entity.Owner, ref entity.Comp, logMissing);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public TComp1? CompOrNull(EntityUid uid)
	{
		if (TryGetComponent(uid, out TComp1 component))
		{
			return component;
		}
		return default(TComp1);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public TComp1 Comp(EntityUid uid)
	{
		return GetComponent(uid);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal TComp1 GetComponentInternal(EntityUid uid)
	{
		if (_traitDict.TryGetValue(uid, out IComponent value))
		{
			return (TComp1)value;
		}
		throw new KeyNotFoundException($"Entity {uid} does not have a component of type {typeof(TComp1)}");
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal bool TryGetComponentInternal([NotNullWhen(true)] EntityUid? uid, [NotNullWhen(true)] out TComp1? component)
	{
		if (!uid.HasValue)
		{
			component = default(TComp1);
			return false;
		}
		return TryGetComponentInternal(uid.Value, out component);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal bool TryGetComponentInternal(EntityUid uid, [NotNullWhen(true)] out TComp1? component)
	{
		if (_traitDict.TryGetValue(uid, out IComponent value))
		{
			component = (TComp1)value;
			return true;
		}
		component = default(TComp1);
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal bool HasComponentInternal(EntityUid uid)
	{
		if (_traitDict.TryGetValue(uid, out IComponent value))
		{
			return !value.Deleted;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal bool ResolveInternal(EntityUid uid, [NotNullWhen(true)] ref TComp1? component, bool logMissing = true)
	{
		if (component != null)
		{
			return true;
		}
		if (_traitDict.TryGetValue(uid, out IComponent value))
		{
			component = (TComp1)value;
			return true;
		}
		if (logMissing)
		{
			_entMan.ResolveSawmill.Error($"Can't resolve \"{typeof(TComp1)}\" on entity {_entMan.ToPrettyString(uid)}!\n{new StackTrace(1, fNeedFileInfo: true)}");
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal TComp1? CompOrNullInternal(EntityUid uid)
	{
		if (TryGetComponent(uid, out TComp1 component))
		{
			return component;
		}
		return default(TComp1);
	}
}
