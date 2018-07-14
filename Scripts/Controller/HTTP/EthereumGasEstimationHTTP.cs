using System.Collections.Generic;
using UnityEngine;
using YourCommonTools;

namespace YourEthereumController
{
	public class EthereumGasEstimationHTTP : BaseDataHTTP, IHTTPComms
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
            string phpFile = (string)_list[0] + "^api?module=proxy&action=eth_estimateGas^";
#if !ENABLE_MY_OFUSCATION || UNITY_EDITOR
			phpFile = phpFile.Replace("^", "");
#endif
			m_urlRequest = phpFile;

            m_urlRequest += "&apikey=" + (string)_list[1];
            m_urlRequest += "&to=" + (string)_list[2];
            m_urlRequest += "&value=" + (string)_list[3];
            m_urlRequest += "&gasPrice=" + (string)_list[4];
            m_urlRequest += "&gas=" + (string)_list[5];            

            return "";
        }

		public override void Response(string _response)
		{
			ResponseCode(_response);

			EthereumEventController.Instance.DispatchEthereumEvent(EthereumController.EVENT_ETHEREUMCONTROLLER_GET_ESTIMATION_GAS, m_jsonResponse);
		}
	}
}

