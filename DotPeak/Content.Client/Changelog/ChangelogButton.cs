// Decompiled with JetBrains decompiler
// Type: Content.Client.Changelog.ChangelogButton
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using System;

#nullable enable
namespace Content.Client.Changelog;

public sealed class ChangelogButton : Button
{
  [Dependency]
  private ChangelogManager _changelogManager;

  public ChangelogButton()
  {
    IoCManager.InjectDependencies<ChangelogButton>(this);
    this.Text = " ";
  }

  protected virtual void EnteredTree()
  {
    ((Control) this).EnteredTree();
    this._changelogManager.NewChangelogEntriesChanged += new Action(this.UpdateStuff);
    this.UpdateStuff();
  }

  protected virtual void ExitedTree()
  {
    ((Control) this).ExitedTree();
    this._changelogManager.NewChangelogEntriesChanged -= new Action(this.UpdateStuff);
  }

  private void UpdateStuff()
  {
    if (this._changelogManager.NewChangelogEntries)
    {
      this.Text = Loc.GetString("changelog-button-new-entries");
      ((Control) this).StyleClasses.Add("Caution");
    }
    else
    {
      this.Text = Loc.GetString("changelog-button");
      ((Control) this).StyleClasses.Remove("Caution");
    }
  }
}
