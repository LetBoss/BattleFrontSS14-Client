using System;
using System.Reflection;
using Robust.Shared.GameObjects;
using Robust.Shared.Reflection;

namespace Robust.Shared.ViewVariables;

internal sealed class ViewVariablesFieldOrPropertyPath : ViewVariablesPath
{
	private readonly IEntityManager _entMan;

	private readonly object? _object;

	private readonly MemberInfo _member;

	private readonly VVAccess? _access;

	public override Type Type => _member.GetUnderlyingType();

	internal ViewVariablesFieldOrPropertyPath(object? obj, MemberInfo member, IEntityManager entMan)
	{
		if ((!(member is FieldInfo) && !(member is PropertyInfo)) || 1 == 0)
		{
			throw new ArgumentException("Member must be either a field or a property!", "member");
		}
		_object = obj;
		_member = member;
		_entMan = entMan;
		ViewVariablesUtility.TryGetViewVariablesAccess(member, out _access);
	}

	public override object? Get()
	{
		if (!_access.HasValue)
		{
			return null;
		}
		try
		{
			return (_object != null) ? _member.GetValue(_object) : null;
		}
		catch (Exception)
		{
			return null;
		}
	}

	public override void Set(object? value)
	{
		if (_access == VVAccess.ReadWrite)
		{
			if (_object != null)
			{
				_member.SetValue(_object, value);
			}
			if (ParentComponent != null)
			{
				_entMan.Dirty(ParentComponent.Owner, ParentComponent.Component);
			}
		}
	}

	public override object? Invoke(object?[]? parameters)
	{
		return null;
	}
}
