using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared._RMC14.MotionDetector;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Client._RMC14.MotionDetector;

public sealed class MotionDetectorOverlay : Overlay
{
	[Dependency]
	private IEntityManager _entity;

	[Dependency]
	private IGameTiming _timing;

	private TimeSpan _last;

	private readonly List<(Vector2 Pos, bool QueenEye)> _blips = new List<(Vector2, bool)>();

	private readonly MotionDetectorOverlaySystem _motionDetector;

	private readonly SpriteSystem _sprite;

	public override OverlaySpace Space => (OverlaySpace)4;

	public MotionDetectorOverlay()
	{
		IoCManager.InjectDependencies<MotionDetectorOverlay>(this);
		_motionDetector = _entity.System<MotionDetectorOverlaySystem>();
		_sprite = _entity.System<SpriteSystem>();
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Expected O, but got Unknown
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Expected O, but got Unknown
		Texture frame = _sprite.GetFrame((SpriteSpecifier)new Rsi(new ResPath("/Textures/_RMC14/Objects/Tools/motion_detector.rsi"), "detector_blip"), _timing.CurTime, true);
		Texture frame2 = _sprite.GetFrame((SpriteSpecifier)new Rsi(new ResPath("/Textures/_RMC14/Objects/Tools/motion_detector.rsi"), "queen_eye_blip"), _timing.CurTime, true);
		_motionDetector.DrawBlips<MotionDetectorComponent>(((OverlayDrawArgs)(ref args)).WorldHandle, ref _last, _blips, frame, frame2);
	}
}
