// Decompiled with JetBrains decompiler
// Type: Content.Shared.Examine.ExaminedEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared.Examine;

public sealed class ExaminedEvent : EntityEventArgs
{
  private bool _hasDescription;
  private ExaminedEvent.ExamineMessagePart? _currentGroupPart;

  private FormattedMessage Message { get; }

  private List<ExaminedEvent.ExamineMessagePart> Parts { get; } = new List<ExaminedEvent.ExamineMessagePart>();

  public bool IsInDetailsRange { get; }

  public EntityUid Examiner { get; }

  public EntityUid Examined { get; }

  public ExaminedEvent(
    FormattedMessage message,
    EntityUid examined,
    EntityUid examiner,
    bool isInDetailsRange,
    bool hasDescription)
  {
    this.Message = message;
    this.Examined = examined;
    this.Examiner = examiner;
    this.IsInDetailsRange = isInDetailsRange;
    this._hasDescription = hasDescription;
  }

  public FormattedMessage GetTotalMessage()
  {
    List<ExaminedEvent.ExamineMessagePart> list = this.Parts.ToList<ExaminedEvent.ExamineMessagePart>();
    FormattedMessage totalMessage = new FormattedMessage(this.Message);
    list.Sort(new Comparison<ExaminedEvent.ExamineMessagePart>(Comparison));
    if (this._hasDescription && list.Count > 0)
      totalMessage.PushNewline();
    foreach (ExaminedEvent.ExamineMessagePart examineMessagePart in list)
    {
      totalMessage.AddMessage(examineMessagePart.Message);
      if (examineMessagePart.DoNewLine && list.Last<ExaminedEvent.ExamineMessagePart>() != examineMessagePart)
        totalMessage.PushNewline();
    }
    totalMessage.TrimEnd();
    return totalMessage;

    static int Comparison(ExaminedEvent.ExamineMessagePart a, ExaminedEvent.ExamineMessagePart b)
    {
      if (a.Priority != b.Priority)
        return -a.Priority.CompareTo(b.Priority);
      return a.Group != b.Group ? string.Compare(a.Group, b.Group, StringComparison.Ordinal) : string.Compare(a.Message.ToString(), b.Message.ToString(), StringComparison.Ordinal);
    }
  }

  public ExaminedEvent.ExamineGroupDisposable PushGroup(string groupName, int priority = 0)
  {
    this._currentGroupPart = new ExaminedEvent.ExamineMessagePart(new FormattedMessage(), priority, false, groupName);
    return new ExaminedEvent.ExamineGroupDisposable(this);
  }

  private void PopGroup()
  {
    if (this._currentGroupPart != (ExaminedEvent.ExamineMessagePart) null && !this._currentGroupPart.Message.IsEmpty)
      this.Parts.Add(this._currentGroupPart);
    this._currentGroupPart = (ExaminedEvent.ExamineMessagePart) null;
  }

  public void PushMessage(FormattedMessage message, int priority = 0)
  {
    if (message.Nodes.Count == 0)
      return;
    if (this._currentGroupPart != (ExaminedEvent.ExamineMessagePart) null)
    {
      message.PushNewline();
      this._currentGroupPart.Message.AddMessage(message);
    }
    else
      this.Parts.Add(new ExaminedEvent.ExamineMessagePart(message, priority, true, (string) null));
  }

  public void PushMarkup(string markup, int priority = 0)
  {
    this.PushMessage(FormattedMessage.FromMarkupOrThrow(markup), priority);
  }

  public void PushText(string text, int priority = 0)
  {
    FormattedMessage message = new FormattedMessage();
    message.AddText(text);
    this.PushMessage(message, priority);
  }

  public void AddMessage(FormattedMessage message, int priority = 0)
  {
    if (message.Nodes.Count == 0)
      return;
    if (this._currentGroupPart != (ExaminedEvent.ExamineMessagePart) null)
      this._currentGroupPart.Message.AddMessage(message);
    else
      this.Parts.Add(new ExaminedEvent.ExamineMessagePart(message, priority, false, (string) null));
  }

  public void AddMarkup(string markup, int priority = 0)
  {
    this.AddMessage(FormattedMessage.FromMarkupOrThrow(markup), priority);
  }

  public void AddText(string text, int priority = 0)
  {
    FormattedMessage message = new FormattedMessage();
    message.AddText(text);
    this.AddMessage(message, priority);
  }

  public struct ExamineGroupDisposable(ExaminedEvent @event) : IDisposable
  {
    private ExaminedEvent _event = @event;

    public void Dispose() => this._event.PopGroup();
  }

  private record ExamineMessagePart(
    FormattedMessage Message,
    int Priority,
    bool DoNewLine,
    string? Group)
  ;
}
