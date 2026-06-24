// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Console.ConsoleShell
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Player;
using Robust.Shared.Utility;

#nullable enable
namespace Robust.Shared.Console;

public sealed class ConsoleShell : IConsoleShell
{
  public ConsoleShell(IConsoleHost host, ICommonSession? session, bool isLocal)
  {
    this.ConsoleHost = host;
    this.Player = session;
    this.IsLocal = isLocal;
  }

  public IConsoleHost ConsoleHost { get; }

  public bool IsServer => this.ConsoleHost.IsServer;

  public ICommonSession? Player { get; }

  public bool IsLocal { get; }

  public void ExecuteCommand(string command)
  {
    this.ConsoleHost.ExecuteCommand(this.Player, command);
  }

  public void RemoteExecuteCommand(string command)
  {
    this.ConsoleHost.RemoteExecuteCommand(this.Player, command);
  }

  public void WriteLine(string text) => this.ConsoleHost.WriteLine(this.Player, text);

  public void WriteLine(FormattedMessage message)
  {
    this.ConsoleHost.WriteLine(this.Player, message);
  }

  public void WriteError(string text) => this.ConsoleHost.WriteError(this.Player, text);

  public void Clear()
  {
    if (this.Player != null)
      return;
    this.ConsoleHost.ClearLocalConsole();
  }
}
