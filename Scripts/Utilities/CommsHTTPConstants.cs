using UnityEngine;
using YourCommonTools;

namespace YourEthereumController
{
	public class CommsHTTPConstants
	{
		// ----------------------------------------------
		// COMM EVENTS
		// ----------------------------------------------	
		public const string EVENT_COMM_ETHEREUM_EXCHANGE_INFO           = "YourEthereumController.EthereumExchangeHTTP";
		public const string EVENT_COMM_ETHEREUM_JSON_EXCHANGE_TABLE     = "YourEthereumController.EthereumJSONExchangeTableHTTP";
		public const string EVENT_COMM_ETHEREUM_JSON_TRANSACTION_FEE    = "YourEthereumController.EthereumJSONFeeHTTP";

        public const string EVENT_COMM_ETHEREUM_JSON_GAS_ESTIMATION     = "YourEthereumController.EthereumGasEstimationHTTP";
        public const string EVENT_COMM_ETHEREUM_JSON_GAS_PRICE          = "YourEthereumController.EthereumGasPriceHTTP";
        public const string EVENT_COMM_ETHEREUM_JSON_TRANSACTION_HISTORY= "YourEthereumController.EthereumTransactionsHistoryHTTP";

        // -------------------------------------------
        /* 
		 * DisplayLog
		 */
        public static void DisplayLog(string _data)
		{
			CommController.Instance.DisplayLog(_data);
		}

        // -------------------------------------------
        /* 
		 * GetEthereumExchangeFromCurrency
		 */
        public static void GetEthereumExchangeFromCurrency(string _currency, int _value)
		{
			CommController.Instance.Request(EVENT_COMM_ETHEREUM_EXCHANGE_INFO, false, _currency, _value.ToString());
		}

        // -------------------------------------------
        /* 
		 * GetEthereumExchangeRatesTable
		 */
        public static void GetEthereumExchangeRatesTable()
		{
			CommController.Instance.Request(EVENT_COMM_ETHEREUM_JSON_EXCHANGE_TABLE, false);
		}

        // -------------------------------------------
        /* 
		 * GetEthereumTransactionFee
		 */
        public static void GetEthereumTransactionFee()
		{
			CommController.Instance.Request(EVENT_COMM_ETHEREUM_JSON_TRANSACTION_FEE, false);
		}

        // -------------------------------------------
        /* 
		 * GetEthereumRequestGasEstimation
		 */
        public static void GetEthereumRequestGasEstimation(string _urlAPIServer,
                                                           string _apiKey,
                                                           string _addressTo,
                                                           string _valueAmount,
                                                           string _gasPrice,
                                                           string _gas)
        {
            string valueAmount = "0x" + _valueAmount;
            string gasPrice = "0x" + _gasPrice;
            string gas = "0x" + _gas;
            CommController.Instance.Request(EVENT_COMM_ETHEREUM_JSON_GAS_ESTIMATION, false, _urlAPIServer, _apiKey, _addressTo, valueAmount, gasPrice, gas);
        }

        // -------------------------------------------
        /* 
		 * GetEthereumRequestGasPrice
		 */
        public static void GetEthereumRequestGasPrice(string _urlAPIServer, string _apiKey)
        {
            CommController.Instance.Request(EVENT_COMM_ETHEREUM_JSON_GAS_PRICE, false, _urlAPIServer, _apiKey);
        }

        // -------------------------------------------
        /* 
		 * GetEthereumRequestTransactionHistory
		 */
        public static void GetEthereumRequestTransactionHistory(string _urlAPIServer, string _apiKey, string _address)
        {
            CommController.Instance.Request(EVENT_COMM_ETHEREUM_JSON_TRANSACTION_HISTORY, false, _urlAPIServer, _apiKey, _address);
        }

    }

}
