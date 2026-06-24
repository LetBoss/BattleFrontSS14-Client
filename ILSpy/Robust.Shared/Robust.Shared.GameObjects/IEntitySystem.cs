using System;
using System.Collections.Generic;
using Robust.Shared.Analyzers;

namespace Robust.Shared.GameObjects;

[NotContentImplementable]
public interface IEntitySystem : IEntityEventSubscriber
{
	IEnumerable<Type> UpdatesAfter { get; }

	IEnumerable<Type> UpdatesBefore { get; }

	bool UpdatesOutsidePrediction { get; }

	void Initialize();

	void Shutdown();

	void Update(float frameTime);

	void FrameUpdate(float frameTime);
}
