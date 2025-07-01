namespace PFDB.ParsingUtility;


/// <summary>
/// The search targets for statistics
/// </summary>
public enum SearchTargets
{
	/// <summary>
	/// Rank. Common to all weapons.
	/// </summary>
	Rank,

	/////////////////////////guns

	/// <summary>
	/// Damage. Base damage whenever a bullet hits an opponent. Only for guns.
	/// </summary>
	Damage,

	/// <summary>
	/// Damage range (in studs). Defines where damages occur. Only for guns.
	/// </summary>
	DamageRange,

	/// <summary>
	/// Firerate (in rounds per minute). Only for guns.
	/// </summary>
	Firerate,

	/// <summary>
	/// Ammo capacity. Written as {currentMagazine} / {reserve}, where "currentMagazine" indicates how many bullets
	/// are in the current magazine/clip, and "reserve" indicates how many spare bullets you have remaining after 
	/// your current magazine/clip. Only for guns.
	/// </summary>
	AmmoCapacity,

	/// <summary>
	/// Head multiplier for damage. Only for guns and melees.
	/// </summary>
	HeadMultiplier,

	/// <summary>
	/// Torso multiplier for damage. Only for guns and melees.
	/// </summary>
	TorsoMultiplier,

	/// <summary>
	/// Limb multiplier for damage. Only for guns and melees.
	/// </summary>
	LimbMultiplier,

	/// <summary>
	/// Muzzle velocity (in studs per second). Defines the speed/velocity (I am aware of the difference) of a bullet. Only for guns.
	/// </summary>
	MuzzleVelocity,

	/// <summary>
	/// Suppression factor. This is a statistic that defines how much opponents' screens shake when you fire. Only for guns.
	/// </summary>
	Suppression,

	/// <summary>
	/// Penetration depth (in studs). Defines how deep a bullet with penetrate into walls. Only for guns.
	/// </summary>
	PenetrationDepth,

	/// <summary>
	/// Reload time (in seconds). This indicates how long the animation reload takes when there are more than 0 bullets in your current magazine. Only for guns.
	/// </summary>
	ReloadTime,

	/// <summary>
	/// Reload time (in seconds). This indicates how long the animation reload takes when there are 0 bullets in your current magazine. Only for guns.
	/// </summary>
	EmptyReloadTime,

	/// <summary>
	/// Weapon walkspeed. Defines how fast you walk with the gun in your hands. Despite its name, it is only for guns.
	/// </summary>
	WeaponWalkspeed,

	/// <summary>
	/// Aiming walkspeed. Defines how fast you walk while aiming down the sights of the gun. Only for guns.
	/// </summary>
	AimingWalkspeed,

	/// <summary>
	/// Ammo type. Usually defines the caliber of the weapon, while some notable exceptions do not (e.g. E-Gun and Coilgun with Angry Pancakes). Only for guns.
	/// </summary>
	AmmoType,

	/// <summary>
	/// Sight magnification. Defines zoom of the weapon when aiming.
	/// </summary>
	SightMagnification,

	/// <summary>
	/// Minimum time to kill. Defines the time interval (in seconds) it takes to deal lethal (100) damage to an opponent from 0 studs away. Only for guns.
	/// </summary>
	MinimumTimeToKill,

	/// <summary>
	/// Hipfire spread factor. Measure of the maximum spread increase (or variability) when firing from the hip. Only for guns.
	/// </summary>
	HipfireSpreadFactor,

	/// <summary>
	/// Hipfire recovery speed. Measure of the restoring speed when finished firing. Only for guns.
	/// </summary>
	HipfireRecoverySpeed,

	/// <summary>
	/// Hipfire spread damping. Measure of how much the variability is suppressed by. Only for guns.
	/// </summary>
	HipfireSpreadDamping,

	/// <summary>
	/// Hip choke. Measure of the spread of pellets when firing from the hip. Set to 0.00 for guns without pellets. Only for shotguns.
	/// </summary>
	HipChoke,

	/// <summary>
	/// Aim choke. Measure of the spread of pellets when firing from the shoulder (aiming down the sights). Set to 0.00 for guns without pellets. Only for shotguns.
	/// </summary>
	AimChoke,

	/// <summary>
	/// Equip speed. How fast it takes to equip a weapon. The lower the number, the slower it is. Only for guns.
	/// </summary>
	EquipSpeed,

	/// <summary>
	/// Aim model speed. How fast it takes to aim down the model. The lower the number, the slower it is. Only for guns.
	/// </summary>
	AimModelSpeed,

	/// <summary>
	/// Aim magnification speed. How fast it takes for the magnification to fully zoom in/out. Only for guns.
	/// </summary>
	AimMagnificationSpeed,

	/// <summary>
	/// Crosshair size. The size of the crosshair when held standing and stationary. Only for guns.
	/// </summary>
	CrosshairSize,

	/// <summary>
	/// Crosshair spread rate. Only for guns.
	/// </summary>
	CrosshairSpreadRate,

	/// <summary>
	/// Crosshair recover rate. How fast it takes for the crosshair to return to its original size. Only for guns.
	/// </summary>
	CrosshairRecoverRate,

	/// <summary>
	/// Fire modes. Defines the modes the gun is capable of, along with different firerates for the different modes, if applicable. Only for guns.
	/// </summary>
	FireModes,



	////////////////////////grenades

	/// <summary>
	/// Blast radius. Defines the maximum amount of studs the damaging particle effects will travel. Only for grenades.
	/// </summary>
	BlastRadius,

	/// <summary>
	/// Killing radius. Defines the maxmimum amount of studs the damaging particle effects will deal a lethal (100 damage) blow. May have weird results with grenades that deal less than 100 damage. Only for grenades.
	/// </summary>
	KillingRadius,

	/// <summary>
	/// Maximum damage. Defines the absolute maximum damage the grenade will deal at 0 studs. Only for grenades.
	/// </summary>
	MaximumDamage,

	/// <summary>
	/// Trigger mechanism. Defines how the grenade explodes. Only for grenades.
	/// </summary>
	TriggerMechanism,

	/// <summary>
	/// Special effects. Any special effects the grenade has. Only for grenades. 
	/// </summary>
	SpecialEffects,

	/// <summary>
	/// Throw velocity. How fast a grenade is thrown from the hand. Only for grenades.
	/// </summary>
	ThrowVelocity,

	/// <summary>
	/// Throw angle. From which angle offset the grenade is thrown from the hand. Only for grenades.
	/// </summary>
	ThrowAngle,

	/// <summary>
	/// Stored capacity. The amount of the specific grenade you can carry. Only for grenades.
	/// </summary>
	StoredCapacity,

	//////////////////////////////////melees

	/// <summary>
	/// Front stab damage. The base damage the melee will deal when striking the opponent from the front. Only for melees.
	/// </summary>
	FrontStabDamage,

	/// <summary>
	/// Back stab damage. The base damage the melee will deal when striking the opponent from the back. Only for melees.
	/// </summary>
	BackStabDamage,

	/// <summary>
	/// Main attack time. The duration of the animation time where you deal damage. This attack happens when you have the melee out in your hand and left-click. Only for melees.
	/// </summary>
	MainAttackTime,

	/// <summary>
	/// Main attack delay. The duration of the animation time where you do not deal damage. This attack happens when you have the melee out in your hand and left-click. Only for melees.
	/// </summary>
	MainAttackDelay,

	/// <summary>
	/// Alt attack time. The duration of the animation time where you deal damage. This attack happens when you have the melee out in your hand and right-click. Only for melees.
	/// </summary>
	AltAttackTime,

	/// <summary>
	/// Alt attack delay. The duration of the animation time where you do not deal damage. This attack happens when you have the melee out in your hand and right-click. Only for melees.
	/// </summary>
	AltAttackDelay,

	/// <summary>
	/// Quick attack time. The duration of the animation time where you deal damage. This attack happens when you press "f". Only for melees.
	/// </summary>
	QuickAttackTime,

	/// <summary>
	/// Quick attack delay. The duration of the animation time where you do not deal damage. This attack happens when you press "f". Only for melees.
	/// </summary>
	QuickAttackDelay,

	/// <summary>
	/// Walkspeed. Defines how fast you walk with the melee weapon in your hands. Only for melees.
	/// </summary>
	Walkspeed
}