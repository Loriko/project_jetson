using System.Linq;
using System.Threading;
using BackEndServer.Classes.EntityDefinitionClasses;
using BackEndServer.Models.ViewModels;
using BackEndServer.Services.AbstractServices;

namespace BackEndServer.Services
{
    public class AlertSummaryService
    {
        private readonly AbstractAlertService _alertService;
        private bool SendAlertSummary { get; set; }
        private object SendAlertSummaryLock { get; set; }
        private static readonly int TIME_BETWEEN_SUMMARIES_MS = 30000;
        
        public AlertSummaryService(AbstractAlertService alertService, bool sendFramesAsJpg)
        {
            _alertService = alertService;
            SendAlertSummary = false;
            SendAlertSummaryLock = new object();
            //This being disabled will prevent images from being sent to the application by the Image Recognition system
            if (sendFramesAsJpg)
            {
                Thread alertSummaryBoolUpdateThread = new Thread(TurnOnSendAlertSummaryPeriodically);
                alertSummaryBoolUpdateThread.Start();
            }
        }

        private void TurnOnSendAlertSummaryPeriodically()
        {
            while (true)
            {
                lock (SendAlertSummaryLock) {
                    SendAlertSummary = true;
                }
                Thread.Sleep(TIME_BETWEEN_SUMMARIES_MS);
            }
        }

        public DataReceivalResponse GetReceivalResponse(DataMessage receivedMessage)
        {
            DataReceivalResponse response = new DataReceivalResponse();
            response.NumberOfReceivedStats = receivedMessage.GetLength();
            bool sendAlertSummaryValue;
            lock (SendAlertSummaryLock)
            {
                sendAlertSummaryValue = SendAlertSummary;
                SendAlertSummary = false;
            }
            if (sendAlertSummaryValue && receivedMessage.RealTimeStats.Length > 0)
            {
                //Assumes the entire message is only reporting for one camera
                response.ActiveAlertsForCamera = _alertService.GetAllActiveAlertsForCameraKey(receivedMessage.RealTimeStats.First().CameraKey);
            }

            return response;
        }
    }
}