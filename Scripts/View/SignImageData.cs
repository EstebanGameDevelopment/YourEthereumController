using UnityEngine;
using System.Collections.Generic;
using YourCommonTools;
using System;

namespace YourEthereumController
{
	/******************************************
	 * 
	 * SignTextData
	 * 
	 * Allows to sign the data with your private key
	 * in order to prove that the data is yours
	 * 
	 * @author Esteban Gallardo
	 */
	public class SignImageData : MonoBehaviour
	{
		// ----------------------------------------------
		// PUBLIC MEMBERS
		// ----------------------------------------------
		public GUISkin SkinUI;
		public Texture2D[] MyImages;

        // ----------------------------------------------
        // PRIVATE CONSTANTS MEMBERS
        // ----------------------------------------------
        private const string PRIVATE_ROOT_KEY = "0x1c6c36b151745ed7c93a6e353d0919a98a1b365474136723a743b8d0fa8144f6";
        private const string PUBLICK_ROOT_KEY = "0x80d1F26a9eaAc1110645EDCCE85e9295F8f49c29";

        // ----------------------------------------------
        // PRIVATE MEMBERS
        // ----------------------------------------------
        private List<string> m_displayMessages = new List<string>();
		private Vector2 m_scrollPosition = Vector2.zero;
		private bool m_activateTextArea = false;

        private string m_textContractID = "";
        private int m_originImageToSign = 0;
		private int m_targetImageToTest = 0;

        private bool m_allDataLoaded = false;

        // -------------------------------------------
        /* 
		 * Runs as soon as the object is active
		 */
        void Start()
		{
			EthereumController.Instance.Init(EthereumController.OPTION_NETWORK_TEST);

            EthereumEventController.Instance.EthereumEvent += new EthereumEventHandler(OnEthereumEvent);
            UIEventController.Instance.UIEvent += new UIEventHandler(OnUIEvent);
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

            // BUTTON CLEAR LOG
            float yGlobalPosition = 10;
            if (GUI.Button(new Rect(new Vector2(10, yGlobalPosition), new Vector2(Screen.width - 20, 2 * fontSize)), "Clear Log"))
            {
                m_activateTextArea = false;
                m_displayMessages.Clear();
            }
            yGlobalPosition += 2.2f * fontSize;

            // SIGN DATA
            if (GUI.Button(new Rect(new Vector2(10, yGlobalPosition), new Vector2((Screen.width - 20) / 2, 4 * fontSize)), "++SIGN IMAGE[" + m_originImageToSign + "] HASH++"))
            {
                if (m_originImageToSign>=0)
                {
                    string hashSpriteOrigin = Utilities.ComputeHashCode(Utilities.GetBytesPNG(MyImages[m_originImageToSign]));
                    EthereumController.Instance.SignTextData("TestSignedImage", hashSpriteOrigin, PRIVATE_ROOT_KEY);
                }
            }
            string originImageToSign = GUI.TextField(new Rect(new Vector2(10 + ((Screen.width - 20) / 2), yGlobalPosition), new Vector2(2 * fontSize, 2 * fontSize)), m_originImageToSign.ToString());
            if (!int.TryParse(originImageToSign, out m_originImageToSign))
            {
                m_originImageToSign = 0;
            }
            if (m_originImageToSign > 3) m_originImageToSign = 3;
            GUI.Label(new Rect(new Vector2(10 + ((Screen.width - 20) / 2) + (2 * fontSize), yGlobalPosition), new Vector2(4 * fontSize, 4 * fontSize)), MyImages[m_originImageToSign]);
            yGlobalPosition += 4.2f * fontSize;

            // TITLE
            GUI.Label(new Rect(new Vector2(10, yGlobalPosition), new Vector2(Screen.width - 20, 2 * fontSize)), "Write the contract id to verify the signing process:");
            yGlobalPosition += 2.2f * fontSize;

            // TEXTFIELD CONTRACT ID
            m_textContractID = GUI.TextField(new Rect(new Vector2(10, yGlobalPosition), new Vector2((Screen.width - 20), 3 * fontSize)), m_textContractID);
            yGlobalPosition += 3.2f * fontSize;

            // SIGN DATA
            if (GUI.Button(new Rect(new Vector2(10, yGlobalPosition), new Vector2((Screen.width - 20) / 2, 4 * fontSize)), "--VERIFY IMAGE[" + m_targetImageToTest + "] HASH--"))
            {
                if (m_targetImageToTest >= 0)
                {
                    string hashSpriteTarget = Utilities.ComputeHashCode(Utilities.GetBytesPNG(MyImages[m_targetImageToTest]));
                    EthereumController.Instance.VerifySignedData(m_textContractID, "TestSignedImage", hashSpriteTarget);
                }
            }
            string targetImageToSign = GUI.TextField(new Rect(new Vector2(10 + ((Screen.width - 20) / 2), yGlobalPosition), new Vector2(2 * fontSize, 2 * fontSize)), m_targetImageToTest.ToString());
            if (!int.TryParse(targetImageToSign, out m_targetImageToTest))
            {
                m_targetImageToTest = 0;
            }
            if (m_targetImageToTest > 3) m_targetImageToTest = 3;
            GUI.Label(new Rect(new Vector2(10 + ((Screen.width - 20) / 2) + (2 * fontSize), yGlobalPosition), new Vector2(4 * fontSize, 4 * fontSize)), MyImages[m_targetImageToTest]);
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
		* Ethereum event listener
		*/
        private void OnEthereumEvent(string _nameEvent, object[] _list)
        {
            if (_nameEvent == EthereumController.EVENT_ETHEREUMCONTROLLER_ALL_DATA_COLLECTED)
            {
                m_allDataLoaded = true;
            }
            if (_nameEvent == EthereumController.EVENT_ETHEREUMCONTROLLER_SIGNED_DOCUMENT_DONE)
            {
                if ((bool)_list[0])
                {
                    AddLog("THE IMAGE HAS BEEN SIGNED SUCCESSFULLY= " + (string)_list[1]);
                    AddLog("IT CAN TAKE SOME TIME UNTIL THE TRANSACTION IS PERFORMED...");
                }
                else
                {
                    AddLog("ERROR TO SIGN THE IMAGE="+ ((_list.Length>1)?(string)_list[1]:""));
                }
            }
            if (_nameEvent == EthereumController.EVENT_ETHEREUMCONTROLLER_VERIFICATION_SIGNED_DATA)
            {
                if ((bool)_list[0])
                {
                    AddLog("THE IMAGE HAS BEEN VERIFIED SUCCESSFULLY");
                }
                else
                {
                    AddLog("THE VERIFICATION HAS FAILED, THE IMAGE IS NOT SIGNED BY THE PERSON");
                }
            }
        }

	}
}