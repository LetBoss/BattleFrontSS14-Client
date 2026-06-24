using System.Threading.Channels;

namespace Robust.Shared.Serialization;

public sealed class SerializationHookContext
{
	public static readonly SerializationHookContext DoSkipHooks = new SerializationHookContext(null, skipHooks: true);

	public static readonly SerializationHookContext DontSkipHooks = new SerializationHookContext(null, skipHooks: false);

	public readonly ChannelWriter<ISerializationHooks>? DeferQueue;

	public readonly bool SkipHooks;

	public SerializationHookContext(ChannelWriter<ISerializationHooks>? deferQueue, bool skipHooks)
	{
		DeferQueue = deferQueue;
		SkipHooks = skipHooks;
	}

	public static SerializationHookContext ForSkipHooks(bool skip)
	{
		if (!skip)
		{
			return DontSkipHooks;
		}
		return DoSkipHooks;
	}
}
