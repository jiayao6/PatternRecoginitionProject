﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meowtrix.ComponentModel;
using Windows.UI.Xaml;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using Windows.Storage;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Controls;
using Windows.Web.Http;

namespace PR
{
    public class MainPageViewModel : NotificationObject
    {
        #region ViewModel Definitions
        private ObservableCollection<string> classifierSelect = new ObservableCollection<string>();
        public ObservableCollection<string> ClassifierSelect
        {
            get => classifierSelect;
            set { classifierSelect = value; OnPropertyChanged(); }
        }

        private int classifierSelectIndex = 0;
        public int ClassifierSelectIndex
        {
            get => classifierSelectIndex;
            set { classifierSelectIndex = value; OnPropertyChanged(); }
        }

        private double testAccuracy;
        public double TestAccuracy
        {
            get => testAccuracy;
            set { testAccuracy = value; OnPropertyChanged(); }
        }

        private double trainAccuracy;
        public double TrainAccuracy
        {
            get => trainAccuracy;
            set { trainAccuracy = value; OnPropertyChanged(); }
        }

        private string dataDisplay;
        public string DataDisplay
        {
            get => dataDisplay;
            set { dataDisplay = value; OnPropertyChanged(); }
        }

        private double addX;
        public double AddX
        {
            get => addX;
            set { addX = value; OnPropertyChanged(); }
        }

        private double addY;
        public double AddY
        {
            get => addY;
            set { addY = value; OnPropertyChanged(); }
        }

        private double addLabel;
        public double AddLabel
        {
            get => addLabel;
            set { addLabel = value; OnPropertyChanged(); }
        }
        #endregion


        public List<Data> traindatas = new List<Data>();
        public List<Data> testdatas = new List<Data>();
        string serverip = "127.0.0.1";

        public void AddTrainData()
        {
            Data tempdata = new Data();
            tempdata.x = AddX;
            tempdata.y = AddY;
            tempdata.label = AddLabel;
            traindatas.Add(tempdata);
            FormatDataDisplay();
        }

        public void AddTestData()
        {
            Data tempdata = new Data();
            tempdata.x = AddX;
            tempdata.y = AddY;
            tempdata.label = AddLabel;
            testdatas.Add(tempdata);
            FormatDataDisplay();
        }

        public async void Train()
        {
            SendData senddata = new SendData();
            senddata.traindata = traindatas;
            senddata.action = 0;
            senddata.classifiertype = ClassifierSelectIndex;

            string send = JsonConvert.SerializeObject(senddata);
            try
            {
                await SendInfo(send, 0);
            }
            catch
            { }

        }

        public async void Test()
        {
            SendData senddata = new SendData();
            senddata.testdata = testdatas;
            senddata.action = 1;

            string send = JsonConvert.SerializeObject(senddata);
            try
            {
                await SendInfo(send, 1);
            }
            catch
            { }

        }


        #region Utility functions
        private void FormatDataDisplay()
        {
            string formatteddata = "Train Data\n[x, y]: label\n";
            foreach (Data sample in traindatas)
            {
                formatteddata += string.Format("[{0}, {1}]: {2}\n", sample.x, sample.y, sample.label);
            }
            formatteddata += "\nTest Data\n[x, y]: label\n";
            foreach (Data sample in testdatas)
            {
                formatteddata += string.Format("[{0}, {1}]: {2}\n", sample.x, sample.y, sample.label);
            }
            DataDisplay = formatteddata;
        }

        public async Task SendInfo(string send, int func)
        {
            string str_uri = string.Format("http://{0}/post", serverip);
            HttpResponseMessage httpresponse = new HttpResponseMessage();
            string httpresponsebody;
            Uri requestUri = new Uri(str_uri);
            HttpClient httpclient = new HttpClient();

            try
            {
                httpclient.DefaultRequestHeaders.Accept.Add(new Windows.Web.Http.Headers.HttpMediaTypeWithQualityHeaderValue("application/json"));
                httpresponse = await httpclient.PostAsync(requestUri, new HttpStringContent(send, Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/json"));

                httpresponsebody = await httpresponse.Content.ReadAsStringAsync();

                //TODO

            }
            catch (Exception ex)
            {
                httpresponsebody = JsonConvert.SerializeObject("Error: " + ex.HResult.ToString("x") + "Message: " + ex.Message);
                DisplayDialog("Connection failed", "Please check server IP address and your network status and try again");
                throw (ex);

            }
        }

        private async void DisplayDialog(string title, string content)
        {
            ContentDialog noWifiDialog = new ContentDialog
            {
                Title = title,
                Content = content,
                CloseButtonText = "Ok"
            };
            try
            {
                ContentDialogResult result = await noWifiDialog.ShowAsync();
            }
            catch
            {

            }
        }
        #endregion

    }
}
