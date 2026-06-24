using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Robust.Shared.Exceptions;
using Robust.Shared.IoC;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Timing;
using Robust.Shared.ViewVariables;

namespace Robust.Shared.GameObjects;

[RegisterComponent]
[Obsolete("Use a system update loop instead")]
public sealed class TimerComponent : Component, ISerializationGenerated<TimerComponent>, ISerializationGenerated
{
	[Dependency]
	private readonly IRuntimeLog _runtimeLog;

	private readonly List<(Robust.Shared.Timing.Timer timer, CancellationToken source)> _timers = new List<(Robust.Shared.Timing.Timer, CancellationToken)>();

	public int TimerCount => _timers.Count;

	[ViewVariables(VVAccess.ReadWrite)]
	public bool RemoveOnEmpty { get; set; } = true;

	public void Update(float frameTime)
	{
		for (int i = 0; i < _timers.Count; i++)
		{
			var (timer, cancellationToken) = _timers[i];
			if (!cancellationToken.IsCancellationRequested)
			{
				timer.Update(frameTime, _runtimeLog);
			}
		}
		_timers.RemoveAll(((Robust.Shared.Timing.Timer timer, CancellationToken source) tuple2) => !tuple2.timer.IsActive || tuple2.source.IsCancellationRequested);
	}

	public void AddTimer(Robust.Shared.Timing.Timer timer, CancellationToken cancellationToken = default(CancellationToken))
	{
		_timers.Add((timer, cancellationToken));
	}

	public Task Delay(int milliseconds, CancellationToken cancellationToken = default(CancellationToken))
	{
		TaskCompletionSource<object?> tcs = new TaskCompletionSource<object>();
		Spawn(milliseconds, delegate
		{
			tcs.SetResult(null);
		}, cancellationToken);
		return tcs.Task;
	}

	public Task Delay(TimeSpan duration, CancellationToken cancellationToken = default(CancellationToken))
	{
		return Delay((int)duration.TotalMilliseconds, cancellationToken);
	}

	public void Spawn(int milliseconds, Action onFired, CancellationToken cancellationToken = default(CancellationToken))
	{
		Robust.Shared.Timing.Timer timer = new Robust.Shared.Timing.Timer(milliseconds, isRepeating: false, onFired);
		AddTimer(timer, cancellationToken);
	}

	public void Spawn(TimeSpan duration, Action onFired, CancellationToken cancellationToken = default(CancellationToken))
	{
		Spawn((int)duration.TotalMilliseconds, onFired, cancellationToken);
	}

	public void SpawnRepeating(int milliseconds, Action onFired, CancellationToken cancellationToken)
	{
		Robust.Shared.Timing.Timer timer = new Robust.Shared.Timing.Timer(milliseconds, isRepeating: true, onFired);
		AddTimer(timer, cancellationToken);
	}

	public void SpawnRepeating(TimeSpan duration, Action onFired, CancellationToken cancellationToken)
	{
		SpawnRepeating((int)duration.TotalMilliseconds, onFired, cancellationToken);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref TimerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component target2 = target;
		base.InternalCopy(ref target2, serialization, hookCtx, context);
		target = (TimerComponent)target2;
		serialization.TryCustomCopy(this, ref target, hookCtx, hasHooks: false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref TimerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		TimerComponent target2 = (TimerComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		TimerComponent target2 = (TimerComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		TimerComponent target2 = (TimerComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override TimerComponent Instantiate()
	{
		return new TimerComponent();
	}
}
