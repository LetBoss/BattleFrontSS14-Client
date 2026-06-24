// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.Airdrop.PubgAirdropSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._PUBG.Airdrop;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client._PUBG.Airdrop;

public sealed class PubgAirdropSystem : EntitySystem
{
  public bool Active { get; private set; }

  public Vector2 Position { get; private set; }

  public int RemainingSeconds { get; private set; }

  public MapId MapId { get; private set; }

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<PubgAirdropStateEvent>(new EntitySessionEventHandler<PubgAirdropStateEvent>(this.OnAirdropState), (Type[]) null, (Type[]) null);
  }

  private void OnAirdropState(PubgAirdropStateEvent ev, EntitySessionEventArgs args)
  {
    this.Active = ev.Active;
    this.Position = ev.Position;
    this.RemainingSeconds = ev.RemainingSeconds;
    this.MapId = ev.MapId;
  }
}
