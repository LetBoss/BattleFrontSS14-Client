// Decompiled with JetBrains decompiler
// Type: Content.Client.NPC.HTN.HTNSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.NPC;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client.NPC.HTN;

public sealed class HTNSystem : EntitySystem
{
  private bool _enableOverlay;

  public bool EnableOverlay
  {
    get => this._enableOverlay;
    set
    {
      IOverlayManager ioverlayManager = IoCManager.Resolve<IOverlayManager>();
      this._enableOverlay = value;
      if (this._enableOverlay)
      {
        ioverlayManager.AddOverlay((Overlay) new HTNOverlay((IEntityManager) this.EntityManager, IoCManager.Resolve<IResourceCache>()));
        this.RaiseNetworkEvent((EntityEventArgs) new RequestHTNMessage()
        {
          Enabled = true
        });
      }
      else
      {
        ioverlayManager.RemoveOverlay<HTNOverlay>();
        this.RaiseNetworkEvent((EntityEventArgs) new RequestHTNMessage()
        {
          Enabled = false
        });
      }
    }
  }

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<HTNMessage>(new EntityEventHandler<HTNMessage>(this.OnHTNMessage), (Type[]) null, (Type[]) null);
  }

  private void OnHTNMessage(HTNMessage ev)
  {
    HTNComponent htnComponent;
    if (!this.TryComp<HTNComponent>(this.GetEntity(ev.Uid), ref htnComponent))
      return;
    htnComponent.DebugText = ev.Text;
  }
}
