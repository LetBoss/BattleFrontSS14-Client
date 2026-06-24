// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Invocation.OldShellInvocationContext
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Utility;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Robust.Shared.Toolshed.Invocation;

internal sealed class OldShellInvocationContext : IInvocationContext
{
  private readonly List<IConError> _errors = new List<IConError>();
  public IConsoleShell? Shell;

  [field: Dependency]
  public ToolshedManager Toolshed { get; }

  public ToolshedEnvironment Environment => this.Toolshed.DefaultEnvironment;

  public NetUserId? User { get; }

  public ICommonSession? Session => this.Shell?.Player;

  public OldShellInvocationContext(IConsoleShell shell)
  {
    IoCManager.InjectDependencies<OldShellInvocationContext>(this);
    this.Shell = shell;
    this.User = this.Session?.UserId;
  }

  public void WriteLine(string line) => this.Shell?.WriteLine(line);

  public void WriteLine(FormattedMessage line) => this.Shell?.WriteLine(line);

  public void ReportError(IConError err) => this._errors.Add(err);

  public IEnumerable<IConError> GetErrors() => (IEnumerable<IConError>) this._errors;

  public bool HasErrors => this._errors.Count > 0;

  public void ClearErrors() => this._errors.Clear();

  public object? ReadVar(string name)
  {
    if (name == "self")
    {
      EntityUid? attachedEntity = (EntityUid?) this.Session?.AttachedEntity;
      if (attachedEntity.HasValue)
        return (object) attachedEntity.GetValueOrDefault();
    }
    return this.Variables.GetValueOrDefault<string, object>(name);
  }

  public void WriteVar(string name, object? value)
  {
    if (name == "self")
      this.ReportError((IConError) new ReadonlyVariableError("self"));
    else
      this.Variables[name] = value;
  }

  public bool IsReadonlyVar(string name) => name == "self";

  public IEnumerable<string> GetVars()
  {
    ICommonSession session = this.Session;
    return (session != null ? (session.AttachedEntity.HasValue ? 1 : 0) : 0) == 0 ? (IEnumerable<string>) this.Variables.Keys : this.Variables.Keys.Append<string>("self");
  }

  public Dictionary<string, object?> Variables { get; } = new Dictionary<string, object>();
}
