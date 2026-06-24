// Decompiled with JetBrains decompiler
// Type: Content.Client.NPC.NPCSteeringSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.NPC;
using Content.Shared.NPC.Events;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client.NPC;

public sealed class NPCSteeringSystem : SharedNPCSteeringSystem
{
  [Dependency]
  private IOverlayManager _overlay;
  private bool _debugEnabled;

  public bool DebugEnabled
  {
    get => this._debugEnabled;
    set
    {
      if (this._debugEnabled == value)
        return;
      this._debugEnabled = value;
      if (this._debugEnabled)
      {
        this._overlay.AddOverlay((Overlay) new NPCSteeringOverlay((IEntityManager) this.EntityManager));
        this.RaiseNetworkEvent((EntityEventArgs) new RequestNPCSteeringDebugEvent()
        {
          Enabled = true
        });
      }
      else
      {
        this._overlay.RemoveOverlay<NPCSteeringOverlay>();
        this.RaiseNetworkEvent((EntityEventArgs) new RequestNPCSteeringDebugEvent()
        {
          Enabled = false
        });
        AllEntityQueryEnumerator<NPCSteeringComponent> entityQueryEnumerator = this.AllEntityQuery<NPCSteeringComponent>();
        EntityUid entityUid;
        NPCSteeringComponent steeringComponent;
        while (entityQueryEnumerator.MoveNext(ref entityUid, ref steeringComponent))
          this.RemCompDeferred<NPCSteeringComponent>(entityUid);
      }
    }
  }

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<NPCSteeringDebugEvent>(new EntityEventHandler<NPCSteeringDebugEvent>(this.OnDebugEvent), (Type[]) null, (Type[]) null);
  }

  private void OnDebugEvent(NPCSteeringDebugEvent ev)
  {
    if (!this.DebugEnabled)
      return;
    foreach (NPCSteeringDebugData steeringDebugData in ev.Data)
    {
      EntityUid entity = this.GetEntity(steeringDebugData.EntityUid);
      if (this.Exists(entity))
      {
        NPCSteeringComponent steeringComponent = this.EnsureComp<NPCSteeringComponent>(entity);
        steeringComponent.Direction = steeringDebugData.Direction;
        steeringComponent.DangerMap = steeringDebugData.Danger;
        steeringComponent.InterestMap = steeringDebugData.Interest;
        steeringComponent.DangerPoints = steeringDebugData.DangerPoints;
      }
    }
  }
}
