namespace BankingTCPIPLib.Banking_System.Interfaces
{
    public interface IAccountToAccountTransaction
    {
        void MakeInterAccountTransaction(AccountHolder depositHolder, AccountHolder creditHolder,
            decimal amount);
    }
}