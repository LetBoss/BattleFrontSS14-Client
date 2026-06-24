using System;
using System.Collections.Generic;
using System.Linq;
using Robust.Shared.Log;

namespace Robust.Shared.ViewVariables;

public abstract class ViewVariablesTypeHandler
{
	internal abstract ViewVariablesPath? HandlePath(ViewVariablesPath path, string relativePath);

	internal abstract IEnumerable<string> ListPath(ViewVariablesPath path);
}
public sealed class ViewVariablesTypeHandler<T> : ViewVariablesTypeHandler
{
	private sealed class TypeHandlerData
	{
		public readonly HandleTypePath Handle;

		public readonly ListTypeCustomPaths List;

		public readonly object? OriginalHandle;

		public readonly object? OriginalList;

		public TypeHandlerData(HandleTypePath handle, ListTypeCustomPaths list, object? origHandle = null, object? origList = null)
		{
			Handle = handle;
			List = list;
			OriginalHandle = origHandle;
			OriginalList = origList;
		}
	}

	private readonly List<TypeHandlerData> _handlers = new List<TypeHandlerData>();

	private readonly Dictionary<string, PathHandler> _paths = new Dictionary<string, PathHandler>();

	private readonly ISawmill _sawmill;

	internal ViewVariablesTypeHandler(ISawmill sawmill)
	{
		_sawmill = sawmill;
	}

	public ViewVariablesTypeHandler<T> AddHandler(HandleTypePath<T> handle, ListTypeCustomPaths<T> list)
	{
		_handlers.Add(new TypeHandlerData(HandleWrapper, ListWrapper, handle, list));
		return this;
		ViewVariablesPath? HandleWrapper(ViewVariablesPath path, string relativePath)
		{
			object obj = path.Get();
			if (obj != null)
			{
				return handle((T)obj, relativePath);
			}
			return null;
		}
		IEnumerable<string> ListWrapper(ViewVariablesPath path)
		{
			object obj = path.Get();
			if (obj != null)
			{
				return list((T)obj);
			}
			return Enumerable.Empty<string>();
		}
	}

	public ViewVariablesTypeHandler<T> AddHandlerNullable(HandleTypePathNullable<T> handle, ListTypeCustomPathsNullable<T> list)
	{
		_handlers.Add(new TypeHandlerData(HandleWrapper, ListWrapper, handle, list));
		return this;
		ViewVariablesPath? HandleWrapper(ViewVariablesPath path, string relativePath)
		{
			return handle((T)path.Get(), relativePath);
		}
		IEnumerable<string> ListWrapper(ViewVariablesPath path)
		{
			return list((T)path.Get());
		}
	}

	public ViewVariablesTypeHandler<T> AddHandler(HandleTypePathComponent<T> handle, ListTypeCustomPathsComponent<T> list)
	{
		_handlers.Add(new TypeHandlerData(HandleWrapper, ListWrapper, handle, list));
		return this;
		ViewVariablesPath? HandleWrapper(ViewVariablesPath path, string relativePath)
		{
			if (!(path is ViewVariablesComponentPath viewVariablesComponentPath) || !(viewVariablesComponentPath.Get() is T comp))
			{
				return null;
			}
			return handle(viewVariablesComponentPath.Owner, comp, relativePath);
		}
		IEnumerable<string> ListWrapper(ViewVariablesPath path)
		{
			if (!(path is ViewVariablesComponentPath viewVariablesComponentPath) || !(viewVariablesComponentPath.Get() is T comp))
			{
				return Enumerable.Empty<string>();
			}
			return list(viewVariablesComponentPath.Owner, comp);
		}
	}

	public ViewVariablesTypeHandler<T> AddHandler(HandleTypePath handle, ListTypeCustomPaths list)
	{
		_handlers.Add(new TypeHandlerData(handle, list));
		return this;
	}

	public ViewVariablesTypeHandler<T> RemoveHandler(HandleTypePath<T> handle, ListTypeCustomPaths<T> list)
	{
		return RemoveHandlerInternal(handle, list, originalCompare: true);
	}

	public ViewVariablesTypeHandler<T> RemoveHandlerNullable(HandleTypePathNullable<T> handle, ListTypeCustomPathsNullable<T> list)
	{
		return RemoveHandlerInternal(handle, list, originalCompare: true);
	}

	public ViewVariablesTypeHandler<T> RemoveHandler(HandleTypePathComponent<T> handle, ListTypeCustomPathsComponent<T> list)
	{
		return RemoveHandlerInternal(handle, list, originalCompare: true);
	}

	public ViewVariablesTypeHandler<T> RemoveHandler(HandleTypePath handle, ListTypeCustomPaths list)
	{
		return RemoveHandlerInternal(handle, list);
	}

	private ViewVariablesTypeHandler<T> RemoveHandlerInternal(object handle, object list, bool originalCompare = false)
	{
		for (int i = 0; i < _handlers.Count; i++)
		{
			TypeHandlerData typeHandlerData = _handlers[i];
			if ((originalCompare || (handle.Equals(typeHandlerData.Handle) && list.Equals(typeHandlerData.List))) && (!originalCompare || (handle.Equals(typeHandlerData.OriginalHandle) && list.Equals(typeHandlerData.OriginalList))))
			{
				_handlers.RemoveAt(i);
				return this;
			}
		}
		throw new ArgumentException("The specified arguments were not found in the list!");
	}

	public ViewVariablesTypeHandler<T> AddPath(string path, PathHandler<T> handler)
	{
		return AddPathNullable(path, Wrapper);
		ViewVariablesPath? Wrapper(T? t)
		{
			if (t == null)
			{
				return null;
			}
			return handler(t);
		}
	}

	public ViewVariablesTypeHandler<T> AddPathNullable(string path, PathHandlerNullable<T> handler)
	{
		return AddPath(path, Wrapper);
		ViewVariablesPath? Wrapper(ViewVariablesPath p)
		{
			return handler((T)p.Get());
		}
	}

	public ViewVariablesTypeHandler<T> AddPath(string path, PathHandlerComponent<T> handler)
	{
		return AddPath(path, Wrapper);
		ViewVariablesPath? Wrapper(ViewVariablesPath p)
		{
			if (p is ViewVariablesComponentPath viewVariablesComponentPath)
			{
				object obj = viewVariablesComponentPath.Get();
				if (obj != null)
				{
					return handler(viewVariablesComponentPath.Owner, (T)obj);
				}
			}
			return null;
		}
	}

	public ViewVariablesTypeHandler<T> AddPath<TValue>(string path, ComponentPropertyGetter<T, TValue> getter, ComponentPropertySetter<T, TValue>? setter = null)
	{
		return AddPath(path, Wrapper);
		ViewVariablesPath? Wrapper(ViewVariablesPath p)
		{
			ViewVariablesComponentPath pc = p as ViewVariablesComponentPath;
			if (pc != null)
			{
				object obj = pc.Get();
				if (obj != null)
				{
					T comp = (T)obj;
					ViewVariablesFakePath viewVariablesFakePath = ViewVariablesPath.FromGetter(() => getter(pc.Owner, comp), typeof(TValue));
					if (setter != null)
					{
						viewVariablesFakePath = viewVariablesFakePath.WithSetter(delegate(object? value)
						{
							try
							{
								setter(pc.Owner, (TValue)value, comp);
							}
							catch (NullReferenceException exception)
							{
								_sawmill.Log(LogLevel.Error, exception, $"NRE caught in setter for path \"{path}\" for type \"{typeof(T).Name}\"...");
							}
						});
					}
					return viewVariablesFakePath;
				}
			}
			return null;
		}
	}

	public ViewVariablesTypeHandler<T> AddPath(string path, PathHandler handler)
	{
		_paths.Add(path, handler);
		return this;
	}

	public ViewVariablesTypeHandler<T> RemovePath(string path)
	{
		_paths.Remove(path);
		return this;
	}

	internal override ViewVariablesPath? HandlePath(ViewVariablesPath path, string relativePath)
	{
		foreach (TypeHandlerData handler in _handlers)
		{
			ViewVariablesPath viewVariablesPath = handler.Handle(path, relativePath);
			if (viewVariablesPath != null)
			{
				return viewVariablesPath;
			}
		}
		if (!_paths.TryGetValue(relativePath, out PathHandler value))
		{
			return null;
		}
		return value(path);
	}

	internal override IEnumerable<string> ListPath(ViewVariablesPath path)
	{
		foreach (TypeHandlerData handler in _handlers)
		{
			foreach (string item in handler.List(path))
			{
				yield return item;
			}
		}
		foreach (var (text2, pathHandler2) in _paths)
		{
			if (pathHandler2(path) != null)
			{
				yield return text2;
			}
		}
	}
}
