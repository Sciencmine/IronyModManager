﻿// ***********************************************************************
// Assembly         : IronyModManager.Services.Common
// Author           : Mario
// Created          : 02-12-2020
//
// Last Modified By : Mario
// Last Modified On : 02-12-2020
// ***********************************************************************
// <copyright file="IGameService.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using IronyModManager.Models.Common;

namespace IronyModManager.Services.Common
{
    /// <summary>
    /// Interface IGameService
    /// </summary>
    public interface IGameService
    {
        #region Methods

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns>IEnumerable&lt;IGame&gt;.</returns>
        IEnumerable<IGame> Get();

        /// <summary>
        /// Gets the selected.
        /// </summary>
        /// <returns>IGame.</returns>
        IGame GetSelected();

        /// <summary>
        /// Saves the specified game.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool Save(IGame game);

        /// <summary>
        /// Sets the selected.
        /// </summary>
        /// <param name="games">The games.</param>
        /// <param name="selectedGame">The selected game.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool SetSelected(IEnumerable<IGame> games, IGame selectedGame);

        #endregion Methods
    }
}