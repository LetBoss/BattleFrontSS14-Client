// Decompiled with JetBrains decompiler
// Type: Content.Client.Voting.UI.CreateVoteOption
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Client.Voting.UI;

public record struct CreateVoteOption(
  string name,
  List<Dictionary<string, string>> dropdowns,
  bool enableVoteWarning,
  int? followDropdownId)
{
  public string Name = name;
  public List<Dictionary<string, string>> Dropdowns = dropdowns;
  public bool EnableVoteWarning = enableVoteWarning;
  public int? FollowDropdownId = followDropdownId;

  [CompilerGenerated]
  public override readonly int GetHashCode()
  {
    return ((EqualityComparer<string>.Default.GetHashCode(this.Name) * -1521134295 + EqualityComparer<List<Dictionary<string, string>>>.Default.GetHashCode(this.Dropdowns)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.EnableVoteWarning)) * -1521134295 + EqualityComparer<int?>.Default.GetHashCode(this.FollowDropdownId);
  }

  [CompilerGenerated]
  public readonly bool Equals(CreateVoteOption other)
  {
    return EqualityComparer<string>.Default.Equals(this.Name, other.Name) && EqualityComparer<List<Dictionary<string, string>>>.Default.Equals(this.Dropdowns, other.Dropdowns) && EqualityComparer<bool>.Default.Equals(this.EnableVoteWarning, other.EnableVoteWarning) && EqualityComparer<int?>.Default.Equals(this.FollowDropdownId, other.FollowDropdownId);
  }
}
