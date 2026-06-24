using System;
using System.Collections.Generic;
using System.Linq;
using Robust.Shared.GameObjects;
using Robust.Shared.Utility;

namespace Content.Shared.Examine;

public sealed class ExaminedEvent : EntityEventArgs
{
	public struct ExamineGroupDisposable(ExaminedEvent @event) : IDisposable
	{
		private ExaminedEvent _event = @event;

		public void Dispose()
		{
			_event.PopGroup();
		}
	}

	private record ExamineMessagePart(FormattedMessage Message, int Priority, bool DoNewLine, string? Group);

	private bool _hasDescription;

	private ExamineMessagePart? _currentGroupPart;

	private FormattedMessage Message { get; }

	private List<ExamineMessagePart> Parts { get; } = new List<ExamineMessagePart>();

	public bool IsInDetailsRange { get; }

	public EntityUid Examiner { get; }

	public EntityUid Examined { get; }

	public ExaminedEvent(FormattedMessage message, EntityUid examined, EntityUid examiner, bool isInDetailsRange, bool hasDescription)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		Message = message;
		Examined = examined;
		Examiner = examiner;
		IsInDetailsRange = isInDetailsRange;
		_hasDescription = hasDescription;
	}

	public FormattedMessage GetTotalMessage()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		List<ExamineMessagePart> parts = Parts.ToList();
		FormattedMessage totalMessage = new FormattedMessage(Message);
		parts.Sort(Comparison);
		if (_hasDescription && parts.Count > 0)
		{
			totalMessage.PushNewline();
		}
		foreach (ExamineMessagePart part in parts)
		{
			totalMessage.AddMessage(part.Message);
			if (part.DoNewLine && parts.Last() != part)
			{
				totalMessage.PushNewline();
			}
		}
		totalMessage.TrimEnd();
		return totalMessage;
		static int Comparison(ExamineMessagePart a, ExamineMessagePart b)
		{
			if (a.Priority != b.Priority)
			{
				return -a.Priority.CompareTo(b.Priority);
			}
			if (a.Group != b.Group)
			{
				return string.Compare(a.Group, b.Group, StringComparison.Ordinal);
			}
			return string.Compare(((object)a.Message).ToString(), ((object)b.Message).ToString(), StringComparison.Ordinal);
		}
	}

	public ExamineGroupDisposable PushGroup(string groupName, int priority = 0)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Expected O, but got Unknown
		_currentGroupPart = new ExamineMessagePart(new FormattedMessage(), priority, DoNewLine: false, groupName);
		return new ExamineGroupDisposable(this);
	}

	private void PopGroup()
	{
		if (_currentGroupPart != null && !_currentGroupPart.Message.IsEmpty)
		{
			Parts.Add(_currentGroupPart);
		}
		_currentGroupPart = null;
	}

	public void PushMessage(FormattedMessage message, int priority = 0)
	{
		if (message.Nodes.Count != 0)
		{
			if (_currentGroupPart != null)
			{
				message.PushNewline();
				_currentGroupPart.Message.AddMessage(message);
			}
			else
			{
				Parts.Add(new ExamineMessagePart(message, priority, DoNewLine: true, null));
			}
		}
	}

	public void PushMarkup(string markup, int priority = 0)
	{
		PushMessage(FormattedMessage.FromMarkupOrThrow(markup), priority);
	}

	public void PushText(string text, int priority = 0)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		FormattedMessage msg = new FormattedMessage();
		msg.AddText(text);
		PushMessage(msg, priority);
	}

	public void AddMessage(FormattedMessage message, int priority = 0)
	{
		if (message.Nodes.Count != 0)
		{
			if (_currentGroupPart != null)
			{
				_currentGroupPart.Message.AddMessage(message);
			}
			else
			{
				Parts.Add(new ExamineMessagePart(message, priority, DoNewLine: false, null));
			}
		}
	}

	public void AddMarkup(string markup, int priority = 0)
	{
		AddMessage(FormattedMessage.FromMarkupOrThrow(markup), priority);
	}

	public void AddText(string text, int priority = 0)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		FormattedMessage msg = new FormattedMessage();
		msg.AddText(text);
		AddMessage(msg, priority);
	}
}
