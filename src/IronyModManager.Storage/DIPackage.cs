﻿// ***********************************************************************
// Assembly         : IronyModManager.Storage
// Author           : Mario
// Created          : 01-11-2020
//
// Last Modified By : Mario
// Last Modified On : 02-07-2020
// ***********************************************************************
// <copyright file="DIPackage.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.ComponentModel;
using IronyModManager.DI.Extensions;
using IronyModManager.Shared;
using IronyModManager.Storage.Common;
using SimpleInjector;
using SimpleInjector.Packaging;
using Container = SimpleInjector.Container;

namespace IronyModManager.Storage
{
    /// <summary>
    /// Class DIPackage.
    /// Implements the <see cref="SimpleInjector.Packaging.IPackage" />
    /// </summary>
    /// <seealso cref="SimpleInjector.Packaging.IPackage" />
    [ExcludeFromCoverage("Should not test external DI.")]
    public class DIPackage : IPackage
    {
        #region Fields

        /// <summary>
        /// The property changed
        /// </summary>
        private static readonly string PropChanged = nameof(INotifyPropertyChanged.PropertyChanged);

        #endregion Fields

        #region Methods

        /// <summary>
        /// Registers the set of services in the specified <paramref name="container" />.
        /// </summary>
        /// <param name="container">The container the set of services is registered into.</param>
        public void RegisterServices(Container container)
        {
            container.Register<IThemeType, ThemeType>();
            container.RegisterSingleton<IDatabase, Database>();
            container.InterceptWith<PropertyChangedInterceptor<IDatabase>>(x => x == typeof(IDatabase), true);

            var tracker = new Jot.Tracker(new JsonStore());
            tracker.Configure<Database>().PersistOn(PropChanged);
            container.RegisterInitializer(d =>
            {
                if (d.Instance is IDatabase)
                {
                    tracker.Track(d.Instance);
                }
            }, ctx => ctx.Registration.Lifestyle == Lifestyle.Singleton);

            container.Register<IStorageProvider, Storage>();
        }

        #endregion Methods
    }
}