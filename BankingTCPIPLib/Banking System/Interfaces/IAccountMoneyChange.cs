namespace BankingTCPIPLib.Banking_System.Interfaces
{
    public interface IAccountMoneyChange
    {
        void WithdrawalAccrueMoney(AccountHolder account, decimal money);
    }
}