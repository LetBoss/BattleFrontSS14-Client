using Content.Shared.Damage;
using Content.Shared.Weapons.Reflect;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Weapons.Ranged;

[Prototype(null, 1)]
public sealed class HitscanPrototype : IPrototype, IShootable
{
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("staminaDamage", false, 1, false, false, null)]
	public float StaminaDamage;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("damage", false, 1, false, false, null)]
	public DamageSpecifier? Damage;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("muzzleFlash", false, 1, false, false, null)]
	public SpriteSpecifier? MuzzleFlash;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("travelFlash", false, 1, false, false, null)]
	public SpriteSpecifier? TravelFlash;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("impactFlash", false, 1, false, false, null)]
	public SpriteSpecifier? ImpactFlash;

	[DataField("collisionMask", false, 1, false, false, null)]
	public int CollisionMask = 1;

	[DataField("reflective", false, 1, false, false, null)]
	public ReflectType Reflective = ReflectType.Energy;

	[DataField("sound", false, 1, false, false, null)]
	public SoundSpecifier? Sound;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("forceSound", false, 1, false, false, null)]
	public bool ForceSound;

	[DataField("maxLength", false, 1, false, false, null)]
	public float MaxLength = 20f;

	[ViewVariables]
	[IdDataField(1, null)]
	public string ID { get; private set; }
}
