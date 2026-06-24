using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.CrewManifest;

[Serializable]
[NetSerializable]
public sealed class RequestCrewManifestMessage : EntityEventArgs
{
	public NetEntity Id { get; }

	public RequestCrewManifestMessage(NetEntity id)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Id = id;
	}
}
