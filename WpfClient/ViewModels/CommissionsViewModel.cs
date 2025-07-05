using CommunityToolkit.Mvvm.ComponentModel;
using RequestManagement.Common.Interfaces;
using WpfClient.Services.Interfaces;
using System.Windows.Data;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using WpfClient.Messages;
using Commissions = RequestManagement.Common.Models.Commissions;
using Driver = RequestManagement.Common.Models.Driver;


namespace WpfClient.ViewModels;

public partial class CommissionsViewModel : ObservableObject
{
    public bool EditMode { get; set; }
    private readonly IMessageBus _messageBus;
    private readonly ICommissionsService _commissionsService;
    private CommissionType _commissionType = CommissionType.None;
    [ObservableProperty] private string _commissionsName = "";
    [ObservableProperty] private string _branchName = "";
    [ObservableProperty] private Commissions? _selectedCommissions = new();
    [ObservableProperty] private Driver _selectedApproveForAct = new();
    [ObservableProperty] private Driver _selectedApproveForDefectAndLimit = new();
    [ObservableProperty] private Driver _selectedChairman = new();
    [ObservableProperty] private Driver _selectedMember1 = new();
    [ObservableProperty] private Driver _selectedMember2 = new();
    [ObservableProperty] private Driver _selectedMember3 = new();
    [ObservableProperty] private Driver _selectedMember4 = new();
    [ObservableProperty] private ObservableCollection<Commissions> _commissionsList = [];
    [ObservableProperty] private CollectionViewSource _commissionsViewSource;
    public event EventHandler CloseWindowRequested;

    private enum CommissionType { None, Act, DefectAndLimit, Chairman, Member1, Member2, Member3, Member4 };

    public CommissionsViewModel(IMessageBus messageBus, ICommissionsService commissionsService)
    {
        _messageBus = messageBus;
        _commissionsService = commissionsService;
        _commissionsViewSource = new CollectionViewSource { Source = CommissionsList };
        _messageBus.Subscribe<SelectResultMessage>(OnSelect);
    }
    public async Task Load()
    {
        await LoadCommissionAsync();
    }
    private async Task LoadCommissionAsync()
    {
        var commissionsList = await _commissionsService.GetAllCommissionsAsync();
        CommissionsList = new ObservableCollection<Commissions>(commissionsList);
        CommissionsViewSource.Source = CommissionsList;
    }

    partial void OnSelectedCommissionsChanged(Commissions? value)
    {
        if (value == null) return;
        CommissionsName = value.Name;
        BranchName = value.BranchName;
        if (value.ApproveForAct != null) SelectedApproveForAct = value.ApproveForAct;
        if (value.ApproveForDefectAndLimit != null) SelectedApproveForDefectAndLimit = value.ApproveForDefectAndLimit;
        if (value.Chairman != null) SelectedChairman = value.Chairman;
        if (value.Member1 != null) SelectedMember1 = value.Member1;
        if (value.Member2 != null) SelectedMember2 = value.Member2;
        if (value.Member3 != null) SelectedMember3 = value.Member3;
        if (value.Member4 != null) SelectedMember4 = value.Member4;
    }
    private Task OnSelect(SelectResultMessage arg)
    {
        if (arg.Caller == typeof(CommissionsViewModel) && arg is { Item: not null, Message: MessagesEnum.SelectDriver })
        {

            switch (_commissionType)
            {
                case CommissionType.Act:
                    SelectedApproveForAct = (Driver)arg.Item;
                    break;
                case CommissionType.DefectAndLimit:
                    SelectedApproveForDefectAndLimit = (Driver)arg.Item;
                    break;
                case CommissionType.Chairman:
                    SelectedChairman = (Driver)arg.Item;
                    break;
                case CommissionType.Member1:
                    SelectedMember1 = (Driver)arg.Item;
                    break;
                case CommissionType.Member2:
                    SelectedMember2 = (Driver)arg.Item;
                    break;
                case CommissionType.Member3:
                    SelectedMember3 = (Driver)arg.Item;
                    break;
                case CommissionType.Member4:
                    SelectedMember4 = (Driver)arg.Item;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            CommissionsViewSource.View.Refresh();
            _commissionType = CommissionType.None;
        }

        return null!;
    }

    public CommissionsViewModel()
    {
    }

    [RelayCommand]
    private void DoubleClick()
    {
        CloseWindowRequested?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void Click()
    {
        if (SelectedCommissions == null) return;
        CommissionsName = SelectedCommissions.Name;
        BranchName = SelectedCommissions.BranchName;
        if (SelectedCommissions.ApproveForAct != null) SelectedApproveForAct = SelectedCommissions.ApproveForAct;
        if (SelectedCommissions.ApproveForDefectAndLimit != null) SelectedApproveForDefectAndLimit = SelectedCommissions.ApproveForDefectAndLimit;
        if (SelectedCommissions.Chairman != null) SelectedChairman = SelectedCommissions.Chairman;
        if (SelectedCommissions.Member1 != null) SelectedMember1 = SelectedCommissions.Member1;
        if (SelectedCommissions.Member2 != null) SelectedMember2 = SelectedCommissions.Member2;
        if (SelectedCommissions.Member3 != null) SelectedMember3 = SelectedCommissions.Member3;
        if (SelectedCommissions.Member4 != null) SelectedMember4 = SelectedCommissions.Member4;
    }

    [RelayCommand]
    private void ClearBranchNameText()
    {
        BranchName = string.Empty;
    }

    [RelayCommand]
    private void ClearNameText()
    {
        CommissionsName = string.Empty;
    }

    [RelayCommand]
    private async Task SelectApproveForAct()
    {
        _commissionType = CommissionType.Act;
        await _messageBus.Publish(new SelectTaskMessage(MessagesEnum.SelectDriver, typeof(CommissionsViewModel)));
    }
    [RelayCommand]
    private void ClearSelectedApproveForAct()
    {
        SelectedApproveForAct = new();
    }
    [RelayCommand]
    private async Task SelectApproveForDefectAndLimit()
    {
        _commissionType = CommissionType.DefectAndLimit;
        await _messageBus.Publish(new SelectTaskMessage(MessagesEnum.SelectDriver, typeof(CommissionsViewModel)));
    }
    [RelayCommand]
    private void ClearSelectedApproveForDefectAndLimit()
    {
        SelectedApproveForDefectAndLimit = new();
    }
    [RelayCommand]
    private async Task SelectChairman()
    {
        _commissionType = CommissionType.Chairman;
        await _messageBus.Publish(new SelectTaskMessage(MessagesEnum.SelectDriver, typeof(CommissionsViewModel)));
    }
    [RelayCommand]
    private void ClearSelectedChairman()
    {
        SelectedChairman = new();
    }
    [RelayCommand]
    private async Task SelectMember1()
    {
        _commissionType = CommissionType.Member1;
        await _messageBus.Publish(new SelectTaskMessage(MessagesEnum.SelectDriver, typeof(CommissionsViewModel)));
    }
    [RelayCommand]
    private void ClearSelectedMember1()
    {
        SelectedMember1 = new();
    }
    [RelayCommand]
    private async Task SelectMember2()
    {
        _commissionType = CommissionType.Member2;
        await _messageBus.Publish(new SelectTaskMessage(MessagesEnum.SelectDriver, typeof(CommissionsViewModel)));
    }
    [RelayCommand]
    private void ClearSelectedMember2()
    {
        SelectedMember2 = new();
    }
    [RelayCommand]
    private async Task SelectMember3()
    {
        _commissionType = CommissionType.Member3;
        await _messageBus.Publish(new SelectTaskMessage(MessagesEnum.SelectDriver, typeof(CommissionsViewModel)));
    }
    [RelayCommand]
    private void ClearSelectedMember3()
    {
        SelectedMember3= new();
    }
    [RelayCommand]
    private async Task SelectMember4()
    {
        _commissionType = CommissionType.Member4;
        await _messageBus.Publish(new SelectTaskMessage(MessagesEnum.SelectDriver, typeof(CommissionsViewModel)));
    }
    [RelayCommand]
    private void ClearSelectedMember4()
    {
        SelectedMember4 = new();
    }

    [RelayCommand]
    private async Task AddCommissions()
    {
        if (CommissionsName != "" && BranchName != "")
        {
            var commission = new Commissions
            {
                Name = CommissionsName,
                BranchName = BranchName,
                ApproveForAct = SelectedApproveForAct,
                ApproveForDefectAndLimit = SelectedApproveForDefectAndLimit,
                Chairman = SelectedChairman,
                Member1 = SelectedMember1,
                Member2 = SelectedMember2,
                Member3 = SelectedMember3,
                Member4 = SelectedMember4,
            };
            await _commissionsService.CreateCommissionsAsync(commission);
            await LoadCommissionAsync();
        }
    }

    [RelayCommand]
    private async Task SaveCommissions()
    {
        if (SelectedCommissions != null && CommissionsName != "" && BranchName != "")
        {
            var commission = new Commissions()
            {
                Id = SelectedCommissions.Id,
                Name = CommissionsName,
                BranchName = BranchName,
                ApproveForAct = SelectedApproveForAct,
                ApproveForDefectAndLimit = SelectedApproveForDefectAndLimit,
                Chairman = SelectedChairman,
                Member1 = SelectedMember1,
                Member2 = SelectedMember2,
                Member3 = SelectedMember3,
                Member4 = SelectedMember4,
            };
            await _commissionsService.UpdateCommissionsAsync(commission);
            await LoadCommissionAsync();
        }
    }

    [RelayCommand]
    private async Task RemoveCommissions()
    {
        if (SelectedCommissions != null)
        {
            await _commissionsService.DeleteCommissionsAsync(SelectedCommissions.Id);
            await LoadCommissionAsync();
        }
    }
}