// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Commands.Math.OrValueCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Toolshed.Syntax;

#nullable enable
namespace Robust.Shared.Toolshed.Commands.Math;

[ToolshedCommand(Name = "or?")]
public sealed class OrValueCommand : ToolshedCommand
{
  [CommandImplementation(null)]
  [TakesPipedTypeAsGeneric]
  public T OrValue<T>(IInvocationContext ctx, [PipedArgument] T? value, ValueRef<T> alternate) where T : class
  {
    return value ?? alternate.Evaluate(ctx);
  }

  [CommandImplementation(null)]
  [TakesPipedTypeAsGeneric]
  public T OrValue<T>(IInvocationContext ctx, [PipedArgument] T? value, ValueRef<T> alternate) where T : unmanaged
  {
    return !value.HasValue ? alternate.Evaluate(ctx) : value.Value;
  }
}
