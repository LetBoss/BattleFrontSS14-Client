// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Intel.IntelDetectorOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._RMC14.MotionDetector;
using Content.Shared._RMC14.Intel.Detector;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client._RMC14.Intel;

public sealed class IntelDetectorOverlay : Overlay
{
  [Dependency]
  private IEntityManager _entity;
  [Dependency]
  private IGameTiming _timing;
  private TimeSpan _last;
  private readonly List<(Vector2 Pos, bool QueenEye)> _blips = new List<(Vector2, bool)>();
  private readonly MotionDetectorOverlaySystem _motionDetector;
  private readonly SpriteSystem _sprite;

  public virtual OverlaySpace Space => (OverlaySpace) 4;

  public IntelDetectorOverlay()
  {
    IoCManager.InjectDependencies<IntelDetectorOverlay>(this);
    this._motionDetector = this._entity.System<MotionDetectorOverlaySystem>();
    this._sprite = this._entity.System<SpriteSystem>();
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    Texture frame = this._sprite.GetFrame((SpriteSpecifier) new SpriteSpecifier.Rsi(new ResPath("/Textures/_RMC14/Objects/Tools/intel_detector.rsi"), "data_blip"), this._timing.CurTime, true);
    this._motionDetector.DrawBlips<IntelDetectorComponent>(((OverlayDrawArgs) ref args).WorldHandle, ref this._last, this._blips, frame, frame);
  }
}
