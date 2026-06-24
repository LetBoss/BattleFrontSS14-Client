using System;
using System.Collections.Generic;
using Content.Shared._CIV14merka.Commander;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Client._CIV14merka.Commander;

public sealed class CivAirstrikeFlybySystem : EntitySystem
{
	[Dependency]
	private IOverlayManager _overlayManager;

	[Dependency]
	private IGameTiming _timing;

	private CivAirstrikeFlybyOverlay? _overlay;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<CivAirstrikeFlybyEvent>((EntityEventHandler<CivAirstrikeFlybyEvent>)OnFlyby, (Type[])null, (Type[])null);
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		if (_overlay != null && _overlayManager.HasOverlay<CivAirstrikeFlybyOverlay>())
		{
			_overlayManager.RemoveOverlay((Overlay)(object)_overlay);
		}
		_overlay = null;
	}

	public override void Update(float frameTime)
	{
		((EntitySystem)this).Update(frameTime);
		if (_overlay == null)
		{
			return;
		}
		TimeSpan realTime = _timing.RealTime;
		List<CivAirstrikeFlybyInstance> instances = _overlay.Instances;
		for (int num = instances.Count - 1; num >= 0; num--)
		{
			CivAirstrikeFlybyInstance civAirstrikeFlybyInstance = instances[num];
			double totalSeconds = (realTime - civAirstrikeFlybyInstance.StartTime).TotalSeconds;
			float num2 = civAirstrikeFlybyInstance.Total / civAirstrikeFlybyInstance.Speed;
			if (totalSeconds > (double)num2 + 0.5)
			{
				instances.RemoveAt(num);
			}
		}
	}

	private void OnFlyby(CivAirstrikeFlybyEvent ev)
	{
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		EnsureOverlay();
		if (_overlay != null)
		{
			TimeSpan startTime = _timing.RealTime + TimeSpan.FromSeconds(ev.StartDelay);
			_overlay.Instances.Add(new CivAirstrikeFlybyInstance
			{
				Origin = ev.Origin,
				Entry = ev.Entry,
				EntryCtr = ev.EntryCtr,
				Approach = ev.Approach,
				Target = ev.Target,
				RunEnd = ev.RunEnd,
				ExitTurn = ev.ExitTurn,
				ExitCtr = ev.ExitCtr,
				Exit = ev.Exit,
				EntryLineLen = ev.EntryLineLen,
				EntryArcLen = ev.EntryArcLen,
				ExitLen = ev.ExitLen,
				EntryCcw = ev.EntryCcw,
				ExitCcw = ev.ExitCcw,
				Speed = ev.Speed,
				Count = ev.Count,
				Spacing = ev.Spacing,
				Alpha = ev.Alpha,
				ScaleMin = ev.ScaleMin,
				ScaleMax = ev.ScaleMax,
				Side = ev.Side,
				MapId = ev.MapId,
				StartTime = startTime
			});
		}
	}

	private void EnsureOverlay()
	{
		if (_overlay == null)
		{
			_overlay = new CivAirstrikeFlybyOverlay();
			if (!_overlayManager.HasOverlay<CivAirstrikeFlybyOverlay>())
			{
				_overlayManager.AddOverlay((Overlay)(object)_overlay);
			}
		}
	}
}
