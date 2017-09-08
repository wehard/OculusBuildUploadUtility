using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OculusBuildUploadUtility {
	public class Config<T> {
		public bool FileExists { get { return _fileExists; } }

		private bool _fileExists;
		public T ConfigData {
			get {
				
				return data;
			} }
		private T data;

		public delegate void OnConfigDataChangedDelegate(T data);
		public event OnConfigDataChangedDelegate OnConfigDataChanged;

		public T Load(string filePath) {
			_fileExists = System.IO.File.Exists(filePath);
			if(!FileExists) {
				return default(T);
			} else {
				string s = System.IO.File.ReadAllText(filePath);
				data = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(s);
				if(OnConfigDataChanged != null) {
					OnConfigDataChanged(data);
				}
				return data;
			}
		}

		public void Save(T data, string filePath) {
			this.data = data;
			string s = Newtonsoft.Json.JsonConvert.SerializeObject(data);
			System.IO.File.WriteAllText(filePath, s);
			if(OnConfigDataChanged != null) {
				OnConfigDataChanged(data);
			}
		}
	}
}
