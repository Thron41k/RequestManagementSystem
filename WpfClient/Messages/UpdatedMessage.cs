﻿using RequestManagement.Common.Models;
using RequestManagement.Common.Models.Interfaces;

namespace RequestManagement.WpfClient.Messages;

public class UpdatedMessage(MessagesEnum message, Type? caller = null)
{
    public MessagesEnum Message { get; } = message;
}
public class SelectTaskMessage(MessagesEnum message, Type caller)
{
    public MessagesEnum Message { get; } = message;
    public Type Caller { get; } = caller;
}

public class ShowTaskMessage(MessagesEnum message, Type caller, bool editMode, int id, DateTime? date, decimal quantity,int? term, params IEntity?[] items)
{
    public MessagesEnum Message { get; } = message;
    public Type Caller { get; } = caller;
    public bool EditMode { get; } = editMode;
    public int Id { get; } = id;
    public DateTime? Date { get; } = date;
    public decimal Quantity { get; } = quantity;
    public IEntity?[] Item { get; } = items;
    public int? Term { get; set; } = term;
}
public class ShowTaskPrintDialogMessageExpense(MessagesEnum message, Type caller, bool editMode, List<Expense> items, DateTime fromDate, DateTime toDate)
{
    public MessagesEnum Message { get; } = message;
    public Type Caller { get; } = caller;
    public bool EditMode { get; } = editMode;
    public List<Expense> Item { get; } = items;
    public DateTime FromDate { get; } = fromDate;
    public DateTime ToDate { get; } = toDate;
}

public class ShowTaskPrintDialogMessageIncoming(MessagesEnum message, Type caller, bool editMode, List<Incoming> items, DateTime fromDate, DateTime toDate)
{
    public MessagesEnum Message { get; } = message;
    public Type Caller { get; } = caller;
    public bool EditMode { get; } = editMode;
    public List<Incoming> Item { get; } = items;
    public DateTime FromDate { get; } = fromDate;
    public DateTime ToDate { get; } = toDate;
}

public class SelectResultMessage(MessagesEnum message, Type caller, IEntity? item = null)
{
    public MessagesEnum Message { get; } = message;
    public Type Caller { get; } = caller;
    public IEntity? Item { get; } = item;
}
public class ShowResultMessage(MessagesEnum message, Type caller,List<Incoming> items)
{
    public MessagesEnum Message { get; } = message;
    public Type Caller { get; } = caller;
    public List<Incoming> Items { get; } = items;
}