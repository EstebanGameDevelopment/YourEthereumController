pragma solidity ^0.4.22;

/// @title Keeps the information about the data that belong to the owner.
contract SignedData {

	address owner;

	struct SignatureInformation {
		address addr;
		int256 url;
        int256 hashcode;
    }

	SignatureInformation[] public signedDocuments;
	
	/*
	IMPORTANT TODO: NEthereum doesn't properly calls the constructor, so it's impossible 
					to set up the owner. This is a problem to control who has permissions
					to sign the document of the contract in the function "signDocument".
					It's up to the developer of NEthereum to fix this problem.
	function SignedData() {
        owner = msg.sender;
    }
	*/
	
	// Owner save the hashcode of the data he belongs
	function signDocument(int256 url, int256 hashcode)
	{
		// IMPORTANT TODO: When contructor works in NEthereum uncomment this line to control
		//					who has permissions to modify the contract
		// if (msg.sender != owner) return;
	
		int indexDocument = getSignedDocument(url);
		if (indexDocument == -1)
		{		
			var newSignedDocument = SignatureInformation(msg.sender, url, hashcode);
			signedDocuments.push(newSignedDocument);
		}
		else
		{
			signedDocuments[uint(indexDocument)].hashcode=hashcode;
		}	
	}

	// Any user can check if the owner has signed the data
	function verifyDocument(int256 url, int256 hashcode) returns(int)
	{
		int indexDocument = getSignedDocument(url);
		if (indexDocument != -1)
		{		
			SignatureInformation storage currentSignedDocument = signedDocuments[uint(indexDocument)];
			if (currentSignedDocument.hashcode == hashcode)
			{
			    return indexDocument;
			}
			else
			{
			    return -1;
			}
		}
		else
		{
			return -1;
		}	
	}

	// Total number of documents signed
	function getCountSignedDocuments() returns(uint) {
        return signedDocuments.length;
    }
    
	// Private function to get the hashcode by url
	function getSignedDocument(int256 url) private returns(int) 
	{
		for (uint i = 0; i < signedDocuments.length; i++)
		{
			SignatureInformation storage currentSignedDocument = signedDocuments[i];
			if (currentSignedDocument.url == url)
			{
				return int(i);
			}
		}
        return -1; 
    }
}