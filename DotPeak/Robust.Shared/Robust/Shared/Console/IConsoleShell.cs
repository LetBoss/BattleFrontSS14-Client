// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Console.IConsoleShell
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Player;
using Robust.Shared.Utility;

#nullable enable
namespace Robust.Shared.Console;

[NotContentImplementable]
public interface IConsoleShell
{
  IConsoleHost ConsoleHost { get; }

  bool IsClient => !this.IsServer;

  bool IsLocal { get; }

  bool IsServer { get; }

  ICommonSession? Player { get; }

  void ExecuteCommand(string command);

  void RemoteExecuteCommand(string command);

  void WriteLine(string text);

  void WriteLine(FormattedMessage message);

  void WriteMarkup(string markup) => this.WriteLine(FormattedMessage.FromMarkupPermissive(markup));

  void WriteError(string text);

  void Clear();
}
