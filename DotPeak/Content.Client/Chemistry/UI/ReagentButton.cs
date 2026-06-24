// Decompiled with JetBrains decompiler
// Type: Content.Client.Chemistry.UI.ReagentButton
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Chemistry;
using Content.Shared.Chemistry.Reagent;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;

#nullable enable
namespace Content.Client.Chemistry.UI;

public sealed class ReagentButton : Button
{
  public bool IsBuffer = true;

  public ChemMasterReagentAmount Amount { get; set; }

  public ReagentId Id { get; set; }

  public ReagentButton(
    string text,
    ChemMasterReagentAmount amount,
    ReagentId id,
    bool isBuffer,
    string styleClass)
  {
    ((Control) this).AddStyleClass(styleClass);
    this.Text = text;
    this.Amount = amount;
    this.Id = id;
    this.IsBuffer = isBuffer;
  }
}
