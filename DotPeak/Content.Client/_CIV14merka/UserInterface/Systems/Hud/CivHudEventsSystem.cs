// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.UserInterface.Systems.Hud.CivHudEventsSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._CIV14merka;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client._CIV14merka.UserInterface.Systems.Hud;

public sealed class CivHudEventsSystem : EntitySystem
{
  public event Action<CivHudStatusEvent>? OnStatusReceived;

  public CivHudStatusEvent? LastStatus { get; private set; }

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<CivHudStatusEvent>(new EntitySessionEventHandler<CivHudStatusEvent>(this.OnStatus), (Type[]) null, (Type[]) null);
  }

  private void OnStatus(CivHudStatusEvent msg, EntitySessionEventArgs args)
  {
    this.LastStatus = msg;
    Action<CivHudStatusEvent> onStatusReceived = this.OnStatusReceived;
    if (onStatusReceived == null)
      return;
    onStatusReceived(msg);
  }
}
