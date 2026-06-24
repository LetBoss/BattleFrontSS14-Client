// Decompiled with JetBrains decompiler
// Type: Content.Shared.Examine.ExamineSystemMessages
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Examine;

public static class ExamineSystemMessages
{
  [NetSerializable]
  [Serializable]
  public sealed class RequestExamineInfoMessage : EntityEventArgs
  {
    public readonly NetEntity NetEntity;
    public readonly int Id;
    public readonly bool GetVerbs;

    public RequestExamineInfoMessage(NetEntity netEntity, int id, bool getVerbs = false)
    {
      this.NetEntity = netEntity;
      this.Id = id;
      this.GetVerbs = getVerbs;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class ExamineInfoResponseMessage : EntityEventArgs
  {
    public readonly NetEntity EntityUid;
    public readonly int Id;
    public readonly FormattedMessage Message;
    public List<Verb>? Verbs;
    public readonly bool CenterAtCursor;
    public readonly bool OpenAtOldTooltip;
    public readonly bool KnowTarget;

    public ExamineInfoResponseMessage(
      NetEntity entityUid,
      int id,
      FormattedMessage message,
      List<Verb>? verbs = null,
      bool centerAtCursor = true,
      bool openAtOldTooltip = true,
      bool knowTarget = true)
    {
      this.EntityUid = entityUid;
      this.Id = id;
      this.Message = message;
      this.Verbs = verbs;
      this.CenterAtCursor = centerAtCursor;
      this.OpenAtOldTooltip = openAtOldTooltip;
      this.KnowTarget = knowTarget;
    }
  }
}
