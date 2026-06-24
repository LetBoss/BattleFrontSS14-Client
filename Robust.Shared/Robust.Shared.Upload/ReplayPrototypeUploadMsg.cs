using System;
using Robust.Shared.Serialization;

namespace Robust.Shared.Upload;

[Serializable]
[NetSerializable]
public sealed class ReplayPrototypeUploadMsg
{
	public string PrototypeData;
}
