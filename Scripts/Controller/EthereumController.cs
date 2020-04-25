using UnityEngine;
using System;
using System.Linq;
using YourCommonTools;
using System.Collections.Generic;
using System.Text;
#if ENABLE_ETHEREUM
using YourEthereumManager;
using System.Numerics;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Contracts.CQS;
using Newtonsoft.Json;
using Nethereum.JsonRpc.UnityClient;
using Nethereum.Signer;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.Signer.Crypto;
#endif

namespace YourEthereumController
{
    public static class ArrayExtensions
	{
		public static T[] SubArray<T>(this T[] data, int index, int length)
		{
			var result = new T[length];
			Array.Copy(data, index, result, 0, length);
			return result;
		}
    }

#if ENABLE_ETHEREUM
    public class ArrayUint256DynamicDeployment : ContractDeploymentMessage
    {
        public static string BYTECODE = "608060405234801561001057600080fd5b50610127806100206000396000f300608060405260043610603e5763ffffffff7c01000000000000000000000000000000000000000000000000000000006000350416634b04bc0481146043575b600080fd5b348015604e57600080fd5b50605560a3565b60408051602080825283518183015283519192839290830191858101910280838360005b83811015608f5781810151838201526020016079565b505050509050019250505060405180910390f35b6040805160028082526060808301845292602083019080388339019050509050600181600081518110151560d357fe5b6020908102909101015280516002908290600190811060ee57fe5b60209081029091010152905600a165627a7a72305820aa3f79a3ff9e580c10090fb0e8830490f2acbd918c76c8488c40bdaf1c9180800029";
        public ArrayUint256DynamicDeployment() : base(BYTECODE) { }
        public ArrayUint256DynamicDeployment(string byteCode) : base(byteCode) { }
    }

    [Function("GiveMeTheArray", "uint256[]")]
    public class GiveMeTheArrayFunction : FunctionMessage
    {

    }

    [FunctionOutput]
    public class GiveMeTheArrayOutputDTO : IFunctionOutputDTO
    {
        [Parameter("uint256[]", "result", 1)]
        public List<BigInteger> Result { get; set; }
    }
#endif

    /******************************************
	 * 
	 * EthereumController
     * 
     * 1. Install: Install-Package Nethereum.Portable
     * 
     * 2. Copy all the DLLs except "Nethereum.Web3.dll" to your assets folder
     * 
	 * @author Esteban Gallardo
	 */
    public class EthereumController : MonoBehaviour
    {
        public const string ETHERSCAN_API_KEY = "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX";   // Get your own key at: https://etherscan.io
        public const string INFURA_API_KEY = "YYYYYYYYYYYYYYYYY";   // Get your own key at: https://infura.io/

        public const decimal ETHER_WAI_FACTOR = 1000000000000000000;

        // ----------------------------------------------
        // EVENTS
        // ----------------------------------------------	
        public const string EVENT_ETHEREUMCONTROLLER_BALANCE_WALLET             = "EVENT_ETHEREUMCONTROLLER_BALANCE_WALLET";
        public const string EVENT_ETHEREUMCONTROLLER_PAYMENTS_DONE              = "EVENT_ETHEREUMCONTROLLER_PAYMENTS_DONE";
        public const string EVENT_ETHEREUMCONTROLLER_TRANSACTION_DONE           = "EVENT_ETHEREUMCONTROLLER_TRANSACTION_DONE";
        public const string EVENT_ETHEREUMCONTROLLER_TRANSACTION_COMPLETED      = "EVENT_ETHEREUMCONTROLLER_TRANSACTION_COMPLETED";
        public const string EVENT_ETHEREUMCONTROLLER_EXCHANGE_DATA              = "EVENT_ETHEREUMCONTROLLER_EXCHANGE_DATA";
        public const string EVENT_ETHEREUMCONTROLLER_JSON_EXCHANGE_TABLE        = "EVENT_ETHEREUMCONTROLLER_JSON_EXCHANGE_TABLE";
        public const string EVENT_ETHEREUMCONTROLLER_JSON_FEE_TABLE             = "EVENT_ETHEREUMCONTROLLER_JSON_FEE_TABLE";
        public const string EVENT_ETHEREUMCONTROLLER_ALL_DATA_COLLECTED         = "EVENT_ETHEREUMCONTROLLER_ALL_DATA_COLLECTED";
        public const string EVENT_ETHEREUMCONTROLLER_ALL_DATA_INITIALIZED       = "EVENT_ETHEREUMCONTROLLER_ALL_DATA_INITIALIZED";
        public const string EVENT_ETHEREUMCONTROLLER_SELECTED_PRIVATE_KEY       = "EVENT_ETHEREUMCONTROLLER_SELECTED_PRIVATE_KEY";
        public const string EVENT_ETHEREUMCONTROLLER_SELECTED_PUBLIC_KEY        = "EVENT_ETHEREUMCONTROLLER_SELECTED_PUBLIC_KEY";
        public const string EVENT_ETHEREUMCONTROLLER_NEW_CURRENCY_SELECTED      = "EVENT_ETHEREUMCONTROLLER_NEW_CURRENCY_SELECTED";
        public const string EVENT_ETHEREUMCONTROLLER_CURRENCY_CHANGED           = "EVENT_ETHEREUMCONTROLLER_CURRENCY_CHANGED";
        public const string EVENT_ETHEREUMCONTROLLER_UPDATE_ACCOUNT_DATA        = "EVENT_ETHEREUMCONTROLLER_UPDATE_ACCOUNT_DATA";
        public const string EVENT_ETHEREUMCONTROLLER_PUBLIC_KEY_SELECTED        = "EVENT_ETHEREUMCONTROLLER_PUBLIC_KEY_SELECTED";
        public const string EVENT_ETHEREUMCONTROLLER_USER_DATA_UPDATED          = "EVENT_ETHEREUMCONTROLLER_USER_DATA_UPDATED";
        public const string EVENT_ETHEREUMCONTROLLER_TRANSACTION_USER_ACKNOWLEDGE = "EVENT_ETHEREUMCONTROLLER_TRANSACTION_USER_ACKNOWLEDGE";

        public const string EVENT_ETHEREUMCONTROLLER_GET_BLOCK_NUMBER           = "EVENT_ETHEREUMCONTROLLER_GET_BLOCK_NUMBER";
        public const string EVENT_ETHEREUMCONTROLLER_VALIDATE_PUBLIC_KEY        = "EVENT_ETHEREUMCONTROLLER_VALIDATE_PUBLIC_KEY";
        public const string EVENT_ETHEREUMCONTROLLER_BALANCE_ENOUGH_TO_PAY      = "EVENT_ETHEREUMCONTROLLER_BALANCE_ENOUGH_TO_PAY";
        public const string EVENT_ETHEREUMCONTROLLER_SIGNED_DOCUMENT_DONE       = "EVENT_ETHEREUMCONTROLLER_SIGNED_DOCUMENT_DONE";
        public const string EVENT_ETHEREUMCONTROLLER_VERIFICATION_SIGNED_DATA   = "EVENT_ETHEREUMCONTROLLER_VERIFICATION_SIGNED_DATA";
        public const string EVENT_ETHEREUMCONTROLLER_GET_TRANSACTIONS_LIST      = "EVENT_ETHEREUMCONTROLLER_GET_TRANSACTIONS_LIST";
        public const string EVENT_ETHEREUMCONTROLLER_GET_GAS_PRICE              = "EVENT_ETHEREUMCONTROLLER_GET_GAS_PRICE";
        public const string EVENT_ETHEREUMCONTROLLER_GET_ESTIMATION_GAS         = "EVENT_ETHEREUMCONTROLLER_GET_ESTIMATION_GAS";
        public const string EVENT_ETHEREUMCONTROLLER_TRANSACTION_HISTORY        = "EVENT_ETHEREUMCONTROLLER_TRANSACTION_HISTORY";

        public const string COUROUTINE_REQUEST_VALIDATE_PUBLIC_KEY = "COUROUTINE_REQUEST_VALIDATE_PUBLIC_KEY";

        public const string NETWORK_TEST = "TEST_";
        public const string NETWORK_MAIN = "MAIN_";

        public const string ETHEREUM_PRIVATE_KEYS           = "ETHEREUM_PRIVATE_KEYS";
        public const string ETHEREUM_DEFAULT_CURRENCY       = "ETHEREUM_DEFAULT_CURRENCY";
        public const string ETHEREUM_PRIVATE_KEY_SELECTED   = "ETHEREUM_PRIVATE_KEY_SELECTED";
        public const string ETHEREUM_ADDRESSES_LIST         = "ETHEREUM_ADDRESSES_LIST";

        public const string ETHEREUM_CONTRACT_ID_PAYMENT     = "ETHEREUM_CONTRACT_ID_PAYMENT";
        public const string ETHEREUM_CONTRACT_ID_SIGNED_DATA = "ETHEREUM_CONTRACT_ID_SIGNED_DATA";
        
        public const char SEPARATOR_ITEMS = ';';
        public const char SEPARATOR_COMA = ',';

        public const int TOTAL_SIZE_PRIVATE_KEY_ETHEREUM = 66;
        public const int TOTAL_SIZE_PUBLIC_KEY_ETHEREUM = 42;

#if !ENABLE_MY_OFUSCATION || UNITY_EDITOR
        public const string ENCRYPTION_KEY = "ps38NwtKl521rLyTwr4252IctREhuP26";  // CRITICAL!!! CHANGE THIS KEY TO YOUR OWN AND DO NOT SHARE IT, PLEASE!!
#else
	public const string ENCRYPTION_KEY = "^ps38NwtKl521rLyTwr4252IctREhuP26^";  // CRITICAL!!! CHANGE THIS KEY TO YOUR OWN AND DO NOT SHARE IT, PLEASE!!
#endif

        public const string BASE_URL_ADDRESS_ACCESS_ETHEREUM_NETWORK_TESTNET = "https://rinkeby.infura.io/";
        public const string BASE_URL_ADDRESS_ACCESS_ETHEREUM_NETWORK_MAINNET = "https://mainnet.infura.io/";

        public const string ETHERSCAN_API_ADDRESS_ACCESS_ETHEREUM_NETWORK_TESTNET = "https://rinkeby.etherscan.io/";
        public const string ETHERSCAN_API_ADDRESS_ACCESS_ETHEREUM_NETWORK_MAINNET = "https://api.etherscan.io/";

        public const string CODE_DOLLAR = "USD";
        public const string CODE_EURO = "EUR";
        public const string CODE_YEN = "JPY";
        public const string CODE_RUBLO = "RUB";
        public const string CODE_POUND = "GBP";
        public const string CODE_BITCOIN = "BTC";
        public const string CODE_ETHEREUM = "ETH";

        public static readonly string[] CURRENCY_CODE = { CODE_DOLLAR, CODE_EURO, CODE_YEN, CODE_RUBLO, CODE_POUND, CODE_BITCOIN, CODE_ETHEREUM };

        public const string OPTION_NETWORK_COOKIE = "OPTION_NETWORK_COOKIE";

        public const string OPTION_NETWORK_TEST = "Test Network";
        public const string OPTION_NETWORK_MAIN = "Main Network";

        public static readonly string[] OPTIONS_NETWORK = { OPTION_NETWORK_TEST, OPTION_NETWORK_MAIN };

        // ----------------------------------------------
        // SINGLETON
        // ----------------------------------------------	
        private static EthereumController _instance;

        public static EthereumController Instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = GameObject.FindObjectOfType(typeof(EthereumController)) as EthereumController;
                    if (!_instance)
                    {
                        GameObject container = new GameObject();
                        container.name = "EthereumController";
                        _instance = container.AddComponent(typeof(EthereumController)) as EthereumController;
                        DontDestroyOnLoad(_instance);
                    }
                }
                return _instance;
            }
        }

        // ----------------------------------------------
        // PRIVATE MEMBERS
        // ----------------------------------------------
        private bool m_initialized = false;
        // private NBitcoin.Network m_network;
        private bool m_isMainNetwork = false;
        private Dictionary<string, decimal> m_privateKeys = new Dictionary<string, decimal>();
        private Dictionary<string, string> m_publicKeys = new Dictionary<string, string>();
        private string m_currentPrivateKey = "";
        private string m_currentPublicKey = "";
        private string m_backupCurrentPrivateKey = "";
        private string m_currentCurrency = CODE_DOLLAR;

        private Dictionary<string, decimal> m_publicKeysCollected = new Dictionary<string, decimal>();

        private string m_titleTransaction;
        private string m_publicKeyTarget;

        private bool m_requestedToPay = false;
        private bool m_requestedToPayConfirmationFee = false;
        private decimal m_finalValueEther = -1m;
        private decimal m_finalFeeAmount = -1m;

        private decimal m_balanceWallet = -1;
        private float m_paymentsWallet = -1;

        private int m_stepMining = 0;

#if ENABLE_ETHEREUM
        private PaymentContractService m_paymentContractService = null;
        private string m_contractIDPayment = "";

        private SignDocumentContractService m_signDocumentContractService = null;
        private string m_contractIDSignedDocument = "";
#endif

        private bool m_runningSigningProcess = false;
        private bool m_runningPaymentProcess = false;

        private string m_publicAdressCheckHistory = "";

        private List<ItemMultiObjectEntry> m_allTransactionsHistory = new List<ItemMultiObjectEntry>();
        private List<ItemMultiObjectEntry> m_inTransactionsHistory = new List<ItemMultiObjectEntry>();
        private List<ItemMultiObjectEntry> m_outTransactionsHistory = new List<ItemMultiObjectEntry>();

        private Dictionary<string, decimal> m_walletBalanceCurrencies = new Dictionary<string, decimal>();
        private Dictionary<string, decimal> m_currenciesExchange = new Dictionary<string, decimal>();
        private Dictionary<string, decimal> m_feesTransactions = new Dictionary<string, decimal>();

        private Dictionary<string, string> m_addressesList = new Dictionary<string, string>();

        public bool IsMainNetwork
        {
            get { return m_isMainNetwork; }
            set
            {
                m_isMainNetwork = value;
                PlayerPrefs.SetString(OPTION_NETWORK_COOKIE, (m_isMainNetwork ? OPTION_NETWORK_MAIN : OPTION_NETWORK_TEST));
            }
        }
        public string NetworkAPI
        {
            get { 
                if (m_isMainNetwork)
                {
                    return BASE_URL_ADDRESS_ACCESS_ETHEREUM_NETWORK_MAINNET + INFURA_API_KEY;
                }
                else
                {
                    return BASE_URL_ADDRESS_ACCESS_ETHEREUM_NETWORK_TESTNET + INFURA_API_KEY;
                }
            }
        }
        public string EtherscanAPI
        {
            get
            {
                if (m_isMainNetwork)
                {
                    return ETHERSCAN_API_ADDRESS_ACCESS_ETHEREUM_NETWORK_MAINNET;
                }
                else
                {
                    return ETHERSCAN_API_ADDRESS_ACCESS_ETHEREUM_NETWORK_TESTNET;
                }
            }
        }
        public Dictionary<string, decimal> WalletBalanceCurrencies
        {
            get { return m_walletBalanceCurrencies; }
        }
        public List<ItemMultiObjectEntry> AllTransactionsHistory
        {
            get { return m_allTransactionsHistory; }
        }
        public List<ItemMultiObjectEntry> InTransactionsHistory
        {
            get { return m_inTransactionsHistory; }
        }
        public List<ItemMultiObjectEntry> OutTransactionsHistory
        {
            get { return m_outTransactionsHistory; }
        }
        public Dictionary<string, decimal> CurrenciesExchange
        {
            get { return m_currenciesExchange; }
        }
        public Dictionary<string, decimal> FeesTransactions
        {
            get { return m_feesTransactions; }
        }
        public Dictionary<string, decimal> PrivateKeys
        {
            get { return m_privateKeys; }
        }
        public Dictionary<string, string> PublicKeys
        {
            get { return m_publicKeys; }
        }
        public string CodeNetwork
        {
            get
            {
                if (m_isMainNetwork)
                {
                    return NETWORK_MAIN;
                }
                else
                {
                    return NETWORK_TEST;
                }
            }
        }
#if ENABLE_ETHEREUM
        public BigInteger NetworkChainID
        {
            get
            {
                if (m_isMainNetwork)
                {
                    return new BigInteger((int)Chain.ClassicMainNet);
                }
                else
                {
                    return new BigInteger((int)Chain.ClassicTestNet);
                }
            }
        }
#endif
        public string CurrentPrivateKey
        {
            get { return m_currentPrivateKey; }
            set {
                if (m_currentPrivateKey.Length > 0) m_backupCurrentPrivateKey = "" + m_currentPrivateKey;
                m_currentPrivateKey = value;
                if (m_currentPrivateKey.Length != 0)
                {
                    m_currentPublicKey = GetPublicKey(m_currentPrivateKey);
                    PlayerPrefs.SetString(CodeNetwork + ETHEREUM_PRIVATE_KEY_SELECTED, RJEncryptor.EncryptStringWithKey(m_currentPrivateKey, ENCRYPTION_KEY));
                    EthereumEventController.Instance.DispatchEthereumEvent(EVENT_ETHEREUMCONTROLLER_SELECTED_PRIVATE_KEY);
                }
            }
        }
        public string CurrentPublicKey
        {
            get { return m_currentPublicKey; }
        }
        public string BackupCurrentPrivateKey
        {
            set
            {
                m_backupCurrentPrivateKey = value;
                if (m_backupCurrentPrivateKey.Length == 0)
                {
                    m_currentPrivateKey = "";
                }
                EthereumEventController.Instance.DispatchEthereumEvent(EVENT_ETHEREUMCONTROLLER_SELECTED_PRIVATE_KEY);
            }
        }
        public string CurrentCurrency
        {
            get { return m_currentCurrency; }
            set { m_currentCurrency = value;
                PlayerPrefs.SetString(CodeNetwork + ETHEREUM_DEFAULT_CURRENCY, m_currentCurrency);
            }
        }
        public Dictionary<string, string> AddressesList
        {
            get { return m_addressesList; }
        }

        // -------------------------------------------
        /* 
		 * Initialitzation
		 */
        public void Init(params string[] _list)
        {
            if (m_initialized)
            {
                return;
            }
            m_initialized = true;

            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

#if DEBUG_MODE_DISPLAY_LOG
            Debug.Log("EthereumController Initialized");
#endif
            string currentNetworkUsed = PlayerPrefs.GetString(OPTION_NETWORK_COOKIE, OPTION_NETWORK_TEST);
            m_isMainNetwork = (currentNetworkUsed == OPTION_NETWORK_MAIN);
            if ((_list != null) && (_list.Length > 0))
            {
                m_isMainNetwork = (_list[0] == OPTION_NETWORK_MAIN);
            }
            EthereumEventController.Instance.EthereumEvent += new EthereumEventHandler(OnEthereumEvent);
            BasicSystemEventController.Instance.BasicSystemEvent += new BasicSystemEventHandler(OnBasicSystemEvent);

            m_currentCurrency = PlayerPrefs.GetString(CodeNetwork + ETHEREUM_DEFAULT_CURRENCY, CODE_DOLLAR);

#if ENABLE_ETHEREUM
            m_contractIDPayment = PlayerPrefs.GetString(CodeNetwork + ETHEREUM_CONTRACT_ID_PAYMENT, "");
            m_contractIDSignedDocument = PlayerPrefs.GetString(CodeNetwork + ETHEREUM_CONTRACT_ID_SIGNED_DATA, "");
#endif

            CommsHTTPConstants.GetEthereumExchangeRatesTable();
        }

        // -------------------------------------------
        /* 
		 * Destroy
		 */
        public void Destroy()
        {
            EthereumEventController.Instance.EthereumEvent -= OnEthereumEvent;
            BasicSystemEventController.Instance.BasicSystemEvent -= OnBasicSystemEvent;
            Destroy(_instance.gameObject);
            _instance = null;
        }

        // -------------------------------------------
        /* 
		* RestoreCurrentPrivateKey
		*/
        public void RestoreCurrentPrivateKey()
        {
            if ((m_currentPrivateKey.Length == 0) && (m_backupCurrentPrivateKey.Length > 0))
            {
                CurrentPrivateKey = m_backupCurrentPrivateKey;
            }
        }

        // -------------------------------------------
        /* 
		* GetCurrentExchange
		*/
        public decimal GetCurrentExchange()
        {
            bool found = false;
            for (int i = 0; i < CURRENCY_CODE.Length; i++)
            {
                if (m_currentCurrency == CURRENCY_CODE[i])
                {
                    found = true;
                }
            }
            if (!found)
            {
                m_currentCurrency = CODE_DOLLAR;
            }
            return m_currenciesExchange[m_currentCurrency];
        }

        // -------------------------------------------
        /* 
		* ClearListDataAddresses
		*/
        public void ClearListDataAddresses()
        {
            m_addressesList.Clear();
        }

        // -------------------------------------------
        /* 
		* LoadDataAddresses
		*/
        public void LoadDataAddresses()
        {
            string encryptedAddresses = PlayerPrefs.GetString(CodeNetwork + ETHEREUM_ADDRESSES_LIST, "");
            if ((encryptedAddresses == null) || (encryptedAddresses == "")) return;

            string dataAddresses = RJEncryptor.DecryptStringWithKey(encryptedAddresses, ENCRYPTION_KEY);
            m_addressesList.Clear();

            string[] arrayAddresses = dataAddresses.Split(SEPARATOR_ITEMS);
            for (int i = 0; i < arrayAddresses.Length; i++)
            {
                string[] addressWithLabel = arrayAddresses[i].Split(SEPARATOR_COMA);
                if (addressWithLabel.Length == 2)
                {
                    string address = addressWithLabel[0];
                    string label = addressWithLabel[1];
                    m_addressesList.Add(address, label);
#if DEBUG_MODE_DISPLAY_LOG
                    Debug.Log("address[" + address + "]::label[" + label + "]");
#endif
                }
            }
        }

        // -------------------------------------------
        /* 
		* SaveAddresses
		*/
        public void SaveAddresses(string _publicKeyAddress, string _labelAddress)
        {
            LoadDataAddresses();

            m_addressesList.Remove(_publicKeyAddress);
            m_addressesList.Add(_publicKeyAddress, _labelAddress);

            SaveAddressesStorage();
        }

        // -------------------------------------------
        /* 
		* SaveAddresses
		*/
        private void SaveAddressesStorage()
        {
            string dataAddresses = "";
            foreach (KeyValuePair<string, string> publicAddress in m_addressesList)
            {
                if (dataAddresses.Length > 0)
                {
                    dataAddresses += SEPARATOR_ITEMS;
                }
                dataAddresses += publicAddress.Key + SEPARATOR_COMA + publicAddress.Value;
            }
            PlayerPrefs.SetString(CodeNetwork + ETHEREUM_ADDRESSES_LIST, RJEncryptor.EncryptStringWithKey(dataAddresses, ENCRYPTION_KEY));
        }

        // -------------------------------------------
        /* 
		* ContainsAddress
		*/
        public bool ContainsAddress(string _publicKeyAddress)
        {
            LoadDataAddresses();
            return m_addressesList.Keys.Contains(_publicKeyAddress);
        }

        // -------------------------------------------
        /* 
		* GetListDataAddresses
		*/
        public List<ItemMultiObjectEntry> GetListDataAddresses(bool _excludeOwnerAccounts, params string[] _excludeAddresses)
        {
            LoadDataAddresses();
            List<ItemMultiObjectEntry> output = new List<ItemMultiObjectEntry>();
            foreach (KeyValuePair<string, string> address in m_addressesList)
            {
                bool shouldBeIncluded = true;
                for (int i = 0; i < _excludeAddresses.Length; i++)
                {
                    if (_excludeAddresses[i] == address.Key)
                    {
                        shouldBeIncluded = false;
                    }
                }
                if (_excludeOwnerAccounts)
                {
                    if (ContainsPublicKey(address.Key))
                    {
                        shouldBeIncluded = false;
                    }
                }
                if (shouldBeIncluded)
                {
                    output.Add(new ItemMultiObjectEntry(address.Key, address.Value));
                }
            }
            return output;
        }

        // -------------------------------------------
        /* 
		* AddressToLabel
		*/
        public string AddressToLabel(params string[] _publicKeyAddress)
        {
            LoadDataAddresses();
            string labelAddresses = "";
            string originalAddress = "";
            for (int i = 0; i < _publicKeyAddress.Length; i++)
            {
                if (originalAddress.Length > 0)
                {
                    originalAddress += ":";
                }
                originalAddress += _publicKeyAddress[i];
                if (m_addressesList.Keys.Contains(_publicKeyAddress[i]))
                {
                    if (labelAddresses.Length > 0)
                    {
                        labelAddresses += ":";
                    }
                    string labelAddress = "";
                    if (m_addressesList.TryGetValue(_publicKeyAddress[i], out labelAddress))
                    {
                        labelAddresses += labelAddress;
                    }
                }
            }
            if (labelAddresses.Length > 0)
            {
                return labelAddresses;
            }
            else
            {
                return originalAddress;
            }
        }

        // -------------------------------------------
        /* 
		* AddressToLabel
		*/
        public string AddressToLabelUpperCase(params string[] _publicKeyAddress)
        {
            LoadDataAddresses();
            string labelAddresses = "";
            string originalAddress = "";
            for (int i = 0; i < _publicKeyAddress.Length; i++)
            {
                if (originalAddress.Length > 0)
                {
                    originalAddress += ":";
                }
                originalAddress += _publicKeyAddress[i];
                bool isFound = false;
                string valueFound = "";
                for (int j = 0; j < m_addressesList.Keys.Count; j++)
                {
                    if (m_addressesList.Keys.ElementAt(j).ToUpper().Equals(_publicKeyAddress[i].ToUpper()))
                    {
                        isFound = true;
                        valueFound = m_addressesList.Keys.ElementAt(j);
                        break;
                    }
                }
                if (isFound)
                {
                    if (labelAddresses.Length > 0)
                    {
                        labelAddresses += ":";
                    }
                    string labelAddress = "";
                    if (m_addressesList.TryGetValue(valueFound, out labelAddress))
                    {
                        labelAddresses += labelAddress;
                    }
                }
            }
            if (labelAddresses.Length > 0)
            {
                return labelAddresses;
            }
            else
            {
                return originalAddress;
            }
        }

        // -------------------------------------------
        /* 
		* LoadPrivateKeys
		*/
        public void LoadPrivateKeys(bool _getBalance)
        {
            string currentPrivateKeySelected = PlayerPrefs.GetString(CodeNetwork + ETHEREUM_PRIVATE_KEY_SELECTED, "");
            if (currentPrivateKeySelected.Length > 0)
            {
                m_currentPrivateKey = RJEncryptor.DecryptStringWithKey(currentPrivateKeySelected, ENCRYPTION_KEY);
                m_currentPublicKey = GetPublicKey(m_currentPrivateKey);
            }

            string encryptedKeys = PlayerPrefs.GetString(CodeNetwork + ETHEREUM_PRIVATE_KEYS, "");
            if ((encryptedKeys == null) || (encryptedKeys == "")) return;

            string dataKeys = RJEncryptor.DecryptStringWithKey(encryptedKeys, ENCRYPTION_KEY);
            m_privateKeys.Clear();

            string[] arrayKeys = dataKeys.Split(SEPARATOR_ITEMS);
            for (int i = 0; i < arrayKeys.Length; i++)
            {
                string[] keyWithBalance = arrayKeys[i].Split(SEPARATOR_COMA);
                if (keyWithBalance.Length == 2)
                {
                    string key = keyWithBalance[0];
                    decimal balance = decimal.Parse(keyWithBalance[1]);
                    m_privateKeys.Add(key, balance);
                    m_publicKeys.Add(key, GetPublicKey(key));
#if DEBUG_MODE_DISPLAY_LOG
                    Debug.Log("key[" + key + "]::BALANCE[" + balance + "]");
#endif
                }
            }

            if (_getBalance)
            {
                RequestBalancePrivateKeys(true, EVENT_ETHEREUMCONTROLLER_BALANCE_WALLET);
            }
        }

        // -------------------------------------------
        /* 
        * RequestBalancePrivateKeys
        */
        private void RequestBalancePrivateKeys(bool _accessNetwork, string _nameEvent)
        {
            // REQUEST TO REFRESH THE BALANCE
            foreach (KeyValuePair<string, decimal> privateKey in m_privateKeys)
            {
                GetBalancePrivateKey(_accessNetwork, privateKey.Key, _nameEvent);
            }
        }

        // -------------------------------------------
        /* 
        * RefreshBalancePrivateKeys
        */
        public void RefreshBalancePrivateKeys()
        {
            Dictionary<string, decimal> newDataPrivateKeys = new Dictionary<string, decimal>();
            m_publicKeys.Clear();
            foreach (KeyValuePair<string, decimal> privateKey in m_privateKeys)
            {
                newDataPrivateKeys.Add(privateKey.Key, -1);
                m_publicKeys.Add(privateKey.Key, GetPublicKey(privateKey.Key));
            }
            m_privateKeys.Clear();
            m_privateKeys = newDataPrivateKeys;

            RequestBalancePrivateKeys(true, EVENT_ETHEREUMCONTROLLER_BALANCE_WALLET);
        }

        // -------------------------------------------
        /* 
		* SavePrivateKeys
		*/
        public void SavePrivateKeys()
        {
            string dataKeys = "";
            foreach (KeyValuePair<string, decimal> privateKey in m_privateKeys)
            {
                if (dataKeys.Length > 0)
                {
                    dataKeys += SEPARATOR_ITEMS;
                }
                dataKeys += privateKey.Key + SEPARATOR_COMA + privateKey.Value;
            }
            PlayerPrefs.SetString(CodeNetwork + ETHEREUM_PRIVATE_KEYS, RJEncryptor.EncryptStringWithKey(dataKeys, ENCRYPTION_KEY));
        }

        // -------------------------------------------
        /* 
		* AddPrivateKey
		*/
        public void AddPrivateKey(string _privateKey, string _nameEvent)
        {
            if (!m_privateKeys.Keys.Contains(_privateKey))
            {
                m_privateKeys.Add(_privateKey, -1);
                m_publicKeys.Add(_privateKey, GetPublicKey(_privateKey));
            }

            if (_nameEvent.Length > 0) GetBalancePrivateKey(true, _privateKey, _nameEvent);
        }

        // -------------------------------------------
        /* 
		* ContainsPrivateKey
		*/
        public bool ContainsPrivateKey(string _privateKey)
        {
            return m_privateKeys.Keys.Contains(_privateKey);
        }

        // -------------------------------------------
        /* 
		* ContainsPublicKeysKey
		*/
        public bool ContainsPublicKey(string _publicKey)
        {
            return m_publicKeys.Values.Contains(_publicKey);
        }

        // -------------------------------------------
        /* 
		* RemovePrivateKey
		*/
        public bool RemovePrivateKey(string _privateKey)
        {
            string finalKey = _privateKey;
            if (m_privateKeys.Remove(_privateKey))
            {
                if (m_addressesList.Remove(m_publicKeys[_privateKey]))
                {
                    SaveAddressesStorage();
                }
                if (m_publicKeys.Remove(_privateKey))
                {
                    SavePrivateKeys();
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        // -------------------------------------------
        /* 
		* GetListPrivateKeys
		*/
        public List<ItemMultiObjectEntry> GetListPrivateKeys(params string[] _excludePrivateKeys)
        {
            SortedDictionary<string, ItemMultiObjectEntry> orderedKeys = new SortedDictionary<string, ItemMultiObjectEntry>();
            foreach (KeyValuePair<string, decimal> privateKey in m_privateKeys)
            {
                bool includeKey = true;
                if ((_excludePrivateKeys != null) && (_excludePrivateKeys.Length > 0))
                {
                    for (int i = 0; i < _excludePrivateKeys.Length; i++)
                    {
                        if (_excludePrivateKeys[i] == privateKey.Key)
                        {
                            includeKey = false;
                        }
                    }
                }
                if (includeKey)
                {
                    orderedKeys.Add(AddressToLabel(m_publicKeys[privateKey.Key]), new ItemMultiObjectEntry(privateKey.Key, privateKey.Value));
                }
            }

            List<ItemMultiObjectEntry> output = new List<ItemMultiObjectEntry>();
            foreach (KeyValuePair<string, ItemMultiObjectEntry> itemPair in orderedKeys)
            {
                output.Add(itemPair.Value);
            }

            return output;
        }

        // -------------------------------------------
        /* 
		 * ValidatePrivateKey
		 */
        public bool ValidatePrivateKey(string _privateKey, string _nameEvent)
        {
#if ENABLE_ETHEREUM
            if (_privateKey == null) return false;
            if (_privateKey == "") return false;
            if (_privateKey == "null") return false;

            try
            {
                EthECKey privateKey = new EthECKey(_privateKey);
#if DEBUG_MODE_DISPLAY_LOG
                Debug.Log("ValidatePrivateKey::privateKey.GetPublicAddress()=" + privateKey.GetPublicAddress());
#endif
                GetBalancePrivateKey(false, _privateKey, _nameEvent);
                return true;
            }
            catch (Exception err)
            {
#if DEBUG_MODE_DISPLAY_LOG
                Debug.Log("ValidatePrivateKey::ERROR[" + err.Message + "]==========" + err.StackTrace);
#endif
            }
            return false;
#else
            return false;
#endif
        }

        // -------------------------------------------
        /* 
		 * ValidatePublicKey
		 */
        public void ValidatePublicKey(string _publicKey)
        {
            try
            {
                GetBalancePublicKey(false, _publicKey, EVENT_ETHEREUMCONTROLLER_VALIDATE_PUBLIC_KEY);
            } catch (Exception err)
            {
#if DEBUG_MODE_DISPLAY_LOG
                Debug.Log("ValidatePublicKey::ERROR[" + err.Message + "]==========" + err.StackTrace);
#endif
            }
        }

        // -------------------------------------------
        /* 
		* GetPublicKey
		*/
        public string GetPublicKey(string _privateKey)
        {
#if ENABLE_ETHEREUM
            try
            {
                EthECKey privateKey = new EthECKey(_privateKey);
#if DEBUG_MODE_DISPLAY_LOG
                Debug.Log("ValidatePrivateKey::privateKey.GetPublicAddress()=" + privateKey.GetPublicAddress());
#endif
                return privateKey.GetPublicAddress();
            }
            catch (Exception err)
            {
#if DEBUG_MODE_DISPLAY_LOG
                Debug.Log("ValidatePrivateKey::ERROR[" + err.Message + "]======" + err.StackTrace);
#endif
            }
            return "";
#else
            return "";
#endif
        }

#if ENABLE_ETHEREUM
        // -------------------------------------------
        /* 
		* ToWei
		*/
        public static BigInteger ToWei(decimal _ether)
        {
            decimal etherWei = _ether * ETHER_WAI_FACTOR;
            return (BigInteger)etherWei;
        }

        // -------------------------------------------
        /* 
		* FromWei
		*/
        public static decimal FromWei(BigInteger _wai)
        {
            decimal ether = (decimal)_wai / ETHER_WAI_FACTOR;
            return ether;
        }
#endif

        // -------------------------------------------
        /* 
		* GetBalancePrivateKey
		*/
        public void GetBalancePrivateKey(bool _accessNetwork, string _privateKey, string _nameEvent)
        {
            if (_accessNetwork || (!m_privateKeys.Keys.Contains(_privateKey)))
            {
                StartCoroutine(ThreadGetBalancePrivateKey(NetworkAPI, _privateKey, _nameEvent));
            }
            else
            {
                if (m_privateKeys.Keys.Contains(_privateKey))
                {
                    decimal balance = m_privateKeys[_privateKey];
                    if (_nameEvent.Length > 0) EthereumEventController.Instance.DelayEthereumEvent(_nameEvent, 0.1f, _privateKey, true, balance);
                }
                else
                {
                    if (_nameEvent.Length > 0) EthereumEventController.Instance.DelayEthereumEvent(_nameEvent, 0.1f, _privateKey, false, "No private key found");
                }
            }
        }

        // -------------------------------------------------------------------------
        /* 
        * ThreadCheckBalance
        */
        public System.Collections.IEnumerator ThreadGetBalancePrivateKey(string _baseURL, string _privateKey, string _nameEvent)
        {
#if ENABLE_ETHEREUM
            EthECKey privateKey = new EthECKey(_privateKey);
            string publicAddress = privateKey.GetPublicAddress();
            var balanceRequest = new EthGetBalanceUnityRequest(_baseURL);
            yield return balanceRequest.SendRequest(publicAddress, BlockParameter.CreateLatest());
            if (balanceRequest.Exception == null)
            {
                decimal balance = EthereumController.FromWei(balanceRequest.Result.Value);
                if (m_privateKeys.Keys.Contains(_privateKey))
                {
                    m_privateKeys[_privateKey] = balance;
                }                        
                if (_nameEvent.Length > 0) EthereumEventController.Instance.DispatchEthereumEvent(_nameEvent, _privateKey, true, balance);
#if DEBUG_MODE_DISPLAY_LOG
                Utilities.DebugLogError("+++++++++++++++ThreadCheckBalance::_nameEvent["+ _nameEvent + "]::Balance: " + balanceRequest.Result.Value.ToString());
#endif
            }
            else
            {
                if (_nameEvent.Length > 0) EthereumEventController.Instance.DispatchEthereumEvent(_nameEvent, _privateKey, false, balanceRequest.Exception.ToString());
#if DEBUG_MODE_DISPLAY_LOG
                Debug.LogError("+++++++++++++++ThreadCheckBalance::blockNumberRequest.Exception: " + balanceRequest.Exception.ToString());
#endif
            }
#else
            yield return new WaitForSeconds(1);
#endif
        }

        // -------------------------------------------
        /* 
		* GetBalancePrivateKey
		*/
        public void GetBalancePublicKey(bool _accessNetwork, string _publicKey, string _nameEvent)
        {
            if (_accessNetwork || (!m_publicKeysCollected.Keys.Contains(_publicKey)))
            {
                StartCoroutine(ThreadGetBalancePublicKey(NetworkAPI, _publicKey, _nameEvent));
            }
            else
            {
                if (m_publicKeysCollected.Keys.Contains(_publicKey))
                {
                    decimal balance = m_publicKeysCollected[_publicKey];
                    if (_nameEvent.Length > 0) EthereumEventController.Instance.DelayEthereumEvent(_nameEvent, 0.1f, _publicKey, true, balance);
                }
                else
                {
                    if (_nameEvent.Length > 0) EthereumEventController.Instance.DelayEthereumEvent(_nameEvent, 0.1f, _publicKey, false, "No public key found");
                }
            }
        }


        // -------------------------------------------------------------------------
        /* 
        * ThreadGetBalancePublicKey
        */
        public System.Collections.IEnumerator ThreadGetBalancePublicKey(string _baseURL, string _publicKey, string _nameEvent)
        {
#if ENABLE_ETHEREUM
            var balanceRequest = new EthGetBalanceUnityRequest(_baseURL);
            yield return balanceRequest.SendRequest(_publicKey, BlockParameter.CreateLatest());
            if (balanceRequest.Exception == null)
            {
                decimal balance = EthereumController.FromWei(balanceRequest.Result.Value);
                if (m_publicKeysCollected.Keys.Contains(_publicKey))
                {
                    m_publicKeysCollected[_publicKey] = balance;
                }
                if (_nameEvent.Length > 0) EthereumEventController.Instance.DispatchEthereumEvent(_nameEvent, _publicKey, true, balance);
#if DEBUG_MODE_DISPLAY_LOG
                Utilities.DebugLogError("ThreadCheckBalance::Balance: " + balanceRequest.Result.Value.ToString());
#endif
            }
            else
            {
                if (_nameEvent.Length > 0) EthereumEventController.Instance.DispatchEthereumEvent(_nameEvent, _publicKey, false, balanceRequest.Exception.ToString());
#if DEBUG_MODE_DISPLAY_LOG
                Debug.LogError("ThreadCheckBalance::blockNumberRequest.Exception: " + balanceRequest.Exception.ToString());
#endif
            }
#else
            yield return new WaitForSeconds(1);
#endif
        }

        // -------------------------------------------
        /* 
		* GetAllInformation
		*/
        public void GetAllInformation(string _publicKeyAdress)
		{
#if DEBUG_MODE_DISPLAY_LOG
			Debug.Log("GetBalance::TRYING TO GET HISTORY FOR ["+ _publicKeyAdress + "]");
#endif

            m_publicAdressCheckHistory = _publicKeyAdress;
            m_publicAdressCheckHistory = m_publicAdressCheckHistory.ToUpper();
            CommsHTTPConstants.GetEthereumRequestTransactionHistory(EtherscanAPI, ETHERSCAN_API_KEY, _publicKeyAdress);
        }

        // -------------------------------------------
        /* 
        * GetSummaryAccount
        */
        private void GetSummaryAccount(JSONNode _history)
        {
            m_allTransactionsHistory = new List<ItemMultiObjectEntry>();
            m_inTransactionsHistory = new List<ItemMultiObjectEntry>();
            m_outTransactionsHistory = new List<ItemMultiObjectEntry>();
            
            for (int i = 0; i < _history.Count; i++)
            {
                JSONNode tx = _history[i];

                string transactionID = tx["hash"];
                decimal transactionAmount = decimal.Parse(tx["value"]);
                string addressFrom = tx["from"];
                string addressTo = tx["to"];
                string gas = tx["gas"];
                DateTime transactionDate = DateConverter.TimeStampToDateTime(double.Parse(tx["timeStamp"]));
                string transactionMessage = "";
                List<ItemMultiTextEntry> transactionsAddresses = new List<ItemMultiTextEntry>();
                if (m_publicAdressCheckHistory.ToUpper().Equals(addressFrom.ToUpper()))
                {
                    transactionAmount = -transactionAmount;
                    if (addressTo != null)
                    {
                        transactionsAddresses.Add(new ItemMultiTextEntry(transactionAmount.ToString(), addressTo));
                    }                    
                }
                else
                {
                    if (addressFrom != null)
                    {
                        transactionsAddresses.Add(new ItemMultiTextEntry(transactionAmount.ToString(), addressFrom));
                    }
                }

                if (transactionAmount != 0)
                {
                    if (transactionAmount > 0)
                    {
                        m_inTransactionsHistory.Insert(0, new ItemMultiObjectEntry(transactionID, transactionDate, transactionAmount, gas, transactionMessage, transactionsAddresses));
                    }
                    else
                    {
                        if (transactionAmount < 0)
                        {
                            m_outTransactionsHistory.Insert(0, new ItemMultiObjectEntry(transactionID, transactionDate, transactionAmount, gas, transactionMessage, transactionsAddresses));
                        }
                    }
                    m_allTransactionsHistory.Insert(0, new ItemMultiObjectEntry(transactionID, transactionDate, transactionAmount, gas, transactionMessage, transactionsAddresses));
                }
            }
            EthereumEventController.Instance.DispatchEthereumEvent(EVENT_ETHEREUMCONTROLLER_TRANSACTION_HISTORY);
        }

        // -------------------------------------------
        /* 
        * ToStringTransaction
        */
        public static string ToStringTransaction(ItemMultiObjectEntry _transaction)
		{
			string transactionID = (string)_transaction.Objects[0];
			DateTime transactionDate = (DateTime)_transaction.Objects[1];
			decimal transactionAmount = (decimal)_transaction.Objects[2];
            string transactionGas = (string)_transaction.Objects[3];
			string transactionMessage = (string)_transaction.Objects[4];
			List<ItemMultiTextEntry> transactionScriptPubKey = (List<ItemMultiTextEntry>)_transaction.Objects[5];

			string addresses = "";
			for (int i = 0; i < transactionScriptPubKey.Count; i++)
			{
				ItemMultiTextEntry item = transactionScriptPubKey[i];
				if (addresses.Length > 0)
				{
					addresses += "::";
				}				

				addresses += item.Items[1];
			}

			return "DATE["+transactionDate.ToString() + "];AMOUNT["+ transactionAmount + "];GAS["+ transactionGas + "];MESSAGE["+ transactionMessage + "];ADDRESSES["+ addresses + "]";
		}


		// -------------------------------------------
		/* 
		* SignTextData
		*/
		public void SignTextData(string _url, string _data, string _currentPrivateKey, int _sizeData)
		{
            m_resultSigningProccess = new object[_sizeData];
            StartCoroutine(RunSignDataDocument(_url, _data, _currentPrivateKey, GetPublicKey(_currentPrivateKey)));
		}

        private string m_privateKeySigningData;
        private string m_publicKeySigningData;
        private string m_urlSigningData;
        private string m_dataSigningData;
#if ENABLE_ETHEREUM
        private BigInteger m_nonceLastTransaction;
#endif

        // -------------------------------------------
        /* 
		* RunSignDataDocument
		*/
        System.Collections.IEnumerator RunSignDataDocument(string _url, string _data, string _privateKey, string _publicKey)
        {
#if ENABLE_ETHEREUM
            // DEPLOY THE CONTRACT AND TRUE INDICATES WE WANT TO ESTIMATE THE GAS
            UIEventController.Instance.DispatchUIEvent(ScreenInformationView.EVENT_SCREEN_UPDATE_TEXT_DESCRIPTION, LanguageController.Instance.GetText("screen.ethereum.send.deploy.contract"));
            var transactionRequest = new TransactionSignedUnityRequest(NetworkAPI, _privateKey);
            yield return transactionRequest.SignAndSendDeploymentContractTransaction(new ArrayUint256DynamicDeployment("608060405234801561001057600080fd5b506103a1806100206000396000f3006080604052600436106100615763ffffffff7c010000000000000000000000000000000000000000000000000000000060003504166341c5812081146100665780636b7a0be6146100835780636fac8ca9146100d0578063b2367047146100fd575b600080fd5b34801561007257600080fd5b50610081600435602435610112565b005b34801561008f57600080fd5b5061009b60043561023d565b6040805173ffffffffffffffffffffffffffffffffffffffff9094168452602084019290925282820152519081900360600190f35b3480156100dc57600080fd5b506100eb600435602435610285565b60408051918252519081900360200190f35b34801561010957600080fd5b506100eb6102de565b600061011c61033d565b610125846102e4565b91508160001914156102115750604080516060810182523381526020810185815291810184815260018054808201825560009190915282517fb10e2d527612073b26eecdfd717e6a320cf44b4afac2b0732d9fcbe2b7fa0cf66003909202918201805473ffffffffffffffffffffffffffffffffffffffff191673ffffffffffffffffffffffffffffffffffffffff90921691909117905592517fb10e2d527612073b26eecdfd717e6a320cf44b4afac2b0732d9fcbe2b7fa0cf7840155517fb10e2d527612073b26eecdfd717e6a320cf44b4afac2b0732d9fcbe2b7fa0cf890920191909155610237565b8260018381548110151561022157fe5b9060005260206000209060030201600201819055505b50505050565b600180548290811061024b57fe5b600091825260209091206003909102018054600182015460029092015473ffffffffffffffffffffffffffffffffffffffff909116925083565b6000806000610293856102e4565b915060001982146102d05760018054839081106102ac57fe5b9060005260206000209060030201905083816002015414156102d0578192506102d6565b60001992505b505092915050565b60015490565b600080805b60015482101561033057600180548390811061030157fe5b90600052602060002090600302019050838160010154141561032557819250610336565b6001909101906102e9565b60001992505b5050919050565b606060405190810160405280600073ffffffffffffffffffffffffffffffffffffffff168152602001600081526020016000815250905600a165627a7a72305820623e5a4475bf3a5f24ef50c1069bb311dbeed33060ec4d85484668dbfe8c53e10029"));

            if (transactionRequest.Exception != null)
            {
                Debug.LogError("---------------ERROR DEPLOYING THE CONTRACT");
                Debug.Log(transactionRequest.Exception.Message);
                EthereumEventController.Instance.DispatchEthereumEvent(EVENT_ETHEREUMCONTROLLER_SIGNED_DOCUMENT_DONE, false, transactionRequest.Exception.Message);
                yield break;
            }

            var transactionHash = transactionRequest.Result;
#if DEBUG_MODE_DISPLAY_LOG
            Utilities.DebugLogError("+++++++++++++++transactionHash=" + transactionHash);
#endif

            // GET LASTEST TRANSACTION NUMBER TO DEFINE A NONCE THE IS GREATER THAN THE PREVIOUS ONE PERFORMED BY THE ACCOUNT
            UIEventController.Instance.DispatchUIEvent(ScreenInformationView.EVENT_SCREEN_UPDATE_TEXT_DESCRIPTION, LanguageController.Instance.GetText("screen.ethereum.send.calculate.nonce"));
            BigInteger nonceLastTransaction = new BigInteger(1000);
            var counterTransactions = new EthGetTransactionByHashUnityRequest(NetworkAPI);
            yield return counterTransactions.SendRequest(transactionHash);
            if (counterTransactions.Exception == null)
            {
                if (counterTransactions.Result != null)
                {
                    nonceLastTransaction = counterTransactions.Result.Nonce.Value;
                    nonceLastTransaction += new BigInteger(1);
#if DEBUG_MODE_DISPLAY_LOG
                    Utilities.DebugLogError("+++++++++++++++NEXT nonceLastTransaction=" + nonceLastTransaction);
#endif
                }
            }
            else
            {
                Debug.LogError("---------------ERROR RETRIEVING THE LAST TRANSACTION COUNT NUMBER FOR THE NONCE");
                EthereumEventController.Instance.DispatchEthereumEvent(EVENT_ETHEREUMCONTROLLER_SIGNED_DOCUMENT_DONE, false, counterTransactions.Exception.Message);
                yield break;
            }

            // REQUEST FOR THE CONTRACT ADDRESS MINED
            if (m_contractIDSignedDocument.Length == 0)
            {
                m_stepMining = 0;
                UIEventController.Instance.DispatchUIEvent(ScreenInformationView.EVENT_SCREEN_UPDATE_TEXT_DESCRIPTION, LanguageController.Instance.GetText("screen.ethereum.send.mining.transaction", m_stepMining));
                EthereumEventController.Instance.DelayEthereumEvent(ScreenInformationView.EVENT_SCREEN_UPDATE_TEXT_DESCRIPTION, 2);
                var transactionReceiptPolling = new TransactionReceiptPollingRequest(NetworkAPI);
                yield return transactionReceiptPolling.PollForReceipt(transactionHash, 2);
                if (transactionReceiptPolling.Exception == null)
                {
                    m_stepMining = -1;
                    m_contractIDSignedDocument = transactionReceiptPolling.Result.ContractAddress;
#if DEBUG_MODE_DISPLAY_LOG
                    Utilities.DebugLogError("+++++++++++++++ContractAddress=" + m_contractIDSignedDocument);
#endif
                }
                else
                {
                    Debug.LogError("---------------ERROR CREATING THE CONTRACT ADDRESS");
                    EthereumEventController.Instance.DispatchEthereumEvent(EVENT_ETHEREUMCONTROLLER_SIGNED_DOCUMENT_DONE, false, transactionReceiptPolling.Exception.Message);
                    yield break;
                }

                PlayerPrefs.SetString(CodeNetwork + ETHEREUM_CONTRACT_ID_SIGNED_DATA, m_contractIDSignedDocument);
            }
            else
            {
                Utilities.DebugLogError("++++++++++++USING ALREADY CREATED CONTRACT NUMBER=" + m_contractIDSignedDocument);
            }

            UIEventController.Instance.DispatchUIEvent(ScreenInformationView.EVENT_SCREEN_UPDATE_TEXT_DESCRIPTION, LanguageController.Instance.GetText("screen.ethereum.send.sign.send.transaction"));

            // CREATE THE CONTRACT
            if (m_signDocumentContractService == null)
            {
                m_signDocumentContractService = new SignDocumentContractService(m_contractIDSignedDocument);
            }

            // CREATE THE TRANSACTION INPUT
            m_privateKeySigningData = _privateKey;
            m_publicKeySigningData = _publicKey;
            m_urlSigningData = _url;
            m_dataSigningData = _data;
            m_nonceLastTransaction = nonceLastTransaction;

            m_runningSigningProcess = true;

            CommsHTTPConstants.GetEthereumRequestGasPrice(EtherscanAPI, ETHERSCAN_API_KEY);
#else
            yield return new WaitForSeconds(1);
#endif
        }

        // -------------------------------------------
        /* 
		* RunSignDataDocumentEnd
		*/
        System.Collections.IEnumerator RunSignDataDocumentEnd(
#if ENABLE_ETHEREUM
                BigInteger _currentGasTransaction
#endif
            )
        {
#if ENABLE_ETHEREUM

#if DEBUG_MODE_DISPLAY_LOG
            Utilities.DebugLogError("++++++++++++++Signing gas estimation: " + _currentGasTransaction.ToString());
#endif

            var transactionInput = m_signDocumentContractService.CreateSignedDocumentTransactionInput(m_publicKeySigningData, m_urlSigningData, m_dataSigningData, new HexBigInteger(_currentGasTransaction));
            transactionInput.Nonce = new HexBigInteger(m_nonceLastTransaction);

            // SEND AND WAIT
            var transactionSignedDocumentRequest = new TransactionSignedUnityRequest(NetworkAPI, m_privateKeySigningData);
            yield return transactionSignedDocumentRequest.SignAndSendTransaction(transactionInput);
            if (transactionSignedDocumentRequest.Exception == null)
            {
                //get transaction receipt
#if DEBUG_MODE_DISPLAY_LOG
                Utilities.DebugLogError("++++++++++++Signed submitted contract: " + m_contractIDSignedDocument);
                Utilities.DebugLogError("++++++++++++Signed submitted tx: " + transactionSignedDocumentRequest.Result);
#endif

                EthereumEventController.Instance.DispatchEthereumEvent(EVENT_ETHEREUMCONTROLLER_SIGNED_DOCUMENT_DONE, true, m_contractIDSignedDocument, transactionSignedDocumentRequest.Result);
            }
            else
            {
#if DEBUG_MODE_DISPLAY_LOG
                Utilities.DebugLogError("-------------Signed Error: " + transactionSignedDocumentRequest.Exception.Message);
#endif
                EthereumEventController.Instance.DispatchEthereumEvent(EVENT_ETHEREUMCONTROLLER_SIGNED_DOCUMENT_DONE, false, transactionSignedDocumentRequest.Exception.Message);
            }
#else
            yield return new WaitForSeconds(1);
#endif
        }

        // -------------------------------------------
        /* 
		* ThreadRunCountItemsSigned
		*/
        System.Collections.IEnumerator RunCountItemsSigned(string _contractID)
        {
#if ENABLE_ETHEREUM
            SignDocumentContractService countDocumentContractService = new SignDocumentContractService(_contractID);

#if DEBUG_MODE_DISPLAY_LOG
            Utilities.DebugLogError("++++++++++++Contract ID to use for verification: " + _contractID);
#endif

            var requestCountSignedItems = new EthCallUnityRequest(NetworkAPI);

            var countSignedItemsCallInput = countDocumentContractService.CreateGetCountSignedDocuments();
            yield return requestCountSignedItems.SendRequest(countSignedItemsCallInput, Nethereum.RPC.Eth.DTOs.BlockParameter.CreateLatest());

            if (requestCountSignedItems.Exception == null)
            {
                int totalNumberSignedItems = countDocumentContractService.DecodeCountSignedDocuments(requestCountSignedItems.Result);

#if DEBUG_MODE_DISPLAY_LOG
                Utilities.DebugLogError("++++++++++++totalNumberSignedItems: " + totalNumberSignedItems);
#endif
            }
            else
            {
                Debug.LogError("-------------------ERROR CHECKING THE NUMBER OF SIGNED ITEMS: " + requestCountSignedItems.Exception.Message);
            }
#else
            yield return new WaitForSeconds(1);
#endif
        }

        // -------------------------------------------
        /* 
		* VerifySignedData
		*/
        public void VerifySignedData(string _contractID, string _url, string _data, int _sizeData)
        {
            m_resultSigningProccess = new object[_sizeData];
            StartCoroutine(RunVerifyDataDocument(_contractID, _url, _data));
        }

        // -------------------------------------------
        /* 
		* ThreadRunVerifyDataDocument
		*/
        System.Collections.IEnumerator RunVerifyDataDocument(string _contractID, string _url, string _data)
        {
#if ENABLE_ETHEREUM
            SignDocumentContractService verifyDocumentContractService = new SignDocumentContractService(_contractID);

#if DEBUG_MODE_DISPLAY_LOG
            Utilities.DebugLogError("++++++++++++Contract ID to use for verification: " + _contractID + ":: FOR DATA="+ _data);
            Debug.LogError("++++++++++++Contract ID to use for verification: " + _contractID + ":: FOR DATA=" + _data);
#endif

            var requestVerifyDataSigned = new EthCallUnityRequest(NetworkAPI);
            
            var verifyDataSignedCallInput = verifyDocumentContractService.CreateVerifyDocumentTransactionInput(_url, _data);
            yield return requestVerifyDataSigned.SendRequest(verifyDataSignedCallInput, Nethereum.RPC.Eth.DTOs.BlockParameter.CreateLatest());
            
            if (requestVerifyDataSigned.Exception == null)
            {
                int indexHashInQueue = verifyDocumentContractService.DecodeVerifyDocument(requestVerifyDataSigned.Result);

#if DEBUG_MODE_DISPLAY_LOG
                Utilities.DebugLogError("++++++++++++indexHashInQueue: " + indexHashInQueue);
                Debug.LogError("++++++++++++indexHashInQueue: " + indexHashInQueue);
#endif

                EthereumEventController.Instance.DispatchEthereumEvent(EVENT_ETHEREUMCONTROLLER_VERIFICATION_SIGNED_DATA, (indexHashInQueue != -1));
            }
            else
            {
                EthereumEventController.Instance.DispatchEthereumEvent(EVENT_ETHEREUMCONTROLLER_VERIFICATION_SIGNED_DATA, false);
            }
#else
            yield return new WaitForSeconds(1);
#endif
        }

        // -------------------------------------------
        /* 
		* PayExperience
		*/
        private void ExecuteTransaction(string _title, decimal _amount, string _toAddress, string _privateKey)
		{
#if ENABLE_ETHEREUM

#if DEBUG_MODE_DISPLAY_LOG
            Debug.LogError("*****************************************************************************");
            Debug.LogError("PAYMENT:");
            Debug.LogError("\t\t m_currentPrivateKey="+ m_currentPrivateKey);
            Debug.LogError("\t\t _toAddress=" + _toAddress);
            Debug.LogError("\t\t _amount=" + _amount);
            Debug.LogError("\t\t weiAmount=" + ToWei(_amount));
            Debug.LogError("*****************************************************************************");
#endif

            StartCoroutine(RunTransaction(_title, _toAddress, ToWei(_amount), _privateKey, GetPublicKey(_privateKey)));
#endif
        }

#if ENABLE_ETHEREUM
        // -------------------------------------------
        /* 
		* ThreadRunTransaction
		*/
        System.Collections.IEnumerator RunTransaction(string _title, string _toAddress, BigInteger _weiAmount, string _privateKeyFrom, string _publicKeyFrom)
        {
            // PREPARE TRANSFER
            var ethTransfer = new EthTransferUnityRequest(NetworkAPI, _privateKeyFrom, NetworkChainID);

            UIEventController.Instance.DispatchUIEvent(ScreenInformationView.EVENT_SCREEN_UPDATE_TEXT_DESCRIPTION, LanguageController.Instance.GetText("screen.ethereum.send.deploy.contract"));

            // SEND TRANSFER TO
            yield return ethTransfer.TransferEther(_toAddress, FromWei(_weiAmount));

            if (ethTransfer.Exception != null)
            {
                Debug.Log(ethTransfer.Exception.Message);
#if DEBUG_MODE_DISPLAY_LOG
                Utilities.DebugLogError("-------------Payment Error: " + ethTransfer.Exception.Message);
#endif
                EthereumEventController.Instance.DispatchEthereumEvent(EVENT_ETHEREUMCONTROLLER_TRANSACTION_DONE, false, ethTransfer.Exception.Message);
                yield break;
            }

            var transactionHash = ethTransfer.Result;

#if DEBUG_MODE_DISPLAY_LOG
            Utilities.DebugLogError("++++++++++++++TRANSFER SUCCESS transaction hash: " + transactionHash.ToString());
#endif
            m_stepMining = 0;
            EthereumEventController.Instance.DispatchEthereumEvent(ScreenInformationView.EVENT_SCREEN_UPDATE_TEXT_DESCRIPTION);
            
            // Create a poll to get the receipt when mined
            var transactionReceiptPolling = new TransactionReceiptPollingRequest(NetworkAPI);
            yield return transactionReceiptPolling.PollForReceipt(transactionHash, 2);

            m_stepMining = -1;

#if DEBUG_MODE_DISPLAY_LOG
            Utilities.DebugLogError("++++++++++++++Transaction mined");
#endif

            // RETRIEVED BALANCE
            EthereumEventController.Instance.DispatchEthereumEvent(EVENT_ETHEREUMCONTROLLER_TRANSACTION_DONE, true, transactionHash);
        }
#endif


        // -------------------------------------------
        /* 
		* SignTextData
		*/
        public string SignTextData(string _data, string _currentPrivateKey)
        {
#if ENABLE_ETHEREUM
            MessageSigner signer = new MessageSigner();
            string output = signer.HashAndSign(_data, _currentPrivateKey);

            return output;
#endif
            return "";
        }

        // -------------------------------------------
        /* 
		* VerifySignedData
		*/
        public bool VerifySignedData(string _dataOriginal, string _dataSigned, string _currentPublicKey)
        {
#if ENABLE_ETHEREUM
            MessageSigner signer = new MessageSigner();
            string publicKey = signer.HashAndEcRecover(_dataOriginal, _dataSigned);

            return publicKey == _currentPublicKey;
#endif
            return false;
        }


        // -------------------------------------------
        /* 
        * GetBalanceForCurrency
        */
        public decimal GetBalanceForCurrency(string _currency)
		{
			decimal currencyForBalance = -1;
			m_walletBalanceCurrencies.TryGetValue(_currency, out currencyForBalance);
			return currencyForBalance;
		}

		// -------------------------------------------
		/* 
		* Main call to make the payment
		*/
		public void Pay(string _currentPrivateKey,  string _publicKey, string _title, decimal _finalValueEther)
		{
			m_titleTransaction = _title;
			m_currentPrivateKey = _currentPrivateKey;
			m_publicKeyTarget = _publicKey;
			m_finalValueEther = _finalValueEther;

			// CUSTOMER HAS ENOUGH FUNDS?
			GetBalancePrivateKey(true, m_currentPrivateKey, EVENT_ETHEREUMCONTROLLER_BALANCE_ENOUGH_TO_PAY);
        }
        
        // -------------------------------------------
        /* 
		* Check if the payment can be done with enough balance
		*/
        private void PayWithEnoughBalance(decimal _balanceCurrent)
        {
            if (_balanceCurrent < m_finalValueEther)
            {
                string balanceCurrencyTrimmed = Utilities.Trim((m_currenciesExchange[m_currentCurrency] * _balanceCurrent).ToString());
                balanceCurrencyTrimmed += " " + m_currentCurrency;
                string transactionCurrencyTrimmed = Utilities.Trim((m_currenciesExchange[m_currentCurrency] * m_finalValueEther).ToString());
                transactionCurrencyTrimmed += " " + m_currentCurrency;
                EthereumEventController.Instance.DispatchEthereumEvent(EVENT_ETHEREUMCONTROLLER_TRANSACTION_DONE, false, "There is not enough balance to perform the transaction. Current balance=" + balanceCurrencyTrimmed + " and Transaction requires=" + transactionCurrencyTrimmed);
                return;
            }

            // EXECUTE PAYMENT
            ExecuteTransaction(m_titleTransaction, m_finalValueEther, m_publicKeyTarget, m_currentPrivateKey);
        }

        private object[] m_resultSigningProccess;

        // -------------------------------------------
        /* 
        * Manager of basic system events
        */
        private void OnBasicSystemEvent(string _nameEvent, object[] _list)
        {
            if (_nameEvent == BasicSystemEventController.EVENT_BASICSYSTEMEVENT_REQUEST_SIGN_TEXT_DATA)
            {
                SignTextData((string)_list[0], (string)_list[0], (string)_list[1], _list.Length - 1);
                int counter = 1;
                for (int i = 2; i < _list.Length; i++)
                {
                    m_resultSigningProccess[counter] = _list[i];
                    counter++;
                }
            }
            if (_nameEvent == BasicSystemEventController.EVENT_BASICSYSTEMEVENT_REQUEST_VERIFY_TEXT_DATA)
            {
                VerifySignedData((string)_list[1], (string)_list[0], (string)_list[0], _list.Length - 1);
                int counter = 1;
                for (int i = 3; i < _list.Length; i++)
                {
                    m_resultSigningProccess[counter] = _list[i];
                    counter++;
                }
            }
        }

        // -------------------------------------------
        /* 
        * Manager of Ethereum events
        */
        private void OnEthereumEvent(string _nameEvent, params object[] _list)
		{
            if (_nameEvent == EVENT_ETHEREUMCONTROLLER_BALANCE_WALLET)
            {
                string privateKey = (string)_list[0];
                bool isOk = (bool)_list[1];
                if (isOk)
                {
                    decimal balance = (decimal)_list[2];
                    m_privateKeys[privateKey] = balance;
                }
            }
            if (_nameEvent == EVENT_ETHEREUMCONTROLLER_BALANCE_ENOUGH_TO_PAY)
            {
                string privateKey = (string)_list[0];
                bool isOk = (bool)_list[1];
                if (isOk)
                {
                    decimal balance = (decimal)_list[2];
                    m_privateKeys[privateKey] = balance;
                    PayWithEnoughBalance(balance);
                }
            }
            if (_nameEvent == EVENT_ETHEREUMCONTROLLER_JSON_EXCHANGE_TABLE)
			{
				m_walletBalanceCurrencies.Clear();
				m_currenciesExchange.Clear();
				JSONNode jsonExchangeTable = JSON.Parse((string)_list[0]);
#if DEBUG_MODE_DISPLAY_LOG
				Debug.Log("ETHEREUM IN WALLET[" + m_balanceWallet + "]");
#endif
				for (int i = 0; i < CURRENCY_CODE.Length; i++)
				{
					string currencyCode = CURRENCY_CODE[i];
					decimal exchangeValue = decimal.Parse(jsonExchangeTable[currencyCode]);
					m_walletBalanceCurrencies.Add(currencyCode, m_balanceWallet * exchangeValue);
					m_currenciesExchange.Add(currencyCode, exchangeValue);
#if DEBUG_MODE_DISPLAY_LOG
					Debug.Log("ETHEREUM IN[" + currencyCode + "] IS[" + (m_balanceWallet * exchangeValue) + "]");
#endif
				}
                EthereumEventController.Instance.DelayEthereumEvent(EVENT_ETHEREUMCONTROLLER_ALL_DATA_COLLECTED, 0.2f);
            }
            if (_nameEvent == ScreenInformationView.EVENT_SCREEN_UPDATE_TEXT_DESCRIPTION)
            {
                if (m_stepMining >= 0)
                {
                    m_stepMining++;
                    UIEventController.Instance.DispatchUIEvent(ScreenInformationView.EVENT_SCREEN_UPDATE_TEXT_DESCRIPTION, LanguageController.Instance.GetText("screen.ethereum.send.mining.transaction", m_stepMining));
                    EthereumEventController.Instance.DelayEthereumEvent(ScreenInformationView.EVENT_SCREEN_UPDATE_TEXT_DESCRIPTION, 2);
                }
            }
            if (_nameEvent == EVENT_ETHEREUMCONTROLLER_TRANSACTION_DONE)
			{
				if (!(bool)_list[0])
				{
					EthereumEventController.Instance.DispatchEthereumEvent(EVENT_ETHEREUMCONTROLLER_TRANSACTION_COMPLETED, false, (string)_list[1], m_finalValueEther.ToString());
				}
				else
				{
					EthereumEventController.Instance.DispatchEthereumEvent(EVENT_ETHEREUMCONTROLLER_TRANSACTION_COMPLETED, true, (string)_list[1], m_publicKeyTarget, m_finalValueEther.ToString());

					// UPDATE WALLET
					AddPrivateKey(m_currentPrivateKey, "");
				}
			}
            if (_nameEvent == EVENT_ETHEREUMCONTROLLER_SIGNED_DOCUMENT_DONE)
            {
                if ((bool)_list[0])
                {
                    m_resultSigningProccess[0] = (string)_list[1]; // CONTRACT ID
                    BasicSystemEventController.Instance.DispatchBasicSystemEvent(BasicSystemEventController.EVENT_BASICSYSTEMEVENT_RESPONSE_SIGNED_TEXT_DATA, m_resultSigningProccess);
                    // StartCoroutine(RunCountItemsSigned((string)_list[1]));
                }
            }
            if (_nameEvent == EthereumController.EVENT_ETHEREUMCONTROLLER_VERIFICATION_SIGNED_DATA)
            {
                m_resultSigningProccess[0] = (bool)_list[0]; // VERIFICATION RESULT
                BasicSystemEventController.Instance.DispatchBasicSystemEvent(BasicSystemEventController.EVENT_BASICSYSTEMEVENT_RESPONSE_VERIFICATION_TEXT_DATA, m_resultSigningProccess);
            }
            if (_nameEvent == EVENT_ETHEREUMCONTROLLER_GET_TRANSACTIONS_LIST)
            {
#if DEBUG_MODE_DISPLAY_LOG
                Debug.LogError("EVENT_ETHEREUMCONTROLLER_GET_TRANSACTIONS_LIST=" + _list[0]);
#endif
                JSONNode jsonTransactionHistory = JSON.Parse((string)_list[0]);
                GetSummaryAccount(jsonTransactionHistory["result"]);
            }
            if (_nameEvent == EVENT_ETHEREUMCONTROLLER_GET_GAS_PRICE)
            {
                JSONNode jsonGasPriceHex = JSON.Parse((string)_list[0]);
#if ENABLE_ETHEREUM
                HexBigInteger gasPriceHex = new HexBigInteger(jsonGasPriceHex["result"]);
                if (m_runningSigningProcess)
                {                    
                    CommsHTTPConstants.GetEthereumRequestGasEstimation(EtherscanAPI, ETHERSCAN_API_KEY, m_publicKeySigningData, new HexBigInteger(0).Value.ToString(), gasPriceHex.Value.ToString(), new HexBigInteger(0xffffff).Value.ToString());
                }
#endif
            }
            if (_nameEvent == EVENT_ETHEREUMCONTROLLER_GET_ESTIMATION_GAS)
            {
                JSONNode jsonGasPriceHex = JSON.Parse((string)_list[0]);
#if ENABLE_ETHEREUM
                HexBigInteger gasEstimationHex = new HexBigInteger(jsonGasPriceHex["result"]);
                if (m_runningSigningProcess)
                {
                    m_runningSigningProcess = false;
                    StartCoroutine(RunSignDataDocumentEnd(gasEstimationHex.Value * 10));
                }
#endif
            }
        }
    }

}