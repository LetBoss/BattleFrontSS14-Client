// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Commands.Generic.ListGeneration.ToCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Robust.Shared.Toolshed.Commands.Generic.ListGeneration;

[ToolshedCommand]
public sealed class ToCommand : ToolshedCommand
{
  [CommandImplementation(null)]
  [TakesPipedTypeAsGeneric]
  public IEnumerable<T> To<T>([PipedArgument] T start, T end) where T : INumber<T>
  {
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    return Enumerable.Range(int.CreateTruncating<T>(start), 1 + int.CreateTruncating<T>(end - start)).Select<int, T>(ToCommand.\u003CTo\u003EO__0_0<T>.\u003C0\u003E__CreateTruncating ?? (ToCommand.\u003CTo\u003EO__0_0<T>.\u003C0\u003E__CreateTruncating = new Func<int, T>(INumberBase<T>.CreateTruncating<int>)));
  }
}
