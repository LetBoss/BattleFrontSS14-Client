namespace Robust.Shared.GameObjects;

[ByRefEvent]
public readonly struct EntParentChangedMessage
{
	public readonly EntityUid? OldMapId;

	public EntityUid Entity { get; }

	public EntityUid? OldParent { get; }

	public TransformComponent Transform { get; }

	public EntParentChangedMessage(EntityUid entity, EntityUid? oldParent, EntityUid? oldMapId, TransformComponent xform)
	{
		Entity = entity;
		OldParent = oldParent;
		Transform = xform;
		OldMapId = oldMapId;
	}
}
