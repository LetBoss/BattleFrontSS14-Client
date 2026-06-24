// Decompiled with JetBrains decompiler
// Type: Content.Client.Administration.UI.BanList.IBanListLine`1
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Administration.BanList;
using Robust.Client.UserInterface.Controls;

#nullable enable
namespace Content.Client.Administration.UI.BanList;

public interface IBanListLine<T> where T : SharedServerBan
{
  T Ban { get; }

  Label Reason { get; }

  Label BanTime { get; }

  Label Expires { get; }

  Label BanningAdmin { get; }
}
