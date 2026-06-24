using Robust.Shared.Audio;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._CIV14merka.HeliSupply;

[Prototype(null, 1)]
public sealed class CivHeliConfigPrototype : IPrototype
{
	public const string DefaultId = "CivHeliDefault";

	[DataField(null, false, 1, false, false, null)]
	public float Speed = 12f;

	[DataField(null, false, 1, false, false, null)]
	public float Smoothing = 8f;

	[DataField(null, false, 1, false, false, null)]
	public int SmoothPasses = 3;

	[DataField(null, false, 1, false, false, null)]
	public float TurnSlowdown = 4f;

	[DataField(null, false, 1, false, false, null)]
	public float DropSlowZone = 14f;

	[DataField(null, false, 1, false, false, null)]
	public float DropSlowFactor = 0.35f;

	[DataField(null, false, 1, false, false, null)]
	public float Alpha = 0.8f;

	[DataField(null, false, 1, false, false, null)]
	public float Scale = 0.85f;

	[DataField(null, false, 1, false, false, null)]
	public float TakeoffScale = 1.15f;

	[DataField(null, false, 1, false, false, null)]
	public float DropScale = 1.15f;

	[DataField(null, false, 1, false, false, null)]
	public float TakeoffZone = 10f;

	[DataField(null, false, 1, false, false, null)]
	public float DropZone = 30f;

	[DataField(null, false, 1, false, false, null)]
	public float ClimbZone = 12f;

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier? FlySound;

	[DataField(null, false, 1, false, false, null)]
	public float DustRate = 10f;

	[DataField(null, false, 1, false, false, null)]
	public float SpriteAngleDeg;

	[DataField(null, false, 1, false, false, null)]
	public int MaxCargo = 30;

	[DataField(null, false, 1, false, false, null)]
	public int MaxWaypoints = 12;

	[DataField(null, false, 1, false, false, null)]
	public int CooldownSeconds = 20;

	[DataField(null, false, 1, false, false, null)]
	public int LaunchCost = 100;

	[DataField(null, false, 1, false, false, null)]
	public int MaxConcurrentFlights = 1;

	[DataField(null, false, 1, false, false, null)]
	public float PvoAbortMinDistance = 8f;

	[DataField(null, false, 1, false, false, null)]
	public EntProtoId Crate = EntProtoId.op_Implicit("CivHeliSupplyCrate");

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier? DropSound;

	[IdDataField(1, null)]
	public string ID { get; private set; }
}
