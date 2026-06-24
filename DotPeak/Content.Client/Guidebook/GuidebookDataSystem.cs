// Decompiled with JetBrains decompiler
// Type: Content.Client.Guidebook.GuidebookDataSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Guidebook;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Guidebook;

public sealed class GuidebookDataSystem : EntitySystem
{
  private GuidebookData? _data;

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<UpdateGuidebookDataEvent>(new EntityEventHandler<UpdateGuidebookDataEvent>(this.OnServerUpdated), (Type[]) null, (Type[]) null);
    this.RaiseNetworkEvent((EntityEventArgs) new RequestGuidebookDataEvent());
  }

  private void OnServerUpdated(UpdateGuidebookDataEvent args)
  {
    this._data = args.Data;
    this._data.Freeze();
  }

  public bool TryGetValue(string prototype, string component, string field, out object? value)
  {
    if (this._data != null)
      return this._data.TryGetValue(prototype, component, field, out value);
    value = (object) null;
    return false;
  }
}
