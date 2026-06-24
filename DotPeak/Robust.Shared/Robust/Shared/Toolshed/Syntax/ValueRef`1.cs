// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Syntax.ValueRef`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Toolshed.Syntax;

public abstract class ValueRef<T>
{
  public abstract T? Evaluate(IInvocationContext ctx);

  internal static T? EvaluateParameter(object? obj, IInvocationContext ctx)
  {
    switch (obj)
    {
      case null:
        return default (T);
      case T parameter:
        return parameter;
      case ValueRef<T> valueRef:
        return valueRef.Evaluate(ctx);
      default:
        throw new Exception($"Failed to parse command parameter. This likely is a toolshed bug and should be reported.\nTarget type: {typeof (T).PrettyName()}.\nInput type: {obj.GetType()}.\nInput: {obj}");
    }
  }

  internal static T[] EvaluateParamsCollection(object? obj, IInvocationContext ctx)
  {
    if (!(obj is List<object> objectList))
      throw new Exception("Failed to parse command parameter. This likely is a toolshed bug and should be reported.");
    int num = 0;
    T[] paramsCollection = new T[objectList.Count];
    foreach (object obj1 in objectList)
      paramsCollection[num++] = ValueRef<T>.EvaluateParameter(obj1, ctx);
    return paramsCollection;
  }
}
