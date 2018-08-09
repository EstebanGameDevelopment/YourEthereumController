This project is based on NEthereum:

https://github.com/Nethereum/Nethereum/releases/tag/3.0.0-rc1

You can get the necessary DLLs to make the project work here:

https://github.com/Nethereum/Nethereum/releases/download/3.0.0-rc1/unitynet35dlls.zip

-------------------------
YOUR ETHEREUM CONTROLLER
-------------------------

Thanks to this plugin you will be able to include Ethereum cryptocurreny and Blockchain signning authentication
in your games and applications.

The video tutorials make reference to Bitcoin but the methodology works almost the same way with Ethereum.

TUTORIAL BASIC OPERATIONS
--------------------------

  1. SET UP THE RUNTIME:

	First of all, we make sure that the Scripting Runtime Version: ".NET 4.x Equivalent" in order for the Nethereum DLLs to work.
 
  2. INSTALLATION OF NEthereum DLL:
 
	On Visual Studio open the NuGet console and run the command line: "Install-Package Nethereum.Portable"
	
	After installing the DLL you will be able to run the code without problems.

  3. GET YOUR OWN KEYS AND SET UP AT AT CLASS: EthereumController.cs

		public const string ETHERSCAN_API_KEY = "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX";   // Get your own key at: https://etherscan.io
        public const string INFURA_API_KEY = "YYYYYYYYYYYYYYYYY";   // Get your own key at: https://infura.io/
		
  4. RUN THE SCENE:

		Assets\YourEthereumController\Scenes\BasicManager.unity 
		
	 and you will be able to perform operations with Ethereum. 
	 
	 Follow the instructions on the video tutorial:
	 
		https://youtu.be/5Nj8FKymhjc
		
  5. Run the scene CreateKeys.unity to create new wallets
  
	 Follow the instructions on the last part of the video tutorial (time 2:32):
	 
		https://youtu.be/5Nj8FKymhjc?t=2m32s
		
  6. Run the scenes SignTextData.unity and SignImageData.unity to sign data and document using your keys
  
	 Follow the instructions on the video tutorial:
	 
		https://youtu.be/5Nj8FKymhjc?t=3m16s
