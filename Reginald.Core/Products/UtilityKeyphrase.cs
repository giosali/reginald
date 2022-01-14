namespace Reginald.Core.Products
{
    using System;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Microsoft.WindowsAPICodePack.Shell;
    using Reginald.Core.AbstractProducts;
    using Reginald.Core.DataModels;
    using Reginald.Core.Helpers;
    using Reginald.Core.IO;
    using Reginald.Core.Utilities;

    /// <summary>
    /// Specifies the type of utility.
    /// </summary>
    public enum Utility
    {
        /// <summary>
        /// Specifies utility related to the Recycle Bin.
        /// </summary>
        Recycle,
    }

    public class UtilityKeyphrase : Keyphrase
    {
        private string _altDescription;

        private bool _requiresConfirmation;

        private string _confirmationMessage;

        private Utility _utility;

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

        public string AltDescription
        {
            get => _altDescription;
            set
            {
                _altDescription = value;
                NotifyOfPropertyChange(() => AltDescription);
            }
        }

        public bool RequiresConfirmation
        {
            get => _requiresConfirmation;
            set
            {
                _requiresConfirmation = value;
                NotifyOfPropertyChange(() => RequiresConfirmation);
            }
        }

        public string ConfirmationMessage
        {
            get => _confirmationMessage;
            set
            {
                _confirmationMessage = value;
                NotifyOfPropertyChange(() => ConfirmationMessage);
            }
        }

        public Utility Utility
        {
            get => _utility;
            set
            {
                _utility = value;
                NotifyOfPropertyChange(() => Utility);
            }
        }

        public override void EnterDown(bool isAltDown, Action action)
        {
        }

        public override async Task<bool> EnterDownAsync(bool isAltDown, Action action, object o)
        {
            bool isPrompted = (bool)o;
            switch (Utility)
            {
                case Utility.Recycle:
                    if (isAltDown)
                    {
                        action();
                        IKnownFolder recycleBin = WindowsShell.GetKnownFolder(RecycleBin.RecycleBinFolderGuid);
                        ProcessUtility.OpenFromPath(recycleBin.Path);
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
            }

            return true;
        }

        public override (string Description, string Caption) AltDown()
        {
            return (AltDescription, null);
        }

        public override (string Description, string Caption) AltUp()
        {
            return (Description, null);
        }

        public override bool Predicate(Keyphrase keyphrase, Regex rx, string input)
        {
            return rx.IsMatch(keyphrase.Phrase);
        }
    }
}
