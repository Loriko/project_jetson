using BackEndServer.Classes.EntityDefinitionClasses;
using BackEndServer.Classes.ErrorResponseClasses;
using System;

namespace BackEndServer.Services.AbstractServices
{
    public interface AbstractDataMessageService
    {
        bool checkDataMessageValidity(DataMessage message);
        InvalidDataMessageResponseBody createInvalidDataMessageResponseBody(DataMessage message);
        bool storeStatsFromDataMessage(DataMessage message);
        bool checkTimeIntervalValidity(TimeInterval timeInterval);
        DataMessage retrievePerSecondStatsBetweenInterval(TimeInterval timeInterval);
    }
}
