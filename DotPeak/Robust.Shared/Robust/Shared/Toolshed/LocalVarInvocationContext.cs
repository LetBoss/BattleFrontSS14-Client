// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.LocalVarInvocationContext
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Toolshed.Errors;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Toolshed;

public sealed class LocalVarInvocationContext(IInvocationContext inner) : IInvocationContext
{
  public Dictionary<string, object?> LocalVars = new Dictionary<string, object>();
  public HashSet<string>? ReadonlyVars;

  public bool CheckInvokable(CommandSpec command, out IConError? error)
  {
    return inner.CheckInvokable(command, out error);
  }

  public ICommonSession? Session => inner.Session;

  public ToolshedManager Toolshed => inner.Toolshed;

  public NetUserId? User => inner.User;

  public ToolshedEnvironment Environment => inner.Environment;

  public void WriteLine(string line) => inner.WriteLine(line);

  public void ReportError(IConError err) => inner.ReportError(err);

  public IEnumerable<IConError> GetErrors() => inner.GetErrors();

  public bool HasErrors => inner.HasErrors;

  public void ClearErrors() => inner.ClearErrors();

  public void SetLocal(string name, object? value) => this.LocalVars[name] = value;

  public void SetLocal(string name, object? value, bool @readonly)
  {
    this.LocalVars[name] = value;
    this.SetReadonly(name, @readonly);
  }

  public void SetReadonly(string name, bool @readonly)
  {
    if (@readonly)
    {
      if (this.ReadonlyVars == null)
        this.ReadonlyVars = new HashSet<string>();
      this.ReadonlyVars.Add(name);
    }
    else
      this.ReadonlyVars?.Remove(name);
  }

  public void ClearLocal(string name)
  {
    this.LocalVars.Remove(name);
    this.ReadonlyVars?.Remove(name);
  }

  public object? ReadVar(string name)
  {
    object obj;
    return !this.LocalVars.TryGetValue(name, out obj) ? inner.ReadVar(name) : obj;
  }

  public void WriteVar(string name, object? value)
  {
    if (this.ReadonlyVars != null && this.ReadonlyVars.Contains(name))
      this.ReportError((IConError) new ReadonlyVariableError(name));
    else if (this.LocalVars.ContainsKey(name))
      this.LocalVars[name] = value;
    else
      inner.WriteVar(name, value);
  }

  public bool IsReadonlyVar(string name)
  {
    return this.ReadonlyVars != null && this.ReadonlyVars.Contains(name);
  }

  public IEnumerable<string> GetVars()
  {
    foreach (string key in this.LocalVars.Keys)
      yield return key;
    foreach (string var in inner.GetVars())
    {
      if (!this.LocalVars.ContainsKey(var))
        yield return var;
    }
  }
}
