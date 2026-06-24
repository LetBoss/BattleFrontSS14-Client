// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Input.InputCmdContext
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System.Collections;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Input;

internal sealed class InputCmdContext : IInputCmdContext, IEnumerable<BoundKeyFunction>, IEnumerable
{
  private readonly List<BoundKeyFunction> _commands = new List<BoundKeyFunction>();
  private readonly IInputCmdContext? _parent;

  public string Name { get; }

  internal InputCmdContext(IInputCmdContext? parent, string name)
  {
    this._parent = parent;
    this.Name = name;
  }

  internal InputCmdContext(string name) => this.Name = name;

  public void AddFunction(BoundKeyFunction function) => this._commands.Add(function);

  public bool FunctionExists(BoundKeyFunction function) => this._commands.Contains(function);

  public bool FunctionExistsHierarchy(BoundKeyFunction function)
  {
    if (this._commands.Contains(function))
      return true;
    return this._parent != null && this._parent.FunctionExistsHierarchy(function);
  }

  public void RemoveFunction(BoundKeyFunction function) => this._commands.Remove(function);

  public IEnumerator<BoundKeyFunction> GetEnumerator()
  {
    return (IEnumerator<BoundKeyFunction>) this._commands.GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();
}
