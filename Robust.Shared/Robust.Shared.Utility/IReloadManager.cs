using System;

namespace Robust.Shared.Utility;

internal interface IReloadManager
{
	event Action<ResPath>? OnChanged;

	internal void Register(string directory, string filter);

	internal void Register(ResPath directory, string filter);

	void Initialize();
}
