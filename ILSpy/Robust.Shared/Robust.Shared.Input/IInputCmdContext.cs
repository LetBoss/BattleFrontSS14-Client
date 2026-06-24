using System.Collections;
using System.Collections.Generic;
using Robust.Shared.Analyzers;

namespace Robust.Shared.Input;

[NotContentImplementable]
public interface IInputCmdContext : IEnumerable<BoundKeyFunction>, IEnumerable
{
	string Name { get; }

	void AddFunction(BoundKeyFunction function);

	bool FunctionExists(BoundKeyFunction function);

	bool FunctionExistsHierarchy(BoundKeyFunction function);

	void RemoveFunction(BoundKeyFunction function);
}
