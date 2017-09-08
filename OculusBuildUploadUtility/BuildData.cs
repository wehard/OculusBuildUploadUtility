using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OculusBuildUploadUtility {
	public class BuildData {
		public string AppID;
		public string SecretKey;
		public string DirectoryName;
		public string ExecutableName;
		public AppVersion AppVersion = new AppVersion();
		public string BuildChannel;
		[Newtonsoft.Json.JsonConstructor]
		public BuildData() {
		}
		public BuildData(string appID, string secretKey, string dir, string exe, AppVersion ver, string channel) {
			this.AppID = appID;
			this.SecretKey = secretKey;
			this.DirectoryName = dir;
			this.ExecutableName = exe;
			this.AppVersion = ver;
			this.BuildChannel = channel;
		}
		public override string ToString() {
			return " -a " + AppID + " -s " + SecretKey + " -d " + "\"" + DirectoryName + "\"" + " -l " + "\"" + ExecutableName + "\"" + " -v " + AppVersion.ToString() + " -c " + BuildChannel;
		}
	}
}
