using System;

namespace Transportlaget
{
	public enum TransSize
	{
		CHKSUMSIZE = 2,
		ACKSIZE = 4
	};

	public enum TransCHKSUM
	{
		CHKSUMHIGH = 0,
		CHKSUMLOW = 1,
		SEQNO = 2,
		TYPE = 3
	};

	public enum TransType
	{
		DATA = 0,
		ACK = 1
	};
}