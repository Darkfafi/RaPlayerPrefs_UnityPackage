using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace RaStorages
{
	public class RaStorage
	{
		private const string COUNT_KEY = "_Count";

		public string Id
		{
			get; private set;
		}

		public RaStorage(string id)
		{
			Id = id;
		}

		private string GetPlayerPrefsKey(string key)
		{
			return Id + key;
		}

		public void Save(string key, int value)
		{
			PlayerPrefs.SetInt(GetPlayerPrefsKey(key), value);
		}

		public int Load(string key, int defaultValue)
		{
			return PlayerPrefs.GetInt(GetPlayerPrefsKey(key), defaultValue);
		}

		public void Save(string key, float value)
		{
			PlayerPrefs.SetFloat(GetPlayerPrefsKey(key), value);
		}

		public float Load(string key, float defaultValue)
		{
			return PlayerPrefs.GetFloat(GetPlayerPrefsKey(key), defaultValue);
		}

		public void Save(string key, string value)
		{
			PlayerPrefs.SetString(GetPlayerPrefsKey(key), value);
		}

		public string Load(string key, string defaultValue)
		{
			return PlayerPrefs.GetString(GetPlayerPrefsKey(key), defaultValue);
		}

		public void Save(string key, bool value)
		{
			PlayerPrefs.SetInt(GetPlayerPrefsKey(key), value ? 1 : 0);
		}

		public bool Load(string key, bool defaultValue)
		{
			return PlayerPrefs.GetInt(GetPlayerPrefsKey(key), defaultValue ? 1 : 0) == 1;
		}

		public void Save<T>(string key, T value) where T : IRaStorable
		{
			PlayerPrefs.SetString(GetPlayerPrefsKey(key), value.Serialize());
		}

		public T Load<T>(string key, Func<string, T> deserialize) where T : IRaStorable
		{
			string serializedData = PlayerPrefs.GetString(GetPlayerPrefsKey(key), string.Empty);
			if(!string.IsNullOrEmpty(serializedData))
			{
				return deserialize(serializedData);
			}
			return default;
		}

		public void Save<T>(string key, T[] values) where T : IRaStorable
		{
			ClearArrayKeys(key, values.Length);

			PlayerPrefs.SetInt(GetPlayerPrefsKey(key) + COUNT_KEY, values.Length);
			for(int i = 0; i < values.Length; i++)
			{
				PlayerPrefs.SetString(GetPlayerPrefsKey(key) + "_" + i, values[i].Serialize());
			}
		}

		public T[] LoadArray<T>(string key, Func<string, T> deserialize) where T : IRaStorable
		{
			int count = PlayerPrefs.GetInt(GetPlayerPrefsKey(key) + COUNT_KEY, 0);
			List<T> values = new List<T>();
			for(int i = 0; i < count; i++)
			{
				string serializedData = PlayerPrefs.GetString(GetPlayerPrefsKey(key) + "_" + i, string.Empty);
				if(!string.IsNullOrEmpty(serializedData))
				{
					values.Add(deserialize(serializedData));
				}
			}
			return values.ToArray();
		}

		public void Save(string key, DateTime value)
		{
			long binaryDate = value.ToBinary();
			PlayerPrefs.SetString(GetPlayerPrefsKey(key), binaryDate.ToString(CultureInfo.InvariantCulture));
		}

		public DateTime Load(string key, DateTime defaultValue)
		{
			string binaryString = PlayerPrefs.GetString(GetPlayerPrefsKey(key), string.Empty);
			if(!string.IsNullOrEmpty(binaryString))
			{
				long binaryDate;
				if(long.TryParse(binaryString, NumberStyles.Any, CultureInfo.InvariantCulture, out binaryDate))
				{
					return DateTime.FromBinary(binaryDate);
				}
			}
			return defaultValue;
		}

		public void Save(string key, Vector3 value)
		{
			PlayerPrefs.SetFloat(GetPlayerPrefsKey(key) + "_x", value.x);
			PlayerPrefs.SetFloat(GetPlayerPrefsKey(key) + "_y", value.y);
			PlayerPrefs.SetFloat(GetPlayerPrefsKey(key) + "_z", value.z);
		}

		public Vector3 Load(string key, Vector3 defaultValue)
		{
			if(PlayerPrefs.HasKey(GetPlayerPrefsKey(key) + "_x") &&
				PlayerPrefs.HasKey(GetPlayerPrefsKey(key) + "_y") &&
				PlayerPrefs.HasKey(GetPlayerPrefsKey(key) + "_z"))
			{
				float x = PlayerPrefs.GetFloat(GetPlayerPrefsKey(key) + "_x");
				float y = PlayerPrefs.GetFloat(GetPlayerPrefsKey(key) + "_y");
				float z = PlayerPrefs.GetFloat(GetPlayerPrefsKey(key) + "_z");
				return new Vector3(x, y, z);
			}
			return defaultValue;
		}

		public void Save(string key, Color value)
		{
			PlayerPrefs.SetFloat(GetPlayerPrefsKey(key) + "_r", value.r);
			PlayerPrefs.SetFloat(GetPlayerPrefsKey(key) + "_g", value.g);
			PlayerPrefs.SetFloat(GetPlayerPrefsKey(key) + "_b", value.b);
			PlayerPrefs.SetFloat(GetPlayerPrefsKey(key) + "_a", value.a);
		}

		public Color Load(string key, Color defaultValue)
		{
			if(PlayerPrefs.HasKey(GetPlayerPrefsKey(key) + "_r") &&
				PlayerPrefs.HasKey(GetPlayerPrefsKey(key) + "_g") &&
				PlayerPrefs.HasKey(GetPlayerPrefsKey(key) + "_b") &&
				PlayerPrefs.HasKey(GetPlayerPrefsKey(key) + "_a"))
			{
				float r = PlayerPrefs.GetFloat(GetPlayerPrefsKey(key) + "_r");
				float g = PlayerPrefs.GetFloat(GetPlayerPrefsKey(key) + "_g");
				float b = PlayerPrefs.GetFloat(GetPlayerPrefsKey(key) + "_b");
				float a = PlayerPrefs.GetFloat(GetPlayerPrefsKey(key) + "_a");
				return new Color(r, g, b, a);
			}
			return defaultValue;
		}

		public void Save(string key, int[] values)
		{
			ClearArrayKeys(key, values.Length);
			PlayerPrefs.SetInt(GetPlayerPrefsKey(key) + COUNT_KEY, values.Length);
			for(int i = 0; i < values.Length; i++)
			{
				PlayerPrefs.SetInt(GetPlayerPrefsKey(key) + "_" + i, values[i]);
			}
		}

		public int[] LoadArray(string key, int[] defaultValues)
		{
			int count = PlayerPrefs.GetInt(GetPlayerPrefsKey(key) + "_Count", -1);

			if(count == -1)
				return defaultValues;

			List<int> values = new List<int>();
			for(int i = 0; i < count; i++)
			{
				if(PlayerPrefs.HasKey(GetPlayerPrefsKey(key) + "_" + i))
				{
					values.Add(PlayerPrefs.GetInt(GetPlayerPrefsKey(key) + "_" + i));
				}
				else
				{
					break;
				}
			}
			return values.ToArray();
		}

		public void Save(string key, float[] values)
		{
			ClearArrayKeys(key, values.Length);
			PlayerPrefs.SetInt(GetPlayerPrefsKey(key) + COUNT_KEY, values.Length);
			for(int i = 0; i < values.Length; i++)
			{
				PlayerPrefs.SetFloat(GetPlayerPrefsKey(key) + "_" + i, values[i]);
			}
		}

		public float[] LoadArray(string key, float[] defaultValues)
		{
			int count = PlayerPrefs.GetInt(GetPlayerPrefsKey(key) + "_Count", -1);
			if(count == -1)
				return defaultValues;

			List<float> values = new List<float>();
			for(int i = 0; i < count; i++)
			{
				if(PlayerPrefs.HasKey(GetPlayerPrefsKey(key) + "_" + i))
				{
					values.Add(PlayerPrefs.GetFloat(GetPlayerPrefsKey(key) + "_" + i));
				}
				else
				{
					break;
				}
			}
			return values.ToArray();
		}


		public void Save(string key, string[] values)
		{
			ClearArrayKeys(key, values.Length);
			PlayerPrefs.SetInt(GetPlayerPrefsKey(key) + COUNT_KEY, values.Length);
			for(int i = 0; i < values.Length; i++)
			{
				PlayerPrefs.SetString(GetPlayerPrefsKey(key) + "_" + i, values[i]);
			}
		}
		public string[] LoadArray(string key, string[] defaultValues)
		{
			int count = PlayerPrefs.GetInt(GetPlayerPrefsKey(key) + "_Count", -1);
			if(count == -1)
				return defaultValues;

			List<string> values = new List<string>();
			for(int i = 0; i < count; i++)
			{
				if(PlayerPrefs.HasKey(GetPlayerPrefsKey(key) + "_" + i))
				{
					values.Add(PlayerPrefs.GetString(GetPlayerPrefsKey(key) + "_" + i));
				}
				else
				{
					break;
				}
			}
			return values.ToArray();
		}

		public void Save(string key, bool[] values)
		{
			ClearArrayKeys(key, values.Length);
			PlayerPrefs.SetInt(GetPlayerPrefsKey(key) + COUNT_KEY, values.Length);
			for(int i = 0; i < values.Length; i++)
			{
				PlayerPrefs.SetInt(GetPlayerPrefsKey(key) + "_" + i, values[i] ? 1 : 0);
			}
		}

		public bool[] LoadArray(string key, bool[] defaultValues)
		{
			int count = PlayerPrefs.GetInt(GetPlayerPrefsKey(key) + "_Count", -1);
			if(count == -1)
				return defaultValues;

			List<bool> values = new List<bool>();
			for(int i = 0; i < count; i++)
			{
				if(PlayerPrefs.HasKey(GetPlayerPrefsKey(key) + "_" + i))
				{
					values.Add(PlayerPrefs.GetInt(GetPlayerPrefsKey(key) + "_" + i) == 1);
				}
				else
				{
					break;
				}
			}
			return values.ToArray();
		}

		private void ClearArrayKeys(string key, int newLength)
		{
			int oldCount = PlayerPrefs.GetInt(GetPlayerPrefsKey(key) + COUNT_KEY, 0);
			for(int i = newLength; i < oldCount; i++)
			{
				PlayerPrefs.DeleteKey(GetPlayerPrefsKey(key) + "_" + i);
			}
		}

		public void WriteToDisk()
		{
			PlayerPrefs.Save();
		}
	}
}
