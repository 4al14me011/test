using System;
using System.IO;
using System.Activities;
using System.Threading;
using System.Threading.Tasks;
using SFTP.Activities.Properties;
using UiPath.Shared.Activities;
using UiPath.Shared.Activities.Localization;
using Renci.SshNet;
using Renci.SshNet.Common;
using Renci.SshNet.Sftp;

namespace SFTP.Activities
{
    [LocalizedDisplayName(nameof(Resources.SFTPFileDownload_DisplayName))]
    [LocalizedDescription(nameof(Resources.SFTPFileDownload_Description))]
    public class SFTPFileDownload : ContinuableAsyncCodeActivity
    {
        #region Properties

        /// <summary>
        /// If set, continue executing the remaining activities even if the current activity has failed.
        /// </summary>
        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.ContinueOnError_DisplayName))]
        [LocalizedDescription(nameof(Resources.ContinueOnError_Description))]
        public override InArgument<bool> ContinueOnError { get; set; }

        [LocalizedDisplayName(nameof(Resources.SFTPFileDownload_Host_DisplayName))]
        [LocalizedDescription(nameof(Resources.SFTPFileDownload_Host_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> Host { get; set; }

        [LocalizedDisplayName(nameof(Resources.SFTPFileDownload_Port_DisplayName))]
        [LocalizedDescription(nameof(Resources.SFTPFileDownload_Port_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<int> Port { get; set; }

        [LocalizedDisplayName(nameof(Resources.SFTPFileDownload_UserName_DisplayName))]
        [LocalizedDescription(nameof(Resources.SFTPFileDownload_UserName_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> UserName { get; set; }

        [LocalizedDisplayName(nameof(Resources.SFTPFileDownload_Password_DisplayName))]
        [LocalizedDescription(nameof(Resources.SFTPFileDownload_Password_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> Password { get; set; }

        [LocalizedDisplayName(nameof(Resources.SFTPFileDownload_RemoteFullPath_DisplayName))]
        [LocalizedDescription(nameof(Resources.SFTPFileDownload_RemoteFullPath_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> RemoteFullPath { get; set; }

        [LocalizedDisplayName(nameof(Resources.SFTPFileDownload_LocalPath_DisplayName))]
        [LocalizedDescription(nameof(Resources.SFTPFileDownload_LocalPath_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> LocalPath { get; set; }

        #endregion


        #region Constructors

        public SFTPFileDownload()
        {
        }

        #endregion


        #region Protected Methods

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            if (Host == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(Host)));
            if (Port == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(Port)));
            if (UserName == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(UserName)));
            if (Password == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(Password)));

            base.CacheMetadata(metadata);
        }

        protected override async Task<Action<AsyncCodeActivityContext>> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken)
        {
            // Inputs
            var host = Host.Get(context);
            var port = Port.Get(context);
            var userName = UserName.Get(context);
            var password = Password.Get(context);
            var remoteFullPath = RemoteFullPath.Get(context);
            var localPath = LocalPath.Get(context);

            ///////////////////////////
            // Add execution logic HERE
            ///////////////////////////
            ///

            using (var sftp = new SftpClient(host, port, userName, password))
            {
                sftp.Connect();

                using (var file = File.OpenWrite(Path.Combine(localPath, remoteFullPath.Split("/".ToCharArray())[remoteFullPath.Split("/".ToCharArray()).Length - 1])))
                {
                    sftp.DownloadFile(remoteFullPath, file);
                }

                sftp.Disconnect();
            }

            // Outputs
            return (ctx) => {
            };
        }

        #endregion
    }
}

