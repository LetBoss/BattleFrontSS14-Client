// Decompiled with JetBrains decompiler
// Type: Robust.Shared.ViewVariables.ViewVariablesTypeHandler`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Log;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Robust.Shared.ViewVariables;

public sealed class ViewVariablesTypeHandler<T> : ViewVariablesTypeHandler
{
  private readonly List<ViewVariablesTypeHandler<
  #nullable disable
  T>.TypeHandlerData> _handlers = new List<ViewVariablesTypeHandler<T>.TypeHandlerData>();
  private readonly 
  #nullable enable
  Dictionary<string, PathHandler> _paths = new Dictionary<string, PathHandler>();
  private readonly ISawmill _sawmill;

  internal ViewVariablesTypeHandler(ISawmill sawmill) => this._sawmill = sawmill;

  public ViewVariablesTypeHandler<T> AddHandler(
    HandleTypePath<T> handle,
    ListTypeCustomPaths<T> list)
  {
    this._handlers.Add(new ViewVariablesTypeHandler<T>.TypeHandlerData(new HandleTypePath(HandleWrapper), new ListTypeCustomPaths(ListWrapper), (object) handle, (object) list));
    return this;

    ViewVariablesPath? HandleWrapper(ViewVariablesPath path, string relativePath)
    {
      object obj = path.Get();
      return obj != null ? handle((T) obj, relativePath) : (ViewVariablesPath) null;
    }

    IEnumerable<string> ListWrapper(ViewVariablesPath path)
    {
      object obj = path.Get();
      return obj != null ? list((T) obj) : Enumerable.Empty<string>();
    }
  }

  public ViewVariablesTypeHandler<T> AddHandlerNullable(
    HandleTypePathNullable<T> handle,
    ListTypeCustomPathsNullable<T> list)
  {
    this._handlers.Add(new ViewVariablesTypeHandler<T>.TypeHandlerData(new HandleTypePath(HandleWrapper), new ListTypeCustomPaths(ListWrapper), (object) handle, (object) list));
    return this;

    ViewVariablesPath? HandleWrapper(ViewVariablesPath path, string relativePath)
    {
      return handle((T) path.Get(), relativePath);
    }

    IEnumerable<string> ListWrapper(ViewVariablesPath path) => list((T) path.Get());
  }

  public ViewVariablesTypeHandler<T> AddHandler(
    HandleTypePathComponent<T> handle,
    ListTypeCustomPathsComponent<T> list)
  {
    this._handlers.Add(new ViewVariablesTypeHandler<T>.TypeHandlerData(new HandleTypePath(HandleWrapper), new ListTypeCustomPaths(ListWrapper), (object) handle, (object) list));
    return this;

    ViewVariablesPath? HandleWrapper(ViewVariablesPath path, string relativePath)
    {
      return path is ViewVariablesComponentPath variablesComponentPath1 && variablesComponentPath1.Get() is T comp1 ? handle(variablesComponentPath1.Owner, comp1, relativePath) : (ViewVariablesPath) null;
    }

    IEnumerable<string> ListWrapper(ViewVariablesPath path)
    {
      return path is ViewVariablesComponentPath variablesComponentPath2 && variablesComponentPath2.Get() is T comp2 ? list(variablesComponentPath2.Owner, comp2) : Enumerable.Empty<string>();
    }
  }

  public ViewVariablesTypeHandler<T> AddHandler(HandleTypePath handle, ListTypeCustomPaths list)
  {
    this._handlers.Add(new ViewVariablesTypeHandler<T>.TypeHandlerData(handle, list));
    return this;
  }

  public ViewVariablesTypeHandler<T> RemoveHandler(
    HandleTypePath<T> handle,
    ListTypeCustomPaths<T> list)
  {
    return this.RemoveHandlerInternal((object) handle, (object) list, true);
  }

  public ViewVariablesTypeHandler<T> RemoveHandlerNullable(
    HandleTypePathNullable<T> handle,
    ListTypeCustomPathsNullable<T> list)
  {
    return this.RemoveHandlerInternal((object) handle, (object) list, true);
  }

  public ViewVariablesTypeHandler<T> RemoveHandler(
    HandleTypePathComponent<T> handle,
    ListTypeCustomPathsComponent<T> list)
  {
    return this.RemoveHandlerInternal((object) handle, (object) list, true);
  }

  public ViewVariablesTypeHandler<T> RemoveHandler(HandleTypePath handle, ListTypeCustomPaths list)
  {
    return this.RemoveHandlerInternal((object) handle, (object) list);
  }

  private ViewVariablesTypeHandler<T> RemoveHandlerInternal(
    object handle,
    object list,
    bool originalCompare = false)
  {
    for (int index = 0; index < this._handlers.Count; ++index)
    {
      ViewVariablesTypeHandler<T>.TypeHandlerData handler = this._handlers[index];
      if ((originalCompare || handle.Equals((object) handler.Handle) && list.Equals((object) handler.List)) && (!originalCompare || handle.Equals(handler.OriginalHandle) && list.Equals(handler.OriginalList)))
      {
        this._handlers.RemoveAt(index);
        return this;
      }
    }
    throw new ArgumentException("The specified arguments were not found in the list!");
  }

  public ViewVariablesTypeHandler<T> AddPath(string path, PathHandler<T> handler)
  {
    return this.AddPathNullable(path, new PathHandlerNullable<T>(Wrapper));

    ViewVariablesPath? Wrapper(T? t) => (object) t == null ? (ViewVariablesPath) null : handler(t);
  }

  public ViewVariablesTypeHandler<T> AddPathNullable(string path, PathHandlerNullable<T> handler)
  {
    return this.AddPath(path, new PathHandler(Wrapper));

    ViewVariablesPath? Wrapper(ViewVariablesPath p) => handler((T) p.Get());
  }

  public ViewVariablesTypeHandler<T> AddPath(string path, PathHandlerComponent<T> handler)
  {
    return this.AddPath(path, new PathHandler(Wrapper));

    ViewVariablesPath? Wrapper(ViewVariablesPath p)
    {
      if (p is ViewVariablesComponentPath variablesComponentPath)
      {
        object component = variablesComponentPath.Get();
        if (component != null)
          return handler(variablesComponentPath.Owner, (T) component);
      }
      return (ViewVariablesPath) null;
    }
  }

  public ViewVariablesTypeHandler<T> AddPath<TValue>(
    string path,
    ComponentPropertyGetter<T, TValue> getter,
    ComponentPropertySetter<T, TValue>? setter = null)
  {
    return this.AddPath(path, new PathHandler(Wrapper));

    ViewVariablesPath? Wrapper(ViewVariablesPath p)
    {
      ViewVariablesComponentPath pc = p as ViewVariablesComponentPath;
      if (pc != null)
      {
        object obj = pc.Get();
        if (obj != null)
        {
          T comp = (T) obj;
          ViewVariablesFakePath variablesFakePath = ViewVariablesPath.FromGetter((Func<object>) (() => (object) getter(pc.Owner, comp)), typeof (TValue));
          if (setter != null)
            variablesFakePath = variablesFakePath.WithSetter((Action<object>) (value =>
            {
              try
              {
                setter(pc.Owner, (TValue) value, comp);
              }
              catch (NullReferenceException ex)
              {
                this._sawmill.Log(LogLevel.Error, (Exception) ex, $"NRE caught in setter for path \"{path}\" for type \"{typeof (T).Name}\"...");
              }
            }));
          return (ViewVariablesPath) variablesFakePath;
        }
      }
      return (ViewVariablesPath) null;
    }
  }

  public ViewVariablesTypeHandler<T> AddPath(string path, PathHandler handler)
  {
    this._paths.Add(path, handler);
    return this;
  }

  public ViewVariablesTypeHandler<T> RemovePath(string path)
  {
    this._paths.Remove(path);
    return this;
  }

  internal override ViewVariablesPath? HandlePath(ViewVariablesPath path, string relativePath)
  {
    foreach (ViewVariablesTypeHandler<T>.TypeHandlerData handler in this._handlers)
    {
      ViewVariablesPath viewVariablesPath = handler.Handle(path, relativePath);
      if (viewVariablesPath != null)
        return viewVariablesPath;
    }
    PathHandler pathHandler;
    return !this._paths.TryGetValue(relativePath, out pathHandler) ? (ViewVariablesPath) null : pathHandler(path);
  }

  internal override IEnumerable<string> ListPath(ViewVariablesPath path)
  {
    foreach (ViewVariablesTypeHandler<T>.TypeHandlerData handler in this._handlers)
    {
      IEnumerator<string> enumerator = handler.List(path).GetEnumerator();
      while (enumerator.MoveNext())
        yield return enumerator.Current;
      enumerator = (IEnumerator<string>) null;
    }
    foreach ((string key, PathHandler pathHandler) in this._paths)
    {
      if (pathHandler(path) != null)
        yield return key;
    }
  }

  private sealed class TypeHandlerData
  {
    public readonly HandleTypePath Handle;
    public readonly ListTypeCustomPaths List;
    public readonly object? OriginalHandle;
    public readonly object? OriginalList;

    public TypeHandlerData(
      HandleTypePath handle,
      ListTypeCustomPaths list,
      object? origHandle = null,
      object? origList = null)
    {
      this.Handle = handle;
      this.List = list;
      this.OriginalHandle = origHandle;
      this.OriginalList = origList;
    }
  }
}
