using System.Collections.Generic;
using YourCommonTools;

namespace YourEthereumController
{

	public class EthereumTransactionsHistoryHTTP : BaseDataHTTP, IHTTPComms
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
			string phpFile = (string)_list[0] + "^api?module=account&action=txlist^";
#if !ENABLE_MY_OFUSCATION || UNITY_EDITOR
			phpFile = phpFile.Replace("^", "");
#endif
			m_urlRequest = phpFile;

            m_urlRequest += "&apikey=" + (string)_list[1];
            m_urlRequest += "&address=" + (string)_list[2];
            m_urlRequest += "&startblock=0&endblock=99999999&sort=asc";

            // https://rinkeby.etherscan.io/api?module=account&action=txlist&address=0x80d1F26a9eaAc1110645EDCCE85e9295F8f49c29&startblock=0&endblock=99999999&page=1&offset=10&sort=asc&apikey=8G95X2GVREXZ6F78W6IX2AC5ZP7TCNJC5P

            return "";
		}

		public override void Response(string _response)
		{
			ResponseCode(_response);

			EthereumEventController.Instance.DispatchEthereumEvent(EthereumController.EVENT_ETHEREUMCONTROLLER_GET_TRANSACTIONS_LIST, m_jsonResponse);
		}
	}
}

