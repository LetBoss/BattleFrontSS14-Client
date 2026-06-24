// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Sprite.RMCSpriteSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.CrashLand;
using Content.Shared._RMC14.Mobs;
using Content.Shared._RMC14.Sprite;
using Content.Shared._RMC14.Xenonids.Hide;
using Content.Shared.Ghost;
using Content.Shared.ParaDrop;
using Robust.Client.GameObjects;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Player;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client._RMC14.Sprite;

public sealed class RMCSpriteSystem : SharedRMCSpriteSystem
{
  [Dependency]
  private IPlayerManager _player;
  [Dependency]
  private SpriteSystem _sprite;
  [Dependency]
  private TransformSystem _transform;

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<RMCMobStateDrawDepthComponent, AppearanceChangeEvent>(new EntityEventRefHandler<RMCMobStateDrawDepthComponent, AppearanceChangeEvent>((object) this, __methodptr(OnDrawDepthAppearanceChange)), (Type[]) null, (Type[]) null);
  }

  private void OnDrawDepthAppearanceChange(
    Entity<RMCMobStateDrawDepthComponent> ent,
    ref AppearanceChangeEvent args)
  {
    if (!args.AppearanceData.ContainsKey((Enum) RMCSpriteDrawDepth.Key))
      return;
    int num = (int) this.UpdateDrawDepth(Entity<RMCMobStateDrawDepthComponent>.op_Implicit(ent));
  }

  public override Content.Shared.DrawDepth.DrawDepth UpdateDrawDepth(EntityUid sprite)
  {
    Content.Shared.DrawDepth.DrawDepth drawDepth = base.UpdateDrawDepth(sprite);
    SpriteComponent spriteComponent;
    if (!this.TryComp<SpriteComponent>(sprite, ref spriteComponent))
      return drawDepth;
    this._sprite.SetDrawDepth(Entity<SpriteComponent>.op_Implicit((sprite, spriteComponent)), (int) drawDepth);
    return drawDepth;
  }

  public void UpdatePosition(EntityUid uid)
  {
    Vector2 worldPosition = ((SharedTransformSystem) this._transform).GetWorldPosition(uid);
    if (MapId.op_Equality(this.Transform(uid).MapID, MapId.Nullspace))
    {
      SpriteComponent spriteComponent;
      if (!this.TryComp<SpriteComponent>(uid, ref spriteComponent))
        return;
      this._sprite.SetOffset(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), new Vector2());
    }
    else
    {
      Vector2 vector2 = worldPosition with
      {
        Y = worldPosition.Y + 0.0001f
      };
      ((SharedTransformSystem) this._transform).SetWorldPosition(uid, vector2);
    }
  }

  public virtual void Update(float frameTime)
  {
    this.UpdateColors();
    this.UpdatePositions();
    this.UpdateLocalDrawDepth();
  }

  private void UpdateColors()
  {
    try
    {
      EntityQueryEnumerator<SpriteColorComponent, SpriteComponent> entityQueryEnumerator = this.EntityQueryEnumerator<SpriteColorComponent, SpriteComponent>();
      EntityUid entityUid;
      SpriteColorComponent spriteColorComponent;
      SpriteComponent spriteComponent;
      while (entityQueryEnumerator.MoveNext(ref entityUid, ref spriteColorComponent, ref spriteComponent))
        this._sprite.SetColor(Entity<SpriteComponent>.op_Implicit((entityUid, spriteComponent)), spriteColorComponent.Color);
    }
    catch (Exception ex)
    {
      this.Log.Error($"Error updating {"SpriteColorComponent"} colors:\n{ex}");
    }
  }

  private void UpdatePositions()
  {
    try
    {
      EntityQueryEnumerator<RMCUpdateClientLocationComponent> entityQueryEnumerator = this.EntityQueryEnumerator<RMCUpdateClientLocationComponent>();
      EntityUid uid;
      RMCUpdateClientLocationComponent locationComponent;
      while (entityQueryEnumerator.MoveNext(ref uid, ref locationComponent))
        this.UpdatePosition(uid);
    }
    catch (Exception ex)
    {
      this.Log.Error($"Error updating {"RMCUpdateClientLocationComponent"} positions:\n{ex}");
    }
  }

  private void UpdateLocalDrawDepth()
  {
    try
    {
      EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
      if (!localEntity.HasValue)
        return;
      EntityUid valueOrDefault = localEntity.GetValueOrDefault();
      XenoHideComponent xenoHideComponent;
      SpriteComponent spriteComponent;
      if (this.HasComp<GhostComponent>(valueOrDefault) || this.TryComp<XenoHideComponent>(valueOrDefault, ref xenoHideComponent) && xenoHideComponent.Hiding || !this.TryComp<SpriteComponent>(valueOrDefault, ref spriteComponent) || this.HasComp<ParaDroppingComponent>(valueOrDefault) || this.HasComp<CrashLandingComponent>(valueOrDefault))
        return;
      this._sprite.SetDrawDepth(Entity<SpriteComponent>.op_Implicit((valueOrDefault, spriteComponent)), 5);
    }
    catch (Exception ex)
    {
      this.Log.Error($"Error updating local draw depth:\n{ex}");
    }
  }
}
