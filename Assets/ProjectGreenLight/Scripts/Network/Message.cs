
#if !UNITY_WEBGL
namespace Noir.Network
{
	public class Message
	{
		public string message;
		public string messageOrigin;
		public bool self;

		public Message (string _message, string _messageOrigin, bool _self)
		{
			message = _message;
			messageOrigin = _messageOrigin;
			self = _self;
		}
	}
}
#endif