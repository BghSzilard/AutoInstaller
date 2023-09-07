using AISL;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AutoInstaller.ViewModels;

public partial class ParameterDataViewModel : ObservableValidator
{
    public ParameterData ParameterData { get; set; }

    public ParameterDataViewModel(ParameterData parameterData)
    {
        ParameterData = parameterData;
        Value = ParameterData.Value;
        //Value = ParameterData.FixedValue != null ? ParameterData.FixedValue : null;
    }

    public string Name => ParameterData.IsOptional ? ParameterData.Name + "*" : ParameterData.Name;

    private string? _value;

    [CustomValidation(typeof(ParameterDataViewModel), nameof(ValidateParameterValue))]
    public string? Value
    {
        get => _value;
        set
        {
	        SetProperty(ref _value, value, true);
        }
    }

    public static ValidationResult ValidateParameterValue(string? v, ValidationContext context)
    {
        ParameterDataViewModel parameterVM = context.ObjectInstance as ParameterDataViewModel;
	    if (string.IsNullOrEmpty(v))
	    {
		    return new ValidationResult("Value cannot be empty");
	    }
	    switch (parameterVM.ParameterType)
	    {
		    case ParameterType.number:
			    if (!int.TryParse(v, out var number))
			    {
				    return new ValidationResult("Parameter not a numerical value");
			    }

			    break;
		    case ParameterType.flag:
			    if (!v.Equals("0") && !v.Equals("1"))
			    {
				    return new ValidationResult("Parameter not a boolean value");
			    }

			    break;

	    }

	    return ValidationResult.Success;
    }

    public ParameterType ParameterType => ParameterData.Type;

    public bool IsReadOnly => ParameterData.IsReadOnly;
}
