using BackEndServer.Models.ViewModels;
using BackEndServer.Services.AbstractServices;
using Castle.Core.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackEndServer.ViewComponents
{
    public class StatFramesCardViewComponent : ViewComponent
    {
        private AbstractCameraService _cameraService;
        private AbstractCameraService CameraService => _cameraService ?? (_cameraService =
                                                           HttpContext.RequestServices.GetService(typeof(AbstractCameraService)) as
                                                               AbstractCameraService);
        
        public IViewComponentResult Invoke(int cameraId)
        {
            JpgStatFrameList frameList = CameraService.GetStatFrameList(cameraId);
            if (frameList == null || frameList.JpgFramePathList.IsNullOrEmpty())
            {
                return Content(string.Empty);
            }
            return View("StatFramesCard", frameList);
        }
    }
}