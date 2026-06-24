// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Syntax.ValueRef`2
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;

#nullable enable
namespace Robust.Shared.Toolshed.Syntax;

[Obsolete("Use EntProtoId / ProtoId<T>")]
public sealed class ValueRef<T, TAuto>(ValueRef<T> inner) : ValueRef<T>
{
  public override T? Evaluate(IInvocationContext ctx) => inner.Evaluate(ctx);
}
