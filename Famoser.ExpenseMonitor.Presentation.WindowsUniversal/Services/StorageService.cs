﻿using System;
using System.Threading.Tasks;
using Windows.Storage;
using Famoser.ExpenseMonitor.Data.Services;
using Famoser.FrameworkEssentials.Logging;

namespace Famoser.ExpenseMonitor.Presentation.WindowsUniversal.Services
{
    class StorageService : IStorageService
    {
        private async Task<string> ReadCache(string filename)
        {
            try
            {
                StorageFile localFile = await ApplicationData.Current.LocalFolder.GetFileAsync(filename);
                if (localFile != null)
                {
                    return await FileIO.ReadTextAsync(localFile);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex, this);
            }
            return null;
        }

        private async Task<string> ReadSettings(string filename)
        {
            try
            {
                StorageFile localFile = await ApplicationData.Current.RoamingFolder.GetFileAsync(filename);
                if (localFile != null)
                {
                    return await FileIO.ReadTextAsync(localFile);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex, this);
            }
            return null;
        }

        private async Task<bool> SaveToCache(string filename, string content)
        {
            try
            {
                StorageFile localFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
                if (localFile != null)
                {
                    await FileIO.WriteTextAsync(localFile, content);
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex, this);
            }
            return false;
        }

        private async Task<bool> SaveToSettings(string filename, string content)
        {
            try
            {
                StorageFile localFile = await ApplicationData.Current.RoamingFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
                if (localFile != null)
                {
                    await FileIO.WriteTextAsync(localFile, content);
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex, this);
            }
            return false;
        }

        public Task<string> GetCachedData()
        {
            return ReadCache("data.json");
        }

        public Task<string> GetUserInformations()
        {
            return ReadSettings("user.json");
        }

        public Task<bool> SetCachedData(string data)
        {
            return SaveToCache("data.json", data);
        }

        public Task<bool> SetUserInformations(string info)
        {
            return SaveToSettings("user.json", info);
        }

        public async Task<bool> ResetApplication()
        {
            try
            {
                var allfiles = await ApplicationData.Current.LocalFolder.GetFilesAsync();
                foreach (var storageFile in allfiles)
                {
                    await storageFile.DeleteAsync();
                }

                allfiles = await ApplicationData.Current.RoamingFolder.GetFilesAsync();
                foreach (var storageFile in allfiles)
                {
                    await storageFile.DeleteAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex, this);
            }
            return false;
        }
    }
}
