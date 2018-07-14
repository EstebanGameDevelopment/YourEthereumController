using System.Collections.Generic;
using YourCommonTools;

namespace YourEthereumController
{

	public class EthereumGasPriceHTTP : BaseDataHTTP, IHTTPComms
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
			string phpFile = (string)_list[0] + "^api?module=proxy&action=eth_gasPrice^";
#if !ENABLE_MY_OFUSCATION || UNITY_EDITOR
			phpFile = phpFile.Replace("^", "");
#endif
			m_urlRequest = phpFile;

            m_urlRequest += "&apikey=" + (string)_list[1];

            return "";
		}

		public override void Response(string _response)
		{
			ResponseCode(_response);

			EthereumEventController.Instance.DispatchEthereumEvent(EthereumController.EVENT_ETHEREUMCONTROLLER_GET_GAS_PRICE, m_jsonResponse);
		}
	}
}

