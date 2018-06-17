using BackEndServer.Classes.EntityDefinitionClasses;
using BackEndServer.Classes.ErrorResponseClasses;

namespace BackEndServer.Services.AbstractServices
{
    public interface AbstractDataMessageService
    {
        bool CheckDataMessageValidity(DataMessage message);
        InvalidDataMessageResponseBody CreateInvalidDataMessageResponseBody(DataMessage message);
        bool StoreStatsFromDataMessage(DataMessage message);
        bool CheckTimeIntervalValidity(TimeInterval timeInterval);
        DataMessage RetrievePerSecondStatsBetweenInterval(TimeInterval timeInterval);
    }
}
