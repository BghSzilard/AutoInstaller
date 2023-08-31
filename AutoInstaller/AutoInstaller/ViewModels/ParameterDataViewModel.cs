using AISL;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;

namespace AutoInstaller.ViewModels;

public partial class ParameterDataViewModel : ObservableObject
{
    public ParameterData ParameterData { get; set; }

    public ParameterDataViewModel(ParameterData parameterData)
    {
        ParameterData = parameterData;
        Value = ParameterData.DefaultValue != null ? ParameterData.DefaultValue : null;
        //Value = ParameterData.FixedValue != null ? ParameterData.FixedValue : null;
    }

    public string Name => ParameterData.IsOptional ? ParameterData.Name + "*" : ParameterData.Name;

    [ObservableProperty] private string? _value;

    partial void OnValueChanged(string? value)
    {
        ParameterData.Value = value;
    }
}
