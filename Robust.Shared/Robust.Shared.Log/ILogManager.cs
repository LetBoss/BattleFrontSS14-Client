using System.Collections.Generic;
using Robust.Shared.Analyzers;

namespace Robust.Shared.Log;

[NotContentImplementable]
public interface ILogManager
{
	ISawmill RootSawmill { get; }

	IEnumerable<ISawmill> AllSawmills { get; }

	ISawmill GetSawmill(string name);
}
