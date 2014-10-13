using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using Codeer.Friendly.Inside.Protocol;
using System;
using Codeer.Friendly;

namespace Codeer.Friendly.Store.Core.CopyDataProtocol
{
	static class CustomSerializer
	{
		public static void Serialize(object obj, List<byte> bin)
		{
			if (obj == null)
			{
				SerializeNormal("null", bin);
			}
			else if (obj.GetType().FullName.IndexOf("Codeer") == 0)
			{
				SerializeNormal(obj.GetType().FullName, bin);
				SerializeCodeer(obj, bin);
			}
			else if (obj.GetType() == typeof(object[]))
			{
				SerializeNormal("[]", bin);
				SerializeArray(obj, bin);
			}
			else
			{
				SerializeNormal("*", bin);
				SerializeNormal(obj, bin);
			}
		}

		public static object Deserialize(byte[] bin, ref int index)
		{
			string name = (string)DeserializeNormal(bin, ref index);
			if (name == "null")
			{
				return null;
			}
			if (name == "[]")
			{
				return DeserializeArray(bin, ref index);
			}
			else if (name == "*")
			{
				return DeserializeNormal(bin, ref index);
			}
			else
			{
				return DeserializeCodeer(name, bin, ref index);
			}
		}

		private static void SerializeInt(int value, List<byte> bin)
		{
			bin.Add((byte)(value & 0xff));
			bin.Add((byte)((value >> 8) & 0xff));
			bin.Add((byte)((value >> 16) & 0xff));
			bin.Add((byte)((value >> 24) & 0xff));
		}

		private static int DeserializeInt(byte[] bin, ref int index)
		{
			int value = 0;
			value += bin[index];
			value += (bin[index + 1] << 8);
			value += (bin[index + 2] << 16);
			value += (bin[index + 3] << 24);
			index += 4;
			return value;
		}

		private static void SerializeNormal(object obj, List<byte> bin)
		{
			using (MemoryStream m = new MemoryStream())
			{
				BinaryFormatter formatter = new BinaryFormatter();
				formatter.Serialize(m, obj);
				byte[] binCore = m.ToArray();
				SerializeInt(binCore.Length, bin);
				bin.AddRange(binCore);
			}
		}

		private static object DeserializeNormal(byte[] bin, ref int index)
		{
			int len = DeserializeInt(bin, ref index);
			using (MemoryStream m = new MemoryStream(bin, index, len))
			{
				BinaryFormatter formatter = new BinaryFormatter();
				index += len;
				return formatter.Deserialize(m);
			}
		}

		private static void SerializeArray(object obj, List<byte> bin)
		{
			object[] array = (object[])obj;
			SerializeInt(array.Length, bin);
			foreach (object o in array)
			{
				Serialize(o, bin);
			}
		}

		private static object DeserializeArray(byte[] bin, ref int index)
		{
			int len = DeserializeInt(bin, ref index);
			object[] array = new object[len];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = Deserialize(bin, ref index);
			}
			return array;
		}

		private static void SerializeCodeer(object obj, List<byte> bin)
		{
			FieldInfo[] infos = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			SerializeInt(infos.Length, bin);
			foreach (FieldInfo f in infos)
			{
				SerializeNormal(f.Name, bin);
				Serialize(f.GetValue(obj), bin);
			}
		}

		private static object DeserializeCodeer(string name, byte[] bin, ref int index)
		{
			object obj = CreateCodeerInstance(name);
			int count = DeserializeInt(bin, ref index);
			for (int i = 0; i < count; i++)
			{
				FieldInfo f = obj.GetType().GetField((string)DeserializeNormal(bin, ref index), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
				f.SetValue(obj, Deserialize(bin, ref index));
			}
			return obj;
		}

		private static object CreateCodeerInstance(string name)
		{
			if (name == typeof(ProtocolInfo).FullName)
			{
				return new ProtocolInfo(ProtocolType.AsyncOperation, new OperationTypeInfo("a"), new VarAddress(0), "a", "a", new object[0]);
			}
			else if (name == typeof(ReturnInfo).FullName)
			{
				return new ReturnInfo();
			}

			else if (name == typeof(ExceptionInfo).FullName)
			{
				return new ExceptionInfo(new Exception());
			}
			else if (name == typeof(VarAddress).FullName)
			{
				return new VarAddress(0);
			}
			else if (name == typeof(OperationTypeInfo).FullName)
			{
				return new OperationTypeInfo("a");
			}
			else if (name == typeof(ProtocolType).FullName)
			{
				return ProtocolType.AsyncOperation;
			}
			else if (name == typeof(SystemControlType).FullName)
			{
				return SystemControlType.EndFriendlyConnectorWindowInApp;
			}
			else if (name == typeof(ContextOrderProtocolInfo).FullName)
			{
				return new ContextOrderProtocolInfo(
					new ProtocolInfo(ProtocolType.AsyncOperation, new OperationTypeInfo("a"), new VarAddress(0), "a", "a", new object[0]),
					IntPtr.Zero);
			}
			else if (name == typeof(SystemControlInfo).FullName)
			{
				return new SystemControlInfo(SystemControlType.EndFriendlyConnectorWindowInApp, null);
			}
			else if (name == typeof(CopyDataProtocolInfo).FullName)
			{
				return new CopyDataProtocolInfo(IntPtr.Zero, null);
			}
			throw new NotSupportedException("未実装");
		}
	}
}
