using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Robust.Shared.GameObjects;

internal interface IEntityLoadContext
{
	bool TryGetComponent(string componentName, [NotNullWhen(true)] out IComponent? component);

	bool TryGetComponent<TComponent>(IComponentFactory componentFactory, [NotNullWhen(true)] out TComponent? component) where TComponent : class, IComponent, new();

	IEnumerable<string> GetExtraComponentTypes();

	bool ShouldSkipComponent(string compName);
}
