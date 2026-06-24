using System;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.Sandbox;

public abstract class SharedSandboxSystem : EntitySystem
{
	[Serializable]
	[NetSerializable]
	protected sealed class MsgSandboxStatus : EntityEventArgs
	{
		public bool SandboxAllowed { get; set; }
	}

	[Serializable]
	[NetSerializable]
	protected sealed class MsgSandboxRespawn : EntityEventArgs
	{
	}

	[Serializable]
	[NetSerializable]
	protected sealed class MsgSandboxGiveAccess : EntityEventArgs
	{
	}

	[Serializable]
	[NetSerializable]
	protected sealed class MsgSandboxGiveAghost : EntityEventArgs
	{
	}

	[Serializable]
	[NetSerializable]
	protected sealed class MsgSandboxSuicide : EntityEventArgs
	{
	}

	[Dependency]
	protected IPrototypeManager PrototypeManager;
}
