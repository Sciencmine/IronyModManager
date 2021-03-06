﻿// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-12-2020
//
// Last Modified By : Mario
// Last Modified On : 03-27-2021
// ***********************************************************************
// <copyright file="DIPackage.Views.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using IronyModManager.Views;
using IronyModManager.Views.Controls;
using Container = SimpleInjector.Container;

namespace IronyModManager.DI
{
    /// <summary>
    /// Class DIPackage.
    /// Implements the <see cref="SimpleInjector.Packaging.IPackage" />
    /// </summary>
    /// <seealso cref="SimpleInjector.Packaging.IPackage" />
    public partial class DIPackage
    {
        /// <summary>
        /// Registers the views.
        /// </summary>
        /// <param name="container">The container.</param>

        #region Methods

        private void RegisterViews(Container container)
        {
            container.Register<MainWindow>();
            container.Register<ThemeControlView>();
            container.Register<LanguageControlView>();
            container.Register<MainControlView>();
            container.Register<GameControlView>();
            container.Register<InstalledModsControlView>();
            container.Register<SortOrderControlView>();
            container.Register<ModHolderControlView>();
            container.Register<SearchModsControlView>();
            container.Register<CollectionModsControlView>();
            container.Register<AddNewCollectionControlView>();
            container.Register<ExportModCollectionControlView>();
            container.Register<MainConflictSolverControlView>();
            container.Register<MergeViewerControlView>();
            container.Register<ModCompareSelectorControlView>();
            container.Register<MergeViewerBinaryControlView>();
            container.Register<ModConflictIgnoreControlView>();
            container.Register<ModifyCollectionControlView>();
            container.Register<OptionsControlView>();
            container.Register<ConflictSolverModFilterControlView>();
            container.Register<ConflictSolverResetConflictsControlView>();
            container.Register<ConflictSolverDBSearchControlView>();
            container.Register<ConflictSolverCustomConflictsControlView>();
            container.Register<ActionsControlView>();
            container.Register<HashReportControlView>();
            container.Register<DLCManagerControlView>();
            container.Register<PatchModControlView>();
        }

        #endregion Methods
    }
}
