// Decompiled with JetBrains decompiler
// Type: Content.Client.Guidebook.Richtext.Table
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.UserInterface.Controls;
using Robust.Client.UserInterface;
using Robust.Shared.Log;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Content.Client.Guidebook.Richtext;

public sealed class Table : TableContainer, IDocumentTag
{
  private static readonly ISawmill Sawmill = Logger.GetSawmill("guidebook.table");

  public bool TryParseTag(Dictionary<string, string> args, [NotNullWhen(true)] out Control? control)
  {
    ((Control) this).HorizontalExpand = true;
    control = (Control) this;
    string s;
    int result;
    if (!args.TryGetValue("Columns", out s) || !int.TryParse(s, out result))
    {
      Table.Sawmill.Error("Guidebook tag \"Table\" does not specify required property \"Columns.\"");
      control = (Control) null;
      return false;
    }
    this.Columns = result;
    return true;
  }
}
