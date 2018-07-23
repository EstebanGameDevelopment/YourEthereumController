#if ENABLE_ETHEREUM
using Nethereum.ABI.Encoders;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Signer;
using System;
using UnityEngine;

namespace YourEthereumManager
{

    /******************************************
	 * 
	 * SignDocumentContractService
	 * 
	 * @author Esteban Gallardo
	 */
    public class SignDocumentContractService
    {
        public static string ABI = @"[{'constant':false,'inputs':[{'name':'url','type':'int256'},{'name':'hashcode','type':'int256'}],'name':'signDocument','outputs':[],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':true,'inputs':[{'name':'','type':'uint256'}],'name':'signedDocuments','outputs':[{'name':'addr','type':'address'},{'name':'url','type':'int256'},{'name':'hashcode','type':'int256'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':false,'inputs':[{'name':'url','type':'int256'},{'name':'hashcode','type':'int256'}],'name':'verifyDocument','outputs':[{'name':'','type':'int256'}],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':false,'inputs':[],'name':'getCountSignedDocuments','outputs':[{'name':'','type':'uint256'}],'payable':false,'stateMutability':'nonpayable','type':'function'}]";

        private Contract m_contract;

        public SignDocumentContractService(string _contractAddress)
        {
            m_contract = new Contract(null, ABI, _contractAddress);
        }

        ///////////////////////////////////////////////
        /// STORE A SIGNED DATA DOCUMENT
        ///////////////////////////////////////////////

        public Function GetFunctionSignDocument()
        {
            return m_contract.GetFunction("signDocument");
        }

        public TransactionInput CreateSignedDocumentTransactionInput(string _addressFrom, string _url, string _data, HexBigInteger _gas = null, HexBigInteger _valueAmount = null)
        {
            int dataHashCode = _data.GetHashCode();
            int urlHashCode = _url.GetHashCode();

#if DEBUG_MODE_DISPLAY_LOG
            Debug.LogError("++REGISTER++ HASCODE TO REGISTER IN THE CONTRACT FOR URL["+ _url + "::" + urlHashCode + "] IS ["+ dataHashCode + "]");
#endif

            var function = GetFunctionSignDocument();
            return function.CreateTransactionInput(_addressFrom, _gas, _valueAmount, urlHashCode, dataHashCode);
        }

        ///////////////////////////////////////////////
        /// VERIFY A SIGNED DATA DOCUMENT
        ///////////////////////////////////////////////

        public Function GetFunctionVerifyDocument()
        {
            return m_contract.GetFunction("verifyDocument");
        }

        public CallInput CreateVerifyDocumentTransactionInput(string _url, string _data)
        {
            int hashCode = _data.GetHashCode();
            int urlHashCode = _url.GetHashCode();

#if DEBUG_MODE_DISPLAY_LOG
            Debug.LogError("**VERIFICATION** HASCODE TO VERIFY IN THE CONTRACT FOR URL[" + _url + "::" + urlHashCode + "] IS [" + hashCode + "]");
#endif

            var function = GetFunctionVerifyDocument();
            return function.CreateCallInput(urlHashCode, hashCode);
        }

        public int DecodeVerifyDocument(string result)
        {
            var function = GetFunctionVerifyDocument();
            return function.DecodeSimpleTypeOutput<int>(result);
        }

        ///////////////////////////////////////////////
        /// GET TOTAL NUMBER OF DOCUMENTS
        ///////////////////////////////////////////////

        public Function GetFunctionCountSignedDocuments()
        {
            return m_contract.GetFunction("getCountSignedDocuments");
        }

        public CallInput CreateGetCountSignedDocuments()
        {
            var function = GetFunctionVerifyDocument();
            return function.CreateCallInput();
        }

        public int DecodeCountSignedDocuments(string result)
        {
            var function = GetFunctionVerifyDocument();
            return function.DecodeSimpleTypeOutput<int>(result);
        }

    }
}
#endif