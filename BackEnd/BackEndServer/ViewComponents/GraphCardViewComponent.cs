using BackEndServer.Models.ViewModels;
using BackEndServer.Services.AbstractServices;
using Microsoft.AspNetCore.Mvc;

namespace BackEndServer.ViewComponents
{
    public class GraphCardViewComponent : ViewComponent
    {
        private AbstractCameraService _cameraService;
        private AbstractCameraService CameraService => _cameraService ?? (_cameraService =
                                                           HttpContext.RequestServices.GetService(typeof(AbstractCameraService)) as
                                                               AbstractCameraService);
        
        public IViewComponentResult Invoke(int cameraId)
        {
            CameraStatistics cameraStatisticsModel = CameraService.getCameraStatisticsForNowById(cameraId);
            return View("GraphCard", cameraStatisticsModel);
        }
    }
}