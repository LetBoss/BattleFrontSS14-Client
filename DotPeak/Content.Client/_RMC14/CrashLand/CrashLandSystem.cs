// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.CrashLand.CrashLandSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._RMC14.ParaDrop;
using Content.Client._RMC14.Sprite;
using Content.Shared._RMC14.CrashLand;
using Content.Shared.ParaDrop;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client._RMC14.CrashLand;

public sealed class CrashLandSystem : SharedCrashLandSystem
{
  [Dependency]
  private AnimationPlayerSystem _animPlayer;
  [Dependency]
  private ParaDropSystem _paraDrop;
  [Dependency]
  private RMCSpriteSystem _rmcSprite;
  [Dependency]
  private SpriteSystem _sprite;
  private const string CrashingAnimationKey = "crashing-animation";

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<CrashLandingComponent, ComponentRemove>(new EntityEventRefHandler<CrashLandingComponent, ComponentRemove>((object) this, __methodptr(OnRemove)), (Type[]) null, (Type[]) null);
  }

  private void OnRemove(Entity<CrashLandingComponent> ent, ref ComponentRemove args)
  {
    if (this.TerminatingOrDeleted(Entity<CrashLandingComponent>.op_Implicit(ent), (MetaDataComponent) null))
      return;
    AnimationPlayerComponent animationPlayerComponent;
    if (this.TryComp<AnimationPlayerComponent>(Entity<CrashLandingComponent>.op_Implicit(ent), ref animationPlayerComponent))
      this._animPlayer.Stop(Entity<AnimationPlayerComponent>.op_Implicit((Entity<CrashLandingComponent>.op_Implicit(ent), animationPlayerComponent)), "crashing-animation");
    SpriteComponent spriteComponent;
    if (!this.TryComp<SpriteComponent>(Entity<CrashLandingComponent>.op_Implicit(ent), ref spriteComponent))
      return;
    this._sprite.SetOffset(Entity<SpriteComponent>.op_Implicit((Entity<CrashLandingComponent>.op_Implicit(ent), spriteComponent)), new Vector2());
  }

  public override void Update(float frameTime)
  {
    base.Update(frameTime);
    EntityQueryEnumerator<CrashLandableComponent, CrashLandingComponent> entityQueryEnumerator = this.EntityQueryEnumerator<CrashLandableComponent, CrashLandingComponent>();
    EntityUid entityUid;
    CrashLandableComponent landableComponent;
    CrashLandingComponent landingComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref landableComponent, ref landingComponent))
    {
      if (!this.HasComp<SkyFallingComponent>(entityUid))
      {
        if (!this._animPlayer.HasRunningAnimation(entityUid, "crashing-animation") && landableComponent.LastCrash.HasValue)
          this._paraDrop.PlayFallAnimation(entityUid, landableComponent.CrashDuration, landingComponent.RemainingTime, landableComponent.FallHeight, "crashing-animation");
        this._rmcSprite.UpdatePosition(entityUid);
      }
    }
  }
}
