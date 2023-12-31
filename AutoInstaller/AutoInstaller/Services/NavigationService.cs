﻿using System;
using CommunityToolkit.Mvvm.ComponentModel;
namespace AutoInstaller.Services;

public sealed partial class NavigationService : ObservableObject
{
    [ObservableProperty] private bool _isNavigationAllowed = true;

    private Type? _currentPageType;

    public event Action<Type>? CurrentPageChanged;

    public Type? CurrentPageType
    {
        get => _currentPageType;

        set
        {
            if (value is null)
            {
                return;
            }

            _currentPageType = value;

            CurrentPageChanged?.Invoke(value);
        }
    }
}