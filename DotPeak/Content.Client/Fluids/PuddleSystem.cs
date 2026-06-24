// Decompiled with JetBrains decompiler
// Type: Content.Client.Fluids.PuddleSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.IconSmoothing;
using Content.Shared.Chemistry.Components;
using Content.Shared.Fluids;
using Content.Shared.Fluids.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using System;

#nullable enable
namespace Content.Client.Fluids;

public sealed class PuddleSystem : SharedPuddleSystem
{
  [Dependency]
  private IconSmoothSystem _smooth;
  [Dependency]
  private SpriteSystem _sprite;

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<PuddleComponent, AppearanceChangeEvent>(new ComponentEventRefHandler<PuddleComponent, AppearanceChangeEvent>((object) this, __methodptr(OnPuddleAppearance)), (Type[]) null, (Type[]) null);
  }

  private void OnPuddleAppearance(
    EntityUid uid,
    PuddleComponent component,
    ref AppearanceChangeEvent args)
  {
    if (args.Sprite == null)
      return;
    float num = 1f;
    object obj1;
    if (args.AppearanceData.TryGetValue((Enum) PuddleVisuals.CurrentVolume, out obj1))
      num = (float) obj1;
    IconSmoothComponent component1;
    if (this.TryComp<IconSmoothComponent>(uid, ref component1))
    {
      if ((double) num < 0.30000001192092896)
      {
        this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), 0, RSI.StateId.op_Implicit(component1.StateBase + "a"));
        this._smooth.SetEnabled(uid, false, component1);
      }
      else if ((double) num < 0.60000002384185791)
      {
        this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), 0, RSI.StateId.op_Implicit(component1.StateBase + "b"));
        this._smooth.SetEnabled(uid, false, component1);
      }
      else if (!component1.Enabled)
      {
        this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), 0, RSI.StateId.op_Implicit(component1.StateBase + "0"));
        this._smooth.SetEnabled(uid, true, component1);
        this._smooth.DirtyNeighbours(uid);
      }
    }
    Color white = Color.White;
    object obj2;
    if (args.AppearanceData.TryGetValue((Enum) PuddleVisuals.SolutionColor, out obj2))
    {
      Color color = (Color) obj2;
      this._sprite.SetColor(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), Color.op_Multiply(ref color, ref white));
    }
    else
    {
      SpriteSystem sprite = this._sprite;
      Entity<SpriteComponent> entity = Entity<SpriteComponent>.op_Implicit((uid, args.Sprite));
      Color color1 = args.Sprite.Color;
      Color color2 = Color.op_Multiply(ref color1, ref white);
      sprite.SetColor(entity, color2);
    }
  }

  public override bool TrySplashSpillAt(
    EntityUid uid,
    EntityCoordinates coordinates,
    Solution solution,
    out EntityUid puddleUid,
    bool sound = true,
    EntityUid? user = null)
  {
    puddleUid = EntityUid.Invalid;
    return false;
  }

  public override bool TrySpillAt(
    EntityCoordinates coordinates,
    Solution solution,
    out EntityUid puddleUid,
    bool sound = true)
  {
    puddleUid = EntityUid.Invalid;
    return false;
  }

  public override bool TrySpillAt(
    EntityUid uid,
    Solution solution,
    out EntityUid puddleUid,
    bool sound = true,
    TransformComponent? transformComponent = null)
  {
    puddleUid = EntityUid.Invalid;
    return false;
  }

  public override bool TrySpillAt(
    TileRef tileRef,
    Solution solution,
    out EntityUid puddleUid,
    bool sound = true,
    bool tileReact = true)
  {
    puddleUid = EntityUid.Invalid;
    return false;
  }
}
