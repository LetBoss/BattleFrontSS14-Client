// Decompiled with JetBrains decompiler
// Type: Content.Client.Administration.UI.CustomControls.AdminLogLabel
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Administration.Logs;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;
using System;

#nullable enable
namespace Content.Client.Administration.UI.CustomControls;

public sealed class AdminLogLabel : RichTextLabel
{
  public AdminLogLabel(ref SharedAdminLog log, HSeparator separator)
  {
    this.Log = log;
    this.Separator = separator;
    this.SetMessage($"{log.Date:HH:mm:ss}: {log.Message}", new Color?());
    ((Control) this).OnVisibilityChanged += new Action<Control>(this.VisibilityChanged);
  }

  public SharedAdminLog Log { get; }

  public HSeparator Separator { get; }

  private void VisibilityChanged(Control control)
  {
    this.Separator.Visible = ((Control) this).Visible;
  }

  [Obsolete]
  protected virtual void Dispose(bool disposing)
  {
    ((Control) this).Dispose(disposing);
    ((Control) this).OnVisibilityChanged -= new Action<Control>(this.VisibilityChanged);
  }
}
