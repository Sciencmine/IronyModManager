﻿// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-07-2020
//
// Last Modified By : Mario
// Last Modified On : 01-29-2021
// ***********************************************************************
// <copyright file="Enums.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.Implementation.Actions
{
    /// <summary>
    /// Enum NotificationType
    /// </summary>
    public enum NotificationType
    {
        /// <summary>
        /// The information
        /// </summary>
        Info,

        /// <summary>
        /// The success
        /// </summary>
        Success,

        /// <summary>
        /// The warning
        /// </summary>
        Warning,

        /// <summary>
        /// The error
        /// </summary>
        Error
    }

    /// <summary>
    /// Enum PromptType
    /// </summary>
    public enum PromptType
    {
        /// <summary>
        /// The yes no
        /// </summary>
        YesNo,

        /// <summary>
        /// The confirm cancel
        /// </summary>
        ConfirmCancel,

        /// <summary>
        /// The ok
        /// </summary>
        OK
    }
}
