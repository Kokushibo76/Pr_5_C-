using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using BankAccountApp.Model;

namespace BankAccountApp.Model
{
    public class BankAccount
    {
        private int _accountNumber;
        private double _balance;
        private double _interestRate;

        public BankAccount(int accountNumber, double balance, double interestRate)
        {
            _accountNumber = accountNumber;
            _balance = balance;
            _interestRate = interestRate;
        }

        public int GetAccountNumber() => _accountNumber;

        public double GetBalance() => _balance;

        public double GetInterest() => _balance * _interestRate * (1 / 12);

        public void Deposit(double amount) => _balance += amount;

        public bool Withdraw(double amount)
        {
            if (amount <= _balance)
            {
                _balance -= amount;
                return true;
            }
            return false;
        }

        public void SetInterestRate(double interestRate) => _interestRate = interestRate;
    }
}

namespace BankAccountApp.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly BankAccount _account;
        private ObservableCollection<string> _transactions;

        public double Balance => _account.GetBalance();

        public double InterestRate => _account.GetInterest();

        public ObservableCollection<string> Transactions
        {
            get => _transactions;
            set
            {
                _transactions = value;
                OnPropertyChanged();
            }
        }

        public ICommand DepositCommand { get; }

        public ICommand WithdrawCommand { get; }

        public ICommand UpdateInterestRateCommand { get; }

        public MainWindowViewModel()
        {
            _account = new BankAccount(123456, 1000, 0.05);
            _transactions = new ObservableCollection<string>();
            DepositCommand = new DelegateCommand(Deposit);
            WithdrawCommand = new DelegateCommand(Withdraw);
            UpdateInterestRateCommand = new DelegateCommand(UpdateInterestRate);
        }

        private void Deposit()
        {
            var depositWindow = new DepositWindow();
            if (depositWindow.ShowDialog() == true)
            {
                double amount = depositWindow.Amount;
                _account.Deposit(amount);
                Transactions.Add($"Депозит {amount}");
                OnPropertyChanged(nameof(Balance));
            }
        }

        private void Withdraw()
        {
            var withdrawWindow = new WithdrawWindow();
            if (withdrawWindow.ShowDialog() == true)
            {
                double amount = withdrawWindow.Amount;
                if (_account.Withdraw(amount))
                {
                    Transactions.Add($"Снято средств {amount}");
                    OnPropertyChanged(nameof(Balance));
                }
                else
                {
                    MessageBox.Show("Недостаточно средств");
                }
            }
        }

        private void UpdateInterestRate()
        {
            var updateInterestRateWindow = new UpdateInterestRateWindow();
            if (updateInterestRateWindow.ShowDialog() == true)
            {
                double interestRate = updateInterestRateWindow.InterestRate;
                _account.SetInterestRate(interestRate);
                Transactions.Add($"Обновлена процентная ставка до {interestRate}");
                OnPropertyChanged(nameof(InterestRate));
            }
        }
    }
}