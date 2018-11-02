using System.IO;
using BackEndServer.Models.ViewModels;
using BackEndServer.Services.AbstractServices;
using Microsoft.AspNetCore.Mvc;

namespace BackEndServer.ViewComponents
{
    public class SharedRoomCardViewComponent : ViewComponent
    {
        private AbstractCameraService _cameraService;
        private AbstractCameraService CameraService => _cameraService ?? (_cameraService =
                                                           HttpContext.RequestServices.GetService(typeof(AbstractCameraService)) as
                                                               AbstractCameraService);
        
        private AbstractLocationService _locationService;
        private AbstractLocationService LocationService => _locationService ?? (_locationService =
                                                           HttpContext.RequestServices.GetService(typeof(AbstractLocationService)) as
                                                                   AbstractLocationService);
        
        public IViewComponentResult Invoke(int roomId)
        {
            CameraInformationList camerasInRoom = CameraService.GetAllCamerasInRoom(roomId);
            if (camerasInRoom.CameraList.Count < 2)
            {
                return Content(string.Empty);
            }
            return View("SharedRoomCard", camerasInRoom);
        }
    }
}