using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace RaStoragePrefsSystem
{
	public class RaStoragePrefs
	{
		private const string COUNT_KEY = "_Count";
		private const string INDEX_SEPARATOR = "_";

		public string Id
		{
			get; private set;
		}

		public RaStoragePrefs(string id)
		{
			Id = id;
		}

		private string GetPlayerPrefsKey(string key)
		{
			return Id + key;
		}

		public void Save(string key, int value)
		{
			string playerPrefsKey = GetPlayerPrefsKey(key);
			PlayerPrefs.SetInt(playerPrefsKey, value);
		}

		public int Load(string key, int defaultValue)
		{
			string playerPrefsKey = GetPlayerPrefsKey(key);
			return PlayerPrefs.GetInt(playerPrefsKey, defaultValue);
		}

		public void Save(string key, float value)
		{
			string playerPrefsKey = GetPlayerPrefsKey(key);
			PlayerPrefs.SetFloat(playerPrefsKey, value);
		}

		public float Load(string key, float defaultValue)
		{
			string playerPrefsKey = GetPlayerPrefsKey(key);
			return PlayerPrefs.GetFloat(playerPrefsKey, defaultValue);
		}

		public void Save(string key, string value)
		{
			string playerPrefsKey = GetPlayerPrefsKey(key);
			PlayerPrefs.SetString(playerPrefsKey, value);
		}

		public string Load(string key, string defaultValue)
		{
			string playerPrefsKey = GetPlayerPrefsKey(key);
			return PlayerPrefs.GetString(playerPrefsKey, defaultValue);
		}

		public void Save(string key, bool value)
		{
			string playerPrefsKey = GetPlayerPrefsKey(key);
			PlayerPrefs.SetInt(playerPrefsKey, value ? 1 : 0);
		}

		public bool Load(string key, bool defaultValue)
		{
			string playerPrefsKey = GetPlayerPrefsKey(key);
			return PlayerPrefs.GetInt(playerPrefsKey, defaultValue ? 1 : 0) == 1;
		}

		public void Save<T>(string key, T value) where T : IRaPrefObject
		{
			string playerPrefsKey = GetPlayerPrefsKey(key);
			PlayerPrefs.SetString(playerPrefsKey, value.Serialize());
		}

		public T Load<T>(string key, Func<string, T> deserialize, Func<T> defaultValue) where T : IRaPrefObject
		{
			string playerPrefsKey = GetPlayerPrefsKey(key);
			string serializedData = PlayerPrefs.GetString(playerPrefsKey, string.Empty);
			if(!string.IsNullOrEmpty(serializedData))
			{
				return deserialize(serializedData);
			}
			return defaultValue();
		}

		public void Save<T>(string key, T[] values) where T : IRaPrefObject
		{
			string playerPrefsKey = GetPlayerPrefsKey(key);
			ClearArrayKeys(playerPrefsKey, values.Length);

			int serializationCount = 0;
			for(int i = 0; i < values.Length; i++)
			{
				try
				{
					string json = values[i].Serialize();
					PlayerPrefs.SetString(playerPrefsKey + INDEX_SEPARATOR + serializationCount, json);
					serializationCount++;
				}
				catch(Exception e)
				{
					Debug.LogError($"{nameof(RaStoragePrefs)} - {key} could not serialize value {i}. Message: {e.Message}");
				}
			}
			PlayerPrefs.SetInt(playerPrefsKey + COUNT_KEY, serializationCount);
		}

		public T[] LoadArray<T>(string key, Func<string, T> deserialize) where T : IRaPrefObject
		{
			string playerPrefsKey = GetPlayerPrefsKey(key);
			int count = PlayerPrefs.GetInt(playerPrefsKey + COUNT_KEY, 0);
			List<T> values = new List<T>();
			for(int i = 0; i < count; i++)
			{
				string serializedData = PlayerPrefs.GetString(playerPrefsKey + INDEX_SEPARATOR + i, string.Empty);
				if(!string.IsNullOrEmpty(serializedData))
				{
					try
					{
						T value = deserialize(serializedData);
						values.Add(value);
					}
					catch(Exception e)
					{
						Debug.LogError($"RaStorage - Could not Deserialize Key {key} under index {i}. Error: " + e?.Message + " | " + e.InnerException?.Message);
					}
				}
			}
			return values.ToArray();
		}

		public void Save(string key, DateTime value)
		{
			string playerPrefsKey = GetPlayerPrefsKey(key);
			long binaryDate = value.ToBinary();
			PlayerPrefs.SetString(playerPrefsKey, binaryDate.ToString(CultureInfo.InvariantCulture));
		}

		public DateTime Load(string key, DateTime defaultValue)
		{
			string playerPrefsKey = GetPlayerPrefsKey(key);
			string binaryString = PlayerPrefs.GetString(playerPrefsKey, string.Empty);
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
			string playerPrefsKey = GetPlayerPrefsKey(key);
			PlayerPrefs.SetFloat(playerPrefsKey + "_x", value.x);
			PlayerPrefs.SetFloat(playerPrefsKey + "_y", value.y);
			PlayerPrefs.SetFloat(playerPrefsKey + "_z", value.z);
		}

		public Vector3 Load(string key, Vector3 defaultValue)
		{
			string playerPrefsKey = GetPlayerPrefsKey(key);
			if(PlayerPrefs.HasKey(playerPrefsKey + "_x") &&
				PlayerPrefs.HasKey(playerPrefsKey + "_y") &&
				PlayerPrefs.HasKey(playerPrefsKey + "_z"))
			{
				float x = PlayerPrefs.GetFloat(playerPrefsKey + "_x");
				float y = PlayerPrefs.GetFloat(playerPrefsKey + "_y");
				float z = PlayerPrefs.GetFloat(playerPrefsKey + "_z");
				return new Vector3(x, y, z);
			}
			return defaultValue;
		}

		public void Save(string key, Color value)
		{
			string playerPrefsKey = GetPlayerPrefsKey(key);
			PlayerPrefs.SetFloat(playerPrefsKey + "_r", value.r);
			PlayerPrefs.SetFloat(playerPrefsKey + "_g", value.g);
			PlayerPrefs.SetFloat(playerPrefsKey + "_b", value.b);
			PlayerPrefs.SetFloat(playerPrefsKey + "_a", value.a);
		}

		public Color Load(string key, Color defaultValue)
		{
			string playerPrefsKey = GetPlayerPrefsKey(key);
			if(PlayerPrefs.HasKey(playerPrefsKey + "_r") &&
				PlayerPrefs.HasKey(playerPrefsKey + "_g") &&
				PlayerPrefs.HasKey(playerPrefsKey + "_b") &&
				PlayerPrefs.HasKey(playerPrefsKey + "_a"))
			{
				float r = PlayerPrefs.GetFloat(playerPrefsKey + "_r");
				float g = PlayerPrefs.GetFloat(playerPrefsKey + "_g");
				float b = PlayerPrefs.GetFloat(playerPrefsKey + "_b");
				float a = PlayerPrefs.GetFloat(playerPrefsKey + "_a");
				return new Color(r, g, b, a);
			}
			return defaultValue;
		}

		public void Save(string key, int[] values)
		{
			string playerPrefsKey = GetPlayerPrefsKey(key);
			ClearArrayKeys(playerPrefsKey, values.Length);

			PlayerPrefs.SetInt(playerPrefsKey + COUNT_KEY, values.Length);
			for(int i = 0; i < values.Length; i++)
			{
				PlayerPrefs.SetInt(playerPrefsKey + INDEX_SEPARATOR + i, values[i]);
			}
		}

		public int[] LoadArray(string key, int[] defaultValues)
		{
			string playerPrefsKey = GetPlayerPrefsKey(key);
			int count = PlayerPrefs.GetInt(playerPrefsKey + COUNT_KEY, -1);

			if(count == -1)
				return defaultValues;

			List<int> values = new List<int>();
			for(int i = 0; i < count; i++)
			{
				string indexKey = playerPrefsKey + INDEX_SEPARATOR + i;
				if(PlayerPrefs.HasKey(indexKey))
				{
					values.Add(PlayerPrefs.GetInt(indexKey));
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
			string playerPrefsKey = GetPlayerPrefsKey(key);
			ClearArrayKeys(playerPrefsKey, values.Length);

			PlayerPrefs.SetInt(playerPrefsKey + COUNT_KEY, values.Length);
			for(int i = 0; i < values.Length; i++)
			{
				PlayerPrefs.SetFloat(playerPrefsKey + INDEX_SEPARATOR + i, values[i]);
			}
		}

		public float[] LoadArray(string key, float[] defaultValues)
		{
			string playerPrefsKey = GetPlayerPrefsKey(key);
			int count = PlayerPrefs.GetInt(playerPrefsKey + COUNT_KEY, -1);

			if(count == -1)
				return defaultValues;

			List<float> values = new List<float>();
			for(int i = 0; i < count; i++)
			{
				string indexKey = playerPrefsKey + INDEX_SEPARATOR + i;
				if(PlayerPrefs.HasKey(indexKey))
				{
					values.Add(PlayerPrefs.GetFloat(indexKey));
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
			string playerPrefsKey = GetPlayerPrefsKey(key);
			ClearArrayKeys(playerPrefsKey, values.Length);

			PlayerPrefs.SetInt(playerPrefsKey + COUNT_KEY, values.Length);
			for(int i = 0; i < values.Length; i++)
			{
				PlayerPrefs.SetString(playerPrefsKey + INDEX_SEPARATOR + i, values[i]);
			}
		}

		public string[] LoadArray(string key, string[] defaultValues)
		{
			string playerPrefsKey = GetPlayerPrefsKey(key);
			int count = PlayerPrefs.GetInt(playerPrefsKey + COUNT_KEY, -1);

			if(count == -1)
				return defaultValues;

			List<string> values = new List<string>();
			for(int i = 0; i < count; i++)
			{
				string indexKey = playerPrefsKey + INDEX_SEPARATOR + i;
				if(PlayerPrefs.HasKey(indexKey))
				{
					values.Add(PlayerPrefs.GetString(indexKey));
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
			string playerPrefsKey = GetPlayerPrefsKey(key);
			ClearArrayKeys(playerPrefsKey, values.Length);

			PlayerPrefs.SetInt(playerPrefsKey + COUNT_KEY, values.Length);
			for(int i = 0; i < values.Length; i++)
			{
				PlayerPrefs.SetInt(playerPrefsKey + INDEX_SEPARATOR + i, values[i] ? 1 : 0);
			}
		}

		public bool[] LoadArray(string key, bool[] defaultValues)
		{
			string playerPrefsKey = GetPlayerPrefsKey(key);
			int count = PlayerPrefs.GetInt(playerPrefsKey + COUNT_KEY, -1);

			if(count == -1)
				return defaultValues;

			List<bool> values = new List<bool>();
			for(int i = 0; i < count; i++)
			{
				string indexKey = playerPrefsKey + INDEX_SEPARATOR + i;
				if(PlayerPrefs.HasKey(indexKey))
				{
					values.Add(PlayerPrefs.GetInt(indexKey) == 1);
				}
				else
				{
					break;
				}
			}
			return values.ToArray();
		}


		private void ClearArrayKeys(string playerPrefsKey, int newLength)
		{
			int oldCount = PlayerPrefs.GetInt(playerPrefsKey + COUNT_KEY, 0);
			for(int i = newLength; i < oldCount; i++)
			{
				PlayerPrefs.DeleteKey(playerPrefsKey + INDEX_SEPARATOR + i);
			}
		}

		public static void WriteToDisk()
		{
			PlayerPrefs.Save();
		}
	}
}