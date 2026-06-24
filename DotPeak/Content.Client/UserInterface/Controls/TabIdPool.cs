// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Controls.TabIdPool
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using System.Collections.Generic;

#nullable enable
namespace Content.Client.UserInterface.Controls;

public sealed class TabIdPool
{
  private readonly Queue<TabId> _freeIds = new Queue<TabId>();
  private int _nextId = 1;

  public TabId Take()
  {
    return this._freeIds.Count > 0 ? this._freeIds.Dequeue() : new TabId(this._nextId++);
  }

  public void Free(TabId id)
  {
    if (this._freeIds.Contains(id))
      return;
    this._freeIds.Enqueue(id);
  }
}
