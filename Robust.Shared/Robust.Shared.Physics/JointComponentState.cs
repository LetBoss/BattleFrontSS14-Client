using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Physics.Dynamics.Joints;
using Robust.Shared.Serialization;

namespace Robust.Shared.Physics;

[Serializable]
[NetSerializable]
public sealed class JointComponentState : ComponentState
{
	public NetEntity? Relay;

	public Dictionary<string, JointState> Joints;

	public JointComponentState(NetEntity? relay, Dictionary<string, JointState> joints)
	{
		Relay = relay;
		Joints = joints;
	}
}
