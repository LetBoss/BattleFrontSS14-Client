// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Input.IInputCmdContext
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using System.Collections;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Input;

[NotContentImplementable]
public interface IInputCmdContext : IEnumerable<BoundKeyFunction>, IEnumerable
{
  void AddFunction(BoundKeyFunction function);

  bool FunctionExists(BoundKeyFunction function);

  bool FunctionExistsHierarchy(BoundKeyFunction function);

  void RemoveFunction(BoundKeyFunction function);

  string Name { get; }
}
