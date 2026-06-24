using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace Robust.Shared.Serialization;

public sealed class YamlNoDocEndDotsFix : IEmitter
{
	private readonly IEmitter _next;

	public YamlNoDocEndDotsFix(IEmitter next)
	{
		_next = next;
	}

	public void Emit(ParsingEvent @event)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		_next.Emit((ParsingEvent)((!(@event is DocumentEnd)) ? ((object)@event) : ((object)new DocumentEnd(true))));
	}
}
