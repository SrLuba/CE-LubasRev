using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MMXOnline;


using System.Threading;
using Discord;
using System.Text.RegularExpressions;

namespace Utilities {
	
	public static class SDKDiscord {
		public static Discord.Discord ds;
		public static Discord.Activity currentActivity;
		public static Discord.ActivityManager activityManager;
		public static Discord.UserManager userManager;
		public static void Start() {
			ds = new Discord.Discord(1317955826979307741, (UInt64)Discord.CreateFlags.Default);
			activityManager = ds.GetActivityManager();
			userManager = ds.GetUserManager();
		}
		public static void SetupPresence(string state, string details) {

			currentActivity = new Discord.Activity {
				State = state,
				Details = details,
				Assets =
					{
						  LargeImage = "embedded_background", // using asset from app's settings
						  LargeText = details,
						  SmallImage = "embedded_cover", // you can also use URLs
						  SmallText = "OMMM",
					},
				Instance = true,
			};

		}
		public static void SetupPresence(string state, string details, string partyID, string serverID) {
			if (Global.localServer == null) return;
			currentActivity = new Discord.Activity {
				State = state,
				Details = details,
				Assets =
					{
						  LargeImage = "embedded_background", // using asset from app's settings
						  LargeText = details,
						  SmallImage = "embedded_cover", // you can also use URLs
						  SmallText = "OMMM",
					},
				Party =
				  {
					  Id = partyID,
					  Size = {
						  CurrentSize = Global.localServer.players.Count,
						  MaxSize = Global.localServer.maxPlayers,
					  },
				  },
				Secrets =
				  {
					  Join = serverID,
				  },
				Instance = true,
			};

		}
		public static void OnJoin(long serverID) {
			JoinMenuP2P M = new JoinMenuP2P(true);
	
			M.LookAndConnect(serverID);
			Menu.change(M);
		}
		public static bool joining = false;
		public static void Update() {
			ds.RunCallbacks();
			
			activityManager.UpdateActivity(currentActivity, (result) => {
				
			});
			activityManager.OnActivityJoin += secret => {
				Console.WriteLine("OnJoin {0}", secret);
				if (!joining) { 
					OnJoin((long)Convert.ToDouble(secret));
					joining = true;
				}
			};

			activityManager.OnActivityJoinRequest += (ref Discord.User user) =>
			{
				Console.WriteLine("OnJoinRequest {0} {1}", user.Username, user.Id);
			};

			
		}
	}
}
