// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Systems.Chat.AdminChatWindowCommand
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Shared.Console;

#nullable enable
namespace Content.Client.UserInterface.Systems.Chat;

public sealed class AdminChatWindowCommand : LocalizedCommands
{
  public virtual string Command => "achatwindow";

  public virtual void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    ChatWindow chatWindow = new ChatWindow();
    chatWindow.ConfigureForAdminChat();
    chatWindow.OpenCentered();
  }
}
