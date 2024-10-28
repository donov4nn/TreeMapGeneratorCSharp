using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;
using Components.TreeMap.Components.TreeMap;

namespace Components;

public class MainViewModel : INotifyPropertyChanged
{
    private ICommand _itemClickCommand;
    private ObservableCollection<TreeMapItem> _items;
    private TreeMapItem _selectedItem;

    public MainViewModel()
    {
        Items = new ObservableCollection<TreeMapItem>
        {
            new() { Label = "BTC", Percentage = 40.555, Color = Brushes.Blue },
            new() { Label = "ETH", Percentage = 18.3, Color = Brushes.Red },
            new() { Label = "BNB", Percentage = 15.7, Color = Brushes.Yellow },
            new() { Label = "XRP", Percentage = 8.2, Color = Brushes.Beige },
            new() { Label = "ADA", Percentage = 5.1, Color = Brushes.LightYellow }
        };
    }

    public TreeMapItem SelectedItem
    {
        get => _selectedItem;
        set
        {
            _selectedItem = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<TreeMapItem> Items
    {
        get => _items;
        set
        {
            _items = value;
            OnPropertyChanged();
        }
    }

    public ICommand ItemClickCommand => _itemClickCommand ??= new RelayCommand<TreeMapItem>(OnItemClick);

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnItemClick(TreeMapItem item)
    {
        SelectedItem = item;
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public class RelayCommand<T> : ICommand
{
    #region Fields

    private readonly Action<T> _execute;
    private readonly Predicate<T> _canExecute;

    #endregion

    #region Constructors

    /// <summary>
    ///     Initializes a new instance of <see cref="DelegateCommand{T}" />.
    /// </summary>
    /// <param name="execute">
    ///     Delegate to execute when Execute is called on the command.  This can be null to just hook up a
    ///     CanExecute delegate.
    /// </param>
    /// <remarks><seealso cref="CanExecute" /> will always return true.</remarks>
    public RelayCommand(Action<T> execute)
        : this(execute, null)
    {
    }

    /// <summary>
    ///     Creates a new command.
    /// </summary>
    /// <param name="execute">The execution logic.</param>
    /// <param name="canExecute">The execution status logic.</param>
    public RelayCommand(Action<T> execute, Predicate<T> canExecute)
    {
        if (execute == null)
            throw new ArgumentNullException("execute");

        _execute = execute;
        _canExecute = canExecute;
    }

    #endregion

    #region ICommand Members

    /// <summary>
    ///     Defines the method that determines whether the command can execute in its current state.
    /// </summary>
    /// <param name="parameter">
    ///     Data used by the command.  If the command does not require data to be passed, this object can
    ///     be set to null.
    /// </param>
    /// <returns>
    ///     true if this command can be executed; otherwise, false.
    /// </returns>
    public bool CanExecute(object parameter)
    {
        return _canExecute == null || _canExecute((T)parameter);
    }

    /// <summary>
    ///     Occurs when changes occur that affect whether or not the command should execute.
    /// </summary>
    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    /// <summary>
    ///     Defines the method to be called when the command is invoked.
    /// </summary>
    /// <param name="parameter">
    ///     Data used by the command. If the command does not require data to be passed, this object can be
    ///     set to <see langword="null" />.
    /// </param>
    public void Execute(object parameter)
    {
        _execute((T)parameter);
    }

    #endregion
}