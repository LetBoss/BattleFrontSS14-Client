// Decompiled with JetBrains decompiler
// Type: Content.Client.Items.UI.PollingItemStatusControl`1
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.UserInterface;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Client.Items.UI;

public abstract class PollingItemStatusControl<TData> : Control where TData : struct, IEquatable<TData>
{
  private TData _lastData;

  protected virtual void FrameUpdate(FrameEventArgs args)
  {
    base.FrameUpdate(args);
    TData data = this.PollData();
    if (data.Equals(this._lastData))
      return;
    this._lastData = data;
    this.Update(in data);
  }

  protected abstract TData PollData();

  protected abstract void Update(in TData data);
}
