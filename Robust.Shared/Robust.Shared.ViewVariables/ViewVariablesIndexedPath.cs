using System;
using System.Reflection;
using Robust.Shared.Reflection;

namespace Robust.Shared.ViewVariables;

internal sealed class ViewVariablesIndexedPath : ViewVariablesPath
{
	private readonly object? _object;

	private readonly PropertyInfo _indexer;

	private readonly object?[] _index;

	private readonly VVAccess? _access;

	public override Type Type => _indexer.GetUnderlyingType();

	internal ViewVariablesIndexedPath(object? obj, PropertyInfo indexer, object?[] index, VVAccess? parentAccess)
	{
		if (indexer.GetIndexParameters().Length == 0)
		{
			throw new ArgumentException("PropertyInfo is not an indexer!", "indexer");
		}
		_object = obj;
		_indexer = indexer;
		_index = index;
		_access = parentAccess;
	}

	public override object? Get()
	{
		if (!_access.HasValue)
		{
			return null;
		}
		try
		{
			return (_object != null) ? _indexer.GetValue(_object, _index) : null;
		}
		catch (Exception)
		{
			return null;
		}
	}

	public override void Set(object? value)
	{
		if (_access == VVAccess.ReadWrite && _object != null)
		{
			_indexer.SetValue(_object, value, _index);
		}
	}

	public override object? Invoke(object?[]? parameters)
	{
		return null;
	}
}
