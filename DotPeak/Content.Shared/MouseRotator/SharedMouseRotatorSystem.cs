// Decompiled with JetBrains decompiler
// Type: Content.Shared.MouseRotator.SharedMouseRotatorSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Interaction;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

#nullable enable
namespace Content.Shared.MouseRotator;

public abstract class SharedMouseRotatorSystem : EntitySystem
{
  [Dependency]
  private RotateToFaceSystem _rotate;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeAllEvent<RequestMouseRotatorRotationEvent>(new EntitySessionEventHandler<RequestMouseRotatorRotationEvent>(this.OnRequestRotation));
  }

  public override void Update(float frameTime)
  {
    base.Update(frameTime);
    Robust.Shared.GameObjects.EntityQueryEnumerator<MouseRotatorComponent, TransformComponent> entityQueryEnumerator = this.EntityQueryEnumerator<MouseRotatorComponent, TransformComponent>();
    EntityUid uid;
    MouseRotatorComponent comp1;
    TransformComponent comp2;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1, out comp2))
    {
      if (comp1.GoalRotation.HasValue && this._rotate.TryRotateTo(uid, comp1.GoalRotation.Value, frameTime, comp1.AngleTolerance, MathHelper.DegreesToRadians(comp1.RotationSpeed), comp2))
      {
        comp1.GoalRotation = new Angle?();
        this.Dirty(uid, (IComponent) comp1);
      }
    }
  }

  private void OnRequestRotation(RequestMouseRotatorRotationEvent msg, EntitySessionEventArgs args)
  {
    EntityUid? attachedEntity = args.SenderSession.AttachedEntity;
    if (attachedEntity.HasValue)
    {
      EntityUid valueOrDefault = attachedEntity.GetValueOrDefault();
      MouseRotatorComponent comp;
      if (this.TryComp<MouseRotatorComponent>(valueOrDefault, out comp))
      {
        comp.GoalRotation = new Angle?(msg.Rotation);
        this.Dirty(valueOrDefault, (IComponent) comp);
        return;
      }
    }
    this.Log.Error($"User {args.SenderSession.Name} ({args.SenderSession.UserId}) tried setting local rotation directly without a valid mouse rotator component attached!");
  }
}
