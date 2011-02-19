//  Koffeinfrei Minus Share
//  Copyright (C) 2011  Alexis Reigel
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.IO;
using BiasedBit.MinusEngine;
using Koffeinfrei.MinusShare.Properties;

namespace Koffeinfrei.MinusShare
{
    public class Minus
    {
        private const String ApiKey = "dummyKey";
        private const string BaseUrl = "http://min.us/m";

        private readonly List<string> queuedFiles;
        private readonly List<String> uploadedFiles;
        private string title;

        private readonly List<string> recipients;
        private readonly MinusApi api;

        public Action<string> InfoLogger { get; set; }
        public Action<string> ErrorLogger { get; set; }

        public Action<MinusResult> GalleryCreated { get; set; }

        public Minus()
        {
            queuedFiles = new List<string>();
            uploadedFiles = new List<String>();

            recipients = new List<string>();

            api = new MinusApi(ApiKey);
        }

        public void AddFiles(List<string> files)
        {
            queuedFiles.AddRange(files);
        }

        public void RemoveFile(string fileName)
        {
            queuedFiles.Remove(fileName);
        }

        public void AddRecipients(List<string> recipients)
        {
            this.recipients.AddRange(recipients);
        }

        public void SetTitle(string title)
        {
            this.title = title;
        }

        public void Create()
        {
            // create a couple of things we're going to need between requests
            CreateGalleryResult galleryCreated = null;

            // set up the listeners for CREATE
            api.CreateGalleryFailed += (sender, e) => LogError(Resources.CreateGalleryFailed + e.Message);

            api.CreateGalleryComplete += (sender, result) =>
            {
                // gallery created, trigger upload of the first file
                galleryCreated = result;
                LogInfo(Resources.GalleryCreated);
                LogInfo(Resources.UploadingFiles);
                FileInfo file = new FileInfo(queuedFiles[uploadedFiles.Count]);
                LogInfo(Resources.UploadingFile + file.Name + "...");
                api.UploadItem(result.EditorId, result.Key, queuedFiles[0]);
            };

            // set up the listeners for UPLOAD
            api.UploadItemFailed += (sender, e) => LogError(Resources.UploadFailed + e.Message);

            api.UploadItemComplete += (sender, result) =>
            {
                // upload complete, either trigger another upload or save the gallery if all files have been uploaded
                LogInfo(Resources.UploadSuccessful);
                uploadedFiles.Add(result.Id);
                if (uploadedFiles.Count == queuedFiles.Count)
                {
                    // if all the elements are uploaded, then save the gallery
                    LogInfo(Resources.AllUploadSuccessful);
                    api.SaveGallery(title ?? "", galleryCreated.EditorId, galleryCreated.Key, uploadedFiles.ToArray());
                }
                else
                {
                    // otherwise just keep uploading
                    FileInfo file = new FileInfo(queuedFiles[uploadedFiles.Count]);
                    LogInfo(Resources.UploadingFile + file.Name + "...");
                    api.UploadItem(galleryCreated.EditorId, galleryCreated.Key, file.FullName);
                }
            };

            // set up the listeners for SAVE
            api.SaveGalleryFailed += (sender, e) => LogInfo(Resources.SaveGalleryFailed + e.Message);

            api.SaveGalleryComplete += sender =>
            {
                string readUrl = BaseUrl + galleryCreated.ReaderId;
                string editUrl = BaseUrl + galleryCreated.EditorId;

                LogInfo(Resources.GallerySaved);
                if (GalleryCreated != null)
                {
                    GalleryCreated(new MinusResult
                    {
                        EditUrl = editUrl,
                        ShareUrl = readUrl
                    });
                }
            };

            // this is the call that actually triggers the whole program
            api.CreateGallery();
        }

        private void LogInfo(string message)
        {
            if (InfoLogger != null)
            {
                InfoLogger(message);
            }
        }

        private void LogError(string message)
        {
            if (ErrorLogger != null)
            {
                ErrorLogger(message);
            }
        }
    }
}