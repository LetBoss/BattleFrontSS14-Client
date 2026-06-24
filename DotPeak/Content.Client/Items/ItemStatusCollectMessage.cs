// Decompiled with JetBrains decompiler
// Type: Content.Client.Items.ItemStatusCollectMessage
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.Items;

public sealed class ItemStatusCollectMessage : EntityEventArgs
{
  public List<Control> Controls = new List<Control>();
}
