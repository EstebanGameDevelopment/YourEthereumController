using UnityEngine;
using System.Collections.Generic;
#if ENABLE_ETHEREUM
    using Nethereum.Signer;
#endif

namespace YourEthereumController
{
	/******************************************
	 * 
	 * CreateKeys
	 * 
	 *  Create keys for free
	 *  
	 *  Fill the keys created with Bitcoins in the Main Network:
	 *  
	 *  https://buy.blockexplorer.com/
	 *  
	 *  Or in the TestNet Network:
	 *  
	 *  https://testnet.manu.backend.hamburg/faucet
	 * 
	 * @author Esteban Gallardo
	 */
	public class CreateKeys : MonoBehaviour
	{
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

		// -------------------------------------------
		/* 
		 * Runs as soon as the object is active
		 */
		void Start()
		{
			EthereumController.Instance.Init(EthereumController.OPTION_NETWORK_TEST);
		}

		// -------------------------------------------
		/* 
		* Display messages on screen and buttons on screen
		*/
		void OnGUI()
		{
			GUI.skin = SkinUI;

			float fontSize = 1.2f * 15;

			// BUTTON CLEAR LOG
			float yGlobalPosition = 10;
			if (GUI.Button(new Rect(new Vector2(10, yGlobalPosition), new Vector2(Screen.width - 20, 2 * fontSize)), "Clear Log"))
			{
				m_activateTextArea = false;
				m_displayMessages.Clear();
			}
			yGlobalPosition += 2.2f * fontSize;

			// GENERATE NEW KEY
			if (GUI.Button(new Rect(new Vector2(10, yGlobalPosition), new Vector2(Screen.width - 20, 4 * fontSize)), "Create free new address on ++" + EthereumController.Instance.NetworkAPI + "++ Network"))
			{
#if ENABLE_ETHEREUM
                EthECKey ecKey = EthECKey.GenerateKey();
                string privateKey = ecKey.GetPrivateKey();

                AddLog("+++GENERATING KEY FOR NETWORK[" + EthereumController.Instance.NetworkAPI + "]+++");
				
				AddLog("++++KEY GENERATED++++");
				AddLog("PRIVATE KEY:");
				AddLog("" + privateKey);
				AddLog("PUBLIC KEY:");
				AddLog("" + ecKey.GetPublicAddress());
#endif
			}
			yGlobalPosition += 4.2f * fontSize;

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
	}
}