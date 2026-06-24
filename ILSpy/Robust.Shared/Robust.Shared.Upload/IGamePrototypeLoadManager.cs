using Robust.Shared.Analyzers;

namespace Robust.Shared.Upload;

[NotContentImplementable]
public interface IGamePrototypeLoadManager
{
	void Initialize();

	void SendGamePrototype(string prototype);
}
