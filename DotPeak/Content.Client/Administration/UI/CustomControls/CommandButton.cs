// Decompiled with JetBrains decompiler
// Type: Content.Client.Administration.UI.CustomControls.CommandButton
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Guidebook.Richtext;
using Robust.Client.Console;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Analyzers;
using Robust.Shared.Console;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Content.Client.Administration.UI.CustomControls;

[Virtual]
public class CommandButton : Button, IDocumentTag
{
  private static readonly ISawmill Sawmill = Logger.GetSawmill("admin.command_button");

  public string? Command { get; set; }

  public CommandButton()
  {
    ((BaseButton) this).OnPressed += new Action<BaseButton.ButtonEventArgs>(this.Execute);
  }

  protected virtual bool CanPress()
  {
    return string.IsNullOrEmpty(this.Command) || ((IClientConGroupImplementation) IoCManager.Resolve<IClientConGroupController>()).CanCommand(this.Command.Split(' ')[0]);
  }

  protected virtual void EnteredTree()
  {
    if (this.CanPress())
      return;
    ((Control) this).Visible = false;
  }

  protected virtual void Execute(BaseButton.ButtonEventArgs obj)
  {
    if (string.IsNullOrEmpty(this.Command))
      return;
    ((IConsoleHost) IoCManager.Resolve<IClientConsoleHost>()).ExecuteCommand(this.Command);
  }

  public bool TryParseTag(Dictionary<string, string> args, [NotNullWhen(true)] out Control? control)
  {
    string str1;
    string str2;
    if (args.Count != 2 || !args.TryGetValue("Text", out str1) || !args.TryGetValue("Command", out str2))
    {
      CommandButton.Sawmill.Error("Invalid arguments passed to CommandButton");
      control = (Control) null;
      return false;
    }
    this.Command = str2;
    this.Text = Loc.GetString(str1);
    control = (Control) this;
    return true;
  }
}
