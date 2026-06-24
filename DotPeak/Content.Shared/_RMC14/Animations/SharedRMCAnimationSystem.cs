// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Animations.SharedRMCAnimationSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Utility;

#nullable enable
namespace Content.Shared._RMC14.Animations;

public abstract class SharedRMCAnimationSystem : EntitySystem
{
  [Dependency]
  private INetManager _net;

  public void Play(Entity<RMCAnimationComponent?> ent, string key)
  {
    NetEntity? netEntity;
    if (this._net.IsClient || !this.TryGetNetEntity((EntityUid) ent, out netEntity))
      return;
    this.RaiseNetworkEvent((EntityEventArgs) new RMCPlayAnimationEvent(netEntity.Value, new RMCAnimationId(key)), Filter.Pvs((EntityUid) ent));
  }

  public void Flick(
    Entity<RMCAnimationComponent?> ent,
    SpriteSpecifier.Rsi animationRsi,
    SpriteSpecifier.Rsi defaultRsi,
    string? layer = null)
  {
    NetEntity? netEntity;
    if (this._net.IsClient || !this.TryGetNetEntity((EntityUid) ent, out netEntity))
      return;
    this.RaiseNetworkEvent((EntityEventArgs) new RMCFlickEvent(netEntity.Value, animationRsi, defaultRsi, layer), Filter.Pvs((EntityUid) ent));
  }

  public void TryFlick(
    Entity<RMCAnimationComponent?> ent,
    SpriteSpecifier.Rsi? animationRsi,
    SpriteSpecifier.Rsi? defaultRsi,
    string? layer = null)
  {
    if (animationRsi == null || defaultRsi == null)
      return;
    this.Flick(ent, animationRsi, defaultRsi, layer);
  }
}
