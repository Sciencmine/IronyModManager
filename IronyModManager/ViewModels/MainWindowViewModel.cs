﻿// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-10-2020
//
// Last Modified By : Mario
// Last Modified On : 01-19-2020
// ***********************************************************************
// <copyright file="MainWindowViewModel.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.Reactive.Disposables;
using Avalonia.Controls;
using IronyModManager.Common.ViewModels;
using IronyModManager.DI;
using IronyModManager.Shared;
using IronyModManager.ViewModels.Controls;
using ReactiveUI;

namespace IronyModManager.ViewModels
{
    /// <summary>
    /// Class MainWindowViewModel.
    /// Implements the <see cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    public class MainWindowViewModel : BaseViewModel
    {
        #region Fields

        /// <summary>
        /// The theme setter
        /// </summary>
        private static readonly Func<Window, bool, bool> themeSetter = (window, isToggled) =>
        {
            if (window != null)
            {
                var style = isToggled ? Constants.Themes.DarkTheme : Constants.Themes.LightTheme;

                window.Styles.Clear();
                window.Styles.Add(style);
                return true;
            }
            return false;
        };

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel" /> class.
        /// </summary>
        public MainWindowViewModel()
        {
            ThemeSelector = DIResolver.Get<ThemeControlViewModel>();
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the main window.
        /// </summary>
        /// <value>The main window.</value>
        public virtual Window MainWindow { get; set; }

        /// <summary>
        /// Gets the theme selector.
        /// </summary>
        /// <value>The theme selector.</value>
        [DoNotNotify]
        public virtual ThemeControlViewModel ThemeSelector { get; protected set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Called when [activated].
        /// </summary>
        /// <param name="disposables">The disposables.</param>
        protected override void OnActivated(CompositeDisposable disposables)
        {
            themeSetter(MainWindow, ThemeSelector.ToggleDarkThemeEnabled);

            var toggleEnabled = this.WhenAnyValue(p => p.ThemeSelector.ToggleDarkThemeEnabled).Subscribe(p =>
            {
                themeSetter(MainWindow, p);
            }).DisposeWith(disposables);
        }

        #endregion Methods
    }
}
