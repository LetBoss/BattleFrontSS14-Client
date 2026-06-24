// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.IInvocationContext
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Utility;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Toolshed;

public interface IInvocationContext
{
  bool CheckInvokable(CommandSpec command, out IConError? error)
  {
    return this.Toolshed.CheckInvokable(command, this.Session, out error);
  }

  ToolshedEnvironment Environment { get; }

  ICommonSession? Session { get; }

  NetUserId? User { get; }

  ToolshedManager Toolshed { get; }

  void WriteLine(string line);

  void WriteLine(FormattedMessage line)
  {
    if (this.Session == null)
      this.WriteLine(line.ToString());
    else
      this.WriteLine(line.ToMarkup());
  }

  void WriteMarkup(string markup) => this.WriteLine(FormattedMessage.FromMarkupPermissive(markup));

  void WriteError(IConError error) => this.WriteLine(error.Describe());

  void ReportError(IConError err);

  IEnumerable<IConError> GetErrors();

  bool HasErrors { get; }

  void ClearErrors();

  object? ReadVar(string name);

  void WriteVar(string name, object? value);

  bool IsReadonlyVar(string name) => false;

  IEnumerable<string> GetVars();
}
