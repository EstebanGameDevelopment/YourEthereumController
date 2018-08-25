using System.Collections.Generic;
using YourCommonTools;

namespace YourEthereumController
{

	public class EthereumJSONExchangeTableHTTP : BaseDataHTTP, IHTTPComms
	{
		private string m_urlRequest;

		private string m_currency;
		private string m_valueItem;


		public string UrlRequest
		{
			get { return m_urlRequest; }
		}

		public override Dictionary<string, string> GetHeaders()
		{
			Dictionary<string, string> headers = new Dictionary<string, string>();
			return headers;
		}

		public string Build(params object[] _list)
		{
			string phpFile = "^https://min-api.cryptocompare.com/data/price?fsym=ETH&tsyms=BTC,USD,EUR,GBP,RUB,JPY,ETH^";
#if !ENABLE_MY_OFUSCATION || UNITY_EDITOR
			phpFile = phpFile.Replace("^", "");
#endif
			m_urlRequest = phpFile;

			return "";
		}

		public override void Response(string _response)
		{
			ResponseCode(_response);
			EthereumEventController.Instance.DispatchEthereumEvent(EthereumController.EVENT_ETHEREUMCONTROLLER_JSON_EXCHANGE_TABLE, m_jsonResponse);
		}
	}
}

