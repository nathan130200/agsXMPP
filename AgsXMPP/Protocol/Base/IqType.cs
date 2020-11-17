using AgsXMPP.Attributes;

namespace AgsXMPP.Protocol.Base
{
	public enum IqType
	{
		[XmppEnumMember("get")]
		Get,

		[XmppEnumMember("set")]
		Set,

		[XmppEnumMember("result")]
		Result,

		[XmppEnumMember("error")]
		Error
	}
}
