// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Controls.EntityListData
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Shared.GameObjects;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Client.UserInterface.Controls;

public record EntityListData(EntityUid Uid) : ListData
{
  [CompilerGenerated]
  public sealed override bool Equals(ListData? other) => this.Equals((object) other);
}
