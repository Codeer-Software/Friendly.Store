using System;

namespace Codeer.Friendly.Store.Core
{
	/// <summary>
	/// システムコントロール情報。
	/// </summary>
	[Serializable]
	public class SystemControlInfo
	{
		SystemControlType _systemControlType;
		object _data;

		/// <summary>
		/// コントロールタイプ。
		/// </summary>
        public SystemControlType SystemControlType { get { return _systemControlType; } }
		
		/// <summary>
		/// データ。
		/// コントロールタイプによって異なる。
		/// </summary>
        public object Data { get { return _data; } }

		/// <summary>
		/// コンストラクタ。
		/// </summary>
		/// <param name="systemControlType">コントロールタイプ。</param>
		/// <param name="data">データ。</param>
        public SystemControlInfo(SystemControlType systemControlType, object data)
		{
			_systemControlType = systemControlType;
			_data = data;
		}
	}
}
