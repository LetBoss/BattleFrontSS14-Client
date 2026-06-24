using System.Reflection;

namespace Robust.Shared.Toolshed;

internal readonly record struct ConcreteCommandMethod(MethodInfo Info, CommandArgument[] Args, CommandMethod Base);
