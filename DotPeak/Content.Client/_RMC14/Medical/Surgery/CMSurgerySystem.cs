// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Medical.Surgery.CMSurgerySystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Medical.Surgery;
using System;

#nullable enable
namespace Content.Client._RMC14.Medical.Surgery;

public sealed class CMSurgerySystem : SharedCMSurgerySystem
{
  public event Action? OnRefresh;

  public virtual void Update(float frameTime)
  {
    Action onRefresh = this.OnRefresh;
    if (onRefresh == null)
      return;
    onRefresh();
  }
}
