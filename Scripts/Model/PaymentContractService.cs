#if ENABLE_ETHEREUM
using Nethereum.ABI.Encoders;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Signer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace YourEthereumManager
{

    /******************************************
	 * 
	 * PaymentContractService
	 * 
	 * @author Esteban Gallardo
	 */
    public class PaymentContractService
    {
        public static string ABI = @"[{'constant':false,'inputs':[{'name':'_to','type':'address'},{'name':'_message','type':'string'}],'name':'pay','outputs':[],'payable':true,'stateMutability':'payable','type':'function'},{'anonymous':false,'inputs':[{'indexed':false,'name':'_from','type':'address'},{'indexed':false,'name':'_to','type':'address'},{'indexed':false,'name':'amount','type':'uint256'},{'indexed':false,'name':'message','type':'string'}],'name':'Payment','type':'event'}]";

        private Contract contract;

        public PaymentContractService(string _contractAddress)
        {
            this.contract = new Contract(null, ABI, _contractAddress);
        }

        public Function GetFunctionPay()
        {
            return contract.GetFunction("pay");
        }

        public CallInput CreateGetFunctionPay()
        {
            var function = GetFunctionPay();
            return function.CreateCallInput();
        }

        public TransactionInput CreatePayTransactionInput(string _addressFrom, string _addressTo, string _message, HexBigInteger _gas, HexBigInteger _valueAmount)
        {
            var function = GetFunctionPay();
            return function.CreateTransactionInput(_addressFrom, _gas, _valueAmount, _addressTo, _message);
        }

        public static string ByteArrayToString(byte[] ba)
        {
            return BitConverter.ToString(ba).Replace("-", "");
        }

        public static byte[] StringToByteArray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }
    }
}
#endif