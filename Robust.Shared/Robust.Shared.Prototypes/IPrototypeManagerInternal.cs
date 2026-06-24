using System;
using Robust.Shared.Serialization.Markdown;

namespace Robust.Shared.Prototypes;

internal interface IPrototypeManagerInternal : IPrototypeManager
{
	event Action<DataNodeDocument>? LoadedData;
}
