using Microsoft.WindowsAPICodePack.Shell;
using Reginald.Core.AbstractProducts;
using Reginald.Core.Base;
using Reginald.Core.DataModels;
using Reginald.Core.Enums;
using Reginald.Core.Helpers;
using Reginald.Core.Utilities;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Reginald.Core.Products
{
    public class UtilityKeyphrase : Keyphrase
    {
        private string _altDescription;
        public string AltDescription
        {
            get => _altDescription;
            set
            {
                _altDescription = value;
                NotifyOfPropertyChange(() => AltDescription);
            }
        }

        private bool _requiresConfirmation;
        public bool RequiresConfirmation
        {
            get => _requiresConfirmation;
            set
            {
                _requiresConfirmation = value;
                NotifyOfPropertyChange(() => RequiresConfirmation);
            }
        }

        private string _confirmationMessage;
        public string ConfirmationMessage
        {
            get => _confirmationMessage;
            set
            {
                _confirmationMessage = value;
                NotifyOfPropertyChange(() => ConfirmationMessage);
            }
        }

        private Utility _utility;
        public Utility Utility
        {
            get => _utility;
            set
            {
                _utility = value;
                NotifyOfPropertyChange(() => Utility);
            }
        }

        public UtilityKeyphrase(UtilityDataModel model)
        {
            if (Guid.TryParse(model.Guid, out Guid guid))
            {
                Guid = guid;
            }
            Name = model.Name;
            Phrase = model.Keyphrase;
            Icon = BitmapImageHelper.FromUri(model.Icon);
            Caption = model.Caption;
            IsEnabled = model.IsEnabled;
            Description = model.Description;
            AltDescription = model.AltDescription;
            RequiresConfirmation = model.RequiresConfirmation;
            ConfirmationMessage = model.ConfirmationMessage;
            if (Enum.TryParse(model.Utility, true, out Utility utility))
            {
                Utility = utility;
            }
        }

        public override bool Predicate(Keyphrase keyphrase, Regex rx, string input)
        {
            return rx.IsMatch(keyphrase.Phrase);
        }

        public override async Task<bool> EnterDown(Keyphrase keyphrase, bool isAltDown, bool isPrompted, Action action)
        {
            UtilityKeyphrase utilityKeyphrase = keyphrase as UtilityKeyphrase;
            switch (utilityKeyphrase.Utility)
            {
                case Utility.Recycle:
                    if (isAltDown)
                    {
                        action();
                        IKnownFolder recycleBin = Applications.GetKnownFolder(Constants.RecycleBinGuid);
                        Processes.OpenFromPath(recycleBin.Path);
                    }
                    else if (isPrompted)
                    {
                        action();
                        Task task = Task.Run(() =>
                        {
                            RecycleBin.Empty();
                        });
                        await task;
                    }
                    else
                    {
                        return false;
                    }
                    break;
            };
            return true;
        }

        public override (string Description, string Caption) AltDown(Keyphrase keyphrase)
        {
            UtilityKeyphrase utilityKeyphrase = keyphrase as UtilityKeyphrase;
            string description = utilityKeyphrase.AltDescription;
            return (description, null);
        }

        public override (string Description, string Caption) AltUp(Keyphrase keyphrase)
        {
            UtilityKeyphrase utilityKeyphrase = keyphrase as UtilityKeyphrase;
            string description = utilityKeyphrase.Description;
            return (description, null);
        }
    }
}
