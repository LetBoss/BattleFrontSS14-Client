using System;
using Robust.Shared.Analyzers;

namespace Robust.Shared.Sandboxing;

[NotContentImplementable]
public interface ISandboxHelper
{
	object CreateInstance(Type type);
}
