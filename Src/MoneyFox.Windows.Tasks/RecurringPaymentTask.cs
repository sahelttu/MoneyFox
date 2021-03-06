﻿using Windows.ApplicationModel.Background;
using Cheesebaron.MvxPlugins.Settings.WindowsCommon;
using MoneyFox.Shared;
using MoneyFox.Shared.DataAccess;
using MoneyFox.Shared.Manager;
using MoneyFox.Shared.Repositories;
using MvvmCross.Plugins.File.WindowsCommon;
using MvvmCross.Plugins.Sqlite.WindowsUWP;

namespace MoneyFox.Windows.Tasks
{
    public sealed class RecurringPaymentTask : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            var deferral = taskInstance.GetDeferral();

            try
            {
                var dbManager = new DatabaseManager(new WindowsSqliteConnectionFactory(),
                    new MvxWindowsCommonFileStore());

                var paymentRepository = new PaymentRepository(new PaymentDataAccess(dbManager));

                var paymentManager = new PaymentManager(paymentRepository,
                    new AccountRepository(new AccountDataAccess(dbManager)),
                    new RecurringPaymentRepository(new RecurringPaymentDataAccess(dbManager)),
                    null);

                new RecurringPaymentManager(paymentManager, paymentRepository,
                    new SettingsManager(new WindowsCommonSettings())).CheckRecurringPayments();
            }
            finally
            {
                deferral.Complete();
            }
        }
    }
}