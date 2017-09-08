using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OculusBuildUploadUtility {
	public class AppVersion {
		public int major, minor, patch;
		public AppVersion() {

		}
		public AppVersion(int major, int minor, int patch) {
			this.major = major;
			this.minor = minor;
			this.patch = patch;
		}

		public AppVersion(string versionstring) {
			var v = versionstring.Split(new char[] { '.' });
			major = int.Parse(v[0]);
			minor = int.Parse(v[1]);
			patch = int.Parse(v[2]);
		}

		public void IncrementMajor() {
			major++;
			minor = 0;
			patch = 0;
		}

		public void IncrementMinor() {
			minor++;
			patch = 0;
		}

		public void IncrementPatch() {
			patch++;
		}

		public override string ToString() {
			return "" + major + "." + minor + "." + patch;
		}
	}
}
