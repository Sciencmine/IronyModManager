﻿// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 09-30-2020
//
// Last Modified By : Mario
// Last Modified On : 03-13-2021
// ***********************************************************************
// <copyright file="NotoSansSCFontFamily.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using IronyModManager.Platform.Fonts;

namespace IronyModManager.Fonts
{
    /// <summary>
    /// Class NotoSansSCFontFamily.
    /// Implements the <see cref="IronyModManager.Platform.Fonts.BaseFontFamily" />
    /// </summary>
    /// <seealso cref="IronyModManager.Platform.Fonts.BaseFontFamily" />
    public class NotoSansSCFontFamily : BaseFontFamily
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NotoSansSCFontFamily" /> class.
        /// </summary>
        public NotoSansSCFontFamily()
        {
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public override string Name => "Noto Sans SC";

        /// <summary>
        /// Gets the folder.
        /// </summary>
        /// <value>The folder.</value>
        protected override string Folder => "NotoSansSC";

        #endregion Properties
    }
}
