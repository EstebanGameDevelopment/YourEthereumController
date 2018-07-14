using System;
using System.Collections.Generic;
using UnityEngine;
using YourCommonTools;

namespace YourEthereumController
{
	/******************************************
	 * 
	 * BasicManager
	 * 
	 * Basic example to test the most transaction basic functionalities
	 * 
	 * 	To get Bitcoins in the Main Network:
	 *  
	 *  https://buy.blockexplorer.com/
	 *  
	 *  Or in the TestNet Network:
	 *  
	 *  https://testnet.manu.backend.hamburg/faucet
	 *
	 * @author Esteban Gallardo
	 */
	public class BasicManager : MonoBehaviour
	{
        public const string EVENT_BASICMANAGER_RUN_TRANSACTION = "EVENT_BASICMANAGER_RUN_TRANSACTION";

        public const string PRIVATE_ROOT_KEY = "0x1c6c36b151745ed7c93a6e353d0919a98a1b365474136723a743b8d0fa8144f6";

		public readonly string[] PRIVATE_KEY_TOTAL =   {
                                                "0x1c6c36b151745ed7c93a6e353d0919a98a1b365474136723a743b8d0fa8144f6", // ROOT_KEY
												"0x2b864ec79fb25a256d5247ceae0398ee2087332b7b6c425bad7ffb3218eabeb9",
                                                "0x1f30969e98355945c2e1200bbc490443c9a3e71316bddc048e8c8a30b97725f7",
                                                "0x5bd8bcbb596f6612bcd06d008d328482d9a986bef6d7a179db7022c35b7b69ae",
                                                "0x2e9942b76cd4b3072dd316c2475c26e55873dbfd16ae6c4f5c03fec7ae61ca61"
                                            };

		public readonly string[] PUBLIC_DESTINATIONS_KEYS =
													{
                                                "0x80d1F26a9eaAc1110645EDCCE85e9295F8f49c29", // ROOT_KEY
												"0x2624faD8679c91dC2B7ebf2860612FeD1a208E38",
                                                "0xb03ccCb943faa199335a4eefF244af2a760f12Da",
                                                "0xA64751A9A87ff51381Da8c93Dc37727497f8F79f",
                                                "0x04C4e1f633a682039a7ff9472fc31358Cf9BAE43"
                                            };

		public readonly string[] CURRENCIES = {
											"USD",
											"EUR",
											"RUB",
											"GBP",
											"JPY"
										};

		// ----------------------------------------------
		// PUBLIC MEMBERS
		// ----------------------------------------------
		public GUISkin SkinUI;

		// ----------------------------------------------
		// PRIVATE MEMBERS
		// ----------------------------------------------
		private List<string> m_displayMessages = new List<string>();
		private Vector2 m_scrollPosition = Vector2.zero;
		private bool m_activateTextArea = false;
		private string m_amountToTransfer = "0.001";

		private int m_retryPush = 0;
		private string m_transactionHex;
		private int m_indexCurrency = 0;
		private string m_currency = "USD";
		private decimal m_exchangeValue = -1;
		private string m_titleOfTransfer = "Title of transfer";

		private List<float> m_balanceWallets = new List<float>();

        private bool m_allDataLoaded = false;

        private string m_privateKeyOrigin;
        private string m_publicKeyDestination;
        private decimal m_amountToTransferTransaction = -1m;
        private string m_titleOfTransferTransaction = "Title of transfer";

        // -------------------------------------------
        /* 
		 * Runs as soon as the object is active
		 */
        void Start()
		{
			EthereumController.Instance.Init(EthereumController.OPTION_NETWORK_TEST);

			EthereumEventController.Instance.EthereumEvent += new EthereumEventHandler(OnEthereumEvent);
            UIEventController.Instance.UIEvent += new UIEventHandler(OnUIEvent);


            for (int i = 0; i < PRIVATE_KEY_TOTAL.Length; i++)
			{
				m_balanceWallets.Add(0);
			}
		}

        // -------------------------------------------
        /* 
		 * Destroy
		 */
        void OnDestroy()
		{
			Destroy();
		}


		// -------------------------------------------
		/* 
		* Destroy
		*/
		public void Destroy()
		{
			EthereumEventController.Instance.EthereumEvent -= OnEthereumEvent;
            UIEventController.Instance.UIEvent -= OnUIEvent;
        }

		// -------------------------------------------
		/* 
		* Display messages on screen and buttons on screen
		*/
		void OnGUI()
		{
			GUI.skin = SkinUI;

			float fontSize = 1.2f * 15;

			if (!m_allDataLoaded)
			{
				GUI.Label(new Rect(new Vector2(10, (Screen.height / 2) - fontSize), new Vector2(Screen.width - 20, 2 * fontSize)), "Connecting with Ethereum Network. Wait...");
				return;
			}

			// NETWORK (MAIN or TESTNET)
			float yGlobalPosition = 10;
			float xPosSetParameters = 10;
			float widthSetParameters = ((Screen.width - 20) / 4);
            /*
			GUI.Label(new Rect(new Vector2(xPosSetParameters, yGlobalPosition), new Vector2(widthSetParameters, 2 * fontSize)), "NETWORK ++[" + EthereumController.Instance.Network.ToString() + "]++");
            */
            GUI.Label(new Rect(new Vector2(xPosSetParameters, yGlobalPosition), new Vector2(widthSetParameters, 2 * fontSize)), "NETWORK ++[" + "EthereumController.Instance.Network.ToString()" + "]++");
            xPosSetParameters += widthSetParameters;
			if (GUI.Button(new Rect(new Vector2(xPosSetParameters, yGlobalPosition), new Vector2(3 * widthSetParameters, 2 * fontSize)), "Clear Log"))
			{
				m_activateTextArea = false;
				m_displayMessages.Clear();
			}
			yGlobalPosition += 2.2f * fontSize;

			// CURRENT INFO
			xPosSetParameters = 10;
			widthSetParameters = ((Screen.width - 20) / 6);
			GUI.Label(new Rect(new Vector2((int)(xPosSetParameters), yGlobalPosition), new Vector2(widthSetParameters, 2 * fontSize)), "CURRENCY");
			xPosSetParameters += widthSetParameters;
			GUI.Label(new Rect(new Vector2((int)(xPosSetParameters), yGlobalPosition), new Vector2(widthSetParameters / 3, 2 * fontSize)), m_currency);
			xPosSetParameters += widthSetParameters / 3;
			if (GUI.Button(new Rect(new Vector2((int)(xPosSetParameters), yGlobalPosition), new Vector2(2 * widthSetParameters / 3, 2 * fontSize)), "CHANGE"))
			{
				m_indexCurrency++;
				if (m_indexCurrency >= CURRENCIES.Length)
				{
					m_indexCurrency = 0;
				}
				m_currency = CURRENCIES[m_indexCurrency];
				m_exchangeValue = EthereumController.Instance.CurrenciesExchange[m_currency];
			}
			xPosSetParameters += 2 * widthSetParameters / 3;
			GUI.Label(new Rect(new Vector2((int)(xPosSetParameters), yGlobalPosition), new Vector2(widthSetParameters, 2 * fontSize)), "EXCHANGE");
			xPosSetParameters += widthSetParameters;
			GUI.Label(new Rect(new Vector2((int)(xPosSetParameters), yGlobalPosition), new Vector2(widthSetParameters, 2 * fontSize)), m_exchangeValue.ToString());
			yGlobalPosition += 2.2f * fontSize;

			// CHECK BALANCE ORIGIN
			float xPosLocalCheck = 10;
			float widthButtonCheck = ((Screen.width - 20) / PRIVATE_KEY_TOTAL.Length);
			for (int l = 0; l < PRIVATE_KEY_TOTAL.Length; l++)
			{
				if (GUI.Button(new Rect(new Vector2((int)xPosLocalCheck, yGlobalPosition), new Vector2(widthButtonCheck, 2 * fontSize)), "BAL[" + l + "]=" + (m_balanceWallets[l] * (float)m_exchangeValue) + " " + m_currency))
				{
					m_activateTextArea = false;
					CheckBalanceOrigin(l);
				}
				xPosLocalCheck += widthButtonCheck;
			}
			yGlobalPosition += 2.2f * fontSize;

			// TEXTFIELD AMOUNT
			xPosSetParameters = 10;
			widthSetParameters = ((Screen.width - 20) / 8);
			m_titleOfTransfer = GUI.TextField(new Rect(new Vector2((int)(xPosSetParameters), yGlobalPosition), new Vector2(widthSetParameters, 2 * fontSize)), m_titleOfTransfer);
			xPosSetParameters += widthSetParameters;
			GUI.Label(new Rect(new Vector2((int)(xPosSetParameters), yGlobalPosition), new Vector2(widthSetParameters, 2 * fontSize)), "AMOUNT");
			xPosSetParameters += widthSetParameters;
			m_amountToTransfer = GUI.TextField(new Rect(new Vector2((int)(xPosSetParameters), yGlobalPosition), new Vector2(widthSetParameters, 2 * fontSize)), m_amountToTransfer);
			xPosSetParameters += widthSetParameters;
			GUI.Label(new Rect(new Vector2((int)(xPosSetParameters), yGlobalPosition), new Vector2(widthSetParameters, 2 * fontSize)), (float.Parse(m_amountToTransfer) * (float)m_exchangeValue).ToString() + " " + m_currency);
			yGlobalPosition += 2.2f * fontSize;

			// SEND MONEY TO MINIONS TRANSACTION SIMPLE
			xPosLocalCheck = 10 + widthButtonCheck;
			for (int l = 1; l < PUBLIC_DESTINATIONS_KEYS.Length; l++)
			{
				if (GUI.Button(new Rect(new Vector2((int)xPosLocalCheck, yGlobalPosition), new Vector2(widthButtonCheck, 2 * fontSize)), "SEND[0->" + l + "]"))
				{
					m_activateTextArea = false;
					RunTransaction(PRIVATE_ROOT_KEY, PUBLIC_DESTINATIONS_KEYS[l], decimal.Parse(m_amountToTransfer), m_titleOfTransfer);
				}
				xPosLocalCheck += widthButtonCheck;
			}
			yGlobalPosition += 2.2f * fontSize;

			// RETURN MONEY TO ROOT TRANSACTION SIMPLE
			xPosLocalCheck = 10 + widthButtonCheck;
			for (int l = 1; l < PUBLIC_DESTINATIONS_KEYS.Length; l++)
			{
				if (GUI.Button(new Rect(new Vector2((int)xPosLocalCheck, yGlobalPosition), new Vector2(widthButtonCheck, 2 * fontSize)), "RETURN[" + l + "->0]"))
				{
					m_activateTextArea = false;
					RunTransaction(PRIVATE_KEY_TOTAL[l], PUBLIC_DESTINATIONS_KEYS[0], decimal.Parse(m_amountToTransfer), m_titleOfTransfer);
				}
				xPosLocalCheck += widthButtonCheck;
			}
			yGlobalPosition += 2.2f * fontSize;

			// GET HISTORY TRANSACTIONS
			xPosLocalCheck = 10;
			for (int l = 0; l < PUBLIC_DESTINATIONS_KEYS.Length; l++)
			{
				if (GUI.Button(new Rect(new Vector2((int)xPosLocalCheck, yGlobalPosition), new Vector2(widthButtonCheck, 2 * fontSize)), "HISTORY[" + l + "]"))
				{
					m_activateTextArea = false;
					GetAllTransactions(PUBLIC_DESTINATIONS_KEYS[l]);
				}
				xPosLocalCheck += widthButtonCheck;
			}
			yGlobalPosition += 2.2f * fontSize;

			// LOG DISPLAY
			GUI.Label(new Rect(0, yGlobalPosition, Screen.width - 20, fontSize), "**PROGRAM LOG**");
			yGlobalPosition += 1.2f * fontSize;
			int linesTextArea = 10;
			if (m_activateTextArea)
			{
				linesTextArea = 10;
			}
			else
			{
				linesTextArea = 2;
			}
			float finalHeighArea = linesTextArea * fontSize;
			m_scrollPosition = GUI.BeginScrollView(new Rect(10, yGlobalPosition, Screen.width - 20, Screen.height - yGlobalPosition), m_scrollPosition, new Rect(0, 0, 200, m_displayMessages.Count * finalHeighArea));
			float yPosition = 0;
			for (int i = 0; i < m_displayMessages.Count; i++)
			{
				string message = m_displayMessages[i];
				GUI.TextArea(new Rect(0, yPosition, Screen.width, finalHeighArea), message);
				yPosition += finalHeighArea;
			}
			GUI.EndScrollView();
		}

		// -------------------------------------------
		/* 
		 * Add Log message
		 */
		private void AddLog(string _message)
		{
			m_displayMessages.Add(_message);
			Debug.Log(_message);
		}

		private int m_indexPrivateKeyAddresToCheckBalance;

		// -------------------------------------------
		/* 
		* CheckBalanceOrigin
		*/
		private void CheckBalanceOrigin(int _index)
		{
			m_indexPrivateKeyAddresToCheckBalance = _index;
			AddLog("++GETTING THE BALANCE FOR THE ACCOUNT["+_index+"]. PLEASE WAIT...++");
			Invoke("CheckBalanceOriginReal", 0.1f);
		}

		// -------------------------------------------
		/* 
		* CheckBalanceOrigin
		*/
		private void CheckBalanceOriginReal()
		{
			EthereumController.Instance.GetBalancePrivateKey(false, PRIVATE_KEY_TOTAL[m_indexPrivateKeyAddresToCheckBalance], EthereumController.EVENT_ETHEREUMCONTROLLER_BALANCE_WALLET);
		}

		private string m_publicKeyAddresToCheckHistory;

		// -------------------------------------------
		/* 
		* GetAllTransactions
		*/
		private void GetAllTransactions(string _publicKeyAdress)
		{
			m_publicKeyAddresToCheckHistory = _publicKeyAdress;
			AddLog("++GETTING ALL THE TRANSACTION HISTORY. PLEASE WAIT...++");
            EthereumController.Instance.GetAllInformation(m_publicKeyAddresToCheckHistory);
        }

		// -------------------------------------------
		/* 
		* GetAllTransactionsReal
		*/
		public void GetAllTransactionsReal()
		{
			AddLog("++INPUT TRANSACTIONS[" + EthereumController.Instance.InTransactionsHistory.Count + "]++");
			for (int i = 0; i < EthereumController.Instance.InTransactionsHistory.Count; i++)
			{
				ItemMultiObjectEntry transaction = EthereumController.Instance.InTransactionsHistory[i];
				AddLog(EthereumController.ToStringTransaction(transaction));
			}

			AddLog("--OUTPUT TRANSACTIONS[" + EthereumController.Instance.OutTransactionsHistory.Count + "]--");
			for (int i = 0; i < EthereumController.Instance.OutTransactionsHistory.Count; i++)
			{
				ItemMultiObjectEntry transaction = EthereumController.Instance.OutTransactionsHistory[i];
				AddLog(EthereumController.ToStringTransaction(transaction));
			}
		}

		// -------------------------------------------
		/* 
		 * Runs the most basic transaction possible
		 */
		private void RunTransaction(string _privateKeyOrigin, string _publicKeyDestination, decimal _amountToTransferTransaction, string _titleOfTransferTransaction)
		{
            m_privateKeyOrigin = _privateKeyOrigin;
            m_publicKeyDestination = _publicKeyDestination;
            m_amountToTransferTransaction = _amountToTransferTransaction;
            m_titleOfTransferTransaction = _titleOfTransferTransaction;
            EthereumController.Instance.ValidatePrivateKey(_privateKeyOrigin, EVENT_BASICMANAGER_RUN_TRANSACTION);
		}

        // -------------------------------------------
        /* 
		 * OnUIEvent
		 */
        private void OnUIEvent(string _nameEvent, object[] _list)
        {
            if (_nameEvent == ScreenInformationView.EVENT_SCREEN_UPDATE_TEXT_DESCRIPTION)
            {
                AddLog((string)_list[0]);
            }
        }

        // -------------------------------------------
        /* 
		 * OnBasicEvent
		 */
        private void OnEthereumEvent(string _nameEvent, params object[] _list)
		{
            if (_nameEvent == EthereumController.EVENT_ETHEREUMCONTROLLER_BALANCE_WALLET)
            {
                if (PRIVATE_KEY_TOTAL[m_indexPrivateKeyAddresToCheckBalance] == (string)_list[0])
                {
                    if ((bool)_list[1])
                    {
                        decimal balance = (decimal)_list[2];
                        m_balanceWallets[m_indexPrivateKeyAddresToCheckBalance] = (float)balance;
                        AddLog("++++CURRENT BALANCE[" + m_indexPrivateKeyAddresToCheckBalance + "][" + balance + "][" + (m_exchangeValue * balance) + " " + m_currency + "]++++");
                    }
                    else
                    {
                        AddLog("++++ERROR RETRIVING BALANCE OF[" + m_indexPrivateKeyAddresToCheckBalance + "]++++");
                    }
                }
            }
            if (_nameEvent == EthereumController.EVENT_ETHEREUMCONTROLLER_ALL_DATA_COLLECTED)
			{
				m_exchangeValue = EthereumController.Instance.CurrenciesExchange[m_currency];
                m_allDataLoaded = true;
			}
            if (_nameEvent == EthereumController.EVENT_ETHEREUMCONTROLLER_TRANSACTION_HISTORY)
            {
                GetAllTransactionsReal();
            }
            if (_nameEvent == EVENT_BASICMANAGER_RUN_TRANSACTION)
            {
                EthereumController.Instance.Pay(m_privateKeyOrigin, m_publicKeyDestination, m_titleOfTransferTransaction, m_amountToTransferTransaction);
            }
            if (_nameEvent == EthereumController.EVENT_ETHEREUMCONTROLLER_TRANSACTION_COMPLETED)
			{
				if ((bool)_list[0])
				{
					AddLog("+++TRANSACTION SUCCESS::PRESS ON BALANCE TO CHECK IF UPDATED, IT COULD TAKE A WHILE, BE PATIENCE");
				}
				else
				{
					string info = ((_list.Length > 1) ? (string)_list[1] : "");
					AddLog("---TRANSACTION FAILED::info=" + info);
				}
			}
		}
	}
}