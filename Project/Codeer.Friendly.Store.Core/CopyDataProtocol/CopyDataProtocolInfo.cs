using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

namespace Codeer.Friendly.Store.Core.CopyDataProtocol
{
	/// <summary>
	/// 通信データ。
	/// </summary>
	[Serializable]
    public class CopyDataProtocolInfo
	{
		IntPtr _returnWindowHandle;
		object _data;

		/// <summary>
		/// 返信ウィンドウハンドル。
		/// </summary>
        public IntPtr ReturnWindowHandle { get { return _returnWindowHandle; } }

		/// <summary>
		/// データ。
		/// </summary>
        public object Data { get { return _data; } }

		/// <summary>
		/// コンストラクタ。
		/// </summary>
		/// <param name="returnWindowHandle">返信ウィンドウ。</param>
		/// <param name="data">データ。</param>
        public CopyDataProtocolInfo(IntPtr returnWindowHandle, object data)
		{
			_returnWindowHandle = returnWindowHandle;
			_data = data;
		}

		/// <summary>
		/// シリアライズ。
		/// </summary>
		/// <returns>バイナリ。</returns>
        public byte[] Serialize()
		{
			List<byte> bin = new List<byte>();
			CustomSerializer.Serialize(this, bin);
			return bin.ToArray();
		}

		/// <summary>
		/// デシリアライズ。
		/// </summary>
		/// <param name="bin">バイナリ。</param>
		/// <returns>データ。</returns>
        public static CopyDataProtocolInfo Deserialize(byte[] bin)
		{
			int index = 0;
			return (CopyDataProtocolInfo)CustomSerializer.Deserialize(bin, ref index);
		}
	}
}
