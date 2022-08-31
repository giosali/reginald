namespace Reginald.Services
{
    using System;
    using System.Collections.Generic;
    using Reginald.Data.ObjectModels;
    using Reginald.Data.Producers;
    using Reginald.Data.Products;
    using Reginald.Services.Watchers;

    internal class ObjectModelService
    {
        private readonly RegistryKeyWatcher[] _watchers;

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
            singleProducers.AddRange(Application.GetApplications());
            SingleProducers = singleProducers.ToArray();
        }
    }
}
