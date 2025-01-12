﻿namespace PlannerCRM.Client.Models;

public class DataManager<TItem, TTemp>
    where TItem : class, new()
    where TTemp : class, new()
{
    public TItem SelectedItem { get; set; } = new();
    public TItem NewItem { get; set; } = new();
    public TTemp TempItem { get; set; } = new();

    public List<TItem> MainItems { get; set; } = [];
    public List<TTemp> TempItems { get; set; } = [];
    public List<TItem> SelectedItems { get; set; } = [];
    public List<string> SelectedProperties { get; set; } = [];
}
