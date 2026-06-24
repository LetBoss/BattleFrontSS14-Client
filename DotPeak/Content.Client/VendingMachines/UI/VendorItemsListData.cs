// Decompiled with JetBrains decompiler
// Type: Content.Client.VendingMachines.UI.VendorItemsListData
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.UserInterface.Controls;
using Robust.Shared.Prototypes;
using System.Runtime.CompilerServices;
using System.Text;

#nullable enable
namespace Content.Client.VendingMachines.UI;

public record VendorItemsListData(EntProtoId ItemProtoID, int ItemIndex) : ListData
{
  public string ItemText = string.Empty;

  public EntProtoId ItemProtoID { get; init; } = ItemProtoID;

  public int ItemIndex { get; init; } = ItemIndex;

  [CompilerGenerated]
  protected override bool PrintMembers(StringBuilder builder)
  {
    if (base.PrintMembers(builder))
      builder.Append(", ");
    builder.Append("ItemProtoID = ");
    builder.Append(this.ItemProtoID.ToString());
    builder.Append(", ItemIndex = ");
    builder.Append(this.ItemIndex.ToString());
    builder.Append(", ItemText = ");
    builder.Append((object) this.ItemText);
    return true;
  }

  [CompilerGenerated]
  public sealed override bool Equals(ListData? other) => this.Equals((object) other);
}
