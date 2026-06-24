// Decompiled with JetBrains decompiler
// Type: Content.Client.MouseRotator.MouseRotatorSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.MouseRotator;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Client.MouseRotator;

public sealed class MouseRotatorSystem : SharedMouseRotatorSystem
{
  [Dependency]
  private IInputManager _input;
  [Dependency]
  private IPlayerManager _player;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private IEyeManager _eye;
  [Dependency]
  private SharedTransformSystem _transform;

  public override void Update(float frameTime)
  {
    base.Update(frameTime);
    if (!this._timing.IsFirstTimePredicted)
      return;
    ScreenCoordinates mouseScreenPosition = this._input.MouseScreenPosition;
    if (!((ScreenCoordinates) ref mouseScreenPosition).IsValid)
      return;
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    MouseRotatorComponent rotatorComponent;
    if (!localEntity.HasValue || !this.TryComp<MouseRotatorComponent>(localEntity, ref rotatorComponent))
      return;
    TransformComponent transformComponent = this.Transform(localEntity.Value);
    MapCoordinates map = this._eye.PixelToMap(this._input.MouseScreenPosition);
    if (MapId.op_Equality(map.MapId, MapId.Nullspace))
      return;
    Angle worldAngle = DirectionExtensions.ToWorldAngle(map.Position - this._transform.GetMapCoordinates(localEntity.Value, transformComponent).Position);
    Angle worldRotation = this._transform.GetWorldRotation(transformComponent);
    if (rotatorComponent.Simple4DirMode)
    {
      Angle rotation = this._eye.CurrentEye.Rotation;
      Angle angle1 = Angle.op_Addition(worldAngle, rotation);
      Direction cardinalDir1 = ((Angle) ref angle1).GetCardinalDir();
      Direction direction = cardinalDir1;
      Angle angle2 = Angle.op_Addition(worldRotation, rotation);
      Direction cardinalDir2 = ((Angle) ref angle2).GetCardinalDir();
      if (direction == cardinalDir2)
        return;
      Angle angle3 = Angle.op_Subtraction(DirectionExtensions.ToAngle(cardinalDir1), rotation);
      if (Angle.op_Implicit(angle3) >= Math.PI)
        angle3 = Angle.op_Subtraction(angle3, Angle.op_Implicit(2.0 * Math.PI));
      else if (Angle.op_Implicit(angle3) < -1.0 * Math.PI)
        angle3 = Angle.op_Addition(angle3, Angle.op_Implicit(2.0 * Math.PI));
      this.RaisePredictiveEvent<RequestMouseRotatorRotationEvent>(new RequestMouseRotatorRotationEvent()
      {
        Rotation = angle3
      });
    }
    else
    {
      if (Math.Abs(Angle.ShortestDistance(ref worldAngle, ref worldRotation).Theta) < rotatorComponent.AngleTolerance.Theta)
        return;
      if (rotatorComponent.GoalRotation.HasValue)
      {
        ref Angle local1 = ref worldAngle;
        Angle angle = rotatorComponent.GoalRotation.Value;
        ref Angle local2 = ref angle;
        if (Math.Abs(Angle.ShortestDistance(ref local1, ref local2).Theta) < rotatorComponent.AngleTolerance.Theta)
          return;
      }
      this.RaisePredictiveEvent<RequestMouseRotatorRotationEvent>(new RequestMouseRotatorRotationEvent()
      {
        Rotation = worldAngle
      });
    }
  }
}
