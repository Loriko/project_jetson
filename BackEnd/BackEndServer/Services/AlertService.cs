using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using BackEndServer.Models.DBModels;
using BackEndServer.Models.ViewModels;
using BackEndServer.Services.AbstractServices;
using BackEndServer.Services.HelperServices;
using Castle.Core.Internal;
using Moq;

namespace BackEndServer.Services
{
    public class AlertService : AbstractAlertService
    {
        private readonly IDatabaseQueryService _dbQueryService;
        private readonly AbstractCameraService _cameraService;

        public AlertService(IDatabaseQueryService dbQueryService, AbstractCameraService cameraService)
        {
            _dbQueryService = dbQueryService;
            _cameraService = cameraService;
        }

        public bool SaveAlert(AlertDetails alertDetails)
        {
            if (alertDetails.AlertId != 0)
            {
                return _dbQueryService.PersistExistingAlert(new DatabaseAlert(alertDetails));
            }
            else
            {
                return _dbQueryService.PersistNewAlert(new DatabaseAlert(alertDetails));
            }
        }

        public List<AlertDetails> GetAllAlertsForUser(int userId)
        {
            List<DatabaseAlert> dbAlerts = _dbQueryService.GetAllAlerts(userId);
            List<AlertDetails> alertList = new List<AlertDetails>();
            foreach (var dbAlert in dbAlerts)
            {
                alertList.Add(new AlertDetails(dbAlert));
            }

            return alertList;
        }

        public SortedDictionary<int, List<AlertDetails>> GetAllAlertsByCameraIdForUser(int userId)
        {
            List<DatabaseAlert> dbAlerts = _dbQueryService.GetAllAlerts(userId);
            
            SortedDictionary<int, List<AlertDetails>> alertMap = new SortedDictionary<int, List<AlertDetails>>();
            foreach (var dbAlert in dbAlerts)
            {
                if (!alertMap.ContainsKey(dbAlert.CameraId))
                {
                    alertMap[dbAlert.CameraId] = new List<AlertDetails>();
                }

                alertMap[dbAlert.CameraId].Add(new AlertDetails(dbAlert));
            }
            
            return alertMap;
        }

        public bool DeleteAlert(int alertId)
        {
            return _dbQueryService.DeleteAlert(alertId);
        }

        public bool DisableAlert(AlertDisablingInformation alertDisablingInformation)
        {
            DatabaseAlert dbAlert = _dbQueryService.GetAlertById(alertDisablingInformation.AlertId);
            DateTime disabledUntilDateTime = DateTime.MaxValue;
            if (!alertDisablingInformation.DisableForever && alertDisablingInformation.DisabledUntil != null)
            {
                disabledUntilDateTime = alertDisablingInformation.DisabledUntil.Value;
            } else if (!alertDisablingInformation.DisableForever && alertDisablingInformation.DisabledUntil == null)
            {
                throw new InvalidDataException("Told to disable alert temporarily but no end date");
            }

            dbAlert.DisabledUntil = disabledUntilDateTime;
            return _dbQueryService.PersistExistingAlert(dbAlert);
        }

        public bool EnableAlert(int alertId)
        {
            DatabaseAlert dbAlert = _dbQueryService.GetAlertById(alertId);
            dbAlert.DisabledUntil = null;
            return _dbQueryService.PersistExistingAlert(dbAlert);
        }

        public List<AlertSummary> GetAllActiveAlertsForCameraKey(string cameraKey)
        {
            int cameraId = _dbQueryService.GetCameraIdFromKey(cameraKey);
            List<DatabaseAlert> dbAlerts = _dbQueryService.GetAlertsByCameraId(cameraId);
            List<AlertSummary> alertList = new List<AlertSummary>();
            foreach (var dbAlert in dbAlerts)
            {
                if (dbAlert.DisabledUntil.GetValueOrDefault(DateTime.MinValue) < DateTime.Now 
                    && (dbAlert.StartTime.IsNullOrEmpty() || dbAlert.StartTime.ToDateTime() < DateTime.Now)
                    && (dbAlert.EndTime.IsNullOrEmpty() || dbAlert.EndTime.ToDateTime() > DateTime.Now))
                {
                    alertList.Add(new AlertSummary(dbAlert));
                }
            }

            return alertList;
        }
    }
}