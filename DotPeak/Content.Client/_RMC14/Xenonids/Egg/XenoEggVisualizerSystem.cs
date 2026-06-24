// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Xenonids.Egg.XenoEggVisualizerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Xenonids.Egg;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.TypeSerializers.Implementations;
using Robust.Shared.Utility;
using System;

#nullable enable
namespace Content.Client._RMC14.Xenonids.Egg;

public sealed class XenoEggVisualizerSystem : EntitySystem
{
  [Dependency]
  private AnimationPlayerSystem _animation;
  [Dependency]
  private IResourceCache _resourceCache;
  [Dependency]
  private SpriteSystem _sprite;
  private const string AnimationKey = "rmc_egg_destroying";

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<XenoEggComponent, ComponentStartup>(new EntityEventRefHandler<XenoEggComponent, ComponentStartup>((object) this, __methodptr(SetVisuals<ComponentStartup>)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<XenoEggComponent, XenoEggStateChangedEvent>(new EntityEventRefHandler<XenoEggComponent, XenoEggStateChangedEvent>((object) this, __methodptr(SetVisuals<XenoEggStateChangedEvent>)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DestroyedXenoEggComponent, ComponentStartup>(new EntityEventRefHandler<DestroyedXenoEggComponent, ComponentStartup>((object) this, __methodptr(OnStartup)), (Type[]) null, (Type[]) null);
  }

  private void SetVisuals<T>(Entity<XenoEggComponent> ent, ref T args)
  {
    SpriteComponent spriteComponent;
    if (!this.TryComp<SpriteComponent>(Entity<XenoEggComponent>.op_Implicit(ent), ref spriteComponent))
      return;
    string currentSprite = ent.Comp.CurrentSprite;
    RSIResource rsiResource;
    if (!this._resourceCache.TryGetResource<RSIResource>(ResPath.op_Division(SpriteSpecifierSerializer.TextureRoot, currentSprite), ref rsiResource))
      return;
    if (spriteComponent.BaseRSI != rsiResource.RSI)
      this._sprite.SetBaseRsi(Entity<SpriteComponent>.op_Implicit((ent.Owner, spriteComponent)), rsiResource.RSI);
    string str1;
    switch (ent.Comp.State)
    {
      case XenoEggState.Item:
        str1 = ent.Comp.ItemState;
        break;
      case XenoEggState.Growing:
        str1 = ent.Comp.GrowingState;
        break;
      case XenoEggState.Grown:
        str1 = ent.Comp.GrownState;
        break;
      case XenoEggState.Opening:
        str1 = ent.Comp.OpeningState;
        break;
      case XenoEggState.Opened:
        str1 = ent.Comp.OpenedState;
        break;
      default:
        str1 = (string) null;
        break;
    }
    string str2 = str1;
    int num;
    if (string.IsNullOrWhiteSpace(str2) || !this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((ent.Owner, spriteComponent)), (Enum) XenoEggLayers.Base, ref num, false))
      return;
    this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((ent.Owner, spriteComponent)), num, RSI.StateId.op_Implicit(str2));
  }

  private void OnStartup(Entity<DestroyedXenoEggComponent> ent, ref ComponentStartup args)
  {
    if (this._animation.HasRunningAnimation(Entity<DestroyedXenoEggComponent>.op_Implicit(ent), "rmc_egg_destroying"))
      return;
    this._animation.Play(Entity<DestroyedXenoEggComponent>.op_Implicit(ent), new Animation()
    {
      Length = ent.Comp.AnimationTime,
      AnimationTracks = {
        (AnimationTrack) new AnimationTrackSpriteFlick()
        {
          LayerKey = (object) ent.Comp.Layer,
          KeyFrames = {
            new AnimationTrackSpriteFlick.KeyFrame(RSI.StateId.op_Implicit(ent.Comp.AnimationState), 0.0f)
          }
        }
      }
    }, "rmc_egg_destroying");
  }
}
