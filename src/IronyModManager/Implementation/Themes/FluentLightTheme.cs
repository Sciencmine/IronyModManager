﻿// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-14-2021
//
// Last Modified By : Mario
// Last Modified On : 03-14-2021
// ***********************************************************************
// <copyright file="FluentLightTheme.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.Platform.Themes;

namespace IronyModManager.Implementation.Themes
{
    /// <summary>
    /// Class FluentLightTheme.
    /// Implements the <see cref="IronyModManager.Platform.Themes.BaseThemeResources" />
    /// </summary>
    /// <seealso cref="IronyModManager.Platform.Themes.BaseThemeResources" />
    public class FluentLightTheme : BaseThemeResources
    {
        #region Properties

        /// <summary>
        /// Gets the styles.
        /// </summary>
        /// <value>The styles.</value>
        public override IReadOnlyCollection<string> Styles => new List<string>() { "avares://Avalonia.Themes.Fluent/FluentLight.xaml", "avares://Avalonia.Themes.Fluent/DensityStyles/Compact.xaml", "avares://IronyModManager/Controls/Themes/FluentLight/ThemeOverride.axaml" };

        /// <summary>
        /// Gets the name of the theme.
        /// </summary>
        /// <value>The name of the theme.</value>
        public override string ThemeName => Services.Common.Constants.Themes.FluentLight.Name;

        #endregion Properties
    }
}
