using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Fluids;

public abstract class SharedPuddleDebugOverlaySystem : EntitySystem
{
	protected const float LocalViewRange = 16f;

	protected TimeSpan? NextTick;

	protected TimeSpan Cooldown = TimeSpan.FromSeconds(0.5);
}
