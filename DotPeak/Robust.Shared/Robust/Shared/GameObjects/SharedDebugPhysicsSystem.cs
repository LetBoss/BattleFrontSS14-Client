// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.SharedDebugPhysicsSystem
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Physics.Collision;
using Robust.Shared.Physics.Dynamics.Contacts;

#nullable enable
namespace Robust.Shared.GameObjects;

public abstract class SharedDebugPhysicsSystem : EntitySystem
{
  public virtual void HandlePreSolve(Contact contact, in Manifold oldManifold)
  {
  }
}
