// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Controls.TabId
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Shared.Toolshed.TypeParsers;
using System;

#nullable disable
namespace Content.Client.UserInterface.Controls;

public record struct TabId(int Id) : IEquatable<int>, IComparable<TabId>, IAsType<int>
{
  public static implicit operator TabId(int id) => new TabId(id);

  public static implicit operator int(TabId id) => id.Id;

  public bool Equals(int other) => this.Id == other;

  public int CompareTo(TabId other) => this.Id.CompareTo(other.Id);

  public int AsType() => this.Id;
}
