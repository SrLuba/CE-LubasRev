using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMXOnline;

namespace MMXOnline;

public class XLoadoutSetup {
	public static List<Weapon> getLoadout(Player player) {
		List<Weapon> weapons = [];
		if (Global.level.isTraining() && (Global.level.server.useLoadout) && !Global.level.is1v1()) {
			weapons = player.loadout.xLoadout.getWeaponsFromLoadout(player);
			if (player.hasArmArmor(3) || player.xArmor1v1 == 2) weapons.Add(new HyperCharge());
			if (player.hasBodyArmor(2) || player.xArmor1v1 == 3) weapons.Add(new GigaCrush());
			return weapons;
		}

		weapons.Add(new XBuster());

		switch (player.xArmor1v1) {
			case 1:
				weapons.Add(new HomingTorpedo());
				weapons.Add(new ChameleonSting());
				weapons.Add(new RollingShield());
				weapons.Add(new FireWave());
				weapons.Add(new StormTornado());
				weapons.Add(new ElectricSpark());
				weapons.Add(new BoomerangCutter());
				weapons.Add(new ShotgunIce());
				break;
			case 2:
				weapons.Add(new CrystalHunter());
				weapons.Add(new BubbleSplash());
				weapons.Add(new SilkShot());
				weapons.Add(new SpinWheel());
				weapons.Add(new SonicSlicer());
				weapons.Add(new StrikeChain());
				weapons.Add(new MagnetMine());
				weapons.Add(new SpeedBurner());
				break;
			case 3:
				weapons.Add(new AcidBurst());
				weapons.Add(new ParasiticBomb());
				weapons.Add(new TriadThunder());
				weapons.Add(new SpinningBlade());
				weapons.Add(new RaySplasher());
				weapons.Add(new GravityWell());
				weapons.Add(new FrostShield());
				weapons.Add(new TornadoFang());
				break;
		}
		return weapons;
	}
}
