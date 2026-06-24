// Decompiled with JetBrains decompiler
// Type: Content.Shared.Administration.SharedBwoinkSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Network;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.Administration;

public abstract class SharedBwoinkSystem : EntitySystem
{
  public static NetUserId SystemUserId { get; } = new NetUserId(Guid.Empty);

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<SharedBwoinkSystem.BwoinkTextMessage>(new EntitySessionEventHandler<SharedBwoinkSystem.BwoinkTextMessage>(this.OnBwoinkTextMessage), (Type[]) null, (Type[]) null);
  }

  protected virtual void OnBwoinkTextMessage(
    SharedBwoinkSystem.BwoinkTextMessage message,
    EntitySessionEventArgs eventArgs)
  {
  }

  protected void LogBwoink(SharedBwoinkSystem.BwoinkTextMessage message)
  {
  }

  [NetSerializable]
  [Serializable]
  public sealed class BwoinkTextMessage : EntityEventArgs
  {
    public readonly bool AdminOnly;

    public DateTime SentAt { get; }

    public NetUserId UserId { get; }

    public NetUserId TrueSender { get; }

    public string Text { get; }

    public bool PlaySound { get; }

    public BwoinkTextMessage(
      NetUserId userId,
      NetUserId trueSender,
      string text,
      DateTime? sentAt = null,
      bool playSound = true,
      bool adminOnly = false)
    {
      this.SentAt = sentAt ?? DateTime.Now;
      this.UserId = userId;
      this.TrueSender = trueSender;
      this.Text = text;
      this.PlaySound = playSound;
      this.AdminOnly = adminOnly;
    }
  }
}
