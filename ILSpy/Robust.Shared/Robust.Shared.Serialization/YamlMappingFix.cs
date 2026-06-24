using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace Robust.Shared.Serialization;

public sealed class YamlMappingFix : IEmitter
{
	private readonly IEmitter _next;

	public YamlMappingFix(IEmitter next)
	{
		_next = next;
	}

	public void Emit(ParsingEvent @event)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		MappingStart val = (MappingStart)(object)((@event is MappingStart) ? @event : null);
		if (val != null)
		{
			@event = (ParsingEvent)new MappingStart(((NodeEvent)val).Anchor, ((NodeEvent)val).Tag, false, val.Style, ((ParsingEvent)val).Start, ((ParsingEvent)val).End);
		}
		_next.Emit(@event);
	}
}
