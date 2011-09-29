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
using Koffeinfrei.Base;
using Koffeinfrei.MinusShare.Properties;
using Resources = Koffeinfrei.MinusShare.Properties.Resources;
using System.Linq;

namespace Koffeinfrei.MinusShare
{
    public class Minus
    {
        private const String ApiKey = "dummyKey";
        private const string BaseUrl = "http://minus.com/m";
        private const string UrlUnavailable = "Unavailable";
        private const string GalleryDeleted = "Deleted";

        private readonly List<string> queuedFiles;
        private readonly List<String> uploadedFiles;
        private string title;

        private readonly List<string> recipients;
        private readonly MinusApi api;

        private string cookie;

        public LoginStatus LoginStatus { get; set; }

        public Action<string> InfoLogger { get; set; }
        public Action<string> ErrorLogger { get; set; }

        public Minus()
        {
            queuedFiles = new List<string>();
            uploadedFiles = new List<String>();

            recipients = new List<string>();

            api = new MinusApi(ApiKey);

            LoginStatus = LoginStatus.None;
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

        public void Share(Action<MinusResult.Share> galleryCreated, MinusResult.Gallery existingGallery)
        {
            // setup gallery result if we've got an existing one
            CreateGalleryResult galleryCreatedResult = 
                existingGallery != null && existingGallery.Id != null 
                ? new CreateGalleryResult(existingGallery.Id, existingGallery.Id, null)
                : null;

            // set up the listeners for CREATE
            api.CreateGalleryFailed += (sender, e) => LogError(Resources.CreateGalleryFailed, e);

            api.CreateGalleryComplete += (sender, result) =>
            {
                // gallery created, trigger upload of the first file
                galleryCreatedResult = result;
                LogInfo(Resources.GalleryCreated);
                LogInfo(Resources.UploadingFiles);
                FileInfo file = new FileInfo(queuedFiles[uploadedFiles.Count]);
                LogInfo(Resources.UploadingFile + file.Name + "...");
                api.UploadItem(cookie, result.EditorId, result.Key, queuedFiles[0]);
            };

            // set up the listeners for UPLOAD
            api.UploadItemFailed += (sender, e) => LogError(Resources.UploadFailed, e);

            api.UploadItemComplete += (sender, result) =>
            {
                // upload complete, either trigger another upload or save the gallery if all files have been uploaded
                LogInfo(Resources.UploadSuccessful);
                uploadedFiles.Add(result.Id);
                if (uploadedFiles.Count == queuedFiles.Count)
                {
                    // if all the elements are uploaded, then save the gallery
                    LogInfo(Resources.AllUploadSuccessful);

                    // guests cannot edit the gallery
                    if (LoginStatus == LoginStatus.Successful)
                    {
                        api.SaveGallery(cookie, title ?? "", galleryCreatedResult.ReaderId, galleryCreatedResult.Key, uploadedFiles.ToArray());
                    }
                    else
                    {
                        galleryCreated(new MinusResult.Share
                        {
                            Url = BaseUrl + galleryCreatedResult.ReaderId
                        });
                    }
                }
                else
                {
                    // otherwise just keep uploading
                    FileInfo file = new FileInfo(queuedFiles[uploadedFiles.Count]);
                    LogInfo(Resources.UploadingFile + file.Name + "...");
                    api.UploadItem(cookie, galleryCreatedResult.EditorId, galleryCreatedResult.Key, file.FullName);
                }
            };

            // set up the listeners for SAVE
            api.SaveGalleryFailed += (sender, e) => LogError(Resources.SaveGalleryFailed, e);

            api.SaveGalleryComplete += sender =>
            {
                LogInfo(Resources.GallerySaved);

                galleryCreated(new MinusResult.Share
                {
                    Url = BaseUrl + galleryCreatedResult.ReaderId
                });
            };

            if (galleryCreatedResult != null)
            {
                api.UploadItem(cookie, galleryCreatedResult.EditorId, galleryCreatedResult.Key, queuedFiles[0]);
            }
            else
            {
                api.CreateGallery(cookie);
            }
        }

        public void GetGalleries(Action<List<MinusResult.Gallery>> gotGalleries)
        {
            if (LoginStatus == LoginStatus.Successful)
            {
                //set up listeners for MyGalleries
                api.MyGalleriesFailed += (sender, e) => LogError(Resources.GetGalleriesFailed, e);
                api.MyGalleriesComplete += (sender, result) =>
                {
                    LogInfo(Resources.GetGalleriesSuccessful);

                    List<MinusResult.Gallery> galleries = result.Galleries.Select(gallery =>
                    {
                        bool hasId = gallery.ReaderId == UrlUnavailable || gallery.ReaderId == GalleryDeleted;

                        return new MinusResult.Gallery
                        {
                            Id = hasId ? null : gallery.ReaderId,
                            Url = hasId ? null : BaseUrl + gallery.ReaderId,
                            ItemCount = gallery.ItemCount,
                            Name = gallery.Name,
                            Deleted = gallery.ReaderId == GalleryDeleted
                        };
                    }).ToList();

                    gotGalleries(galleries);
                };

                api.MyGalleries(cookie);
            }
            else
            {
                gotGalleries(new List<MinusResult.Gallery>());
            }
        }

        public void Login(Action<LoginStatus> loggedIn)
        {
            if (LoginStatus == LoginStatus.None)
            {
                LogInfo(Resources.LoggingIn);

                // set up the listeners for SIGNIN
                api.SignInFailed += (sender, e) =>
                {
                    LogError(Resources.LoginFailed, e);
                    LoginStatus = LoginStatus.Failed;

                    loggedIn(LoginStatus);
                };
                api.SignInComplete += (sender, result) =>
                {
                    if (HasLoginCredentials())
                    {
                        LogInfo(Resources.LoggedIn);
                        LoginStatus = LoginStatus.Successful;
                    }
                    else
                    {
                        LogInfo(Resources.LoggedInAsGuest);
                        LoginStatus = LoginStatus.Anonymous;
                    }

                    cookie = result.CookieHeaders;

                    loggedIn(LoginStatus);
                };

                if (HasLoginCredentials())
                {
                    api.SignIn(Settings.Default.Username, KfEncryption.DecryptString(Settings.Default.Password).ToInsecureString());
                }
                else
                {
                    // no account set
                    api.SignIn();
                }
            }
            else
            {
                loggedIn(LoginStatus);
            }
        }

        private static bool HasLoginCredentials()
        {
            return !string.IsNullOrEmpty(Settings.Default.Username) && !string.IsNullOrEmpty(Settings.Default.Password);
        }

        private void LogInfo(string message)
        {
            if (InfoLogger != null)
            {
                InfoLogger(message);
            }
        }

        private void LogError(string message, Exception exception)
        {
            if (ErrorLogger != null)
            {
                ErrorLogger(message + " " + exception.Message);
            }
        }
    }
}