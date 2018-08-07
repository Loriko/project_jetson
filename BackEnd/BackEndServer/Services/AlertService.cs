using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using BackEndServer.Models.DBModels;
using BackEndServer.Models.ViewModels;
using BackEndServer.Services.AbstractServices;
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
    }
}