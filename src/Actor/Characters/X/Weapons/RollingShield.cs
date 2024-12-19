﻿using System.Collections.Generic;

namespace MMXOnline;

public class RollingShield : Weapon {
	public static RollingShield netWeapon = new();

	public RollingShield() : base() {
		index = (int)WeaponIds.RollingShield;
		killFeedIndex = 3;
		weaponBarBaseIndex = 3;
		weaponBarIndex = weaponBarBaseIndex;
		weaponSlotIndex = 3;
		weaknessIndex = 6;
		shootSounds = new string[] { "rollingShield", "rollingShield", "rollingShield", "" };
		fireRate = 45;
		damage = "2/1";
		effect = "Mobile Shield That Deletes Projectiles.";
		hitcooldown = "0/0.33";	
	}

	public override void shoot(Character character, int[] args) {
		int chargeLevel = args[0];
		Point pos = character.getShootPos();
		int xDir = character.getShootXDir();
		Player player = character.player;

		if (chargeLevel < 3) {
			new RollingShieldProj(this, pos, xDir, player, player.getNextActorNetId(), true);	
		} else {
			new RollingShieldProjCharged(this, pos, xDir, player, player.getNextActorNetId(), true);
		}
	}
}


public class RollingShieldProj : Projectile {
	public RollingShieldProj(
		Weapon weapon, Point pos, int xDir, 
		Player player, ushort netProjId, bool rpc = false
	) : base(
		weapon, pos, xDir, 200, 2, player, "rolling_shield", 
		0, 0, netProjId, player.ownedByLocalPlayer
	) {
		projId = (int)ProjIds.RollingShield;
		fadeSprite = "explosion";
		fadeSound = "explosion";
		useGravity = true;
		collider.wallOnly = true;
		vel.x = 0;
		canBeLocal = false;

		if (rpc) rpcCreate(pos, player, netProjId, xDir);
	}

	public static Projectile rpcInvoke(ProjParameters arg) {
		return new RollingShieldProj(
			RollingShield.netWeapon, arg.pos, arg.xDir, 
			arg.player, arg.netId
		);
	}

	public override void update() {
		if (!ownedByLocalPlayer) {
			base.update();
			return;
		}

		move(new Point(xDir * 200, 0));
		if (Global.level.checkTerrainCollisionOnce(this, 0, -1) == null) {
			var collideData = Global.level.checkTerrainCollisionOnce(this, xDir, 0, vel);
			if (collideData?.hitData?.normal != null && !(collideData.hitData.normal.Value.isAngled())) {
				xDir *= -1;
			}
		} else {
			//this.vel.x = 0;
		}

		base.update();

		if (time > 1.5) {
			destroySelf(fadeSprite, fadeSound);
		}
	}

	public override void onHitDamagable(IDamagable damagable) {
		if (damagable is not TorpedoProj) {
			base.onHitDamagable(damagable);
		}
	}
}

public class RollingShieldProjCharged : Projectile {
	public MegamanX? mmx;
	public LoopingSound? rollingShieldSound;
	public float ammoDecCooldown = 0;
	public RollingShieldProjCharged(
		Weapon weapon, Point pos, int xDir, 
		Player player, ushort netProjId, bool rpc = false
	) : base(
		weapon, pos, xDir, 0, 1, player, "rolling_shield_charge_flash",
		0, 0.33f, netProjId, player.ownedByLocalPlayer
	) {
		projId = (int)ProjIds.RollingShieldCharged;
		fadeSprite = "rolling_shield_charge_break";
		fadeSound = "hit";
		useGravity = false;
		mmx = (player.character as MegamanX);
		rollingShieldSound = new LoopingSound("rollingShieldCharge", "rollingShieldChargeLoop", this);
		if (mmx is not null) {
			mmx.chargedRollingShieldProj = this;
		}
		destroyOnHit = false;
		shouldShieldBlock = false;
		shouldVortexSuck = false;
		neverReflect = true;
		canBeLocal = false;

		if (rpc) rpcCreate(pos, player, netProjId, xDir);
	}

	public static Projectile rpcInvoke(ProjParameters arg) {
		return new RollingShieldProjCharged(
			RollingShield.netWeapon, arg.pos, arg.xDir, 
			arg.player, arg.netId
		);
	}

	public override void update() {
		base.update();
		if (!ownedByLocalPlayer) {
			if (rollingShieldSound != null) {
				rollingShieldSound.play();
			}
			return;
		}
		// In case it gets reflected (somehow) it implodes.
		// This to prevent it from killing X when reflected.
		if (mmx?.player != owner) {
			destroySelf();
			return;
		}
		if (isAnimOver() && sprite.name == "rolling_shield_charge_flash") {
			changeSprite("rolling_shield_charge", true);
		}
		if (mmx?.currentWeapon is RollingShield { ammo: >0 }) {
			destroySelf();
		}
		if (rollingShieldSound != null) {
			rollingShieldSound.play();
		}
		changePos(mmx.getCenterPos());
		if (ammoDecCooldown > 0) {
			ammoDecCooldown += Global.spf;
			if (ammoDecCooldown > 0.2) ammoDecCooldown = 0;
		}
	}

	public override void onHitDamagable(IDamagable damagable) {
		if (mmx is not null) {
			base.onHitDamagable(mmx);
		}
		decAmmo(1);
	}

	public void decAmmo(float amount = 1) {
		if (ammoDecCooldown == 0) {
			ammoDecCooldown = Global.spf;
			damager.owner.weapon.addAmmo(-amount, damager.owner);
		}
	}

	public override void onDestroy() {
		if (damager.owner.character != null) {
			if (mmx is not null) {
				mmx.chargedRollingShieldProj = null;
			}
		}
		if (rollingShieldSound != null) {
			rollingShieldSound.destroy();
			rollingShieldSound = null;
		}
	}
}
