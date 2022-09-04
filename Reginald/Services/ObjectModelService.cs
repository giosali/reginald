namespace Reginald.Services
{
    using System;
    using System.Collections.Generic;
    using Caliburn.Micro;
    using Reginald.Models.ObjectModels;
    using Reginald.Models.Producers;
    using Reginald.Models.Products;
    using Reginald.Services.Watchers;

    internal class ObjectModelService
    {
#pragma warning disable IDE0052
        private readonly RegistryKeyWatcher[] _watchers;
#pragma warning restore IDE0052

        public ObjectModelService()
        {
            SetSingleProducers();

            RegistryKeyWatcher localMachine64Bit = new(RegistryHive.LocalMachine, @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall");
            localMachine64Bit.RegistryKeyChanged += OnRegistryKeyChanged;
            RegistryKeyWatcher localMachine = new(RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
            localMachine.RegistryKeyChanged += OnRegistryKeyChanged;
            RegistryKeyWatcher currentUser = new(RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
            currentUser.RegistryKeyChanged += OnRegistryKeyChanged;
            _watchers = new RegistryKeyWatcher[] { localMachine64Bit, localMachine, currentUser };
        }

        public ISingleProducer<SearchResult>[] SingleProducers { get; set; }

        private void OnRegistryKeyChanged(object sender, EventArgs e)
        {
            SetSingleProducers();
        }

        private void SetSingleProducers()
        {
            List<ISingleProducer<SearchResult>> singleProducers = new();
            if (IoC.Get<DataModelService>().Settings.AreApplicationsEnabled)
            {
                singleProducers.AddRange(Application.GetApplications());
            }

            SingleProducers = singleProducers.ToArray();
        }
    }
}
