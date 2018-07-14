pragma solidity ^0.4.17;

/// @title Pay - Facilitates payments.
contract Pay {

    event Payment(
        address _from,
        address _to,
        uint amount,
		string message
    );

    /// @dev Makes a payment.
    /// @param _to Address to pay to.
    function pay(address _to, string _message) public payable {
        require(msg.value > 0);
        _to.transfer(msg.value);
        Payment(msg.sender, _to, msg.value, _message);
    }
}